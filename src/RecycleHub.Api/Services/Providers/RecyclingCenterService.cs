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

public class RecyclingCenterService(
    ILogger<RecyclingCenterService> logger,
    IUnitOfWork unitOfWork) : IRecyclingCenterService
{
    public async Task<ApiResponse<PagedResponse<RecycleCenterResponse>>> GetAllAsync(CenterFilter filter, CancellationToken ct = default)
    {
        try
        {
            var predicate = PredicateBuilder.True<RecycleCenter>();

            if (!string.IsNullOrWhiteSpace(filter.MaterialId))
                predicate = predicate.And(x => x.Materials.Any(m => m.Id == filter.MaterialId));

            if (!string.IsNullOrWhiteSpace(filter.Search))
                predicate = predicate.And(x => x.Name.ToLower().Contains(filter.Search.ToLower()));

            var hasGeoFilter = filter is { Latitude: not null, Longitude: not null };

            if (hasGeoFilter)
            {
                var lat = filter.Latitude!.Value;
                var lon = filter.Longitude!.Value;
                var latDelta = filter.Radius / 111.0;
                var lonDelta = filter.Radius / (111.0 * Math.Cos(lat * Math.PI / 180.0));

                predicate = predicate.And(x =>
                    x.Latitude >= lat - latDelta && x.Latitude <= lat + latDelta &&
                    x.Longitude >= lon - lonDelta && x.Longitude <= lon + lonDelta);

                var dbContext = (ApplicationDbContext)unitOfWork.GetDbContext();
                var boundingBoxResults = await dbContext.RecycleCenters
                    .AsNoTracking()
                    .Where(predicate)
                    .Include(x => x.Materials)
                    .Include(x => x.Photos)
                    .ProjectToType<RecycleCenterResponse>()
                    .ToListAsync(ct);

                var filtered = boundingBoxResults
                    .Where(c => UtilityConstants.CalculateDistance(c.Latitude, c.Longitude, lat, lon) <= filter.Radius)
                    .ToList();

                var paged = filtered
                    .Skip((filter.PageIndex - 1) * filter.PageSize)
                    .Take(filter.PageSize)
                    .ToList();

                var response = paged.ToPagedResponse(filter.PageIndex, filter.PageSize, filtered.Count);
                return response.ToApiResponse("Success", StatusCodes.Status200OK);
            }

            var result = await unitOfWork.RecycleCenters
                .GetRecycleCentersAsync<RecycleCenterResponse>(filter, predicate, ct: ct);

            return result.ToApiResponse("Success", StatusCodes.Status200OK);
        }
        catch (Exception e)
        {
            logger.LogError(e, "Error fetching recycle centers: {Message}", e.Message);
            return ApiResponse<PagedResponse<RecycleCenterResponse>>.Fail();
        }
    }

    public async Task<ApiResponse<RecycleCenterResponse>> GetByIdAsync(string id, CancellationToken ct = default)
    {
        try
        {
            var center = await unitOfWork.RecycleCenters.GetByIdAsync(
                x => x.Id == id,
                include: q => q.Include(c => c.Materials).Include(c => c.Photos).AsNoTracking(),
                ct: ct);

            if (center == null)
                return ApiResponse<RecycleCenterResponse>.Fail("Center not found", StatusCodes.Status404NotFound);

            var response = center.Adapt<RecycleCenterResponse>();
            return response.ToApiResponse("Success", StatusCodes.Status200OK);
        }
        catch (Exception e)
        {
            logger.LogError(e, "Error fetching recycle center {Id}: {Message}", id, e.Message);
            return ApiResponse<RecycleCenterResponse>.Fail();
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
            center.RecycledProducts = request.RecycledProducts;
            center.OpeningHours = request.OpeningHours;
            center.Certifications = request.Certifications;
            center.UpdatedAt = DateTime.UtcNow;

            var validMaterials = (await unitOfWork.Materials
                .GetAllAsync(m => request.MaterialIds.Contains(m.Id), ct: ct)).ToList();

            if (validMaterials.Count != request.MaterialIds.Count)
                return ApiResponse<bool>.Fail("One or more Material IDs are invalid", StatusCodes.Status400BadRequest);

            center.Materials.Clear();

            foreach (var material in validMaterials)
            {
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

            await unitOfWork.SaveChangesAsync();

            return true.ToApiResponse("Recycle Center updated successfully", StatusCodes.Status200OK);
        }
        catch (Exception e)
        {
            logger.LogError(e, "Error updating center {Id}: {Message}", request.Id, e.Message);
            return ApiResponse<bool>.Fail("Failed to update recycle center");
        }
    }

    public async Task<ApiResponse<bool>> DeleteAsync(string id, CancellationToken ct = default)
    {
        try
        {
            var center = await unitOfWork.RecycleCenters.GetByIdAsync(id, ct);
            if (center == null)
                return ApiResponse<bool>.Fail("Center not found", StatusCodes.Status404NotFound);

            center.IsDeleted = true;
            center.UpdatedAt = DateTime.UtcNow;
            await unitOfWork.RecycleCenters.UpdateAsync(center, saveChanges: true, ct: ct);

            return true.ToApiResponse("Center deleted successfully", StatusCodes.Status200OK);
        }
        catch (Exception e)
        {
            logger.LogError(e, "Error deleting center {Id}: {Message}", id, e.Message);
            return ApiResponse<bool>.Fail("Failed to delete recycle center");
        }
    }
}
