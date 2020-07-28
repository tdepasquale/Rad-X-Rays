using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using API.Dtos;
using FluentValidation;
using Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.TechniqueCharts
{
    public class GetTechniqueChart
    {
        public class Query : IRequest<ChartDto>
        {
            public Guid Id { get; set; }
        }

        public class RequestValidator : AbstractValidator<Query>
        {
            public RequestValidator()
            {
                RuleFor(x => x.Id).NotEmpty();
            }
        }

        public class Handler : IRequestHandler<Query, ChartDto>
        {
            private readonly DataContext _context;
            public Handler(DataContext context)
            {
                _context = context;
            }
            public async Task<ChartDto> Handle(Query request, CancellationToken cancellationToken)
            {
                var chartFromDB = await _context.TechniqueCharts.Where(x => x.Id == request.Id).Include(x => x.Techniques).SingleOrDefaultAsync();

                if (chartFromDB == null) return new ChartDto(); //throw error?

                chartFromDB.Techniques = chartFromDB.Techniques.OrderBy(technique => technique.Index).ToList();

                var chart = new ChartDto
                {
                    Id = chartFromDB.Id.ToString(),
                    OwnerUsername = chartFromDB.OwnerUsername,
                    Name = chartFromDB.Name
                };

                var techniques = new List<TechniqueDto>();

                foreach (var technique in chartFromDB.Techniques)
                {
                    var techniqueToAdd = new TechniqueDto
                    {
                        Id = technique.Id.ToString(),
                        BodyPart = technique.BodyPart,
                        mAs = technique.mAs,
                        kVp = technique.kVp,
                        Notes = technique.Notes,
                        Index = technique.Index
                    };
                    techniques.Add(techniqueToAdd);
                };

                chart.Techniques = techniques;

                return chart;
            }
        }
    }
}