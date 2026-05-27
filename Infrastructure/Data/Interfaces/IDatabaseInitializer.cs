namespace minipdv.Data.Interfaces;

public interface IDatabaseInitializer
{
    bool IsDatabaseSeeded();
    void Seed();
    Task SeedAsync();
}
