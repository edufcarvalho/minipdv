using Microsoft.Extensions.Logging;
using minipdv.Domain.Entities;
using minipdv.Domain.Interfaces;
using minipdv.Infrastructure.Data.Context;

namespace minipdv.Infrastructure.Data.Repositories;

public class AdministradorRepository : Repository<Administrador>, IAdministradorRepository
{
    public AdministradorRepository(MiniPDVContext context, ILogger<AdministradorRepository> logger) : base(context, logger) { }
}
