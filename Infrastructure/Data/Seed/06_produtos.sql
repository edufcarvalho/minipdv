SET IDENTITY_INSERT dbo.Produtos ON;

MERGE INTO dbo.Produtos AS target
USING (VALUES
    (1,  N'Dipirona Sódica 500mg/mL Gotas 20mL Genérico',     1, 10000001, 0, N'500mg/mL', N'1234500010011', 12.50, 150, 1,  1,  1,  NULL, NULL, SYSUTCDATETIME()),
    (2,  N'Paracetamol 750mg 20 Comprimidos Genérico',        1, 10000002, 0, N'750mg',    N'1234500020022', 18.90, 200, 1,  2,  2,  NULL, NULL, SYSUTCDATETIME()),
    (3,  N'Ibuprofeno 600mg 30 Comprimidos Genérico',         1, 10000003, 0, N'600mg',    N'1345600030033', 22.30, 120, 2,  3,  3,  NULL, NULL, SYSUTCDATETIME()),
    (4,  N'AAS 100mg 30 Comprimidos Genérico',                1, 10000004, 0, N'100mg',    N'1456700040044', 8.50,  180, 2,  4,  4,  NULL, NULL, SYSUTCDATETIME()),
    (5,  N'Amoxicilina 500mg 21 Cápsulas Genérico',           1, 10000005, 0, N'500mg',    N'1567800050055', 25.00, 80,  3,  5,  5,  NULL, NULL, SYSUTCDATETIME()),
    (6,  N'Azitromicina 500mg 5 Comprimidos Genérico',        1, 10000006, 0, N'500mg',    N'1678900060066', 35.90, 60,  3,  6,  6,  NULL, NULL, SYSUTCDATETIME()),
    (7,  N'Cefalexina 500mg 14 Cápsulas Genérico',            1, 10000007, 0, N'500mg',    N'1789000070077', 28.40, 90,  3,  1,  7,  NULL, NULL, SYSUTCDATETIME()),
    (8,  N'Losartana Potássica 50mg 30 Comprimidos Genérico', 1, 10000008, 0, N'50mg',     N'1890100080088', 15.60, 250, 4,  7,  8,  NULL, NULL, SYSUTCDATETIME()),
    (9,  N'Enalapril 20mg 30 Comprimidos Genérico',           1, 10000009, 0, N'20mg',     N'1901200090099', 12.80, 130, 4,  2,  9,  NULL, NULL, SYSUTCDATETIME()),
    (10, N'Atenolol 50mg 30 Comprimidos Genérico',            1, 10000010, 0, N'50mg',     N'1012300100100', 14.20, 110, 4,  8,  10, NULL, NULL, SYSUTCDATETIME()),
    (11, N'Metformina 850mg 30 Comprimidos Genérico',         1, 10000011, 0, N'850mg',    N'1123400110111', 19.50, 170, 5,  9,  11, NULL, NULL, SYSUTCDATETIME()),
    (12, N'Omeprazol 20mg 28 Cápsulas Genérico',              1, 10000012, 0, N'20mg',     N'1234500120122', 32.00, 300, 10, 3,  12, NULL, NULL, SYSUTCDATETIME()),
    (13, N'Sertralina 50mg 30 Comprimidos Genérico',          1, 10000013, 0, N'50mg',     N'1345600130133', 45.90, 95,  6,  10, 13, NULL, NULL, SYSUTCDATETIME()),
    (14, N'Fluoxetina 20mg 30 Cápsulas Genérico',             1, 10000014, 0, N'20mg',     N'1456700140144', 55.30, 140, 6,  1,  14, NULL, NULL, SYSUTCDATETIME()),
    (15, N'Rivotril 2mg 30 Comprimidos',                      1, 10000015, 1, N'2mg',      N'1567800150155', 65.00, 45,  7,  4,  15, N'Ansiolítico',      N'B1', SYSUTCDATETIME()),
    (16, N'Ritalina 10mg 30 Comprimidos',                     1, 10000016, 1, N'10mg',     N'1678900160166', 85.50, 25,  7,  6,  16, N'Psicoestimulante',  N'A3', SYSUTCDATETIME()),
    (17, N'Loratadina 10mg 12 Comprimidos Genérico',          1, 10000017, 0, N'10mg',     N'1789000170177', 22.00, 160, 8,  2,  17, NULL, NULL, SYSUTCDATETIME()),
    (18, N'Predsim 20mg 10 Comprimidos',                      1, 10000018, 0, N'20mg',     N'1890100180188', 35.60, 55,  9,  10, 18, NULL, NULL, SYSUTCDATETIME()),
    (19, N'Sinvastatina 20mg 30 Comprimidos Genérico',        1, 10000019, 0, N'20mg',     N'1901200190199', 28.90, 190, 13, 8,  19, NULL, NULL, SYSUTCDATETIME()),
    (20, N'Ciclo 21 21 Comprimidos',                          1, 10000020, 0, N'0,15mg+0,03mg', N'1012300200200', 42.00, 85, 11, 5, 20, NULL, NULL, SYSUTCDATETIME()),
    (21, N'Neosoro Adulto 0,5mg/mL Solução Nasal 30mL',       1, 10000021, 0, N'0,5mg/mL', N'1123400210211', 16.80, 100, 8,  8,  17, NULL, NULL, SYSUTCDATETIME()),
    (22, N'Decadron 4mg 20 Comprimidos',                      1, 10000022, 0, N'4mg',      N'1234500220222', 38.20, 75,  9,  3,  18, NULL, NULL, SYSUTCDATETIME()),
    (23, N'Tylenol 750mg 20 Comprimidos',                     1, 10000023, 0, N'750mg',    N'1345600230233', 22.50, 220, 1,  1,  2,  NULL, NULL, SYSUTCDATETIME()),
    (24, N'Novalgina 1g 10 Comprimidos',                      1, 10000024, 0, N'1g',       N'1456700240244', 18.00, 135, 1,  4,  1,  NULL, NULL, SYSUTCDATETIME()),
    (25, N'Allegra 180mg 10 Comprimidos',                     1, 10000025, 0, N'180mg',    N'1567800250255', 42.50, 65,  8,  4,  17, NULL, NULL, SYSUTCDATETIME()),
    (26, N'Clonazepam 2mg 30 Comprimidos Genérico',           1, 10000026, 1, N'2mg',      N'1678900260266', 35.00, 35,  7,  1,  15, N'Ansiolítico',      N'B1', SYSUTCDATETIME()),
    (27, N'Dorflex 10 Comprimidos',                           1, 10000027, 0, N'300mg+35mg+50mg', N'1789000270277', 12.00, 145, 1,  4,  1,  NULL, NULL, SYSUTCDATETIME()),
    (28, N'Cimegripe 20 Comprimidos',                         1, 10000028, 0, N'400mg+4mg+4mg', N'1890100280288', 15.50, 90,  1,  1,  2,  NULL, NULL, SYSUTCDATETIME()),
    (29, N'Topiramato 50mg 60 Comprimidos Genérico',          1, 10000029, 1, N'50mg',     N'1901200290299', 58.00, 40,  7,  10, 16, N'Anticonvulsivante', N'C1', SYSUTCDATETIME()),
    (30, N'Glifage XR 500mg 30 Comprimidos',                  1, 10000030, 0, N'500mg',    N'1012300300300', 48.00, 105, 5,  2,  11, NULL, NULL, SYSUTCDATETIME())
) AS source (Id, Descricao, Ativo, CodBarra, Controlado, Dosagem, RegistroMS, Preco, Estoque, ProdutoGrupoId, FabricanteId, PrincipioAtivoId, ClasseTerapeutica, Lista, CriadoEm)
ON target.Id = source.Id
WHEN NOT MATCHED THEN
    INSERT (Id, Descricao, Ativo, CodBarra, Controlado, Dosagem, RegistroMS, Preco, Estoque, ProdutoGrupoId, FabricanteId, PrincipioAtivoId, ClasseTerapeutica, Lista, CriadoEm)
    VALUES (source.Id, source.Descricao, source.Ativo, source.CodBarra, source.Controlado, source.Dosagem, source.RegistroMS, source.Preco, source.Estoque, source.ProdutoGrupoId, source.FabricanteId, source.PrincipioAtivoId, source.ClasseTerapeutica, source.Lista, source.CriadoEm);

SET IDENTITY_INSERT dbo.Produtos OFF;
