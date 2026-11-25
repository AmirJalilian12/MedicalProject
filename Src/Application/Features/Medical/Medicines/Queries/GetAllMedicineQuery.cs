using Application.DTOs.Contracts;
using Application.DTOs.Medicines;
using Application.Features.Medical.Medicines.Spec;
using Application.Wrappers;
using Domain.Entities.MedicalEntities.MedicineEntities;
using Mapster;
using MediatR;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Features.Medical.Medicines.Queries
{
    public class GetAllMedicineQuery :
        RequestParametersBasic,
        IRequest<PaginationResponse<MedicineDto>>,
        ICacheQuery
    {
        public int HoursSaveData => 1;
    }

    public class GetAllMedicineQueryHandler :
        IRequestHandler<GetAllMedicineQuery, PaginationResponse<MedicineDto>>
    {
        private readonly IUnitOfWork _uow;

        public GetAllMedicineQueryHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<PaginationResponse<MedicineDto>> Handle(
            GetAllMedicineQuery request,
            CancellationToken cancellationToken)
        {
            var spec = new GetMedicineSpec(request);

            var result = await _uow
                .Repository<Medicine>()
                .ListAnySpec(spec, cancellationToken);

            var count = await _uow
                .Repository<Medicine>()
                .CountAsyncSpec(new MedicineCountSpec(request), cancellationToken);

            var model = result.Adapt<List<MedicineDto>>();

            return new PaginationResponse<MedicineDto>(
                request.PageIndex,
                request.PageSize,
                count,
                model);
        }
    }
}
