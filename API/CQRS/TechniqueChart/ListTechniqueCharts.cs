using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using API.Dtos;
using Core.Entities;
using Core.Interfaces;
using Infrastructure.Data;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Application.TechniqueCharts
{
    public class ListTechniqueCharts
    {
        public class Query : IRequest<List<ChartDto>> { }

        public class Handler : IRequestHandler<Query, List<ChartDto>>
        {
            private readonly UserManager<AppUser> _userManager;
            private readonly IUserAccessor _userAccessor;
            private readonly DataContext _context;
            public Handler(UserManager<AppUser> userManager, IUserAccessor userAccessor, DataContext context)
            {
                _context = context;
                _userAccessor = userAccessor;
                _userManager = userManager;
            }
            public async Task<List<ChartDto>> Handle(Query request, CancellationToken cancellationToken)
            {
                var user = await _userManager.FindByNameAsync(_userAccessor.GetCurrentUsername());

                var chartsFromDB = await _context.TechniqueCharts.Where(x => x.AppUserId == user.Id).ToListAsync();

                var charts = new List<ChartDto>();

                if (chartsFromDB == null) return charts;

                foreach (var chart in chartsFromDB)
                {
                    var chartToAdd = new ChartDto
                    {
                        Id = chart.Id.ToString(),
                        OwnerUsername = chart.OwnerUsername,
                        Name = chart.Name
                    };

                    charts.Add(chartToAdd);
                }

                return charts;
            }
        }
    }
}