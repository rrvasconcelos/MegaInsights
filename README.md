# 🎲 Mega Insights

Web Scraper de resultados da Loteria desenvolvido com .NET 9 e práticas modernas de desenvolvimento.

## 📋 Pré-requisitos

- [.NET 9 SDK](https://dotnet.microsoft.com/download)
- SQL Server (escolha uma opção):
  - [SQL Server Local](https://www.microsoft.com/sql-server/sql-server-downloads)
  - [Docker](https://www.docker.com/products/docker-desktop/)
- [Visual Studio 2022](https://visualstudio.microsoft.com/) ou [VS Code](https://code.visualstudio.com/)

## 🚀 Configuração do Ambiente

### 1. SQL Server com Docker (Recomendado)

```bash
# Pull da imagem
docker pull mcr.microsoft.com/mssql/server:2022-latest

# Criar e executar container
docker run -e "ACCEPT_EULA=Y" -e "MSSQL_SA_PASSWORD=Sua@Senha123" \
   -p 1433:1433 --name sqlserver --hostname sqlserver \
   -d mcr.microsoft.com/mssql/server:2022-latest
```

### 2. Configuração da Connection String

```bash
# No Visual Studio 2022:
1. Clique direito no projeto MI.Scraper
2. Selecione "Manage User Secrets"
3. Adicione sua connection string:

{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost,1433;Database=loteriaDb;User ID=sa;Password=Sua@Senha123;Trusted_Connection=False; TrustServerCertificate=True;"
  }
}
```

### 3. Migrations

```bash
# No terminal, na pasta do projeto MI.Scraper:
dotnet ef database update

# Ou no Package Manager Console do Visual Studio:
Update-Database
```

## 💻 Executando o Projeto

```bash
# Clone o repositório
git clone https://github.com/seu-usuario/mega-insights.git

# Navegue até o diretório
cd mega-insights

# Restaure os pacotes
dotnet restore

# Execute o projeto
dotnet run --project src/MI.Scraper/MI.Scraper.csproj
```

## ⚙️ Configurações

O projeto usa diferentes fontes de configuração:

- `appsettings.json` - Configurações base do scraper
- User Secrets - Connection string (desenvolvimento)
- Variáveis de ambiente (produção)

### Configurações do Scraper

```json
{
  "LotteryScraper": {
    "MaxDraws": 2000,
    "WaitTimeoutSeconds": 20,
    "RetryAttempts": 3,
    "LotteryUrl": "https://loterias.caixa.gov.br/Paginas/Mega-Sena.aspx"
  }
}
```

## 🤝 Contribuindo

1. Faça um Fork do projeto
2. Crie uma Branch para sua Feature (`git checkout -b feature/AmazingFeature`)
3. Commit suas mudanças (`git commit -m 'Add some AmazingFeature'`)
4. Push para a Branch (`git push origin feature/AmazingFeature`)
5. Abra um Pull Request

## 📝 Licença

Este projeto está sob a licença MIT - veja o arquivo [LICENSE.md](LICENSE.md) para detalhes.

## 📊 Features Planejadas

- [ ] Análises estatísticas
- [ ] Dashboard para visualização
- [ ] Suporte a outros tipos de loteria
- [ ] Testes automatizados
- [ ] Pipeline CI/CD

---

<p align="center">
Feito com ❤️ por Rodrigo Vasconcelos
</p>
