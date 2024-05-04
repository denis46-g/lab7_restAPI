using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RestAPI.Models;
using System.Security.Cryptography;

namespace RestAPI.Controllers
{
    /*public partial class HeroPowerInfo
    {
        public int? heroId { get; set; }

        public string? heroName { get; set; }

        public int? powerId { get; set; }

        public string? powerName { get; set; }

    }*/

    [ApiController]
    [Route("[controller]")]
    [Produces("application/json")]
    public class HeroPowerController
    {
        private readonly ILogger<HeroPowerController> _logger;
        private readonly SuperheroesContext _superheroesContext;

        public HeroPowerController(ILogger<HeroPowerController> logger, SuperheroesContext superheroesContext)
        {
            _logger = logger;
            _superheroesContext = superheroesContext;
        }

        [Authorize]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [HttpGet(Name = "GetHeroPowers")]
        public async Task<ActionResult<IEnumerable<HeroPower>>> Get()
        {
            return await _superheroesContext.HeroPowers.ToListAsync();
        }

        [Authorize]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [HttpGet("{hid}/{pid}", Name = "GetHeroPowerById")]
        public async Task<ActionResult<HeroPower>> Get(int hid, int pid)
        {
            var heropower = await _superheroesContext.HeroPowers.FirstOrDefaultAsync(h => h.HeroId == hid && h.PowerId == pid); ;

            if (heropower == null)
                return new NotFoundResult();

            return heropower;
        }

        [Authorize(Roles = "admin")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [HttpPost(Name = "InsertHeroPower")]
        public async Task<ActionResult<HeroPower>> Post(HeroPower heropower)
        {
            if (heropower == null)
                return new BadRequestObjectResult("Error: Object is null");

            _superheroesContext.HeroPowers.Add(heropower);
            await _superheroesContext.SaveChangesAsync();

            return new OkObjectResult(heropower);
        }

        [Authorize(Roles = "admin")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [HttpPut(Name = "UpdateHeroPower")]
        public async Task<ActionResult<HeroPower>> Put(HeroPower heropower)
        {

            if (heropower == null)
                return new BadRequestObjectResult("Error: Object is null");

            var update_heropower = _superheroesContext.HeroPowers.Any(c => c.HeroId == heropower.HeroId && c.PowerId == heropower.PowerId);
            if (!update_heropower)
                return new NotFoundResult();

            _superheroesContext.Update(heropower);
            await _superheroesContext.SaveChangesAsync();

            return new OkObjectResult(heropower);
        }

        [Authorize(Roles = "admin")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [HttpDelete("{hid}/{pid}", Name = "DeleteHeroPower")]
        public async Task<ActionResult<HeroPower>> Delete(int hid, int pid)
        {
            var heropower = await _superheroesContext.HeroPowers.FirstOrDefaultAsync(h => h.HeroId == hid && h.PowerId == pid);

            if (heropower == null)
                return new NotFoundResult();

            _superheroesContext.HeroPowers.Remove(heropower);
            await _superheroesContext.SaveChangesAsync();

            return new OkObjectResult(heropower);
        }
    }
}
