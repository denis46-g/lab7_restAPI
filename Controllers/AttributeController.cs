using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RestAPI.Models;

namespace RestAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Produces("application/json")]
    public class AttributeController
    {
        private readonly ILogger<AttributeController> _logger;
        private readonly SuperheroesContext _superheroesContext;

        public AttributeController(ILogger<AttributeController> logger, SuperheroesContext superheroesContext)
        {
            _logger = logger;
            _superheroesContext = superheroesContext;
        }

        [HttpGet(Name = "GetAttributes")]
        public async Task<ActionResult<IEnumerable<Models.Attribute>>> Get()
        {
            return await _superheroesContext.Attributes.ToListAsync();
        }

        [HttpGet("{id}", Name = "GetAttributeById")]
        public async Task<ActionResult<Models.Attribute>> Get(int id)
        {
            var attribute = await _superheroesContext.Attributes.FindAsync(id);

            if (attribute == null)
                return new NotFoundResult();

            return attribute;
        }

        [HttpPost(Name = "InsertAttribute")]
        public async Task<ActionResult<Models.Attribute>> Post(Models.Attribute attribute)
        {
            if (attribute == null)
                return new BadRequestObjectResult("Error: Object is null");

            //attribute.Id = _superheroesContext.Attributes.Count() + 1;

            _superheroesContext.Attributes.Add(attribute);
            await _superheroesContext.SaveChangesAsync();

            return new OkObjectResult(attribute);
        }

        [HttpPut(Name = "UpdateAttribute")]
        public async Task<ActionResult<Models.Attribute>> Put(Models.Attribute attribute)
        {

            if (attribute == null)
                return new BadRequestObjectResult("Error: Object is null");

            var update_attribute = _superheroesContext.Attributes.Any(attrib => attrib.Id == attribute.Id);
            if (!update_attribute)
                return new NotFoundResult();

            _superheroesContext.Update(attribute);
            await _superheroesContext.SaveChangesAsync();

            return new OkObjectResult(attribute);
        }

        [HttpDelete("{id}", Name = "DeleteAttribute")]
        public async Task<ActionResult<Models.Attribute>> Delete(int id)
        {
            var attribute = await _superheroesContext.Attributes.FindAsync(id);

            if (attribute == null)
                return new NotFoundResult();

            var hero_attrib = _superheroesContext.HeroAttributes.ToList();
            foreach (var attrib in _superheroesContext.HeroAttributes.Where(a=>a.AttributeId == attribute.Id))
                _superheroesContext.HeroAttributes.Remove(attrib);

            _superheroesContext.Attributes.Remove(attribute);
            await _superheroesContext.SaveChangesAsync();

            return new OkObjectResult(attribute);
        }

    }
}
