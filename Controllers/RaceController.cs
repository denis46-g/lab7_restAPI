using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RestAPI.Models;

namespace RestAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Produces("application/json")]
    public class RaceController
    {
        private readonly ILogger<RaceController> _logger;
        private readonly SuperheroesContext _superheroesContext;

        public RaceController(ILogger<RaceController> logger, SuperheroesContext superheroesContext)
        {
            _logger = logger;
            _superheroesContext = superheroesContext;
        }

        [Authorize]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [HttpGet(Name = "GetRaces")]
        public async Task<ActionResult<IEnumerable<Race>>> Get()
        {
            return await _superheroesContext.Races.ToListAsync();
        }

        [Authorize]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [HttpGet("{id}", Name = "GetRaceById")]
        public async Task<ActionResult<Race>> Get(int id)
        {
            var race = await _superheroesContext.Races.FindAsync(id);

            if (race == null)
                return new NotFoundResult();

            return race;
        }

        [Authorize(Roles = "admin")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [HttpPost(Name = "InsertRace")]
        public async Task<ActionResult<Race>> Post(Race race)
        {
            if (race == null)
                return new BadRequestObjectResult("Error: Object is null");

            //race.Id = _superheroesContext.Races.Count() + 1;

            _superheroesContext.Races.Add(race);
            await _superheroesContext.SaveChangesAsync();

            return new OkObjectResult(race);
        }

        [Authorize(Roles = "admin")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [HttpPut(Name = "UpdateRace")]
        public async Task<ActionResult<Race>> Put(Race race)
        {

            if (race == null)
                return new BadRequestObjectResult("Error: Object is null");

            var update_race = _superheroesContext.Races.Any(r => r.Id == race.Id);
            if (!update_race)
                return new NotFoundResult();

            _superheroesContext.Update(race);
            await _superheroesContext.SaveChangesAsync();

            return new OkObjectResult(race);
        }

        [Authorize(Roles = "admin")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [HttpDelete("{id}", Name = "DeleteRace")]
        public async Task<ActionResult<Race>> Delete(int id)
        {
            var race = await _superheroesContext.Races.FindAsync(id);

            if (race == null)
                return new NotFoundResult();

            foreach (var rc in _superheroesContext.Superheroes.Where(r => r.RaceId == race.Id))
                _superheroesContext.Superheroes.Find(rc.Id).RaceId = null;

            _superheroesContext.Races.Remove(race);
            await _superheroesContext.SaveChangesAsync();

            return new OkObjectResult(race);
        }
    }
}
