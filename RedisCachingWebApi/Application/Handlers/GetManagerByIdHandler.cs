using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using RedisCachingWebApi.Application.Models;
using RedisCachingWebApi.Domain;
using RedisCachingWebApi.Interface;

namespace RedisCachingWebApi.Application.Handlers
{
    public class GetManagerByIdHandler
    {
        public class Query : IRequest<Response>
        {
            [FromRoute(Name = "managerId")]
            public int ManagerId { get; set; }
        }

        public class Response
        {
            public ManagerModel FormData { get; set; }
        }

        public class Handler : IRequestHandler<Query, Response>
        {
            private readonly IManagerRepository _managerRepository;
            private readonly IMapper _mapper;

            public Handler(IManagerRepository managerRepository, IMapper mapper)
            {
                _managerRepository = managerRepository;
                _mapper = mapper;
            }

            public async Task<Response> Handle(Query query, CancellationToken cancellationToken)
            {
                Response response = new();

                var mangerData = await _managerRepository.GetManagerDataByIdAsync(query.ManagerId);

                response.FormData = _mapper.Map<ManagerData, ManagerModel>(mangerData);
                return response;
            }
        }

        public class MappingProfile : Profile
        {
            public MappingProfile()
            {
                CreateMap<ManagerData, ManagerModel>();
            }
        }
    }
}