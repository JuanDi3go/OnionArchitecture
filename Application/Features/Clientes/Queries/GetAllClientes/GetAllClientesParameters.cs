﻿using Application.Parameters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Clientes.Queries.GetAllClientes
{
    public class GetAllClientesParameters:RequestParametros
    {
        public string? Nombre { get; set; }
        public string? Apellido { get; set; }
    }
}
