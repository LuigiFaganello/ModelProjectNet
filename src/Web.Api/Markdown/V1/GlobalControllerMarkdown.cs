namespace API.Markdown.V1
{
    public class GlobalControllerMarkdown
    {
        public class Description
        {
            public const string StatusCode200 = @"Requisição bem-sucedida. Dados retornados com sucesso.";
            public const string StatusCode201 = @"Recurso criado com sucesso.";
            public const string StatusCode204 = @"Requisição bem-sucedida, porém não há dados para retorno.";
            public const string StatusCode400 = @"Requisição inválida. Verifique os dados enviados (erro de negócio).";
            public const string StatusCode401 = @"Acesso não autorizado. Autenticação necessária.";
            public const string StatusCode500 = @"Erro interno no servidor. Tente novamente mais tarde.";
        }
    }
}
