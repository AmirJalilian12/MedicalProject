using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Account
{
    public class AddressDto
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public bool IsMain { get; set; }
        public string State { get; set; }
        public string City { get; set; }
        public string FirstName { get; set; }
        public string FullAddress { get; set; }
        public string LastName { get; set; }
        public string Number { get; set; }
        public string PostalCode { get; set; }
    }
}
