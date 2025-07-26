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
        private readonly IViacepService _viacepService;

        public ExampleAppService(ILogger<ExampleAppService> logger, 
                                 IExampleRepository exampleRepository,
                                 IViacepService viacepService)
        {
            _logger = logger;
            _exampleRepository = exampleRepository;
            _viacepService = viacepService;
        }
        public async Task<string> GetAll(CancellationToken cancellationToken)
        {
            try
            {
                var listexampleResult = await _exampleRepository.GetAllAsync(cancellationToken);

                return listexampleResult.ToString();
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

                return listexampleResult.ToString();
            }
            catch (Exception ex)
            {
                _logger.LogError($"ERRO AO VERIFICAR ELEGIBILIDADE A SALA DE ESPERA ATIVA: {ex.Message}");
                return null;
            }
        }
        public async Task SynCity(CancellationToken cancellationToken)
        {
            try
            {
                var listexampleResult = await _viacepService.GetCityByCountry("SP", "Sao%20Paulo");

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
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"ERRO AO VERIFICAR ELEGIBILIDADE A SALA DE ESPERA ATIVA: {ex.Message}");
            }
        }
    }
}
