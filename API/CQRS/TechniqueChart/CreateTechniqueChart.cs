using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using Core.Interfaces;
using Core.Entities;
using API.Dtos;
using Infrastructure.Identity;
using Infrastructure.Data;

namespace Application.TechniqueCharts
{
    public class CreateTechniqueChart
    {
        public class Command : IRequest<ChartDto>
        {
        }

        public class Handler : IRequestHandler<Command, ChartDto>
        {
            private readonly AppIdentityDbContext _identityContext;
            private readonly DataContext _context;
            private readonly IUserAccessor _userAccessor;
            public Handler(AppIdentityDbContext identityContext, DataContext context, IUserAccessor userAccessor)
            {
                _userAccessor = userAccessor;
                _context = context;
                _identityContext = identityContext;
            }

            public async Task<ChartDto> Handle(Command request, CancellationToken cancellationToken)
            {
                var user = await _identityContext.Users.SingleOrDefaultAsync(x =>
                    x.UserName == _userAccessor.GetCurrentUsername());

                var chart = new TechniqueChart
                {
                    AppUserId = user.Id,
                    OwnerUsername = user.UserName,
                    Name = "Blank Chart"
                };

                var techniques = new List<Technique>{
                    new Technique{ BodyPart = "C-Spine AP", mAs = 0, kVp = 0, Notes="", Index = 0},
                    new Technique{ BodyPart = "C-Spine Lat", mAs = 0, kVp = 0, Notes="", Index = 1},
                    new Technique{ BodyPart = "T-Spine", mAs = 0, kVp = 0, Notes="", Index = 2},
                    new Technique{ BodyPart = "Lumbar Spine", mAs = 0, kVp = 0, Notes="", Index = 3},
                    new Technique{ BodyPart = "Lumbar Spine Obliques", mAs = 0, kVp = 0, Notes="", Index = 4},
                    new Technique{ BodyPart = "Lumbar Spine Lateral", mAs = 0, kVp = 0, Notes="", Index = 5},
                    new Technique{ BodyPart = "Sacrum", mAs = 0, kVp = 0, Notes="", Index = 6},
                    new Technique{ BodyPart = "Coccyx", mAs = 0, kVp = 0, Notes="", Index = 7},
                    new Technique{ BodyPart = "Chest", mAs = 0, kVp = 0, Notes="", Index = 8},
                    new Technique{ BodyPart = "Sternum", mAs = 0, kVp = 0, Notes="", Index = 9},
                    new Technique{ BodyPart = "Ribs", mAs = 0, kVp = 0, Notes="", Index = 10},
                    new Technique{ BodyPart = "Abdomen", mAs = 0, kVp = 0, Notes="", Index = 11},
                    new Technique{ BodyPart = "Pelvis", mAs = 0, kVp = 0, Notes="", Index = 12},
                    new Technique{ BodyPart = "S/C Joints", mAs = 0, kVp = 0, Notes="", Index = 13},
                    new Technique{ BodyPart = "A/C Joints", mAs = 0, kVp = 0, Notes="", Index = 14},
                    new Technique{ BodyPart = "Clavicle", mAs = 0, kVp = 0, Notes="", Index = 15},
                    new Technique{ BodyPart = "Shoulder AP", mAs = 0, kVp = 0, Notes="", Index = 16},
                    new Technique{ BodyPart = "Shoulder Y", mAs = 0, kVp = 0, Notes="", Index = 17},
                    new Technique{ BodyPart = "Shoulder Axillary", mAs = 0, kVp = 0, Notes="", Index = 18},
                    new Technique{ BodyPart = "Humerus", mAs = 0, kVp = 0, Notes="", Index = 19},
                    new Technique{ BodyPart = "Elbow", mAs = 0, kVp = 0, Notes="", Index = 20},
                    new Technique{ BodyPart = "Forearm", mAs = 0, kVp = 0, Notes="", Index = 21},
                    new Technique{ BodyPart = "Wrist", mAs = 0, kVp = 0, Notes="", Index = 22},
                    new Technique{ BodyPart = "Hand", mAs = 0, kVp = 0, Notes="", Index = 23},
                    new Technique{ BodyPart = "Fingers", mAs = 0, kVp = 0, Notes="", Index = 24},
                    new Technique{ BodyPart = "Hip", mAs = 0, kVp = 0, Notes="", Index = 25},
                    new Technique{ BodyPart = "Femur", mAs = 0, kVp = 0, Notes="", Index = 26},
                    new Technique{ BodyPart = "Knee AP", mAs = 0, kVp = 0, Notes="", Index = 27},
                    new Technique{ BodyPart = "Knee Tunnel", mAs = 0, kVp = 0, Notes="", Index = 28},
                    new Technique{ BodyPart = "Knee Sunrise", mAs = 0, kVp = 0, Notes="", Index = 29},
                    new Technique{ BodyPart = "Tib / Fib", mAs = 0, kVp = 0, Notes="", Index = 30},
                    new Technique{ BodyPart = "Ankle", mAs = 0, kVp = 0, Notes="", Index = 31},
                    new Technique{ BodyPart = "Foot", mAs = 0, kVp = 0, Notes="", Index = 32},
                    new Technique{ BodyPart = "Toes", mAs = 0, kVp = 0, Notes="", Index = 33}
                };

                chart.Techniques = techniques;

                _context.TechniqueCharts.Add(chart);

                var success = await _context.SaveChangesAsync() > 0;

                if (success)
                {
                    var chartDto = new ChartDto { Id = chart.Id.ToString(), Name = chart.Name, OwnerUsername = chart.OwnerUsername };
                    return chartDto;
                }

                throw new Exception("Problem saving changes");
            }
        }
    }
}