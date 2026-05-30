namespace minipdv.Domain.Enums;

public enum UF
{
    AC,
    AL,
    AP,
    AM,
    BA,
    CE,
    DF,
    ES,
    GO,
    MA,
    MT,
    MS,
    MG,
    PA,
    PB,
    PR,
    PE,
    PI,
    RJ,
    RN,
    RS,
    RO,
    RR,
    SC,
    SP,
    SE,
    TO
}

public static class UfExtensions
{
    public static string GetSigla(this UF uf) => uf.ToString();

    public static string GetNome(this UF uf) => uf switch
    {
        UF.AC => "Acre",
        UF.AL => "Alagoas",
        UF.AP => "Amapá",
        UF.AM => "Amazonas",
        UF.BA => "Bahia",
        UF.CE => "Ceará",
        UF.DF => "Distrito Federal",
        UF.ES => "Espírito Santo",
        UF.GO => "Goiás",
        UF.MA => "Maranhão",
        UF.MT => "Mato Grosso",
        UF.MS => "Mato Grosso do Sul",
        UF.MG => "Minas Gerais",
        UF.PA => "Pará",
        UF.PB => "Paraíba",
        UF.PR => "Paraná",
        UF.PE => "Pernambuco",
        UF.PI => "Piauí",
        UF.RJ => "Rio de Janeiro",
        UF.RN => "Rio Grande do Norte",
        UF.RS => "Rio Grande do Sul",
        UF.RO => "Rondônia",
        UF.RR => "Roraima",
        UF.SC => "Santa Catarina",
        UF.SP => "São Paulo",
        UF.SE => "Sergipe",
        UF.TO => "Tocantins",
        _ => uf.ToString()
    };
}
