using FluentValidation.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Exceptions
{
    public class ValidationException: Exception
    {
        public List<string> Errors { get;}
        public ValidationException(): base("Se han Producido uno o mas errores de validacion")
        {
            Errors = new List<string>();
        }

        public ValidationException(IEnumerable<ValidationFailure> failures): this()
        {
            foreach (var error in failures)
            {
                Errors.Add(error.ErrorMessage);
            }
        }
    }
}
