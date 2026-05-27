using minipdv.Domain.Entities;

namespace minipdv.Application.Interfaces;

public interface IPrincipioAtivoService
{
    Task<IEnumerable<PrincipioAtivo>> GetAllAsync();
    Task<PrincipioAtivo?> GetByIdAsync(int id);
    Task<PrincipioAtivo?> GetByNomeAsync(string nome);
    Task<PrincipioAtivo> AddAsync(PrincipioAtivo entity);
    Task UpdateAsync(PrincipioAtivo entity);
    Task DeleteAsync(int id);
    Task<bool> ExistsAsync(int id);
}
