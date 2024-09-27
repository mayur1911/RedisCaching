using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using RedisCachingWebApi.Interface;

namespace RedisCachingWebApi.Application.Handlers.Manager
{
    public class DeleteManagerByIdHandler
    {
        public class Command : IRequest<Response>
        {
            [FromRoute(Name = "managerId")]
            public int ManagerId { get; set; }
        }

        public class Response
        {
            public bool Status { get; set; }
        }

        public class Handler : IRequestHandler<Command, Response>
        {
            private readonly IManagerRepository _managerRepository;

            public Handler(IManagerRepository managerRepository, IMapper mapper)
            {
                _managerRepository = managerRepository;
            }

            public async Task<Response> Handle(Command command, CancellationToken cancellation)
            {
                Response response = new();
                response.Status = await _managerRepository.DeleteManagerDataByIdAsync(command.ManagerId);
                return response;
            }
        }
    }
}