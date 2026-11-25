using Application.DTOs.Medicines;
using Application.Features.Medical.Medicines.Queries;
using Microsoft.AspNetCore.Mvc;
using Web.Common;

namespace Web.Controllers
{
    public class MedicineController : BaseApiController
    {
        [HttpGet]
        public async Task<ActionResult<IEnumerable<MedicineDto>>> Get([FromQuery] GetAllMedicineQuery request, CancellationToken cancellationToken)
        {
            return Ok(await Mediator.Send(request, cancellationToken));
        }
        [HttpGet("{id:int}")]
        public async Task<ActionResult<MedicineDto>> Get([FromRoute] int id , CancellationToken cancellationToken)
        {
            return Ok(await Mediator.Send(new GetMedicineByIdQuery(id),cancellationToken));
        }
    }
}
