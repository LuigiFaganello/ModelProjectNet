using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace Application.Services
{
    public class ExampleAppService : IExampleAppService
    {

        private readonly ILogger<ExampleAppService> _logger;
        private readonly IExampleRepository _exampleRepository;

        public ExampleAppService(ILogger<ExampleAppService> logger, 
                                 IExampleRepository exampleRepository)
        {
            _logger = logger;
            _exampleRepository = exampleRepository;
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
    }
}
