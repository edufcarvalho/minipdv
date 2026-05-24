namespace minipdv.Application.Interfaces;

public interface IDatabaseInitializer
{
    bool IsDatabaseSeeded();
    void Seed();
    Task SeedAsync();
}
