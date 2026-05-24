using System.Diagnostics.CodeAnalysis;

namespace Web.Api.Markdown.V2
{
    [ExcludeFromCodeCoverage]
    public class ExampleControllerMarkdown
    {
        public class GetAll
        {
            public const string Summary = @"Retorna todos os dados";
            public const string Description = @"Retorna todos os dados.<br/><br/>
            <strong>Banco de dados</strong><br/><br/>
            - Tabela = Examples = Retorna dados da tabela de exemplos";
        }
        public class GetByZipCode
        {
            public const string Summary = @"Retorna por zipcode os dados";
            public const string Description = @"Retorna por zipcode os dados<br/><br/>
            <strong>Banco de dados</strong><br/><br/>
            - Tabela = Examples = Retorna dados da tabela de exemplos";
        }
        public class Sync
        {
            public const string Summary = @"Sincroniza os dados de cidade";
            public const string Description = @"Sincroniza os dados de cidade a partir do serviço externo.<br/><br/>
            <strong>Banco de dados</strong><br/><br/>
            - Tabela = Examples = Persiste os dados retornados do serviço externo na tabela de exemplos";
        }
    }
}
