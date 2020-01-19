using System;
using System.Threading;
using System.Threading.Tasks;
using Domain;
using MediatR;
using Persistence;
using Application.Errors;
using System.Net;
using Microsoft.EntityFrameworkCore;
using AutoMapper;

namespace Application.Activities
{
    public class Details
    {
        public class Query : IRequest<ActivityDTO>
        {
            public Guid Id { get; set; }
        }

        public class Handler : IRequestHandler<Query, ActivityDTO>
        {
            private readonly DataContext _context;
            private readonly IMapper _mapper;
            public Handler(DataContext context, IMapper mapper)
            {
                _mapper = mapper;
                _context = context;
            }

            public async Task<ActivityDTO> Handle(Query request, CancellationToken cancellationToken)
            {
                var activity = await _context.Activities
                    .FindAsync(request.Id);

                if (activity == null)
                    throw new RestException(HttpStatusCode.NotFound, new { Activity = "Not found" });

                var activityToReturn = _mapper.Map<Activity, ActivityDTO>(activity);

                return activityToReturn;
            }
        }
    }
}