using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using API.Dtos;
using Application.Errors;
using Core.Entities;
using Core.Interfaces;
using FluentValidation;
using Infrastructure.Data;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Application.TechniqueCharts
{
    public class UpdateTechniqueChart
    {
        public class Command : IRequest
        {
            public Guid Id { get; set; }
            public string Name { get; set; }
            public List<TechniqueDto> Techniques { get; set; }
        }

        public class CommandValidator : AbstractValidator<Command>
        {
            public CommandValidator()
            {
                RuleFor(x => x.Id).NotEmpty();
                RuleFor(x => x.Name).NotEmpty();
                RuleFor(x => x.Techniques).NotEmpty();
            }
        }

        public class Handler : IRequestHandler<Command>
        {
            private readonly DataContext _context;
            private readonly UserManager<AppUser> _userManager;
            private readonly IUserAccessor _userAccessor;
            public Handler(UserManager<AppUser> userManager, IUserAccessor userAccessor, DataContext context)
            {
                _userAccessor = userAccessor;
                _userManager = userManager;
                _context = context;
            }

            public async Task<Unit> Handle(Command request, CancellationToken cancellationToken)
            {
                var user = await _userManager.FindByNameAsync(_userAccessor.GetCurrentUsername());

                var chart = await _context.TechniqueCharts.Where(x => x.Id == request.Id).Include(x => x.Techniques).SingleOrDefaultAsync();

                if (chart == null)
                    throw new RestException(HttpStatusCode.NotFound, new { TechniqueChart = "Not found" });

                if (chart.AppUserId != user.Id)
                    throw new RestException(HttpStatusCode.BadRequest, new { TechniqueChart = "You do not have permission to update this chart." });

                chart.Name = request.Name;

                var updatedTechniques = new List<Technique>();

                const int maxBodyPartLength = 50;
                const int maxNoteLength = 1000;

                foreach (var technique in request.Techniques)
                {
                    Guid id = new Guid();
                    Guid.TryParse(technique.Id, out id);

                    int bodyPartLength = technique.BodyPart.Length > maxBodyPartLength ? maxBodyPartLength : technique.BodyPart.Length;
                    int notesLength = technique.Notes.Length > maxNoteLength ? maxNoteLength : technique.Notes.Length;

                    var newTechnique = new Technique
                    {
                        Id = id,
                        BodyPart = technique.BodyPart.Substring(0, bodyPartLength),
                        mAs = technique.mAs,
                        kVp = technique.kVp,
                        Notes = technique.Notes.Substring(0, notesLength),
                        Index = technique.Index
                    };

                    updatedTechniques.Add(newTechnique);
                }

                chart.Techniques = updatedTechniques;

                _context.TechniqueCharts.Update(chart);

                var success = await _context.SaveChangesAsync() > 0;

                if (success) return Unit.Value;

                throw new Exception("Problem saving changes");
            }
        }
    }
}