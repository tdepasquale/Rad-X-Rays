using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using API.Dtos;
using Application.TechniqueCharts;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace API.Controllers
{
    public class TechniqueChartsController : BaseApiController
    {
        private readonly ILogger<TechniqueChartsController> _logger;
        public TechniqueChartsController(ILogger<TechniqueChartsController> logger)
        {
            _logger = logger;
        }

        [HttpGet("list")]
        public async Task<ActionResult<List<ChartDto>>> List()
        {
            return await Mediator.Send(new ListTechniqueCharts.Query());
        }

        [AllowAnonymous]
        [HttpGet("{id}")]
        public async Task<ActionResult<ChartDto>> Get(Guid id)
        {
            return await Mediator.Send(new GetTechniqueChart.Query { Id = id });
        }

        [HttpPost("create")]
        public async Task<ActionResult<ChartDto>> CreateChart(CreateTechniqueChart.Command command)
        {
            return await Mediator.Send(command);
        }

        [HttpPut("edit")]
        public async Task<ActionResult<Unit>> Edit(UpdateTechniqueChart.Command command)
        {
            return await Mediator.Send(command);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<Unit>> DeleteChart(Guid id)
        {
            _logger.LogInformation($"THE ID FOR THE CHART TO DELEET IS: {id}");
            return await Mediator.Send(new DeleteTechniqueChart.Command { Id = id });
        }
    }
}