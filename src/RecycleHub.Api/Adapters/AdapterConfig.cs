using Mapster;
using RecycleHub.Api.Dtos.Responses;
using RecycleHub.Pg.Sdk.Entities;

namespace RecycleHub.Api.Adapters;

public class AdapterConfig : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<RecycleCenter, RecycleCenterResponse>()
            .Map(dest => dest.Materials, src => src.Materials.Select(m => m.Name));
            
        // You can add more mappings here for other entities
        // config.NewConfig<User, UserResponse>().Map(...)
    }
}
