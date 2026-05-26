using Application.DTO;
using Application.Interfaces;
using Domain.Common;
using Domain.Entities;
using Domain.Interfaces;
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

        public async Task<Result<IEnumerable<ExampleAppServiceDto>>> GetAll(CancellationToken cancellationToken)
        {
            try
            {
                var listexampleResult = await _exampleRepository.GetAllAsync(cancellationToken);

                //Pode ser substituido por um lib como AutoMapper ou Mapster para mapear os objetos
                var result = listexampleResult.Select(x => new ExampleAppServiceDto
                {
                    ZipCode = x.ZipCode,
                    Street = x.Street,
                    Complement = x.Complement,
                    Unit = x.Unit,
                    Neighborhood = x.Neighborhood,
                    City = x.City,
                    State = x.State
                }).ToList();

                return Result<IEnumerable<ExampleAppServiceDto>>.Success(result);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter todos os exemplos: {Message}", ex.Message);
                return Result<IEnumerable<ExampleAppServiceDto>>.Failure(Error.Internal("Erro ao obter todos os exemplos"));
            }
        }

        public async Task<Result<ExampleAppServiceDto>> GetByZipCode(string zipCode, CancellationToken cancellationToken)
        {
            try
            {
                var exampleResult = await _exampleRepository.GetByZipCodeAsync(zipCode, cancellationToken);

                if (exampleResult == null)
                    return Result<ExampleAppServiceDto>.Failure(Error.NotFound("zipCode", zipCode));

                //Pode ser substituido por um lib como AutoMapper ou Mapster para mapear os objetos
                var result = new ExampleAppServiceDto
                {
                    ZipCode = exampleResult.ZipCode,
                    Street = exampleResult.Street,
                    Complement = exampleResult.Complement,
                    Unit = exampleResult.Unit,
                    Neighborhood = exampleResult.Neighborhood,
                    City = exampleResult.City,
                    State = exampleResult.State
                };

                return Result<ExampleAppServiceDto>.Success(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter exemplo por CEP: {Message}", ex.Message);
                return Result<ExampleAppServiceDto>.Failure(Error.Internal("Erro ao obter exemplo por CEP"));
            }
        }

        public async Task SyncCity(string state, string city, string street, CancellationToken cancellationToken)
        {
            try
            {
                var addresses = await _exampleService.GetAddressesAsync(state, city, street, cancellationToken);

                var examples = addresses
                    .Select(a => new Example(a.ZipCode,
                                             a.Street,
                                             a.Complement,
                                             a.Unit,
                                             a.Neighborhood,
                                             a.City,
                                             a.State))
                    .ToList();

                if (examples.Count == 0)
                    return;

                // Persiste em lote, num único round-trip/SaveChanges (evita N+1).
                await _exampleRepository.AddRangeAsync(examples, cancellationToken);
                await _exampleRepository.SaveChangesAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao realizar o sync da tabela de exemplo por CEP: {Message}", ex.Message);
                throw;
            }
        }
    }
}
