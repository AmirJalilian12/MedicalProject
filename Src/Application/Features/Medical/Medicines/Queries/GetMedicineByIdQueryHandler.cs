using Application.DTOs.Contracts;
using Application.DTOs.Medicines;
using Application.DTOs.Products;
using Application.Features.Medical.Medicines.Spec;
using Domain.Entities.MedicalEntities.MedicineEntities;
using Domain.Exceptions;
using Mapster;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Medical.Medicines.Queries
{
    public class GetMedicineByIdQuery : IRequest<MedicineDto>
    {
        public int Id { get; set; }
        public GetMedicineByIdQuery(int id) => Id = id;
    }

    public class GetMedicineByIdQueryHandler : IRequestHandler<GetMedicineByIdQuery, MedicineDto>
    {
        private readonly IUnitOfWork _uow;
        public GetMedicineByIdQueryHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }
        public async Task<MedicineDto> Handle(GetMedicineByIdQuery request, CancellationToken cancellationToken)
        {
            var spec = new GetMedicineSpec(request.Id);
            var model = await _uow.Repository<Medicine>().GetEntityWithSpec(spec,cancellationToken);
            if (model == null)
                throw new NotFoundException($"محصولی با شناسه {request.Id} یافت نشد");
            return model.Adapt<MedicineDto>();
        }
    }
}
