SET IDENTITY_INSERT dbo.ProdutoTipos ON;

MERGE INTO dbo.ProdutoTipos AS target
USING (VALUES
    (1, N'Medicamento Referência', SYSUTCDATETIME(), NULL),
    (2, N'Medicamento Genérico',     SYSUTCDATETIME(), NULL),
    (3, N'Medicamento Similar',      SYSUTCDATETIME(), NULL)
) AS source (Id, Nome, CriadoEm, AtualizadoEm)
ON target.Id = source.Id
WHEN NOT MATCHED THEN
    INSERT (Id, Nome, CriadoEm, AtualizadoEm)
    VALUES (source.Id, source.Nome, source.CriadoEm, source.AtualizadoEm);

SET IDENTITY_INSERT dbo.ProdutoTipos OFF;
