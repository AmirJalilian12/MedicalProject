using Domain.Entities.BaseEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities.MedicalEntities.MedicineEntities
{
    public class Medicine : BaseAuditableEntity
    {
        public string Title { get; set; }
        public string Form { get; set; }
        public string Description { get; set; }
        public string Dosage { get; set; }
    }
}
