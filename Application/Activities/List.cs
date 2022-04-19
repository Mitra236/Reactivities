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
            public PagingParams Params { get; set; }
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
                                .OrderBy(d => d.Date)
                                .ProjectTo<ActivityDTO>(_mapper.ConfigurationProvider,
                                    new {currentUsername = _userAccessor.GetUsername()})
                                .AsQueryable();

                return Result<PagedList<ActivityDTO>>.Success(
                    await PagedList<ActivityDTO>.CreateAsync(query, request.Params.PageNumber, request.Params.PageSize)
                );
            }
        }
    }
}