using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using minipdv.Application.Interfaces;
using minipdv.Domain.Entities;
using minipdv.Infrastructure.Data.Context;

namespace minipdv.Presentation.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Policy = Policies.RequireFarmaceutico)]
public class FabricantesController : CrudControllerBase<Fabricante, IFabricanteService>
{
    public FabricantesController(IFabricanteService service, MiniPDVContext context)
        : base(service, context) { }
}
