using System.Diagnostics.CodeAnalysis;
using Serilog.Context;

namespace Web.Api.Middleware
{
    [ExcludeFromCodeCoverage]
    public class CorrelationMiddleware
    {
        /// <summary>Header HTTP usado para receber/retornar o id de correlação.</summary>
        public const string CorrelationHeaderKey = "X-Correlation-Id";

        /// <summary>Chave em <see cref="HttpContext.Items"/> onde o id fica disponível para o restante do pipeline.</summary>
        public const string CorrelationItemKey = "CorrelationId";

        private readonly RequestDelegate _next;

        public CorrelationMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            var correlationId = GetOrCreateCorrelationId(context);

            // Disponibiliza o id para o restante do pipeline (ex.: middleware de exceção).
            context.Items[CorrelationItemKey] = correlationId;

            // Garante que o id seja devolvido ao cliente, antes de o corpo da resposta começar a ser escrito.
            context.Response.OnStarting(() =>
            {
                context.Response.Headers[CorrelationHeaderKey] = correlationId;
                return Task.CompletedTask;
            });

            // Enriquece TODOS os logs Serilog emitidos durante a requisição com o id de correlação.
            using (LogContext.PushProperty(CorrelationItemKey, correlationId))
            {
                await _next.Invoke(context);
            }
        }

        private static string GetOrCreateCorrelationId(HttpContext context)
        {
            if (context.Request.Headers.TryGetValue(CorrelationHeaderKey, out var existing)
                && !string.IsNullOrWhiteSpace(existing))
            {
                return existing.ToString();
            }

            return Guid.NewGuid().ToString();
        }
    }
}
