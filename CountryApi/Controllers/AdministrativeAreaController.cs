using CountryApi.ApiModels;
using CountryApi.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace CountryApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AdministrativeAreaController : ControllerBase
    {
        private readonly ILogger<AdministrativeAreaController> _logger;
        private readonly AdministrativeAreasRepository repository;
        public AdministrativeAreaController(ILogger<AdministrativeAreaController> logger, AdministrativeAreasRepository repository)
        {
            _logger = logger;
            this.repository = repository;
        }

        [HttpGet("", Name = "GetAdministrativeArea")]
        public async Task<IActionResult> Get(float latitude, float longitude, CancellationToken token)
        {
            var properties = await this.repository.SearchAsync(latitude, longitude, token);
            return await Task.FromResult(Ok(new AdministrativeArea(properties.NAME_1, properties.NAME_2, properties.NAME_3, latitude, longitude)));
        }
         
    }
}
