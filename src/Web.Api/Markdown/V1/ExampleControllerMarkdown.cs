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
        public class GetById
        {
            public const string Summary = @"Retorna por id os dados";
            public const string Description = @"Retorna por id os dados<br/><br/>
            <strong>Banco de dados</strong><br/><br/>
            - Tabela = Examples = Retorna dados da tabela de exemplos";
        }
        public class Put
        {
            public const string Summary = @"Edita dados existentes";
            public const string Description = @"Edita dados existentes<br/><br/>
            <strong>Banco de dados</strong><br/><br/>
            - Tabela = Examples = Retorna dados da tabela de exemplos";
        }

        public class Delete
        {
            public const string Summary = @"Deleta dados existentes";
            public const string Description = @"Deleta dados existentes<br/><br/>
            <strong>Banco de dados</strong><br/><br/>
            - Tabela = Examples = Retorna dados da tabela de exemplos";
        }
    }
}
