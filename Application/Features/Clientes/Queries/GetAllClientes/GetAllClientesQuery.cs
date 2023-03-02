using Application.DTOs;
using Application.Interface;
using Application.Specification;
using Application.Wrappers;
using AutoMapper;
using Domain.Entities;
using MediatR;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Clientes.Queries.GetAllClientes
{
    public class GetAllClientesQuery:IRequest<PageResponse<List<ClienteDTO>>>
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public string? Nombre { get; set; }
        public string? Apellido { get; set; }
    }

    public class GetAllClientesQueryHandler : IRequestHandler<GetAllClientesQuery, PageResponse<List<ClienteDTO>>>
    {
        private readonly IRepositoryAsync<Cliente> _repositoryAsync;
        private readonly IMapper _mapper;
        private readonly IDistributedCache _distributedCache;
        public GetAllClientesQueryHandler(IMapper mapper, IRepositoryAsync<Cliente> repositoryAsync, IDistributedCache distributedCache)
        {
            _mapper = mapper;
            _repositoryAsync = repositoryAsync;
            _distributedCache = distributedCache;
        }

        public async Task<PageResponse<List<ClienteDTO>>> Handle(GetAllClientesQuery request, CancellationToken cancellationToken)
        {
            var cacheKey = $"ListadoClientes_{request.PageSize}_{request.PageNumber}_{request.Nombre}_{request.Apellido}";
            string serializedListadoClientes;
            var listadoClientes = new List<Cliente>();


            var redisListadoCliente = await _distributedCache.GetAsync(cacheKey);

            if (redisListadoCliente != null)
            {
                serializedListadoClientes = Encoding.UTF8.GetString(redisListadoCliente);
                listadoClientes = JsonConvert.DeserializeObject<List<Cliente>>(serializedListadoClientes);
            }
            else
            {
                listadoClientes = await _repositoryAsync.ListAsync(new PagedClientesSpecification(request.PageSize, request.PageNumber, request.Nombre, request.Apellido));
                serializedListadoClientes = JsonConvert.SerializeObject(listadoClientes);
                redisListadoCliente = Encoding.UTF8.GetBytes(serializedListadoClientes);
                var options = new DistributedCacheEntryOptions().SetAbsoluteExpiration(DateTime.Now.AddMinutes(10)) //si la consulta la solicitan constantemente
                    .SetSlidingExpiration(TimeSpan.FromMinutes(2)); // si nadie la solicita se borra la cache en el tiempo establecido
                await _distributedCache.SetAsync(cacheKey, redisListadoCliente, options);
            }

            var clientesDTo = _mapper.Map<List<ClienteDTO>>(listadoClientes);

            return new PageResponse<List<ClienteDTO>>(clientesDTo, request.PageNumber, request.PageSize);
        }
    }
}
