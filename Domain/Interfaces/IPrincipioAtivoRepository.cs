using minipdv.Domain.Entities;

namespace minipdv.Domain.Interfaces;

public interface IPrincipioAtivoRepository : IRepository<PrincipioAtivo>
{
    Task<PrincipioAtivo?> GetByNomeAsync(string nome);
}
