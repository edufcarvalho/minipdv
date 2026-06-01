namespace minipdv.Infrastructure.Data.Interfaces;

public interface IDatabaseInitializer
{
    bool IsDatabaseSeeded();
    void Seed();
    Task SeedAsync();
    void SeedData();
    Task SeedDataAsync();
}
