SET IDENTITY_INSERT dbo.Vendas ON;

DECLARE @robertoId INT = (SELECT Id FROM dbo.Usuarios WHERE Login = 'roberto.balcao');
DECLARE @camilaId INT = (SELECT Id FROM dbo.Usuarios WHERE Login = 'camila.balcao');

MERGE INTO dbo.Vendas AS target
USING (VALUES
    (1,  1,  @robertoId, '2025-05-15T14:30:00', 31.40),
    (2,  2,  @camilaId,   '2025-05-16T10:15:00', 44.50),
    (3,  3,  @robertoId, '2025-05-18T16:45:00', 72.30),
    (4,  4,  @camilaId,   '2025-05-20T09:00:00', 32.00),
    (5,  5,  @robertoId, '2025-05-22T11:20:00', 87.00),
    (6,  6,  @camilaId,   '2025-05-25T15:10:00', 45.90),
    (7,  7,  @robertoId, '2025-05-28T13:00:00', 100.00),
    (8,  8,  @camilaId,   '2025-06-01T10:30:00', 85.50),
    (9,  9,  @robertoId, '2025-06-03T14:45:00', 81.30),
    (10, 16, @camilaId,   '2025-06-05T17:00:00', 27.50)
) AS source (Id, ClienteId, VendedorId, CriadoEm, Total)
ON target.Id = source.Id
WHEN NOT MATCHED THEN
    INSERT (Id, ClienteId, VendedorId, CriadoEm, Total)
    VALUES (source.Id, source.ClienteId, source.VendedorId, source.CriadoEm, source.Total);

SET IDENTITY_INSERT dbo.Vendas OFF;

MERGE INTO dbo.VendaItem AS target
USING (VALUES
    (1,  1,  1,  1, 12.50),
    (1,  2,  2,  1, 18.90),
    (2,  8,  1,  1, 15.60),
    (2,  19, 2,  1, 28.90),
    (3,  5,  1,  2, 25.00),
    (3,  3,  2,  1, 22.30),
    (4,  12, 1,  1, 32.00),
    (5,  11, 1,  2, 19.50),
    (5,  30, 2,  1, 48.00),
    (6,  13, 1,  1, 45.90),
    (7,  15, 1,  1, 65.00),
    (7,  26, 2,  1, 35.00),
    (8,  16, 1,  1, 85.50),
    (9,  21, 1,  1, 16.80),
    (9,  25, 2,  1, 42.50),
    (9,  17, 3,  1, 22.00),
    (10, 27, 1,  1, 12.00),
    (10, 28, 2,  1, 15.50)
) AS source (VendaId, ProdutoId, Posicao, Quantidade, PrecoUnitario)
ON target.VendaId = source.VendaId AND target.ProdutoId = source.ProdutoId AND target.Posicao = source.Posicao
WHEN NOT MATCHED THEN
    INSERT (VendaId, ProdutoId, Posicao, Quantidade, PrecoUnitario)
    VALUES (source.VendaId, source.ProdutoId, source.Posicao, source.Quantidade, source.PrecoUnitario);

UPDATE p
SET p.Estoque = p.Estoque - vi.Quantidade
FROM dbo.Produtos p
INNER JOIN dbo.VendaItem vi ON p.Id = vi.ProdutoId;

UPDATE e
SET e.Quantidade = e.Quantidade - vi.Quantidade
FROM dbo.ProdutoEstoques e
INNER JOIN dbo.VendaItem vi ON e.ProdutoId = vi.ProdutoId
