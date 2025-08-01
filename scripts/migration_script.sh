#!/bin/bash

# Script para gerar migrations C# .NET
# Uso: ./generate-migration.sh <nome-da-migration>

# Cores para output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
NC='\033[0m' # No Color

# Função para exibir ajuda
show_help() {
    echo -e "${BLUE}=== Gerador de Migration C# .NET ===${NC}"
    echo ""
    echo "Uso: $0 <nome-da-migration> [opções]"
    echo ""
    echo "Parâmetros:"
    echo "  nome-da-migration    Nome da migration (obrigatório)"
    echo ""
    echo "Opções:"
    echo "  -p, --project       Caminho para o projeto (padrão: diretório atual)"
    echo "  -c, --context       Nome do DbContext (padrão: auto-detectar)"
    echo "  -o, --output        Diretório de saída das migrations (padrão: Migrations)"
    echo "  -s, --startup       Projeto de startup (se diferente do projeto principal)"
    echo "  -h, --help          Exibe esta ajuda"
    echo ""
    echo "Exemplos:"
    echo "  $0 AddUserTable"
    echo "  $0 UpdateProductSchema -p ./MyProject"
    echo "  $0 CreateIndexes -c ApplicationDbContext"
}

# Função para validar se dotnet está instalado
check_dotnet() {
    if ! command -v dotnet &> /dev/null; then
        echo -e "${RED}Erro: .NET CLI não encontrado. Instale o .NET SDK.${NC}"
        exit 1
    fi
}

# Função para validar se Entity Framework está instalado
check_ef_tools() {
    if ! dotnet tool list -g | grep -q "dotnet-ef"; then
        echo -e "${YELLOW}Aviso: Entity Framework Tools não encontrado globalmente.${NC}"
        echo -e "${BLUE}Instalando Entity Framework Tools...${NC}"
        dotnet tool install --global dotnet-ef
        if [ $? -ne 0 ]; then
            echo -e "${RED}Erro: Falha ao instalar Entity Framework Tools.${NC}"
            exit 1
        fi
    fi
}

# Função para encontrar arquivos .csproj
find_project_file() {
    local project_path="$1"
    local csproj_files=($(find "$project_path" -maxdepth 1 -name "*.csproj"))
    
    if [ ${#csproj_files[@]} -eq 0 ]; then
        echo -e "${RED}Erro: Nenhum arquivo .csproj encontrado em '$project_path'.${NC}"
        exit 1
    elif [ ${#csproj_files[@]} -gt 1 ]; then
        echo -e "${YELLOW}Múltiplos projetos encontrados:${NC}"
        for i in "${!csproj_files[@]}"; do
            echo "  $((i+1)). ${csproj_files[$i]}"
        done
        echo -n "Selecione o projeto (1-${#csproj_files[@]}): "
        read -r selection
        if [[ "$selection" =~ ^[0-9]+$ ]] && [ "$selection" -ge 1 ] && [ "$selection" -le ${#csproj_files[@]} ]; then
            echo "${csproj_files[$((selection-1))]}"
        else
            echo -e "${RED}Seleção inválida.${NC}"
            exit 1
        fi
    else
        echo "${csproj_files[0]}"
    fi
}

# Função para detectar DbContext
detect_dbcontext() {
    local project_path="$1"
    local contexts=($(find "$project_path" -name "*.cs" -exec grep -l "class.*DbContext\|: DbContext" {} \; | xargs grep -h "class.*DbContext\|: DbContext" | sed 's/.*class \([^ ]*\).*/\1/' | sort -u))
    
    if [ ${#contexts[@]} -eq 0 ]; then
        echo ""
    elif [ ${#contexts[@]} -eq 1 ]; then
        echo "${contexts[0]}"
    else
        echo -e "${YELLOW}Múltiplos DbContext encontrados:${NC}"
        for i in "${!contexts[@]}"; do
            echo "  $((i+1)). ${contexts[$i]}"
        done
        echo -n "Selecione o DbContext (1-${#contexts[@]}): "
        read -r selection
        if [[ "$selection" =~ ^[0-9]+$ ]] && [ "$selection" -ge 1 ] && [ "$selection" -le ${#contexts[@]} ]; then
            echo "${contexts[$((selection-1))]}"
        else
            echo -e "${RED}Seleção inválida.${NC}"
            exit 1
        fi
    fi
}

# Função principal para gerar migration
generate_migration() {
    local migration_name="$1"
    local project_path="$2"
    local context_name="$3"
    local output_dir="$4"
    local startup_project="$5"
    
    echo -e "${BLUE}=== Gerando Migration ===${NC}"
    echo "Nome: $migration_name"
    echo "Projeto: $project_path"
    
    # Construir comando
    local cmd="dotnet ef migrations add \"$migration_name\""
    
    if [ -n "$project_path" ]; then
        cmd="$cmd --project \"$project_path\""
    fi
    
    if [ -n "$context_name" ]; then
        cmd="$cmd --context \"$context_name\""
        echo "DbContext: $context_name"
    fi
    
    if [ -n "$output_dir" ]; then
        cmd="$cmd --output-dir \"$output_dir\""
        echo "Diretório: $output_dir"
    fi
    
    if [ -n "$startup_project" ]; then
        cmd="$cmd --startup-project \"$startup_project\""
        echo "Startup Project: $startup_project"
    fi
    
    echo ""
    echo -e "${YELLOW}Executando: $cmd${NC}"
    echo ""
    
    # Executar comando
    eval $cmd
    
    if [ $? -eq 0 ]; then
        echo ""
        echo -e "${GREEN}✅ Migration '$migration_name' gerada com sucesso!${NC}"
        echo ""
        echo -e "${BLUE}Próximos passos:${NC}"
        echo "1. Revisar a migration gerada"
        echo "2. Executar: dotnet ef database update"
        echo "3. Ou reverter com: dotnet ef migrations remove"
    else
        echo ""
        echo -e "${RED}❌ Erro ao gerar migration.${NC}"
        exit 1
    fi
}

# Função principal
main() {
    local migration_name=""
    local project_path="."
    local context_name=""
    local output_dir=""
    local startup_project=""
    
    # Parse de argumentos
    while [[ $# -gt 0 ]]; do
        case $1 in
            -h|--help)
                show_help
                exit 0
                ;;
            -p|--project)
                project_path="$2"
                shift 2
                ;;
            -c|--context)
                context_name="$2"
                shift 2
                ;;
            -o|--output)
                output_dir="$2"
                shift 2
                ;;
            -s|--startup)
                startup_project="$2"
                shift 2
                ;;
            -*)
                echo -e "${RED}Opção desconhecida: $1${NC}"
                show_help
                exit 1
                ;;
            *)
                if [ -z "$migration_name" ]; then
                    migration_name="$1"
                else
                    echo -e "${RED}Muitos argumentos posicionais.${NC}"
                    show_help
                    exit 1
                fi
                shift
                ;;
        esac
    done
    
    # Validações
    if [ -z "$migration_name" ]; then
        echo -e "${RED}Erro: Nome da migration é obrigatório.${NC}"
        show_help
        exit 1
    fi
    
    # Validar nome da migration
    if [[ ! "$migration_name" =~ ^[A-Za-z][A-Za-z0-9_]*$ ]]; then
        echo -e "${RED}Erro: Nome da migration deve começar com uma letra e conter apenas letras, números e underscore.${NC}"
        exit 1
    fi
    
    # Verificar se diretório do projeto existe
    if [ ! -d "$project_path" ]; then
        echo -e "${RED}Erro: Diretório do projeto '$project_path' não encontrado.${NC}"
        exit 1
    fi
    
    # Verificações de pré-requisitos
    check_dotnet
    check_ef_tools
    
    # Encontrar arquivo do projeto
    project_file=$(find_project_file "$project_path")
    echo -e "${GREEN}Projeto encontrado: $project_file${NC}"
    
    # Auto-detectar DbContext se não fornecido
    if [ -z "$context_name" ]; then
        context_name=$(detect_dbcontext "$project_path")
        if [ -n "$context_name" ]; then
            echo -e "${GREEN}DbContext detectado: $context_name${NC}"
        fi
    fi
    
    # Gerar migration
    generate_migration "$migration_name" "$project_file" "$context_name" "$output_dir" "$startup_project"
}

# Executar script
main "$@"