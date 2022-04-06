

---- TipoNegocioHabilitadoSinBoleto

--IF NOT EXISTS (select 1 from TipoNegocioHabilitadoSinBoleto where TipoNegocioId = 2) BEGIN insert into TipoNegocioHabilitadoSinBoleto(TipoNegocioId) values (2); END

---- ProvinciaNoHabilitadoSinBoleto

--IF NOT EXISTS (select 1 from ProvinciaNoHabilitadoSinBoleto where ProvinciaId = 3) BEGIN insert into ProvinciaNoHabilitadoSinBoleto(ProvinciaId) values (3); END

---- TipoOperacionHabilitadoSinBoleto

--IF NOT EXISTS (select 1 from TipoOperacionHabilitadoSinBoleto where Directo = 1) BEGIN insert into TipoOperacionHabilitadoSinBoleto(Directo) values (1); END

---- MaterialHabilitadoSinBoleto
--IF NOT EXISTS (select 1 from MaterialHabilitadoSinBoleto where MaterialId = 3) BEGIN insert into MaterialHabilitadoSinBoleto(MaterialId) values (3); END

---- CentroHabilitadoSinBoleto

--IF NOT EXISTS (select 1 from CentroHabilitadoSinBoleto where CentroId = 2) BEGIN insert into CentroHabilitadoSinBoleto(CentroId) values (2); END
--IF NOT EXISTS (select 1 from CentroHabilitadoSinBoleto where CentroId = 3) BEGIN insert into CentroHabilitadoSinBoleto(CentroId) values (3); END
--IF NOT EXISTS (select 1 from CentroHabilitadoSinBoleto where CentroId = 5) BEGIN insert into CentroHabilitadoSinBoleto(CentroId) values (5); END
--IF NOT EXISTS (select 1 from CentroHabilitadoSinBoleto where CentroId = 6) BEGIN insert into CentroHabilitadoSinBoleto(CentroId) values (6); END
--IF NOT EXISTS (select 1 from CentroHabilitadoSinBoleto where CentroId = 7) BEGIN insert into CentroHabilitadoSinBoleto(CentroId) values (7); END
--IF NOT EXISTS (select 1 from CentroHabilitadoSinBoleto where CentroId = 8) BEGIN insert into CentroHabilitadoSinBoleto(CentroId) values (8); END
--IF NOT EXISTS (select 1 from CentroHabilitadoSinBoleto where CentroId = 9) BEGIN insert into CentroHabilitadoSinBoleto(CentroId) values (9); END
--IF NOT EXISTS (select 1 from CentroHabilitadoSinBoleto where CentroId = 11) BEGIN insert into CentroHabilitadoSinBoleto(CentroId) values (11); END
--IF NOT EXISTS (select 1 from CentroHabilitadoSinBoleto where CentroId = 12) BEGIN insert into CentroHabilitadoSinBoleto(CentroId) values (12); END
--IF NOT EXISTS (select 1 from CentroHabilitadoSinBoleto where CentroId = 13) BEGIN insert into CentroHabilitadoSinBoleto(CentroId) values (13); END
--IF NOT EXISTS (select 1 from CentroHabilitadoSinBoleto where CentroId = 14) BEGIN insert into CentroHabilitadoSinBoleto(CentroId) values (14); END
--IF NOT EXISTS (select 1 from CentroHabilitadoSinBoleto where CentroId = 15) BEGIN insert into CentroHabilitadoSinBoleto(CentroId) values (15); END

---- ClasificacionHabilitadoSinBoleto

--IF NOT EXISTS (select 1 from ClasificacionHabilitadoSinBoleto where ClasificacionId = 1) BEGIN insert into ClasificacionHabilitadoSinBoleto(ClasificacionId) values (1); END