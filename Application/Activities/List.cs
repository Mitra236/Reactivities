using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.Core;
using Application.Interfaces;
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
        public class Query : IRequest<Result<PagedList<ActivityDTO>>> 
        { 
            public ActivityParams Params { get; set; }
        }

        public class Handler : IRequestHandler<Query, Result<PagedList<ActivityDTO>>>
        {
            private readonly DataContext _context;
            private readonly IMapper _mapper;
            private readonly IUserAccessor _userAccessor;

            public Handler(DataContext context, IMapper mapper, IUserAccessor userAccessor)
            {
                _context = context;
                _mapper = mapper;
                _userAccessor = userAccessor;
            }

            public async Task<Result<PagedList<ActivityDTO>>> Handle(Query request, CancellationToken cancellationToken)
            {
                //Eager Loading with Include 
                // var activities = await _context.Activities
                //                 .Include(x => x.Atendees)
                //                 .ThenInclude(u => u.AppUser)
                //                 .ToListAsync(cancellationToken);

                //var activitiesToReturn = _mapper.Map<List<ActivityDTO>>(activities);

                //AutoMapper Projection
                //asqueryable is not async - deffered execution
                var query =  _context.Activities
                                .Where(d => d.Date >= request.Params.StartDate)
                                .OrderBy(d => d.Date)
                                .ProjectTo<ActivityDTO>(_mapper.ConfigurationProvider,
                                    new {currentUsername = _userAccessor.GetUsername()})
                                .AsQueryable();

                if (request.Params.IsGoing && !request.Params.IsHost)
                {
                    query = query.Where(x => x.Atendees.Any(a => a.Username == _userAccessor.GetUsername()));
                }

                if (request.Params.IsHost && !request.Params.IsGoing)
                {
                    query = query.Where(x => x.HostUsername == _userAccessor.GetUsername());
                }

                return Result<PagedList<ActivityDTO>>.Success(
                    await PagedList<ActivityDTO>.CreateAsync(query, request.Params.PageNumber, request.Params.PageSize)
                );
            }
        }
    }
}