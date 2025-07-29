using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Application.DTO;
using Domain.Entities;
using Domain.Interfaces;
using Infrastructure.ExternalService.Interface;
using Microsoft.Extensions.Logging;

namespace Application.Services
{
    public class ExampleAppService : IExampleAppService
    {
        private readonly ILogger<ExampleAppService> _logger;
        private readonly IExampleRepository _exampleRepository;
        private readonly IExampleService _exampleService;

        public ExampleAppService(ILogger<ExampleAppService> logger, 
                                 IExampleRepository exampleRepository,
                                 IExampleService exampleService)
        {
            _logger = logger;
            _exampleRepository = exampleRepository;
            _exampleService = exampleService;
        }
        public async Task<IEnumerable<ExampleDto>> GetAll(CancellationToken cancellationToken)
        {
            try
            {
                var listexampleResult = await _exampleRepository.GetAllAsync(cancellationToken);

                //Pode ser substituido por um lib como AutoMapper ou Mapster para mapear os objetos
                var result = listexampleResult.Select(x => new ExampleDto
                {
                    ZipCode = x.ZipCode,
                    Street = x.Street,
                    Complement = x.Complement,
                    Unit = x.Unit,
                    Neighborhood = x.Neighborhood,
                    City = x.City,
                    State = x.State,
                    StateName = x.StateName
                }).ToList();

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter todos os exemplos: {Message}", ex.Message);
                return Enumerable.Empty<ExampleDto>();
            }
        }
        public async Task<ExampleDto> GetByZipCode(string zipCode, CancellationToken cancellationToken)
        {
            try
            {
                var exampleResult = await _exampleRepository.GetByZipCodeAsync(zipCode, cancellationToken);

                if (exampleResult == null)
                    return null;

                //Pode ser substituido por um lib como AutoMapper ou Mapster para mapear os objetos
                var result = new ExampleDto
                {
                    ZipCode = exampleResult.ZipCode,
                    Street = exampleResult.Street,
                    Complement = exampleResult.Complement,
                    Unit = exampleResult.Unit,
                    Neighborhood = exampleResult.Neighborhood,
                    City = exampleResult.City,
                    State = exampleResult.State,
                    StateName = exampleResult.StateName
                };

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter exemplo por CEP: {Message}", ex.Message);
                return null;
            }
        }
        public async Task SyncCity(CancellationToken cancellationToken)
        {
            try
            {
                var listexampleResult = await _exampleService.GetCityByCountry("SP", "Sao%20Paulo");

                foreach (var exampleResult in listexampleResult)
                {
                    await _exampleRepository.AddAsync(new Example(exampleResult.Cep,
                                                                  exampleResult.Logradouro,
                                                                  exampleResult.Complemento,
                                                                  exampleResult.Unidade,
                                                                  exampleResult.Bairro,
                                                                  exampleResult.Localidade,
                                                                  exampleResult.Uf,
                                                                  exampleResult.Estado), cancellationToken);


                    await _exampleRepository.SaveChangesAsync(cancellationToken);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao realizar o sync da tabela de exemplo por CEP: {Message}", ex.Message);
            }
        }
    }
}
