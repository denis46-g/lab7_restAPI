using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RestAPI.Models;
using System.Drawing;

namespace RestAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Produces("application/json")]
    public class PublisherController
    {
        private readonly ILogger<PublisherController> _logger;
        private readonly SuperheroesContext _superheroesContext;

        public PublisherController(ILogger<PublisherController> logger, SuperheroesContext superheroesContext)
        {
            _logger = logger;
            _superheroesContext = superheroesContext;
        }

        [HttpGet(Name = "GetPublishers")]
        public async Task<ActionResult<IEnumerable<Publisher>>> Get()
        {
            return await _superheroesContext.Publishers.ToListAsync();
        }

        [HttpGet("{id}", Name = "GetPublisherById")]
        public async Task<ActionResult<Publisher>> Get(int id)
        {
            var publisher = await _superheroesContext.Publishers.FindAsync(id);

            if (publisher == null)
                return new NotFoundResult();

            return publisher;
        }

        [HttpPost(Name = "InsertPublisher")]
        public async Task<ActionResult<Publisher>> Post(Publisher publisher)
        {
            if (publisher == null)
                return new BadRequestObjectResult("Error: Object is null");

            //publisher.Id = _superheroesContext.Publishers.Count() + 1;

            _superheroesContext.Publishers.Add(publisher);
            await _superheroesContext.SaveChangesAsync();

            return new OkObjectResult(publisher);
        }

        [HttpPut(Name = "UpdatePublisher")]
        public async Task<ActionResult<Publisher>> Put(Publisher publisher)
        {

            if (publisher == null)
                return new BadRequestObjectResult("Error: Object is null");

            var update_publisher = _superheroesContext.Publishers.Any(p => p.Id == publisher.Id);
            if (!update_publisher)
                return new NotFoundResult();

            _superheroesContext.Update(publisher);
            await _superheroesContext.SaveChangesAsync();

            return new OkObjectResult(publisher);
        }

        [HttpDelete("{id}", Name = "DeletePublisher")]
        public async Task<ActionResult<Publisher>> Delete(int id)
        {
            var publisher = await _superheroesContext.Publishers.FindAsync(id);

            if (publisher == null)
                return new NotFoundResult();

            foreach (var p in _superheroesContext.Superheroes.Where(s => s.PublisherId == publisher.Id))
                _superheroesContext.Superheroes.Find(p.Id).PublisherId = null;

            _superheroesContext.Publishers.Remove(publisher);
            await _superheroesContext.SaveChangesAsync();

            return new OkObjectResult(publisher);
        }
    }
}
