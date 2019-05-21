using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using WebAppOnDocker.Api.Application.IntegrationEvents.Events;
using WebAppOnDocker.Core.Model;
using WebAppOnDocker.Infrastructure;
using WebAppOnDocker.Shared.EventBus.IntegrationEventLogEF.Services;

namespace WebAppOnDocker.Api.Controllers
{
    public class CategoriesController : BaseApiController
    {
        private readonly ApplicationContext _context;
        private readonly IIntegrationEventService _integrationEventService;

        public CategoriesController(ApplicationContext context, IIntegrationEventService integrationEventService)
        {
            _context = context;
            _integrationEventService = integrationEventService;
        }

        [HttpGet]
        public ActionResult<IEnumerable<string>> Get()
        {
            return new[] { "value1", "value2" };
        }

        [HttpGet("{id}")]
        public ActionResult<string> Get(int id)
        {
            return "value";
        }

        [HttpPost]
        public async Task<IActionResult> AddCategory([FromBody] string name)
        {
            var category = new Category
            {
                Name = name,
                Description = "some desc",
                CategoryTypeId = CategoryType.Azure.Id
            };

            _context.Categories.Add(category);
            var categoryAddedEvent = new CategoryAddedIntegrationEvent(name);

            await _integrationEventService.SaveChangesIncludingEventLogAsync(categoryAddedEvent);
            await _integrationEventService.PublishThroughEventBusAsync(categoryAddedEvent);

            return Ok();
        }

        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}