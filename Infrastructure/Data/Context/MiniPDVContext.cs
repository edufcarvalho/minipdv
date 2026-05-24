using Microsoft.EntityFrameworkCore;

namespace minipdv.Infrastructure.Data.Context;

public class MiniPDVContext : DbContext
{
    public MiniPDVContext(DbContextOptions<MiniPDVContext> options) : base(options)
    {
    }
}
