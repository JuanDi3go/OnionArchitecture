using Application.DTOs;
using Application.Interface;
using Application.Wrappers;
using AutoMapper;
using Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Clientes.Queries.GetClienteById
{
    public class GetClienteByIdQuery: IRequest<Response<ClienteDTO>>
    {
    public int Id { get; set; }

    }
    public class GetClienteByIdQueryHandler: IRequestHandler<GetClienteByIdQuery, Response<ClienteDTO>>
    {
        private readonly IRepositoryAsync<Cliente> _repositoryAsync;
        private readonly IMapper _mapper;

        public GetClienteByIdQueryHandler(IMapper mapper, IRepositoryAsync<Cliente> repositoryAsync)
        {
            _mapper = mapper;
            _repositoryAsync = repositoryAsync;
        }

        public async Task<Response<ClienteDTO>> Handle(GetClienteByIdQuery request, CancellationToken cancellationToken)
        {
            var cliente = await _repositoryAsync.GetByIdAsync(request.Id);
           
            if (cliente == null)
            {
                throw new KeyNotFoundException($"Registro no encontrado con el id {request.Id}");
            }
            else
            {
                var dto = _mapper.Map<ClienteDTO>(cliente);
                return new Response<ClienteDTO>(dto);
            }
            
        }
    }
}
