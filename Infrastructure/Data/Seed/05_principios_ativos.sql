SET IDENTITY_INSERT dbo.PrincipiosAtivos ON;

MERGE INTO dbo.PrincipiosAtivos AS target
USING (VALUES
    (1,  N'Dipirona Sódica',              SYSUTCDATETIME(), NULL),
    (2,  N'Paracetamol',                  SYSUTCDATETIME(), NULL),
    (3,  N'Ibuprofeno',                   SYSUTCDATETIME(), NULL),
    (4,  N'Ácido Acetilsalicílico',       SYSUTCDATETIME(), NULL),
    (5,  N'Amoxicilina',                  SYSUTCDATETIME(), NULL),
    (6,  N'Azitromicina',                 SYSUTCDATETIME(), NULL),
    (7,  N'Cefalexina',                   SYSUTCDATETIME(), NULL),
    (8,  N'Losartana Potássica',          SYSUTCDATETIME(), NULL),
    (9,  N'Enalapril',                    SYSUTCDATETIME(), NULL),
    (10, N'Atenolol',                     SYSUTCDATETIME(), NULL),
    (11, N'Metformina',                   SYSUTCDATETIME(), NULL),
    (12, N'Omeprazol',                    SYSUTCDATETIME(), NULL),
    (13, N'Sertralina',                   SYSUTCDATETIME(), NULL),
    (14, N'Fluoxetina',                   SYSUTCDATETIME(), NULL),
    (15, N'Clonazepam',                   SYSUTCDATETIME(), NULL),
    (16, N'Metilfenidato',                SYSUTCDATETIME(), NULL),
    (17, N'Loratadina',                   SYSUTCDATETIME(), NULL),
    (18, N'Dexametasona',                 SYSUTCDATETIME(), NULL),
    (19, N'Sinvastatina',                 SYSUTCDATETIME(), NULL),
    (20, N'Levonorgestrel + Etinilestradiol', SYSUTCDATETIME(), NULL)
) AS source (Id, Nome, CriadoEm, AtualizadoEm)
ON target.Id = source.Id
WHEN NOT MATCHED THEN
    INSERT (Id, Nome, CriadoEm, AtualizadoEm)
    VALUES (source.Id, source.Nome, source.CriadoEm, source.AtualizadoEm);

SET IDENTITY_INSERT dbo.PrincipiosAtivos OFF;
