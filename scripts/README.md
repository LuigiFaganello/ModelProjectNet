# Scripts

## migration_script.sh
Executar:

```csharp
./generate-migration.sh AddUserTable
./generate-migration.sh UpdateProductSchema -p ./MyProject
./generate-migration.sh CreateIndexes -c ApplicationDbContext
```

### Parâmetros disponíveis

- Nome da migration (obrigatório)
- -p, --project: Caminho do projet
- -c, --context: Nome do DbContext específico
- -o, --output: Diretório das migrations
- -s, --startup: Projeto de startup
- -h, --help: Ajuda