SET IDENTITY_INSERT dbo.Fabricantes ON;

MERGE INTO dbo.Fabricantes AS target
USING (VALUES
    (1,  N'EMS',                               N'EMS S/A',                                         N'57837740000105', 1,  SYSUTCDATETIME(), NULL),
    (2,  N'Eurofarma',                         N'Eurofarma Laboratórios S.A.',                     N'61190090000191', 2,  SYSUTCDATETIME(), NULL),
    (3,  N'Aché',                              N'Aché Laboratórios Farmacêuticos S.A.',            N'60973859000173', 3,  SYSUTCDATETIME(), NULL),
    (4,  N'Sanofi Medley',                     N'Sanofi Medley Farmacêutica Ltda.',                N'10588616000109', 4,  SYSUTCDATETIME(), NULL),
    (5,  N'Bayer',                             N'Bayer S.A.',                                      N'18459628000115', 5,  SYSUTCDATETIME(), NULL),
    (6,  N'Novartis',                          N'Novartis Biociências S.A.',                       N'56994026000110', 6,  SYSUTCDATETIME(), NULL),
    (7,  N'Pfizer',                            N'Pfizer Brasil Ltda.',                             N'46435020000130', 7,  SYSUTCDATETIME(), NULL),
    (8,  N'Hypera Pharma',                     N'Hypera S.A.',                                     N'02932074000146', 8,  SYSUTCDATETIME(), NULL),
    (9,  N'Libbs',                             N'Libbs Farmacêutica Ltda.',                        N'61230551000170', 9,  SYSUTCDATETIME(), NULL),
    (10, N'Cristália',                         N'Cristália Produtos Químicos Farmacêuticos Ltda.', N'44734971000187', 10, SYSUTCDATETIME(), NULL)
) AS source (Id, NomeFantasia, RazaoSocial, Cnpj, ContatoId, CriadoEm, AtualizadoEm)
ON target.Id = source.Id
WHEN NOT MATCHED THEN
    INSERT (Id, NomeFantasia, RazaoSocial, Cnpj, ContatoId, CriadoEm, AtualizadoEm)
    VALUES (source.Id, source.NomeFantasia, source.RazaoSocial, source.Cnpj, source.ContatoId, source.CriadoEm, source.AtualizadoEm);

SET IDENTITY_INSERT dbo.Fabricantes OFF;
