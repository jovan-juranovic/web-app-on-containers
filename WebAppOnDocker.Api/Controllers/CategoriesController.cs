﻿using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using WebAppOnDocker.Shared.EventBus.Abstractions;

namespace WebAppOnDocker.Api.Controllers
{
    public class CategoriesController : BaseApiController
    {
        private readonly IEventBus _bus;

        public CategoriesController(IEventBus bus)
        {
            _bus = bus;
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
        public void Post([FromBody] string value)
        {
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