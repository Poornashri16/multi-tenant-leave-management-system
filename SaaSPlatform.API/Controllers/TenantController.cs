using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SaaSPlatform.API.Services;
using SaaSPlatform.Persistence.Context;

namespace SaaSPlatform.API.Controllers
{
    [Authorize] // Only authenticated users can access
    [ApiController]
    [Route("api/tenants")]
    public class TenantController : ControllerBase
    {
        private readonly TenantService _tenantService;

        public TenantController(TenantService tenantService)
        {
            _tenantService = tenantService;
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            var tenants = _tenantService.GetAll();
            return Ok(tenants);
        }

        [HttpGet("{id}")]
        public IActionResult GetById(Guid id)
        {
            var tenant = _tenantService.GetById(id);
            if (tenant == null) return NotFound();
            return Ok(tenant);
        }

        // Only Admin users can delete tenants
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(Guid id)
        {
            await  _tenantService.Delete(id);
            return NoContent();
        }
    }
}