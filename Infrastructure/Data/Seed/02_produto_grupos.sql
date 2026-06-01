SET IDENTITY_INSERT dbo.ProdutoGrupos ON;

MERGE INTO dbo.ProdutoGrupos AS target
USING (VALUES
    (1,  N'Analgésicos e Antitérmicos',         1, SYSUTCDATETIME(), NULL),
    (2,  N'Anti-inflamatórios',                  1, SYSUTCDATETIME(), NULL),
    (3,  N'Antibióticos',                        1, SYSUTCDATETIME(), NULL),
    (4,  N'Anti-hipertensivos',                  1, SYSUTCDATETIME(), NULL),
    (5,  N'Antidiabéticos Orais',                1, SYSUTCDATETIME(), NULL),
    (6,  N'Antidepressivos',                     1, SYSUTCDATETIME(), NULL),
    (7,  N'Ansiolíticos e Controlados',          1, SYSUTCDATETIME(), NULL),
    (8,  N'Antialérgicos e Anti-histamínicos',   1, SYSUTCDATETIME(), NULL),
    (9,  N'Corticosteroides',                    1, SYSUTCDATETIME(), NULL),
    (10, N'Gastroprotetores e Antiácidos',       1, SYSUTCDATETIME(), NULL),
    (11, N'Hormônios e Anticoncepcionais',       1, SYSUTCDATETIME(), NULL),
    (12, N'Vitaminas e Suplementos',             1, SYSUTCDATETIME(), NULL),
    (13, N'Hipolipemiantes',                     1, SYSUTCDATETIME(), NULL)
) AS source (Id, Nome, Ativo, CriadoEm, AtualizadoEm)
ON target.Id = source.Id
WHEN NOT MATCHED THEN
    INSERT (Id, Nome, Ativo, CriadoEm, AtualizadoEm)
    VALUES (source.Id, source.Nome, source.Ativo, source.CriadoEm, source.AtualizadoEm);

SET IDENTITY_INSERT dbo.ProdutoGrupos OFF;
