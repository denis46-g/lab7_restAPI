using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RestAPI.Models;
using System.Diagnostics;

namespace RestAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Produces("application/json")]
    public class SuperheroController
    {
        private readonly ILogger<SuperheroController> _logger;
        private readonly SuperheroesContext _superheroesContext;

        public SuperheroController(ILogger<SuperheroController> logger, SuperheroesContext superheroesContext)
        {
            _logger = logger;
            _superheroesContext = superheroesContext;
        }

        [HttpGet(Name = "GetSuperheroes")]
        public async Task<ActionResult<IEnumerable<Superhero>>> Get()
        {
            return await _superheroesContext.Superheroes.ToListAsync();
        }

        [HttpGet("{id}", Name = "GetSuperheroById")]
        public async Task<ActionResult<Superhero>> Get(int id)
        {
            var superhero = await _superheroesContext.Superheroes.FindAsync(id);

            if (superhero == null)
                return new NotFoundResult();

            return superhero;
        }

        [HttpPost(Name = "InsertSuperhero")]
        public async Task<ActionResult<Superhero>> Post(Superhero superhero)
        {
            if (superhero == null)
                return new BadRequestObjectResult("Error: Object is null");

            //superhero.Id = _superheroesContext.Superheroes.Count() + 1;

            if (superhero.GenderId != null && (superhero.GenderId < 1 || superhero.GenderId > 3))
                return new BadRequestObjectResult("Error: write correct GenderId (1 - Male, 2 - Female, 3 - N/A)");

            if (superhero.EyeColourId != null && !_superheroesContext.Colours.Any(colour => colour.Id == superhero.EyeColourId))
                superhero.EyeColourId = null;
            if (superhero.HairColourId != null && !_superheroesContext.Colours.Any(colour => colour.Id == superhero.HairColourId))
                superhero.HairColourId = null;
            if (superhero.SkinColourId != null && !_superheroesContext.Colours.Any(colour => colour.Id == superhero.SkinColourId))
                superhero.SkinColourId = null;
            if (superhero.RaceId != null && !_superheroesContext.Races.Any(race => race.Id == superhero.RaceId))
                superhero.RaceId = null;
            if (superhero.PublisherId != null && !_superheroesContext.Publishers.Any(publisher => publisher.Id == superhero.PublisherId))
                superhero.PublisherId = null;

            if (superhero.AlignmentId != null && (superhero.AlignmentId < 1 && superhero.AlignmentId > 4))
                return new BadRequestObjectResult("Error: write correct AlignmentId (1 - Good, 2 - Bad, 3 - Neutral, 4 - N/A)");

            if (superhero.HeightCm < 0)
                superhero.HeightCm = null;
            if (superhero.WeightKg < 0)
                superhero.WeightKg = null;


            _superheroesContext.Superheroes.Add(superhero);
            await _superheroesContext.SaveChangesAsync();

            return new OkObjectResult(superhero);
        }

        [HttpPut(Name = "UpdateSuperhero")]
        public async Task<ActionResult<Superhero>> Put(Superhero superhero)
        {

            if (superhero == null)
                return new BadRequestObjectResult("Error: Object is null");

            var update_superhero = _superheroesContext.Superheroes.Any(sh => sh.Id == superhero.Id);
            if (!update_superhero)
                return new NotFoundResult();

            _superheroesContext.Update(superhero);
            await _superheroesContext.SaveChangesAsync();

            return new OkObjectResult(superhero);
        }

        [HttpDelete("{id}", Name = "DeleteSuperhero")]
        public async Task<ActionResult<Superhero>> Delete(int id)
        {
            var superhero = await _superheroesContext.Superheroes.FindAsync(id);

            if (superhero == null)
                return new NotFoundResult();

            foreach (var ha in _superheroesContext.HeroAttributes.Where(h => h.HeroId == superhero.Id))
                _superheroesContext.HeroAttributes.Remove(ha);
            foreach (var hp in _superheroesContext.HeroPowers.Where(h => h.HeroId == superhero.Id))
                _superheroesContext.HeroPowers.Remove(hp);


            _superheroesContext.Superheroes.Remove(superhero);
            await _superheroesContext.SaveChangesAsync();

            return new OkObjectResult(superhero);
        }
    }
}
