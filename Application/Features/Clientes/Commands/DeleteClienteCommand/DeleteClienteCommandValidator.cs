using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Clientes.Commands.DeleteClienteCommand
{
    public class DeleteClienteCommandValidator: AbstractValidator<DeleteClienteCommand>
    {
        public DeleteClienteCommandValidator()
        {
            RuleFor(p => p.Id).NotEmpty().WithMessage("El campo id no puede estar vacio");
        }
    }
}
