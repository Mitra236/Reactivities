using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Application.Core;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Domain;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Persistence;

namespace Application.Activities
{
    public class List
    {
        public class Query : IRequest<Result<List<ActivityDTO>>> { }

        public class Handler : IRequestHandler<Query, Result<List<ActivityDTO>>>
        {
            private readonly DataContext _context;
            private readonly IMapper _mapper;

            public Handler(DataContext context, IMapper mapper)
            {
                _context = context;
                _mapper = mapper;
            }

            public async Task<Result<List<ActivityDTO>>> Handle(Query request, CancellationToken cancellationToken)
            {
                //Eager Loading with Include 
                // var activities = await _context.Activities
                //                 .Include(x => x.Atendees)
                //                 .ThenInclude(u => u.AppUser)
                //                 .ToListAsync(cancellationToken);

                //var activitiesToReturn = _mapper.Map<List<ActivityDTO>>(activities);

                //AutoMapper Projection
                var activities = await _context.Activities
                                .ProjectTo<ActivityDTO>(_mapper.ConfigurationProvider)
                                .ToListAsync(cancellationToken);

                return Result<List<ActivityDTO>>.Success(activities);
            }
        }
    }
}