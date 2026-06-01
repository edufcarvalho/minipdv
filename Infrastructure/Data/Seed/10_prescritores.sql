SET IDENTITY_INSERT dbo.Prescritores ON;

MERGE INTO dbo.Prescritores AS target
USING (VALUES
    (1,  N'120456',  N'Dra. Camila Rocha',        N'CRM',  N'SP', SYSUTCDATETIME(), NULL),
    (2,  N'789012',  N'Dr. Marcos Andrade',        N'CRM',  N'RJ', SYSUTCDATETIME(), NULL),
    (3,  N'543210',  N'Dra. Renata Vasconcelos',   N'CRM',  N'MG', SYSUTCDATETIME(), NULL),
    (4,  N'987654',  N'Dr. Paulo Nogueira',        N'CRM',  N'SP', SYSUTCDATETIME(), NULL),
    (5,  N'456789',  N'Dra. Juliana Barros',       N'CRM',  N'PR', SYSUTCDATETIME(), NULL),
    (6,  N'321098',  N'Dr. Eduardo Teixeira',      N'CRM',  N'RS', SYSUTCDATETIME(), NULL),
    (7,  N'20456',   N'Dra. Adriana Farias',       N'CRO',  N'SP', SYSUTCDATETIME(), NULL),
    (8,  N'10567',   N'Dr. Rodrigo Campos',        N'CRMV', N'SP', SYSUTCDATETIME(), NULL),
    (9,  N'98701',   N'Dra. Sandra Macedo',        N'CRM',  N'DF', SYSUTCDATETIME(), NULL),
    (10, N'112345',  N'Dr. Henrique Lopes',        N'CRM',  N'BA', SYSUTCDATETIME(), NULL),
    (11, N'55678',   N'Dra. Claudia Freitas',      N'CRM',  N'SC', SYSUTCDATETIME(), NULL),
    (12, N'33456',   N'Dr. Fernando Castro',       N'CRM',  N'PE', SYSUTCDATETIME(), NULL)
) AS source (Id, Numero, Nome, Conselho, Uf, CriadoEm, AtualizadoEm)
ON target.Id = source.Id
WHEN NOT MATCHED THEN
    INSERT (Id, Numero, Nome, Conselho, Uf, CriadoEm, AtualizadoEm)
    VALUES (source.Id, source.Numero, source.Nome, source.Conselho, source.Uf, source.CriadoEm, source.AtualizadoEm);

SET IDENTITY_INSERT dbo.Prescritores OFF;
