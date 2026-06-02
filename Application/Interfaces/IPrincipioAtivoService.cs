using minipdv.Application.Interfaces;
using minipdv.Domain.Entities;

namespace minipdv.Application.Interfaces;

public interface IPrincipioAtivoService : ICrudService<PrincipioAtivo>
{
    Task<PrincipioAtivo?> GetByNomeAsync(string nome);
}
