using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RestAPI.Models;

namespace RestAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Produces("application/json")]
    public class SuperpowerController
    {
        private readonly ILogger<SuperpowerController> _logger;
        private readonly SuperheroesContext _superheroesContext;

        public SuperpowerController(ILogger<SuperpowerController> logger, SuperheroesContext superheroesContext)
        {
            _logger = logger;
            _superheroesContext = superheroesContext;
        }

        [Authorize]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [HttpGet(Name = "GetSuperpowers")]
        public async Task<ActionResult<IEnumerable<Superpower>>> Get()
        {
            return await _superheroesContext.Superpowers.ToListAsync();
        }

        [Authorize]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [HttpGet("{id}", Name = "GetSuperpowerById")]
        public async Task<ActionResult<Superpower>> Get(int id)
        {
            var superpower = await _superheroesContext.Superpowers.FindAsync(id);

            if (superpower == null)
                return new NotFoundResult();

            return superpower;
        }

        [Authorize(Roles = "admin")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [HttpPost(Name = "InsertSuperpower")]
        public async Task<ActionResult<Superpower>> Post(Superpower superpower)
        {
            if (superpower == null)
                return new BadRequestObjectResult("Error: Object is null");

            //superpower.Id = _superheroesContext.Superpowers.Count() + 1;

            _superheroesContext.Superpowers.Add(superpower);
            await _superheroesContext.SaveChangesAsync();

            return new OkObjectResult(superpower);
        }

        [Authorize(Roles = "admin")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [HttpPut(Name = "UpdateSuperpower")]
        public async Task<ActionResult<Superpower>> Put(Superpower superpower)
        {

            if (superpower == null)
                return new BadRequestObjectResult("Error: Object is null");

            var update_superpower = _superheroesContext.Superpowers.Any(sp => sp.Id == superpower.Id);
            if (!update_superpower)
                return new NotFoundResult();

            _superheroesContext.Update(superpower);
            await _superheroesContext.SaveChangesAsync();

            return new OkObjectResult(superpower);
        }

        [Authorize(Roles = "admin")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [HttpDelete("{id}", Name = "DeleteSuperpower")]
        public async Task<ActionResult<Superpower>> Delete(int id)
        {
            var superpower = await _superheroesContext.Superpowers.FindAsync(id);

            if (superpower == null)
                return new NotFoundResult();

            foreach (var sp in _superheroesContext.HeroPowers.Where(s => s.PowerId == superpower.Id))
                _superheroesContext.HeroPowers.Remove(sp);

            _superheroesContext.Superpowers.Remove(superpower);
            await _superheroesContext.SaveChangesAsync();

            return new OkObjectResult(superpower);
        }
    }
}
