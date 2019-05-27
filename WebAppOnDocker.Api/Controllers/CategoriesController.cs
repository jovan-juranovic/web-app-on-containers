using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using WebAppOnDocker.Api.Application.IntegrationEvents.Events;
using WebAppOnDocker.Core.Model;
using WebAppOnDocker.Infrastructure;
using WebAppOnDocker.Infrastructure.Http.OfferRequestProcessor;
using WebAppOnDocker.Shared.EventBus.IntegrationEventLogEF.Services;

namespace WebAppOnDocker.Api.Controllers
{
    public class CategoriesController : BaseApiController
    {
        private readonly ApplicationContext _context;
        private readonly IIntegrationEventService _integrationEventService;
        private readonly IOfferRequestProcessorClient _offerRequestProcessor;

        public CategoriesController(ApplicationContext context, IIntegrationEventService integrationEventService, IOfferRequestProcessorClient offerRequestProcessor)
        {
            _context = context;
            _integrationEventService = integrationEventService;
            _offerRequestProcessor = offerRequestProcessor;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var result = await _offerRequestProcessor.GetAsync();
            return Ok(new[] { result, "value2" });
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