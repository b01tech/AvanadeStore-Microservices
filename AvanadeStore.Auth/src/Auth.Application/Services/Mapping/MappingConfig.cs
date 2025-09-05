using Auth.Application.DTOs.Requests;
using Auth.Application.DTOs.Responses;
using Auth.Domain.Entities;
using Auth.Domain.ValueObjects;
using Mapster;
using MapsterMapper;
using Microsoft.Extensions.DependencyInjection;

namespace Auth.Application.Services.Mapping;
public static class MappingConfig
{
    public static void RegisterMapster(this IServiceCollection services)
    {
        var config = new TypeAdapterConfig();

        config.NewConfig<RequestCreateClientDTO, Client>()
            .MapWith(src => new Client(
                  src.Name,
                  src.Email,
                  src.Password,
                  new Cpf(src.Cpf)
              ));
        config.NewConfig<Client, ResponseCreateUserDTO>()
              .Map(dest => dest.Id, src => src.Id)
              .Map(dest => dest.Name, src => src.Name)
              .Map(dest => dest.CreateAt, src => src.CreatedAt);

        services.AddSingleton(config);
        services.AddScoped<IMapper, ServiceMapper>();
    }
}
