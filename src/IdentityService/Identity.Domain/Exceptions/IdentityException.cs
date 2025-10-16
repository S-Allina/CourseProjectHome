using FluentValidation.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Identity.Domain.Exceptions
{
    public class IdentityException : Exception
    {
        public List<string> Errors { get; }

        public IdentityException(IEnumerable<ValidationFailure> failures) : base("Произошла одна или несколько ошибок проверки.")
        {
            Errors = failures.Select(e => e.ErrorMessage).ToList();
        }

        public IdentityException(string message) : base(message)
        {
            Errors = new List<string> { message };
        }

        public IdentityException(IEnumerable<string> errorMessages) : base("Произошла одна или несколько ошибок.")
        {
            Errors = errorMessages.ToList();
        }
    }
}
