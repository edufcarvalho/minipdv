namespace minipdv.Infrastructure.Data.Interfaces;

public interface IDatabaseInitializer
{
    bool IsDatabaseSeeded();
    Task SeedAsync();
    Task SeedDataAsync();
}
