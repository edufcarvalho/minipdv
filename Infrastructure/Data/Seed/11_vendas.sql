SET IDENTITY_INSERT dbo.Vendas ON;

DECLARE @robertoId INT = (SELECT Id FROM dbo.Usuarios WHERE Login = 'roberto.balcao');
DECLARE @camilaId INT = (SELECT Id FROM dbo.Usuarios WHERE Login = 'camila.balcao');

MERGE INTO dbo.Vendas AS target
USING (VALUES
    (1,  1,  @robertoId, '2025-05-15T14:30:00'),
    (2,  2,  @camilaId,   '2025-05-16T10:15:00'),
    (3,  3,  @robertoId, '2025-05-18T16:45:00'),
    (4,  4,  @camilaId,   '2025-05-20T09:00:00'),
    (5,  5,  @robertoId, '2025-05-22T11:20:00'),
    (6,  6,  @camilaId,   '2025-05-25T15:10:00'),
    (7,  7,  @robertoId, '2025-05-28T13:00:00'),
    (8,  8,  @camilaId,   '2025-06-01T10:30:00'),
    (9,  9,  @robertoId, '2025-06-03T14:45:00'),
    (10, 16, @camilaId,   '2025-06-05T17:00:00')
) AS source (Id, ClienteId, VendedorId, CriadoEm)
ON target.Id = source.Id
WHEN NOT MATCHED THEN
    INSERT (Id, ClienteId, VendedorId, CriadoEm)
    VALUES (source.Id, source.ClienteId, source.VendedorId, source.CriadoEm);

SET IDENTITY_INSERT dbo.Vendas OFF;

MERGE INTO dbo.VendaItem AS target
USING (VALUES
    (1,  1,  1),
    (1,  2,  1),
    (2,  8,  1),
    (2,  19, 1),
    (3,  5,  2),
    (3,  3,  1),
    (4,  12, 1),
    (5,  11, 2),
    (5,  30, 1),
    (6,  13, 1),
    (7,  15, 1),
    (7,  26, 1),
    (8,  16, 1),
    (9,  21, 1),
    (9,  25, 1),
    (9,  17, 1),
    (10, 27, 1),
    (10, 28, 1)
) AS source (VendaId, ProdutoId, Quantidade)
ON target.VendaId = source.VendaId AND target.ProdutoId = source.ProdutoId
WHEN NOT MATCHED THEN
    INSERT (VendaId, ProdutoId, Quantidade)
    VALUES (source.VendaId, source.ProdutoId, source.Quantidade);

UPDATE p
SET p.Estoque = p.Estoque - vi.Quantidade
FROM dbo.Produtos p
INNER JOIN dbo.VendaItem vi ON p.Id = vi.ProdutoId;

UPDATE e
SET e.Quantidade = e.Quantidade - vi.Quantidade
FROM dbo.ProdutoEstoques e
INNER JOIN dbo.VendaItem vi ON e.ProdutoId = vi.ProdutoId
