using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RestAPI.Models;

namespace RestAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Produces("application/json")]
    public class HeroAttributeController
    {
        private readonly ILogger<HeroAttributeController> _logger;
        private readonly SuperheroesContext _superheroesContext;

        public HeroAttributeController(ILogger<HeroAttributeController> logger, SuperheroesContext superheroesContext)
        {
            _logger = logger;
            _superheroesContext = superheroesContext;
        }

        [Authorize]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [HttpGet(Name = "GetHeroAttributes")]
        public async Task<ActionResult<IEnumerable<HeroAttribute>>> Get()
        {
            return await _superheroesContext.HeroAttributes.ToListAsync();
        }

        [Authorize]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [HttpGet("{hid}/{aid}", Name = "GetHeroAttributeById")]
        public async Task<ActionResult<HeroAttribute>> Get(int hid, int aid)
        {
            var heroattribute = await _superheroesContext.HeroAttributes.FirstOrDefaultAsync(h=> h.HeroId == hid && h.AttributeId == aid);

            if (heroattribute == null)
                return new NotFoundResult();

            return heroattribute;
        }

        [Authorize(Roles = "admin")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [HttpPost(Name = "InsertHeroAttribute")]
        public async Task<ActionResult<HeroAttribute>> Post(HeroAttribute heroattribute)
        {
            if (heroattribute == null)
                return new BadRequestObjectResult("Error: Object is null");
            if (heroattribute.AttributeValue < 0)
                heroattribute.AttributeValue = 0;

            _superheroesContext.HeroAttributes.Add(heroattribute);
            await _superheroesContext.SaveChangesAsync();

            return new OkObjectResult(heroattribute);
        }

        [Authorize(Roles = "admin")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [HttpPut(Name = "UpdateHeroAttribute")]
        public async Task<ActionResult<HeroAttribute>> Put(HeroAttribute heroattribute)
        {

            if (heroattribute == null)
                return new BadRequestObjectResult("Error: Object is null");

            var update_heroattribute = _superheroesContext.HeroAttributes.Any(c => c.HeroId == heroattribute.HeroId && c.AttributeId == heroattribute.AttributeId);
            if (!update_heroattribute)
                return new NotFoundResult();

            _superheroesContext.Update(heroattribute);
            await _superheroesContext.SaveChangesAsync();

            return new OkObjectResult(heroattribute);
        }

        [Authorize(Roles = "admin")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [HttpDelete("{hid}/{aid}", Name = "DeleteHeroAttribute")]
        public async Task<ActionResult<HeroAttribute>> Delete(int hid, int aid)
        {
            var heroattribute = await _superheroesContext.HeroAttributes.FirstOrDefaultAsync(h => h.HeroId == hid && h.AttributeId == aid);

            if (heroattribute == null)
                return new NotFoundResult();

            _superheroesContext.HeroAttributes.Remove(heroattribute);
            await _superheroesContext.SaveChangesAsync();

            return new OkObjectResult(heroattribute);
        }
    }
}
