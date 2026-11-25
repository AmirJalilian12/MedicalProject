using Application.DTOs.Common;
using Domain.Entities.MedicalEntities.MedicineEntities;
using Mapster;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Medicines
{
    public class MedicineDto : CommandDto, IRegister
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Form { get; set; }
        public string Description { get; set; }
        public string Dosage { get; set; }
        public void Register(TypeAdapterConfig config)
        {
            config.NewConfig<Medicine, MedicineDto>();
        }
    }
}
