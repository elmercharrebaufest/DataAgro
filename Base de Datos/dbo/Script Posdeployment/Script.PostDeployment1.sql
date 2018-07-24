--BolsaCompraNet
IF NOT EXISTS (select 1 from BolsaCompraNet where Descripcion = 'Bs As') BEGIN insert into BolsaCompraNet (Descripcion) values ('Bs As'); END
IF NOT EXISTS (select 1 from BolsaCompraNet where Descripcion = 'Rosario') BEGIN insert into BolsaCompraNet (Descripcion) values ('Rosario'); END
IF NOT EXISTS (select 1 from BolsaCompraNet where Descripcion = 'Bahía Blanca') BEGIN insert into BolsaCompraNet (Descripcion) values ('Bahía Blanca'); END

--BoletoCompraNet
IF NOT EXISTS (select 1 from BoletoCompraNet where Descripcion = 'Confirma') BEGIN insert into BolsaCompraNet (Descripcion) values ('Confirma'); END
IF NOT EXISTS (select 1 from BoletoCompraNet where Descripcion = 'Físico') BEGIN insert into BolsaCompraNet (Descripcion) values ('Físico'); END
IF NOT EXISTS (select 1 from BoletoCompraNet where Descripcion = 'Ninguno') BEGIN insert into BolsaCompraNet (Descripcion) values ('Ninguno'); END

--ClasificacionCompraNet
IF NOT EXISTS (select 1 from ClasificacionCompraNet where Descripcion = 'Productor') BEGIN insert into BolsaCompraNet (Descripcion) values ('Productor'); END
IF NOT EXISTS (select 1 from ClasificacionCompraNet where Descripcion = 'Acopiador') BEGIN insert into BolsaCompraNet (Descripcion) values ('Acopiador'); END
IF NOT EXISTS (select 1 from ClasificacionCompraNet where Descripcion = 'Otros') BEGIN insert into BolsaCompraNet (Descripcion) values ('Otros'); END
