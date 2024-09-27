using AutoMapper;
using MediatR;
using RedisCachingWebApi.Application.Models;
using RedisCachingWebApi.Domain;
using RedisCachingWebApi.Interface;

namespace RedisCachingWebApi.Application.Handlers.Manager
{
    public class SaveManagerHandler
    {
        // Command class inside the handler
        public class Command : IRequest<Response>
        {
            public ManagerModel FormData { get; set; }
        }

        // Response model for the SaveManager operation
        public class Response
        {
            public bool Status { get; set; }
            public int ManagerId { get; set; }
        }

        public class Handler : IRequestHandler<Command, Response>
        {
            private readonly IManagerRepository _managerRepository;
            private readonly IMapper _mapper;

            public Handler(IManagerRepository managerRepository, IMapper mapper)
            {
                _managerRepository = managerRepository;
                _mapper = mapper;
            }

            public async Task<Response> Handle(Command command, CancellationToken cancellationToken)
            {
                Response response = new();
                var managerData = _mapper.Map<ManagerModel, ManagerData>(command.FormData);
                response.ManagerId = await _managerRepository.AddManagerDataAsync(managerData);
                response.Status = response.ManagerId > 0;
                return response;
            }
        }

        // AutoMapper profile for ManagerModel to ManagerData mapping
        public class MappingProfile : Profile
        {
            public MappingProfile()
            {
                CreateMap<ManagerModel, ManagerData>()
                    .ForMember(dest => dest.NewId, opt => opt.Ignore());  // Ignoring NewId field
            }
        }
    }
}