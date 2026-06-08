using Mapster;
using RecycleHub.Api.Dtos.Responses;
using RecycleHub.Pg.Sdk.Entities;

namespace RecycleHub.Api.Adapters;

public class AdapterConfig : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<RecycleCenter, RecycleCenterResponse>()
            .Map(dest => dest.Materials, src => src.Materials.Select(m => new MaterialResponse
            {
                Id = m.Id,
                Name = m.Name,
            }))
            .Map(dest => dest.Photos, src => src.Photos
                .OrderBy(p => p.SortOrder)
                .Select(p => new PhotoResponse
                {
                    Id = p.Id,
                    Url = p.Url,
                    IsPrimary = p.IsPrimary,
                    SortOrder = p.SortOrder,
                }));
    }
}
