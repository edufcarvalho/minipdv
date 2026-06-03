namespace minipdv.Application.DTOs;

public record ReceitaResponse(
    int Id,
    DateTime DataReceita,
    DateTime DataCadastro,
    int PrescritorId,
    int PacienteId,
    int? CompradorId,
    DateTime CriadoEm,
    DateTime? AtualizadoEm
);
