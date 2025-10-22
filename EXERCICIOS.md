# 🎯 Exercícios Práticos - Sistema de Gerenciamento de Veículos

## 📚 Objetivo dos Exercícios

Este documento contém exercícios práticos para consolidar os conceitos de **Arquitetura em Camadas**, **Injeção de Dependência**, **Repository Pattern**, **Cache Redis** e **Tratamento de Erros** aprendidos no projeto principal.

---

## 🏗️ Exercício 1: Implementar Sistema de Categorias de Veículos

### **Contexto**
Você precisa expandir o sistema para incluir categorias de veículos (SUV, Sedan, Hatchback, etc.).

### **Tarefas**

#### **1.1 - Criar Entidade Category (Domain Layer)**
```csharp
// TODO: Criar a classe Category com as seguintes propriedades:
// - Id (int)
// - Name (string) - Nome da categoria
// - Description (string) - Descrição da categoria
// - IsActive (bool) - Se a categoria está ativa
```

#### **1.2 - Atualizar Entidade Vehicle**
```csharp
// TODO: Adicionar propriedade CategoryId na classe Vehicle
// TODO: Adicionar propriedade de navegação Category
```

#### **1.3 - Criar Interface ICategoryRepository**
```csharp
// TODO: Criar interface com métodos:
// - GetAllCategoriesAsync()
// - GetCategoryByIdAsync(int id)
// - AddCategoryAsync(Category category)
// - UpdateCategoryAsync(Category category)
// - DeleteCategoryAsync(int id)
```

#### **1.4 - Implementar CategoryRepository**
```csharp
// TODO: Implementar todos os métodos da interface
// TODO: Usar Dapper para operações no banco
// TODO: Incluir tratamento de exceções
```

#### **1.5 - Configurar Injeção de Dependência**
```csharp
// TODO: Adicionar ICategoryRepository no Program.cs
// TODO: Configurar como Scoped
```

#### **1.6 - Criar CategoryController**
```csharp
// TODO: Implementar CRUD completo
// TODO: Incluir cache Redis
// TODO: Implementar tratamento de erros
// TODO: Adicionar logging
```

### **Desafio Extra**
- Implementar validação para não permitir exclusão de categoria que possui veículos
- Adicionar endpoint para listar veículos por categoria

---

## 🔄 Exercício 2: Implementar Sistema de Auditoria

### **Contexto**
Implementar um sistema que registre todas as operações realizadas no sistema (CREATE, UPDATE, DELETE).

### **Tarefas**

#### **2.1 - Criar Entidade AuditLog**
```csharp
// TODO: Criar classe AuditLog com:
// - Id (int)
// - EntityName (string) - Nome da entidade (Vehicle, Category)
// - EntityId (int) - ID da entidade modificada
// - Action (string) - CREATE, UPDATE, DELETE
// - OldValues (string) - JSON com valores antigos
// - NewValues (string) - JSON com valores novos
// - UserId (string) - ID do usuário (pode ser "System" por enquanto)
// - Timestamp (DateTime) - Data/hora da operação
```

#### **2.2 - Criar Interface IAuditRepository**
```csharp
// TODO: Criar interface com métodos:
// - LogActionAsync(AuditLog auditLog)
// - GetAuditLogsByEntityAsync(string entityName, int entityId)
// - GetAuditLogsByDateRangeAsync(DateTime startDate, DateTime endDate)
```

#### **2.3 - Implementar AuditRepository**
```csharp
// TODO: Implementar métodos da interface
// TODO: Usar Dapper para inserir logs
// TODO: Implementar consultas com filtros
```

#### **2.4 - Criar Serviço de Auditoria**
```csharp
// TODO: Criar interface IAuditService
// TODO: Implementar AuditService
// TODO: Método para logar operações automaticamente
```

#### **2.5 - Integrar Auditoria nos Controllers**
```csharp
// TODO: Modificar VehicleController para registrar auditoria
// TODO: Usar injeção de dependência para IAuditService
// TODO: Registrar operações CREATE, UPDATE, DELETE
```

### **Desafio Extra**
- Implementar middleware para capturar automaticamente todas as operações
- Criar endpoint para visualizar histórico de alterações de um veículo

---

## 🚀 Exercício 3: Implementar Sistema de Cache Avançado

### **Contexto**
Melhorar o sistema de cache implementando estratégias mais sofisticadas.

### **Tarefas**

#### **3.1 - Criar Interface ICacheService**
```csharp
// TODO: Criar interface genérica para cache:
// - GetAsync<T>(string key)
// - SetAsync<T>(string key, T value, TimeSpan? expiry = null)
// - RemoveAsync(string key)
// - RemoveByPatternAsync(string pattern)
// - ExistsAsync(string key)
```

#### **3.2 - Implementar RedisCacheService**
```csharp
// TODO: Implementar interface usando Redis
// TODO: Serialização/deserialização JSON
// TODO: Tratamento de exceções
// TODO: Logging de operações de cache
```

#### **3.3 - Implementar Cache Decorator para Repository**
```csharp
// TODO: Criar CachedVehicleRepository que implementa IVehicleRepository
// TODO: Usar decorator pattern
// TODO: Cache para GetAllVehiclesAsync
// TODO: Cache para GetVehicleByIdAsync
// TODO: Invalidar cache em operações de escrita
```

#### **3.4 - Configurar Cache no Program.cs**
```csharp
// TODO: Registrar ICacheService
// TODO: Configurar CachedVehicleRepository
// TODO: Manter VehicleRepository original
```

#### **3.5 - Implementar Cache Warming**
```csharp
// TODO: Criar serviço para popular cache na inicialização
// TODO: Implementar background service
// TODO: Recarregar cache periodicamente
```

### **Desafio Extra**
- Implementar cache distribuído com Redis Cluster
- Adicionar métricas de hit/miss do cache

---

## 🔒 Exercício 4: Implementar Sistema de Validação

### **Contexto**
Implementar validações robustas usando FluentValidation.

### **Tarefas**

#### **4.1 - Instalar FluentValidation**
```bash
# TODO: Instalar pacotes:
# dotnet add package FluentValidation
# dotnet add package FluentValidation.AspNetCore
```

#### **4.2 - Criar Validators**
```csharp
// TODO: Criar VehicleValidator:
// - Brand: obrigatório, máximo 100 caracteres
// - Model: obrigatório, máximo 100 caracteres
// - Year: obrigatório, entre 1900 e ano atual + 1
// - Plate: obrigatório, formato brasileiro (ABC-1234 ou ABC1D23)
// - Color: obrigatório, máximo 50 caracteres

// TODO: Criar CategoryValidator:
// - Name: obrigatório, máximo 100 caracteres, único
// - Description: máximo 500 caracteres
// - IsActive: obrigatório
```

#### **4.3 - Configurar Validação no Program.cs**
```csharp
// TODO: Adicionar FluentValidation
// TODO: Configurar validators
// TODO: Configurar resposta de erro personalizada
```

#### **4.4 - Implementar Validação Customizada**
```csharp
// TODO: Validar se placa já existe no banco
// TODO: Validar se categoria existe e está ativa
// TODO: Criar validação assíncrona
```

#### **4.5 - Melhorar Respostas de Erro**
```csharp
// TODO: Criar classe ErrorResponse
// TODO: Mapear erros de validação para resposta padronizada
// TODO: Incluir detalhes dos erros
```

### **Desafio Extra**
- Implementar validação de regras de negócio complexas
- Criar validação condicional baseada em outros campos

---

## 📊 Exercício 5: Implementar Sistema de Métricas e Health Checks

### **Contexto**
Implementar monitoramento e health checks para o sistema.

### **Tarefas**

#### **5.1 - Instalar Pacotes de Monitoramento**
```bash
# TODO: Instalar pacotes:
# dotnet add package Microsoft.Extensions.Diagnostics.HealthChecks
# dotnet add package Microsoft.Extensions.Diagnostics.HealthChecks.EntityFrameworkCore
# dotnet add package AspNetCore.HealthChecks.Redis
# dotnet add package AspNetCore.HealthChecks.MySql
```

#### **5.2 - Implementar Health Checks**
```csharp
// TODO: Criar health check para MySQL
// TODO: Criar health check para Redis
// TODO: Criar health check customizado para verificar conectividade
```

#### **5.3 - Configurar Health Checks no Program.cs**
```csharp
// TODO: Adicionar health checks
// TODO: Configurar endpoint /health
// TODO: Configurar UI para health checks
```

#### **5.4 - Implementar Métricas Customizadas**
```csharp
// TODO: Criar contadores para operações CRUD
// TODO: Implementar métricas de performance
// TODO: Adicionar métricas de cache hit/miss
```

#### **5.5 - Criar Dashboard de Monitoramento**
```csharp
// TODO: Criar endpoint /metrics
// TODO: Implementar visualização de métricas
// TODO: Adicionar alertas para problemas
```

### **Desafio Extra**
- Integrar com Application Insights
- Implementar alertas automáticos

---

## 🧪 Exercício 6: Implementar Testes Unitários

### **Contexto**
Implementar testes unitários para validar a funcionalidade do sistema.

### **Tarefas**

#### **6.1 - Criar Projeto de Testes**
```bash
# TODO: Criar projeto de testes:
# dotnet new xunit -n GerenciamentoVeiculos.Tests
# dotnet add reference ../Domain/Domain.csproj
# dotnet add reference ../Repository/Repository.csproj
# dotnet add reference ../performance-cache/performance-cache.csproj
```

#### **6.2 - Instalar Pacotes de Teste**
```bash
# TODO: Instalar pacotes:
# dotnet add package Microsoft.AspNetCore.Mvc.Testing
# dotnet add package Moq
# dotnet add package FluentAssertions
# dotnet add package Microsoft.EntityFrameworkCore.InMemory
```

#### **6.3 - Implementar Testes para Repository**
```csharp
// TODO: Criar testes para VehicleRepository
// TODO: Testar todos os métodos CRUD
// TODO: Usar banco em memória para testes
// TODO: Testar cenários de erro
```

#### **6.4 - Implementar Testes para Controller**
```csharp
// TODO: Criar testes para VehicleController
// TODO: Mockar dependências
// TODO: Testar todos os endpoints
// TODO: Validar códigos de status HTTP
```

#### **6.5 - Implementar Testes de Integração**
```csharp
// TODO: Criar testes de integração
// TODO: Testar fluxo completo da API
// TODO: Validar cache Redis
// TODO: Testar tratamento de erros
```

### **Desafio Extra**
- Implementar testes de performance
- Criar testes de carga

---

## 🎯 Exercício 7: Implementar Sistema de Paginação

### **Contexto**
Implementar paginação para melhorar performance em listas grandes.

### **Tarefas**

#### **7.1 - Criar Classes de Paginação**
```csharp
// TODO: Criar PagedResult<T>:
// - Data (IEnumerable<T>)
// - TotalCount (int)
// - PageNumber (int)
// - PageSize (int)
// - TotalPages (int)
// - HasPrevious (bool)
// - HasNext (bool)

// TODO: Criar PagedRequest:
// - PageNumber (int)
// - PageSize (int)
// - SearchTerm (string)
// - SortBy (string)
// - SortDirection (string)
```

#### **7.2 - Atualizar Repository**
```csharp
// TODO: Adicionar método GetPagedVehiclesAsync(PagedRequest request)
// TODO: Implementar paginação no SQL
// TODO: Implementar busca por termo
// TODO: Implementar ordenação
```

#### **7.3 - Atualizar Controller**
```csharp
// TODO: Modificar GET para aceitar parâmetros de paginação
// TODO: Retornar PagedResult<Vehicle>
// TODO: Implementar cache para páginas
```

#### **7.4 - Implementar Cache de Paginação**
```csharp
// TODO: Cache por página
// TODO: Invalidação inteligente
// TODO: Otimização de queries
```

### **Desafio Extra**
- Implementar cursor-based pagination
- Adicionar filtros avançados

---

## 📝 Exercício 8: Implementar Sistema de Logs Estruturados

### **Contexto**
Implementar sistema de logs mais robusto com Serilog.

### **Tarefas**

#### **8.1 - Instalar Serilog**
```bash
# TODO: Instalar pacotes:
# dotnet add package Serilog
# dotnet add package Serilog.AspNetCore
# dotnet add package Serilog.Sinks.Console
# dotnet add package Serilog.Sinks.File
# dotnet add package Serilog.Sinks.Seq
```

#### **8.2 - Configurar Serilog**
```csharp
// TODO: Configurar Serilog no Program.cs
// TODO: Configurar sinks (Console, File, Seq)
// TODO: Configurar enrichers
// TODO: Configurar structured logging
```

#### **8.3 - Implementar Logging Customizado**
```csharp
// TODO: Criar interface ILoggerService
// TODO: Implementar LoggerService
// TODO: Adicionar contexto de request
// TODO: Implementar correlation ID
```

#### **8.4 - Melhorar Logs nos Controllers**
```csharp
// TODO: Adicionar logs estruturados
// TODO: Incluir métricas de performance
// TODO: Log de operações de negócio
```

### **Desafio Extra**
- Implementar log aggregation
- Adicionar alertas baseados em logs

---

## 🏆 Exercício Final: Projeto Completo

### **Contexto**
Implementar um sistema completo de gerenciamento de frota de veículos.

### **Requisitos**

#### **Funcionalidades Obrigatórias**
- ✅ CRUD completo de veículos
- ✅ Sistema de categorias
- ✅ Sistema de auditoria
- ✅ Cache Redis
- ✅ Validações robustas
- ✅ Paginação
- ✅ Logs estruturados
- ✅ Health checks
- ✅ Testes unitários

#### **Funcionalidades Extras**
- 🔥 Sistema de usuários e autenticação
- 🔥 Relatórios de veículos
- 🔥 Sistema de notificações
- 🔥 API versioning
- 🔥 Rate limiting
- 🔥 Documentação com Swagger

### **Critérios de Avaliação**
1. **Arquitetura**: Uso correto dos padrões
2. **Código**: Qualidade e organização
3. **Testes**: Cobertura de testes
4. **Performance**: Uso eficiente de cache
5. **Documentação**: README completo
6. **Deploy**: Configuração para produção

---

## 📚 Recursos de Aprendizado

### **Documentação Oficial**
- [.NET 8 Documentation](https://docs.microsoft.com/en-us/dotnet/)
- [ASP.NET Core](https://docs.microsoft.com/en-us/aspnet/core/)
- [Redis Documentation](https://redis.io/documentation)
- [MySQL Documentation](https://dev.mysql.com/doc/)

### **Padrões e Boas Práticas**
- [Repository Pattern](https://docs.microsoft.com/en-us/dotnet/architecture/microservices/microservice-ddd-cqrs-patterns/infrastructure-persistence-layer-design)
- [Dependency Injection](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/dependency-injection)
- [Clean Architecture](https://blog.cleancoder.com/uncle-bob/2012/08/13/the-clean-architecture.html)

### **Ferramentas**
- [Dapper](https://dapper-tutorial.net/)
- [FluentValidation](https://docs.fluentvalidation.net/)
- [Serilog](https://serilog.net/)
- [xUnit](https://xunit.net/)

---

## 🎯 Dicas para Resolução

### **1. Comece Simples**
- Implemente uma funcionalidade por vez
- Teste cada etapa antes de prosseguir
- Use o projeto principal como referência

### **2. Siga os Padrões**
- Mantenha a separação de camadas
- Use injeção de dependência
- Implemente tratamento de erros

### **3. Teste Constantemente**
- Execute testes após cada mudança
- Valide a funcionalidade manualmente
- Use o Swagger para testar a API

### **4. Documente o Processo**
- Comente o código
- Atualize o README
- Registre decisões arquiteturais

---

**Boa sorte com os exercícios! 🚀**

*Lembre-se: a prática leva à perfeição. Use estes exercícios para consolidar seu conhecimento e se preparar para projetos mais complexos.*
