using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RestAPI.Models;
using System;


namespace RestAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Produces("application/json")]
    public class ColourController
    {
        private readonly ILogger<ColourController> _logger;
        private readonly SuperheroesContext _superheroesContext;

        public ColourController(ILogger<ColourController> logger, SuperheroesContext superheroesContext)
        {
            _logger = logger;
            _superheroesContext = superheroesContext;
        }

        [Authorize]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [HttpGet(Name = "GetColours")]
        public async Task<ActionResult<IEnumerable<Colour>>> Get()
        {
            return await _superheroesContext.Colours.ToListAsync();
        }

        [Authorize]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [HttpGet("{id}", Name = "GetColourById")]
        public async Task<ActionResult<Colour>> Get(int id)
        {
            var colour = await _superheroesContext.Colours.FindAsync(id);

            if (colour == null)
                return new NotFoundResult();

            return colour;
        }

        [Authorize(Roles = "admin")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [HttpPost(Name = "InsertColour")]
        public async Task<ActionResult<Colour>> Post(Colour colour)
        {
            if (colour == null)
                return new BadRequestObjectResult("Error: Object is null");

            //colour.Id = _superheroesContext.Colours.Count() + 1;

            _superheroesContext.Colours.Add(colour);
            await _superheroesContext.SaveChangesAsync();

            return new OkObjectResult(colour);
        }

        [Authorize(Roles = "admin")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [HttpPut(Name = "UpdateColour")]
        public async Task<ActionResult<Colour>> Put(Colour colour)
        {

            if (colour == null)
                return new BadRequestObjectResult("Error: Object is null");

            var update_colour = _superheroesContext.Colours.Any(c => c.Id == colour.Id);
            if (!update_colour)
                return new NotFoundResult();

            _superheroesContext.Update(colour);
            await _superheroesContext.SaveChangesAsync();

            return new OkObjectResult(colour);
        }

        [Authorize(Roles = "admin")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [HttpDelete("{id}", Name = "DeleteColour")]
        public async Task<ActionResult<Colour>> Delete(int id)
        {
            var colour = await _superheroesContext.Colours.FindAsync(id);

            if (colour == null)
                return new NotFoundResult();

            foreach (var eye_s in _superheroesContext.Superheroes.Where(s => s.EyeColourId == colour.Id))
                _superheroesContext.Superheroes.Find(eye_s.Id).EyeColourId = null;
            foreach (var hair_s in _superheroesContext.Superheroes.Where(s => s.HairColourId == colour.Id))
                _superheroesContext.Superheroes.Find(hair_s.Id).HairColourId = null;
            foreach (var skin_s in _superheroesContext.Superheroes.Where(s => s.SkinColourId == colour.Id))
                _superheroesContext.Superheroes.Find(skin_s.Id).SkinColourId = null;

            _superheroesContext.Colours.Remove(colour);
            await _superheroesContext.SaveChangesAsync();

            return new OkObjectResult(colour);
        }
    }
}
