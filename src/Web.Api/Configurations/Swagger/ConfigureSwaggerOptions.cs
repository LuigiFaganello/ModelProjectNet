using System.Diagnostics.CodeAnalysis;
using Asp.Versioning.ApiExplorer;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace API.Configurations.Swagger
{
    [ExcludeFromCodeCoverage]
    public class ConfigureSwaggerOptions
            : IConfigureNamedOptions<SwaggerGenOptions>
    {
        private readonly IApiVersionDescriptionProvider _provider;

        public ConfigureSwaggerOptions(
            IApiVersionDescriptionProvider provider)
        {
            _provider = provider;
        }

        public void Configure(SwaggerGenOptions options)
        {
            foreach (var description in _provider.ApiVersionDescriptions)
            {
                options.SwaggerDoc(
                    description.GroupName,
                    CreateVersionInfo(description));
            }
        }

        public void Configure(string name, SwaggerGenOptions options)
        {
            Configure(options);
        }

        private OpenApiInfo CreateVersionInfo(ApiVersionDescription desc)
        {
            var info = new OpenApiInfo
            {
                Version = desc.ApiVersion.ToString(),
                Title = "Projeto modelo em .Net",
                Description = "<div>O escopo deste documento tem como objetivo apresentar de maneira detalhada todas as APIs " +
                                "para integração com modulo: XPTO.<br/>" +
                                "<br/>" +
                                "<strong>Tecnologias / Ferramentas utilizadas</strong><br/><br/>" +
                                "- .Net Core 9<br/> " +
                                "- Mysql<br/> " +
                                "<br/>" +
                                "Qualquer dúvida ou problema favor entrar em contato com a squad XPTO.<br/>",
                Contact = new OpenApiContact { Name = "Nome da empresa", Email = "email@email.br" }
            };

            if (desc.IsDeprecated)
            {
                info.Description += "Esta versão da API está obsoleta. Use uma das novas APIs disponíveis com versão mais recente";
            }

            return info;
        }
    }
}
