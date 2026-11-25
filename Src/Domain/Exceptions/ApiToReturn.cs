using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Exceptions
{
    public class ApiToReturn
    {
        public ApiToReturn()
        {
            Messages = new List<string>();
        }

        public ApiToReturn(string message)
        {
            Message = message;
            Messages = new List<string>();
            Messages.Add(message);
        }

        public ApiToReturn(int statusCode, string message)
        {
            StatusCode = statusCode;
            Message = message;
            Messages = new List<string>();
            Messages.Add(message);
        }

        public ApiToReturn(int statusCode, List<string> messages)
        {
            StatusCode = statusCode;
            Messages = messages ?? new List<string>();
        }

        public ApiToReturn(
            int statusCode,
            string message,
            List<string> messages)
        {
            StatusCode = statusCode;
            Message = message;
            Messages = messages ?? new List<string>();

            if (!string.IsNullOrEmpty(message))
            {
                Messages.Add(message);
            }
        }

        public ApiToReturn(
            int statusCode,
            string message,
            List<string> messages,
            string detail)
        {
            StatusCode = statusCode;
            Message = message;
            Messages = messages ?? new List<string>();
            Detail = detail;
        }

        public string Message { get; set; }

        public int StatusCode { get; set; }

        public string Detail { get; set; }

        public List<string> Messages { get; set; }
    }

}
