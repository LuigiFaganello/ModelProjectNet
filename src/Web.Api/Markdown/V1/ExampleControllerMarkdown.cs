namespace API.Markdown.V1
{
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
        public class Post
        {
            public const string Summary = @"Cria dado";
            public const string Description = @"Criar dado na tabela<br/><br/>
            <strong>Banco de dados</strong><br/><br/>
            - Tabela = Examples = Retorna dados da tabela de exemplos";
        }
    }
}
