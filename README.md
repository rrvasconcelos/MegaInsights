# 🎲 Mega Insights

Um Web Scraper de resultados da Loteria desenvolvido com .NET 9 e práticas modernas de desenvolvimento.

## 📋 Sobre o Projeto

O Mega Insights é um Worker Service que automaticamente coleta e armazena resultados históricos da Mega-Sena, criando uma base de dados estruturada para análises futuras e insights estatísticos.

## 🛠️ Tecnologias e Práticas

### Core
- .NET 9 Worker Service
- Selenium WebDriver
- Entity Framework Core
- SQL Server

### Arquitetura e Padrões
- Clean Architecture
- Repository Pattern
- Dependency Injection
- SOLID Principles

### Resiliência e Logging
- Polly Retry Policies
- Circuit Breaker Pattern
- Structured Logging
- Resource Management (IDisposable)
- Async/Await com CancellationToken

## 🔍 Funcionalidades Principais

- ✅ Coleta automatizada de resultados
- ✅ Tratamento robusto de erros
- ✅ Persistência estruturada
- ✅ Logging detalhado
- ✅ Configurações flexíveis

## 🚀 Como Começar

### Pré-requisitos
- .NET 9 SDK
- SQL Server
- Chrome/Firefox

### Instalação
```bash
# Clone o repositório
git clone https://github.com/seu-usuario/mega-insights.git

# Navegue até o diretório
cd mega-insights

# Restaure os pacotes
dotnet restore

# Configure o banco de dados
dotnet ef database update

# Execute o projeto
dotnet run
