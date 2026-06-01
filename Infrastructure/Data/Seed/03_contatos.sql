SET IDENTITY_INSERT dbo.Contatos ON;

MERGE INTO dbo.Contatos AS target
USING (VALUES
    (1,  N'contato@ems.com.br',       N'(11) 3874-8800', SYSUTCDATETIME(), NULL),
    (2,  N'contato@eurofarma.com.br', N'(11) 5090-8600', SYSUTCDATETIME(), NULL),
    (3,  N'sac@ache.com.br',          N'0800 701 6900',  SYSUTCDATETIME(), NULL),
    (4,  N'atendimento@sanofi.com',   N'0800 703 0014',  SYSUTCDATETIME(), NULL),
    (5,  N'sac@bayer.com',            N'0800 723 1010',  SYSUTCDATETIME(), NULL),
    (6,  N'atendimento@novartis.com', N'0800 888 3003',  SYSUTCDATETIME(), NULL),
    (7,  N'sac@pfizer.com',           N'0800 770 1575',  SYSUTCDATETIME(), NULL),
    (8,  N'contato@hypera.com.br',    N'0800 979 2070',  SYSUTCDATETIME(), NULL),
    (9,  N'sac@libbs.com.br',         N'0800 013 5044',  SYSUTCDATETIME(), NULL),
    (10, N'cristalia@cristalia.com.br', N'(19) 3838-8000', SYSUTCDATETIME(), NULL),
    (11, N'joao.silva@gmail.com',     N'(11) 99876-5432', SYSUTCDATETIME(), NULL),
    (12, N'maria.oliveira@hotmail.com', N'(11) 98765-4321', SYSUTCDATETIME(), NULL),
    (13, N'carlos.santos@outlook.com',  N'(21) 99654-3210', SYSUTCDATETIME(), NULL),
    (14, N'ana.costa@gmail.com',        N'(31) 98543-2109', SYSUTCDATETIME(), NULL),
    (15, N'pedro.ferreira@yahoo.com',   N'(41) 97432-1098', SYSUTCDATETIME(), NULL),
    (16, N'julia.rodrigues@gmail.com',  N'(51) 96321-0987', SYSUTCDATETIME(), NULL),
    (17, N'lucas.almeida@hotmail.com',  N'(11) 95210-9876', SYSUTCDATETIME(), NULL),
    (18, N'mariana.lima@outlook.com',   N'(27) 94109-8765', SYSUTCDATETIME(), NULL),
    (19, N'rafael.souza@gmail.com',     N'(11) 93098-7654', SYSUTCDATETIME(), NULL),
    (20, N'beatriz.pereira@hotmail.com',N'(19) 92087-6543', SYSUTCDATETIME(), NULL),
    (21, N'felipe.farma@minipdv.com',   N'(11) 4002-8922',  SYSUTCDATETIME(), NULL),
    (22, N'amanda.farma@minipdv.com',   N'(11) 4002-8923',  SYSUTCDATETIME(), NULL),
    (23, N'roberto.balcao@minipdv.com', N'(11) 4002-8924',  SYSUTCDATETIME(), NULL),
    (24, N'camila.balcao@minipdv.com',  N'(11) 4002-8925',  SYSUTCDATETIME(), NULL)
) AS source (Id, Email, Telefone, CriadoEm, AtualizadoEm)
ON target.Id = source.Id
WHEN NOT MATCHED THEN
    INSERT (Id, Email, Telefone, CriadoEm, AtualizadoEm)
    VALUES (source.Id, source.Email, source.Telefone, source.CriadoEm, source.AtualizadoEm);

UPDATE u SET u.ContatoId = 21
FROM dbo.Usuarios u
WHERE u.Login = 'felipe.farmacia' AND u.ContatoId IS NULL;

UPDATE u SET u.ContatoId = 22
FROM dbo.Usuarios u
WHERE u.Login = 'amanda.farmacia' AND u.ContatoId IS NULL;

UPDATE u SET u.ContatoId = 23
FROM dbo.Usuarios u
WHERE u.Login = 'roberto.balcao' AND u.ContatoId IS NULL;

UPDATE u SET u.ContatoId = 24
FROM dbo.Usuarios u
WHERE u.Login = 'camila.balcao' AND u.ContatoId IS NULL;
    
SET IDENTITY_INSERT dbo.Contatos OFF;
