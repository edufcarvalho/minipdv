namespace minipdv.Domain.Enums;

public enum Conselho
{
    CRM,
    CRO,
    CRMV,
    CRF,
    COREN,
    CRN,
    CREFITO,
    CRFA,
    CRBIO,
    CRP
}

public static class ConselhoExtensions
{
    public static string GetSigla(this Conselho conselho) => conselho.ToString();

    public static string GetNome(this Conselho conselho) => conselho switch
    {
        Conselho.CRM => "Conselho Regional de Medicina",
        Conselho.CRO => "Conselho Regional de Odontologia",
        Conselho.CRMV => "Conselho Regional de Medicina Veterinária",
        Conselho.CRF => "Conselho Regional de Farmácia",
        Conselho.COREN => "Conselho Regional de Enfermagem",
        Conselho.CRN => "Conselho Regional de Nutrição",
        Conselho.CREFITO => "Conselho Regional de Fisioterapia e Terapia Ocupacional",
        Conselho.CRFA => "Conselho Regional de Fonoaudiologia",
        Conselho.CRBIO => "Conselho Regional de Biologia",
        Conselho.CRP => "Conselho Regional de Psicologia",
        _ => conselho.ToString()
    };
}
