using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Entities;
using Core.Interfaces;
using Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    [AllowAnonymous]
    public class DummyValuesController : BaseApiController
    {
        private readonly IDummyValueRepository _repository;
        public DummyValuesController(IDummyValueRepository repository)
        {
            _repository = repository;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<DummyValue>>> GetDummyValues()
        {
            var dummyValues = await _repository.GetValuesAsync();
            return Ok(dummyValues);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<DummyValue>> GetDummyValue(Guid id)
        {
            return await _repository.GetValueByIdAsync(id);
        }
    }
}