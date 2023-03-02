using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Authenticate.Commands.RegisterCommand
{
    public class RegisterCommandValidator: AbstractValidator<RegisterCommand>
    {
        public RegisterCommandValidator()
        {
            RuleFor(p => p.Nombre).NotEmpty().WithMessage("{PropertyName} no puede ser vacio")
                .MaximumLength(80).WithMessage("{PropertyName} no debe exceder de {MaxLenght} caracteres");
            RuleFor(p => p.Apellido).NotEmpty().WithMessage("{PropertyName} no puede ser vacio")
                .MaximumLength(80).WithMessage("{PropertyName} no debe exceder de {MaxLenght} caracteres");         
            RuleFor(p => p.UserName).NotEmpty().WithMessage("{PropertyName} no puede ser vacio")
                .MaximumLength(10).WithMessage("{PropertyName} no debe exceder de {MaxLenght} caracteres");

            RuleFor(p => p.Password).NotEmpty().WithMessage("{PropertyName} no puede ser vacio")
            .MaximumLength(15).WithMessage("{PropertyName} no debe exceder de {MaxLenght} caracteres");    
            RuleFor(p => p.ConfirmPassword).NotEmpty().WithMessage("{PropertyName} no puede ser vacio")
            .MaximumLength(15).WithMessage("{PropertyName} no debe exceder de {MaxLenght} caracteres").Equal(p => p.Password)
            .WithMessage("Confirm Password debe ser igual a password");

            RuleFor(p => p.Email).NotEmpty().WithMessage("{PropertyName} no puede ser vacio")
                .EmailAddress().WithMessage("{PropertyName} debe ser una direcion de Email Valida")
                .MaximumLength(100).WithMessage("{PropertyName} no debe exceder de {MaxLenght} caracteres");

        }
    }
}
