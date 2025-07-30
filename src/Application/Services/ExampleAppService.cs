using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Application.DTO;
using Domain.Common;
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
                                                                  exampleResult.Estado,
                                                                  exampleResult.Uf), cancellationToken);


                    await _exampleRepository.SaveChangesAsync(cancellationToken);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao realizar o sync da tabela de exemplo por CEP: {Message}", ex.Message);
                throw new Exception(ex.Message);
            }
        }
    }
}
