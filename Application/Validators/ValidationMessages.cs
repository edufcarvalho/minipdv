namespace minipdv.Application.Validators;

public static class ValidationMessages
{
    public const string RequiredField = "Este campo é obrigatório";
    public const string MaxLength200 = "Deve ter no máximo 200 caracteres";
    public const string MaxLength100 = "Deve ter no máximo 100 caracteres";
    public const string MaxLength128 = "Deve ter no máximo 128 caracteres";
    public const string LoginAlphanumericOnly = "Login deve conter apenas letras e números";
    public const string CpfFormat = "CPF deve seguir o formato 123.456.789-01";
    public const string CnpjFormat = "CNPJ deve conter exatamente 14 dígitos numéricos.";
    public const string CodBarraMinLength = "CodBarra deve ter pelo menos 8 dígitos";
    public const string RegistroMSRequired = "RegistroMS é obrigatório para medicamentos controlados";
    public const string TipoUsuarioInvalid = "Tipo de usuário deve ser Usuario, Farmaceutico ou Administrador";
    public const string TipoInvalid = "Tipo deve ser 'Usuario', 'Farmaceutico' ou 'Administrador'";
    public const string CrfRequired = "CRF é obrigatório para Farmacêutico";
    public const string VendaMinimoProduto = "A venda deve conter pelo menos um produto";
    public const string PasswordMinLength = "Senha deve ter pelo menos 8 caracteres";
    public const string PasswordUppercase = "Senha deve conter pelo menos uma letra maiúscula";
    public const string PasswordLowercase = "Senha deve conter pelo menos uma letra minúscula";
    public const string PasswordDigit = "Senha deve conter pelo menos um número";
}
