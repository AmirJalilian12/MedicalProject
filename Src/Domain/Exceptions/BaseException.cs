using FluentValidation.Results;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Domain.Exceptions
{
    public class BaseException : Exception
    {
        public List<string> Messages { get; set; }

        public BaseException(List<string> messages)
            : base(messages?.FirstOrDefault())
        {
            Messages = messages ?? new List<string>();
        }

        public BaseException(string message)
            : base(message)
        {
            Messages = new List<string>();

            if (!string.IsNullOrWhiteSpace(message))
            {
                Messages.Add(message);
            }
        }
        public BaseException(IEnumerable<ValidationFailure> validationFailures)
        {
            Messages = validationFailures.Select(x => x.ErrorMessage).ToList();
        }
    }
}