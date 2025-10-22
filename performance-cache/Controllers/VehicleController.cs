using Domain;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Repository;
using Service;
using System.Net;

namespace performance_cache.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VehicleController : ControllerBase
    {
        private const string cacheKey = "vehicles-cache";
        private readonly IVehicleRepository vehicleRepository;
        private readonly ICacheService cacheService;
        private readonly ILogger<VehicleController> logger;

        public VehicleController(IVehicleRepository vehicleRepository, ICacheService cacheService, ILogger<VehicleController> logger)
        {
            this.vehicleRepository = vehicleRepository;
            this.cacheService = cacheService;
            this.logger = logger;
        }

        // GET: api/vehicle
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            try
            {
                logger.LogInformation("Iniciando busca de veículos");

                // Tentar buscar do cache Redis
                try
                {
                    await cacheService.SetExpiryAsync(cacheKey, TimeSpan.FromMinutes(20));
                    string? cachedVehicles = await cacheService.GetAsync(cacheKey);
                    
                    if (!string.IsNullOrEmpty(cachedVehicles))
                    {
                        logger.LogInformation("Veículos encontrados no cache Redis");
                        return Ok(cachedVehicles);
                    }
                }
                catch (Exception redisEx)
                {
                    logger.LogWarning(redisEx, "Erro ao acessar cache Redis, continuando sem cache");
                }

                // Buscar do banco de dados
                var vehicleList = await vehicleRepository.GetAllVehiclesAsync();

                if (vehicleList == null || !vehicleList.Any())
                {
                    logger.LogInformation("Nenhum veículo encontrado no banco de dados");
                    return Ok(new List<Vehicle>());
                }

                // Tentar salvar no cache
                try
                {
                    var vehicleListJson = JsonConvert.SerializeObject(vehicleList);
                    await cacheService.SetAsync(cacheKey, vehicleListJson, TimeSpan.FromMinutes(20));
                    logger.LogInformation("Dados salvos no cache Redis");
                }
                catch (Exception cacheEx)
                {
                    logger.LogWarning(cacheEx, "Erro ao salvar no cache Redis, mas dados foram retornados");
                }

                logger.LogInformation("Retornando {Count} veículos", vehicleList.Count());
                return Ok(vehicleList);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Erro interno ao buscar veículos");
                return StatusCode((int)HttpStatusCode.InternalServerError,
                    new { message = "Erro interno do servidor ao buscar veículos", timestamp = DateTime.UtcNow });
            }
        }

        // POST: api/vehicle
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Vehicle vehicle)
        {
            try
            {
                if (vehicle == null)
                {
                    logger.LogWarning("Tentativa de criar veículo com dados nulos");
                    return BadRequest(new { message = "Dados do veículo são obrigatórios", timestamp = DateTime.UtcNow });
                }

                // Validação básica dos campos obrigatórios
                if (string.IsNullOrWhiteSpace(vehicle.Brand) ||
                    string.IsNullOrWhiteSpace(vehicle.Model) ||
                    string.IsNullOrWhiteSpace(vehicle.Plate))
                {
                    logger.LogWarning("Tentativa de criar veículo com campos obrigatórios vazios");
                    return BadRequest(new
                    {
                        message = "Marca, modelo e placa são campos obrigatórios",
                        timestamp = DateTime.UtcNow
                    });
                }

                logger.LogInformation("Criando novo veículo: {Brand} {Model} - {Plate}", vehicle.Brand, vehicle.Model, vehicle.Plate);

                var newVehicle = await vehicleRepository.AddVehicleAsync(vehicle);

                if (newVehicle == null)
                {
                    logger.LogError("Falha ao criar veículo - repository retornou null");
                    return StatusCode((int)HttpStatusCode.InternalServerError,
                        new { message = "Erro interno ao criar veículo", timestamp = DateTime.UtcNow });
                }

                await InvalidateCache();
                logger.LogInformation("Veículo criado com sucesso - ID: {Id}", newVehicle);

                return CreatedAtAction(nameof(Get), new { id = newVehicle }, newVehicle);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Erro interno ao criar veículo");
                return StatusCode((int)HttpStatusCode.InternalServerError,
                    new { message = "Erro interno do servidor ao criar veículo", timestamp = DateTime.UtcNow });
            }
        }

        // PUT: api/vehicle/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] Vehicle vehicle)
        {
            try
            {
                if (id <= 0)
                {
                    logger.LogWarning("Tentativa de atualizar veículo com ID inválido: {Id}", id);
                    return BadRequest(new { message = "ID do veículo deve ser maior que zero", timestamp = DateTime.UtcNow });
                }

                if (vehicle == null)
                {
                    logger.LogWarning("Tentativa de atualizar veículo com dados nulos para ID: {Id}", id);
                    return BadRequest(new { message = "Dados do veículo são obrigatórios", timestamp = DateTime.UtcNow });
                }

                // Validação básica dos campos obrigatórios
                if (string.IsNullOrWhiteSpace(vehicle.Brand) ||
                    string.IsNullOrWhiteSpace(vehicle.Model) ||
                    string.IsNullOrWhiteSpace(vehicle.Plate))
                {
                    logger.LogWarning("Tentativa de atualizar veículo com campos obrigatórios vazios para ID: {Id}", id);
                    return BadRequest(new
                    {
                        message = "Marca, modelo e placa são campos obrigatórios",
                        timestamp = DateTime.UtcNow
                    });
                }

                vehicle.Id = id;
                logger.LogInformation("Atualizando veículo ID: {Id} - {Brand} {Model} - {Plate}", id, vehicle.Brand, vehicle.Model, vehicle.Plate);

                await vehicleRepository.UpdateVehicleAsync(vehicle);

                await InvalidateCache();
                logger.LogInformation("Veículo atualizado com sucesso - ID: {Id}", id);
                return NoContent();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Erro interno ao atualizar veículo ID: {Id}", id);
                return StatusCode((int)HttpStatusCode.InternalServerError,
                    new { message = "Erro interno do servidor ao atualizar veículo", timestamp = DateTime.UtcNow });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                if (id <= 0)
                {
                    logger.LogWarning("Tentativa de excluir veículo com ID inválido: {Id}", id);
                    return BadRequest(new { message = "ID do veículo deve ser maior que zero", timestamp = DateTime.UtcNow });
                }

                logger.LogInformation("Excluindo veículo ID: {Id}", id);

                await vehicleRepository.DeleteVehicleAsync(id);

                await InvalidateCache();
                logger.LogInformation("Veículo excluído com sucesso - ID: {Id}", id);
                return NoContent();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Erro interno ao excluir veículo ID: {Id}", id);
                return StatusCode((int)HttpStatusCode.InternalServerError,
                    new { message = "Erro interno do servidor ao excluir veículo", timestamp = DateTime.UtcNow });
            }
        }

        private async Task InvalidateCache()
        {
            try
            {
                await cacheService.DeleteAsync(cacheKey);
                logger.LogInformation("Cache invalidado com sucesso");
            }
            catch (Exception ex)
            {
                logger.LogWarning(ex, "Erro ao invalidar cache Redis, mas operação continuará");
            }
        }
    }
}
