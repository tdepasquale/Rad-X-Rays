using System;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Application.Errors;
using Core.Entities;
using Core.Interfaces;
using FluentValidation;
using Infrastructure.Data;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Application.TechniqueCharts
{
    public class DeleteTechniqueChart
    {
        public class Command : IRequest
        {
            public Guid Id { get; set; }
        }

        public class CommandValidator : AbstractValidator<Command>
        {
            public CommandValidator()
            {
                RuleFor(x => x.Id).NotEmpty();
            }
        }

        public class Handler : IRequestHandler<Command>
        {
            private readonly DataContext _context;
            private readonly UserManager<AppUser> _userManager;
            private readonly IUserAccessor _userAccessor;
            private readonly ILogger<Handler> _logger;
            public Handler(UserManager<AppUser> userManager, IUserAccessor userAccessor, DataContext context, ILogger<Handler> logger)
            {
                _logger = logger;
                _userAccessor = userAccessor;
                _userManager = userManager;
                _context = context;
            }

            public async Task<Unit> Handle(Command request, CancellationToken cancellationToken)
            {
                var user = await _userManager.FindByNameAsync(_userAccessor.GetCurrentUsername());

                var chart = await _context.TechniqueCharts.FindAsync(request.Id);
                // var chart = await _context.TechniqueCharts.Where(x => x.Id == request.Id).Include(x => x.AppUser).FirstOrDefaultAsync();

                if (chart == null)
                {
                    _logger.LogInformation("CHART IS NULL");
                    throw new RestException(HttpStatusCode.NotFound, new { TechniqueChart = "Not found" });
                }

                if (chart.AppUserId != user.Id)
                {
                    _logger.LogInformation("APPUSER AND USER DONT MATCH");
                    _logger.LogInformation($"THE USER IS: {user.Id} AND THE CHARTNAME IS: {chart.AppUserId}");
                    throw new RestException(HttpStatusCode.BadRequest, new { TechniqueChart = "You do not have permission to delete this chart." });
                }

                _context.Remove(chart);

                var success = await _context.SaveChangesAsync() > 0;

                if (success) return Unit.Value;

                throw new Exception("Problem saving changes");
            }
        }
    }
}