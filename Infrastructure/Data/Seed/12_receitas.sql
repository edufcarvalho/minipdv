SET IDENTITY_INSERT dbo.Receitas ON;

MERGE INTO dbo.Receitas AS target
USING (VALUES
    (1, '2025-05-15', '2025-05-20T14:30:00', 1,  7,  7,  7, '2025-05-20T14:30:00'),
    (2, '2025-06-01', '2025-06-01T10:30:00', 4,  8,  8,  8, '2025-06-01T10:30:00'),
    (3, '2025-05-28', '2025-05-28T11:00:00', 1,  10, 10, NULL, '2025-05-28T11:00:00'),
    (4, '2025-06-10', '2025-06-10T09:15:00', 3,  11, 11, NULL, '2025-06-10T09:15:00')
) AS source (Id, DataReceita, DataCadastro, PrescritorId, PacienteId, CompradorId, VendaId, CriadoEm)
ON target.Id = source.Id
WHEN NOT MATCHED THEN
    INSERT (Id, DataReceita, DataCadastro, PrescritorId, PacienteId, CompradorId, VendaId, CriadoEm)
    VALUES (source.Id, source.DataReceita, source.DataCadastro, source.PrescritorId, source.PacienteId, source.CompradorId, source.VendaId, source.CriadoEm);

SET IDENTITY_INSERT dbo.Receitas OFF;

MERGE INTO dbo.ReceitaProdutoEstoque AS target
USING (VALUES
    (1, 15, N'250410B', 1),
    (1, 26, N'250501C', 1),
    (2, 16, N'250510C', 1),
    (3, 29, N'250410D', 1),
    (4, 15, N'250115A', 1)
) AS source (ReceitaId, ProdutoId, Lote, Quantidade)
ON target.ReceitaId = source.ReceitaId AND target.ProdutoId = source.ProdutoId AND target.Lote = source.Lote
WHEN NOT MATCHED THEN
    INSERT (ReceitaId, ProdutoId, Lote, Quantidade)
    VALUES (source.ReceitaId, source.ProdutoId, source.Lote, source.Quantidade);

UPDATE pe
SET pe.Quantidade = pe.Quantidade - rpe.Quantidade
FROM dbo.ProdutoEstoques pe
INNER JOIN dbo.ReceitaProdutoEstoque rpe ON pe.ProdutoId = rpe.ProdutoId AND pe.Lote = rpe.Lote;

UPDATE p
SET p.Estoque = p.Estoque - rpe.Quantidade
FROM dbo.Produtos p
INNER JOIN dbo.ReceitaProdutoEstoque rpe ON p.Id = rpe.ProdutoId;
