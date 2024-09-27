using AutoMapper;
using MediatR;
using RedisCachingWebApi.Application.Models;
using RedisCachingWebApi.Domain;
using RedisCachingWebApi.Interface;

namespace RedisCachingWebApi.Application.Handlers.Manager
{
    public class GetAllManagerHandler
    {
        public class Query : IRequest<Response>
        {
        }

        public class Response
        {
            public ManagerModel[] FormData { get; set; }
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

                var mangerData = await _managerRepository.GetAllManagerDatasAsync();

                response.FormData = _mapper.Map<ManagerModel[]>(mangerData);
                return response;
            }
        }

        // AutoMapper profile to map ManagerData to ManagerModel
        public class MappingProfile : Profile
        {
            public MappingProfile()
            {
                CreateMap<ManagerData, ManagerModel>();
            }
        }
    }
}