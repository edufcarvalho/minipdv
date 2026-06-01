SET IDENTITY_INSERT dbo.Clientes ON;

MERGE INTO dbo.Clientes AS target
USING (VALUES
    (1,  N'João Silva',          N'12345678901', 11, SYSUTCDATETIME(), NULL),
    (2,  N'Maria Oliveira',      N'23456789012', 12, SYSUTCDATETIME(), NULL),
    (3,  N'Carlos Santos',       N'34567890123', 13, SYSUTCDATETIME(), NULL),
    (4,  N'Ana Costa',           N'45678901234', 14, SYSUTCDATETIME(), NULL),
    (5,  N'Pedro Ferreira',      N'56789012345', 15, SYSUTCDATETIME(), NULL),
    (6,  N'Julia Rodrigues',     N'67890123456', 16, SYSUTCDATETIME(), NULL),
    (7,  N'Lucas Almeida',       N'78901234567', 17, SYSUTCDATETIME(), NULL),
    (8,  N'Mariana Lima',        N'89012345678', 18, SYSUTCDATETIME(), NULL),
    (9,  N'Rafael Souza',        N'90123456789', 19, SYSUTCDATETIME(), NULL),
    (10, N'Beatriz Pereira',     N'01234567890', 20, SYSUTCDATETIME(), NULL),
    (11, N'Fernanda Carvalho',   N'11122233344', NULL, SYSUTCDATETIME(), NULL),
    (12, N'Ricardo Nunes',       N'22233344455', NULL, SYSUTCDATETIME(), NULL),
    (13, N'Patricia Mendes',     N'33344455566', NULL, SYSUTCDATETIME(), NULL),
    (14, N'Gustavo Barbosa',     N'44455566677', NULL, SYSUTCDATETIME(), NULL),
    (15, N'Vanessa Cardoso',     N'55566677788', NULL, SYSUTCDATETIME(), NULL),
    (16, N'Cliente Balcão',      N'00000000000', NULL, SYSUTCDATETIME(), NULL)
) AS source (Id, Nome, Cpf, ContatoId, CriadoEm, AtualizadoEm)
ON target.Id = source.Id
WHEN NOT MATCHED THEN
    INSERT (Id, Nome, Cpf, ContatoId, CriadoEm, AtualizadoEm)
    VALUES (source.Id, source.Nome, source.Cpf, source.ContatoId, source.CriadoEm, source.AtualizadoEm);

SET IDENTITY_INSERT dbo.Clientes OFF;
