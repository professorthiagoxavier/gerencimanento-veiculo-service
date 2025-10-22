# 🚀 Service Layer - Cache Redis

## 📋 Visão Geral

O projeto **Service** é responsável por toda a lógica de cache Redis da aplicação. Esta camada implementa o padrão **Service Layer** e encapsula todas as operações relacionadas ao cache, proporcionando uma interface limpa e reutilizável para as outras camadas da aplicação.

## 🏗️ Arquitetura

```
📁 Service/
├── 📄 ICacheService.cs      # Interface do serviço de cache
├── 📄 CacheService.cs       # Implementação do serviço de cache
├── 📄 Service.csproj        # Dependências do projeto
└── 📄 README.md            # Esta documentação
```

## 🎯 Responsabilidades

### ✅ O que o Service Layer faz:
- **Gerenciamento de Conexão**: Estabelece e mantém conexão com Redis
- **Operações de Cache**: Get, Set, Delete, Expiry
- **Tratamento de Erros**: Logs detalhados e fallbacks
- **Configuração**: Leitura de connection strings
- **Abstração**: Interface limpa para outras camadas

### ❌ O que o Service Layer NÃO faz:
- Lógica de negócio específica
- Validação de dados
- Transformação de objetos
- Persistência em banco de dados

## 🔧 Tecnologias

- **StackExchange.Redis** - Cliente Redis para .NET
- **Microsoft.Extensions.Configuration** - Configuração
- **Microsoft.Extensions.Logging** - Logging estruturado

## 📦 Dependências

```xml
<PackageReference Include="StackExchange.Redis" Version="2.8.16" />
<PackageReference Include="Microsoft.Extensions.Configuration.Abstractions" Version="8.0.0" />
<PackageReference Include="Microsoft.Extensions.Logging.Abstractions" Version="8.0.0" />
```

## 🔌 Interface ICacheService

```csharp
public interface ICacheService
{
    Task<string?> GetAsync(string key);
    Task SetAsync(string key, string value, TimeSpan? expiry = null);
    Task DeleteAsync(string key);
    Task<bool> KeyExistsAsync(string key);
    Task<bool> SetExpiryAsync(string key, TimeSpan expiry);
}
```

### Métodos Disponíveis

#### `GetAsync(string key)`
- **Descrição**: Busca um valor no cache
- **Retorno**: `string?` - Valor encontrado ou null
- **Uso**: `var value = await cacheService.GetAsync("minha-chave");`

#### `SetAsync(string key, string value, TimeSpan? expiry = null)`
- **Descrição**: Salva um valor no cache
- **Parâmetros**:
  - `key`: Chave do cache
  - `value`: Valor a ser salvo
  - `expiry`: Tempo de expiração (opcional)
- **Uso**: `await cacheService.SetAsync("chave", "valor", TimeSpan.FromMinutes(10));`

#### `DeleteAsync(string key)`
- **Descrição**: Remove uma chave do cache
- **Uso**: `await cacheService.DeleteAsync("chave-para-remover");`

#### `KeyExistsAsync(string key)`
- **Descrição**: Verifica se uma chave existe no cache
- **Retorno**: `bool` - True se existe, False caso contrário
- **Uso**: `var exists = await cacheService.KeyExistsAsync("minha-chave");`

#### `SetExpiryAsync(string key, TimeSpan expiry)`
- **Descrição**: Define tempo de expiração para uma chave
- **Uso**: `await cacheService.SetExpiryAsync("chave", TimeSpan.FromHours(1));`

## 🏭 Implementação CacheService

### Construtor
```csharp
public CacheService(IConfiguration configuration, ILogger<CacheService> logger)
{
    _logger = logger;
    
    var redisConnectionString = configuration.GetConnectionString("RedisConnection") 
                              ?? "localhost:6379";
    
    try
    {
        _redis = ConnectionMultiplexer.Connect(redisConnectionString);
        _database = _redis.GetDatabase();
        _logger.LogInformation("Conexão com Redis estabelecida com sucesso");
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Erro ao conectar com Redis: {ConnectionString}", redisConnectionString);
        throw;
    }
}
```

### Características da Implementação

#### 🔒 **Thread-Safe**
- Usa `IConnectionMultiplexer` que é thread-safe
- Singleton pattern para reutilização de conexão

#### 📊 **Logging Estruturado**
- Logs de informação para operações bem-sucedidas
- Logs de erro para falhas
- Logs de debug para operações detalhadas

#### 🛡️ **Tratamento de Erros**
- Try-catch em todas as operações
- Fallback gracioso (retorna null em caso de erro)
- Logs detalhados para debugging

#### ⚡ **Performance**
- Conexão reutilizada (Singleton)
- Operações assíncronas
- Timeout configurável

## 🔧 Configuração

### appsettings.json
```json
{
  "ConnectionStrings": {
    "RedisConnection": "localhost:6379"
  }
}
```

### Registro no DI Container
```csharp
// Program.cs
builder.Services.AddSingleton<ICacheService, CacheService>();
```

## 📊 Exemplos de Uso

### 1. Cache Simples
```csharp
public class VehicleService
{
    private readonly ICacheService _cacheService;
    
    public async Task<Vehicle?> GetVehicleAsync(int id)
    {
        var cacheKey = $"vehicle:{id}";
        
        // Tentar buscar do cache
        var cachedVehicle = await _cacheService.GetAsync(cacheKey);
        if (!string.IsNullOrEmpty(cachedVehicle))
        {
            return JsonConvert.DeserializeObject<Vehicle>(cachedVehicle);
        }
        
        // Buscar do banco e salvar no cache
        var vehicle = await _repository.GetByIdAsync(id);
        if (vehicle != null)
        {
            var vehicleJson = JsonConvert.SerializeObject(vehicle);
            await _cacheService.SetAsync(cacheKey, vehicleJson, TimeSpan.FromMinutes(30));
        }
        
        return vehicle;
    }
}
```

### 2. Cache com Expiração
```csharp
public async Task CacheUserSessionAsync(string userId, UserSession session)
{
    var cacheKey = $"session:{userId}";
    var sessionJson = JsonConvert.SerializeObject(session);
    
    // Cache por 1 hora
    await _cacheService.SetAsync(cacheKey, sessionJson, TimeSpan.FromHours(1));
}
```

### 3. Invalidação de Cache
```csharp
public async Task UpdateVehicleAsync(Vehicle vehicle)
{
    // Atualizar no banco
    await _repository.UpdateAsync(vehicle);
    
    // Invalidar cache
    var cacheKey = $"vehicle:{vehicle.Id}";
    await _cacheService.DeleteAsync(cacheKey);
    
    // Invalidar cache da lista também
    await _cacheService.DeleteAsync("vehicles-list");
}
```

### 4. Verificação de Existência
```csharp
public async Task<bool> IsUserOnlineAsync(string userId)
{
    var cacheKey = $"user-online:{userId}";
    return await _cacheService.KeyExistsAsync(cacheKey);
}
```

## 🧪 Testes

### Teste Unitário Simples
```csharp
[Test]
public async Task GetAsync_ShouldReturnValue_WhenKeyExists()
{
    // Arrange
    var mockDatabase = new Mock<IDatabase>();
    mockDatabase.Setup(db => db.StringGetAsync(It.IsAny<RedisKey>(), It.IsAny<CommandFlags>()))
                .ReturnsAsync("test-value");
    
    var cacheService = new CacheService(mockDatabase.Object);
    
    // Act
    var result = await cacheService.GetAsync("test-key");
    
    // Assert
    Assert.AreEqual("test-value", result);
}
```

### Teste de Integração
```csharp
[Test]
public async Task CacheService_ShouldWorkWithRealRedis()
{
    // Arrange
    var configuration = new ConfigurationBuilder()
        .AddInMemoryCollection(new Dictionary<string, string>
        {
            {"ConnectionStrings:RedisConnection", "localhost:6379"}
        })
        .Build();
    
    var logger = new Mock<ILogger<CacheService>>();
    var cacheService = new CacheService(configuration, logger.Object);
    
    // Act
    await cacheService.SetAsync("test-key", "test-value", TimeSpan.FromMinutes(1));
    var result = await cacheService.GetAsync("test-key");
    
    // Assert
    Assert.AreEqual("test-value", result);
    
    // Cleanup
    await cacheService.DeleteAsync("test-key");
}
```

## 🔍 Monitoramento e Debugging

### Logs Importantes
```csharp
// Conexão estabelecida
_logger.LogInformation("Conexão com Redis estabelecida com sucesso");

// Erro de conexão
_logger.LogError(ex, "Erro ao conectar com Redis: {ConnectionString}", redisConnectionString);

// Operação bem-sucedida
_logger.LogDebug("Chave {Key} salva no Redis com sucesso", key);

// Erro em operação
_logger.LogError(ex, "Erro ao buscar chave {Key} no Redis", key);
```

### Comandos Redis para Debugging
```bash
# Conectar ao Redis
redis-cli

# Listar todas as chaves
KEYS *

# Ver valor de uma chave
GET vehicles-cache

# Ver TTL de uma chave
TTL vehicles-cache

# Monitorar comandos em tempo real
MONITOR

# Ver informações do servidor
INFO memory
```

## ⚡ Performance e Otimização

### Boas Práticas

#### 1. **Reutilização de Conexão**
```csharp
// ✅ Bom - Singleton
builder.Services.AddSingleton<ICacheService, CacheService>();

// ❌ Ruim - Scoped (cria nova conexão a cada request)
builder.Services.AddScoped<ICacheService, CacheService>();
```

#### 2. **Serialização Eficiente**
```csharp
// ✅ Bom - JSON compacto
var json = JsonConvert.SerializeObject(data, Formatting.None);

// ❌ Ruim - JSON formatado (maior tamanho)
var json = JsonConvert.SerializeObject(data, Formatting.Indented);
```

#### 3. **TTL Adequado**
```csharp
// ✅ Bom - TTL baseado no tipo de dados
await _cacheService.SetAsync("user-session", data, TimeSpan.FromHours(1));
await _cacheService.SetAsync("static-data", data, TimeSpan.FromDays(1));

// ❌ Ruim - TTL muito curto ou muito longo
await _cacheService.SetAsync("data", data, TimeSpan.FromSeconds(1)); // Muito curto
await _cacheService.SetAsync("data", data, TimeSpan.FromDays(365)); // Muito longo
```

#### 4. **Chaves Descritivas**
```csharp
// ✅ Bom - Chaves claras e organizadas
var cacheKey = $"vehicle:{vehicleId}";
var cacheKey = $"user:{userId}:profile";
var cacheKey = $"vehicles:list:page:{pageNumber}";

// ❌ Ruim - Chaves genéricas
var cacheKey = "data";
var cacheKey = "temp";
```

## 🚨 Troubleshooting

### Problemas Comuns

#### 1. **Redis não conecta**
```bash
# Verificar se Redis está rodando
docker ps | grep redis

# Testar conexão
redis-cli ping
# Deve retornar: PONG
```

#### 2. **Timeout de conexão**
```csharp
// Configurar timeout no connection string
"localhost:6379,connectTimeout=5000,syncTimeout=5000"
```

#### 3. **Memória Redis cheia**
```bash
# Verificar uso de memória
redis-cli INFO memory

# Limpar cache (cuidado!)
redis-cli FLUSHALL
```

#### 4. **Dados não persistem**
```csharp
// Verificar se está usando Redis persistente
// Ou se está usando Redis em memória apenas
```

## 🔮 Extensões Futuras

### Funcionalidades Planejadas

#### 1. **Cache Distribuído**
```csharp
public interface IDistributedCacheService : ICacheService
{
    Task<IEnumerable<string>> GetKeysAsync(string pattern);
    Task<long> GetMemoryUsageAsync();
    Task<bool> IsConnectedAsync();
}
```

#### 2. **Compressão**
```csharp
public interface ICompressedCacheService : ICacheService
{
    Task<T?> GetObjectAsync<T>(string key);
    Task SetObjectAsync<T>(string key, T value, TimeSpan? expiry = null);
}
```

#### 3. **Métricas**
```csharp
public interface ICacheMetrics
{
    int CacheHits { get; }
    int CacheMisses { get; }
    double HitRatio { get; }
    TimeSpan AverageResponseTime { get; }
}
```

## 📚 Padrões de Design Utilizados

### 1. **Service Layer Pattern**
- Encapsula lógica de negócio
- Interface bem definida
- Fácil de testar

### 2. **Dependency Injection**
- Desacoplamento de dependências
- Facilita testes
- Configuração centralizada

### 3. **Singleton Pattern**
- Reutilização de conexão Redis
- Economia de recursos
- Thread-safe

### 4. **Repository Pattern** (para cache)
- Abstração de acesso a dados
- Interface consistente
- Fácil substituição

## 🎯 Conclusão

O **Service Layer** é uma peça fundamental da arquitetura, proporcionando:

- ✅ **Abstração** limpa para operações de cache
- ✅ **Reutilização** em diferentes camadas
- ✅ **Testabilidade** através de interfaces
- ✅ **Manutenibilidade** com código organizado
- ✅ **Performance** com conexões otimizadas

Esta implementação segue as melhores práticas de desenvolvimento e pode ser facilmente estendida para atender necessidades futuras.

---

*Desenvolvido para demonstrar boas práticas de arquitetura em .NET* 🚀

