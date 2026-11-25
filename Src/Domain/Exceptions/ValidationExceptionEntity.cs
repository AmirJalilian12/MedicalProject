using FluentValidation.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Exceptions
{
    public class ValidationExceptionEntity : BaseException
    {
        public ValidationExceptionEntity(List<string> messages) : base(messages)
        {

        }
        public ValidationExceptionEntity(string message) : base(message)
        {

        }
        public ValidationExceptionEntity() : base("خطایی رخ داده است لطفا مجددا تلاش بفرمایید")
        {

        }
        public ValidationExceptionEntity(IEnumerable<ValidationFailure> validationFailures) : base(validationFailures)
        {
            
        }
    }

}
