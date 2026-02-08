using Mapster;
using Microsoft.EntityFrameworkCore;
using RecycleHub.Api.Dtos.Requests;
using RecycleHub.Api.Dtos.Responses;
using RecycleHub.Api.Services.Interfaces;
using RecycleHub.Pg.Sdk;
using RecycleHub.Pg.Sdk.Dtos;
using RecycleHub.Pg.Sdk.Entities;
using RecycleHub.Pg.Sdk.Repositories.Interfaces;
using RecycleHub.Utils;
using RecycleHub.Utils.Extensions;

namespace RecycleHub.Api.Services.Providers;

public class RecyclingCenterService(ILogger<RecyclingCenterService> logger,
    IUnitOfWork unitOfWork) : IRecyclingCenterService
{
    public async Task<ApiResponse<PagedResponse<RecycleCenterResponse>>> GetAllAsync(CenterFilter filter, CancellationToken ct = default)
    {
        try
        {
            var predicate = PredicateBuilder.True<RecycleCenter>();

            if (!string.IsNullOrWhiteSpace(filter.MaterialId))
            {
                predicate = predicate.And(x => x.Materials.Any(m => m.Id == filter.MaterialId));
            }
            
            
            var result = await unitOfWork.RecycleCenters.GetRecycleCentersAsync<RecycleCenterResponse>(filter, predicate,  ct: ct);
            
            
            var response = GetCloseByCenters(filter, result);

            return response.ToApiResponse("Success",StatusCodes.Status200OK);
        }
        catch (Exception e)
        {
            logger.LogError(e, "Error fetching recycle centers: {Message}", e.Message);
            
            return ApiResponse<PagedResponse<RecycleCenterResponse>>.Fail();
        }
    }
    
    public async Task<ApiResponse<bool>> CreateAsync(CreateRecycleCenterRequest request, CancellationToken ct = default)
    {
        try
        {
            var center = request.Adapt<RecycleCenter>();
            
            if (request.MaterialIds.Count != 0)
            {
                var materials = await unitOfWork.Materials
                    .GetAllAsync(m => request.MaterialIds.Contains(m.Id), ct: ct);

                center.Materials = materials.ToList();
                
                unitOfWork.GetDbContext().AttachRange(center.Materials);
            }
            
            await unitOfWork.RecycleCenters.AddAsync(center, saveChanges: true, ct: ct);
            
            return true.ToApiResponse("Recycle Center created successfully", StatusCodes.Status201Created);
        }
        catch (Exception e)
        {
            logger.LogError(e, "Error creating recycle center: {Message}", e.Message);
            
            return ApiResponse<bool>.Fail("Failed to create recycle center");
        }
    }
    
    public async Task<ApiResponse<bool>> UpdateAsync(UpdateRecycleCenterRequest request, CancellationToken ct = default)
    {
        try
        {
            var center = await unitOfWork.RecycleCenters
                .GetByIdAsync(x => x.Id == request.Id, include: q => q.Include(c => c.Materials), ct: ct);

            if (center == null) 
                return ApiResponse<bool>.Fail("Center not found", StatusCodes.Status404NotFound);
            
            center.Name = request.Name;
            center.Address = request.Address;
            center.Description = request.Description;
            center.Latitude = request.Latitude;
            center.Longitude = request.Longitude;
            center.LogoUrl = request.LogoUrl;
            center.PhoneNumber = request.PhoneNumber;
            center.WhatsappNumber = request.WhatsappNumber;
            center.Email = request.Email;
            center.WebsiteUrl = request.WebsiteUrl;
            center.City = request.City;
            center.Region = request.Region;

            var validMaterials = (await unitOfWork.Materials
                .GetAllAsync(m => request.MaterialIds.Contains(m.Id), ct: ct)).ToList();
            
            if (validMaterials.Count != request.MaterialIds.Count)
            {
                return ApiResponse<bool>.Fail("One or more Material IDs are invalid", StatusCodes.Status400BadRequest);
            }

            foreach (var material in validMaterials)
            {
                if (center.Materials.Any(m => m.Id == material.Id)) continue;
                
                var tracked = unitOfWork.GetDbContext().ChangeTracker.Entries<Material>()
                    .FirstOrDefault(e => e.Entity.Id == material.Id)?.Entity;

                if (tracked != null)
                    center.Materials.Add(tracked);
                else
                {
                    unitOfWork.GetDbContext().Attach(material);
                    center.Materials.Add(material);
                }
            }
            
            

            foreach (var material in validMaterials)
            {
                // Check if THIS EXACT material instance is already in the tracker from the 'Include'
                var trackedEntry = unitOfWork.GetDbContext().ChangeTracker.Entries<Material>()
                    .FirstOrDefault(e => e.Entity.Id == material.Id);

                if (trackedEntry != null)
                {
                    // Use the one EF already knows about
                    center.Materials.Add(trackedEntry.Entity);
                }
                else
                {
                    unitOfWork.GetDbContext().Attach(material);
                    center.Materials.Add(material);
                }
            }

            await unitOfWork.SaveChangesAsync();

            return true.ToApiResponse("Recycle Centers updated successfully", StatusCodes.Status200OK);
        }
        catch (Exception e)
        {
            logger.LogError(e, "Error updating center {Id}: {Message}", request.Id, e.Message);
            
            return ApiResponse<bool>.Fail("Failed to update recycle center");
        }
    }

    private static PagedResponse<RecycleCenterResponse> GetCloseByCenters(CenterFilter filter, PagedResponse<RecycleCenterResponse> pagedRecycleCenters)
    {
        var recycleCenters = pagedRecycleCenters.Results.ToList();   
        var closeByCenters = new List<RecycleCenterResponse>();
        
        if (filter is not { Longitude: not null, Latitude: not null })
        {
            return pagedRecycleCenters;
        }
        
        var lon = filter.Longitude!.Value;
        var lat = filter.Latitude!.Value;

        foreach (var center in recycleCenters)
        {
            double distance = UtilityConstants.CalculateDistance(center.Latitude, center.Longitude, lat, lon);

            if (distance <= filter.Radius)
            {
                closeByCenters.Add(center);
            }
        }
        
        return closeByCenters.ToPagedResponse( filter.PageIndex, filter.PageSize, recycleCenters.Count);
    }
}
