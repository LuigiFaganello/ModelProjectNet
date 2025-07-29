using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
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
        public async Task<string> GetAll(CancellationToken cancellationToken)
        {
            try
            {
                var listexampleResult = await _exampleRepository.GetAllAsync(cancellationToken);

                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError($"ERRO AO VERIFICAR ELEGIBILIDADE A SALA DE ESPERA ATIVA: {ex.Message}");
                return null;
            }
        }
        public async Task<string> GetByZipCode(string zipCode, CancellationToken cancellationToken)
        {
            try
            {
                var listexampleResult = await _exampleRepository.GetByZipCodeAsync(zipCode, cancellationToken);

                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError($"ERRO AO VERIFICAR ELEGIBILIDADE A SALA DE ESPERA ATIVA: {ex.Message}");
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
                _logger.LogError($"ERRO AO VERIFICAR ELEGIBILIDADE A SALA DE ESPERA ATIVA: {ex.Message}");
            }
        }
    }
}
