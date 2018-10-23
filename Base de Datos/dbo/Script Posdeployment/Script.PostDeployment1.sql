--BolsaCompraNet
IF NOT EXISTS (select 1 from BolsaCompraNet where Descripcion = 'Bs As') BEGIN insert into BolsaCompraNet (Descripcion,CodigoSap) values ('Bs As','01'); END
IF NOT EXISTS (select 1 from BolsaCompraNet where Descripcion = 'Rosario') BEGIN insert into BolsaCompraNet (Descripcion,CodigoSap) values ('Rosario','02'); END
IF NOT EXISTS (select 1 from BolsaCompraNet where Descripcion = 'Santa Fe') BEGIN insert into BolsaCompraNet (Descripcion,CodigoSap) values ('Santa Fe','03'); END
IF NOT EXISTS (select 1 from BolsaCompraNet where Descripcion = 'Cordoba') BEGIN insert into BolsaCompraNet (Descripcion,CodigoSap) values ('Cordoba','04'); END
IF NOT EXISTS (select 1 from BolsaCompraNet where Descripcion = 'Entre Ríos') BEGIN insert into BolsaCompraNet (Descripcion,CodigoSap) values ('Entre Ríos','05'); END
IF NOT EXISTS (select 1 from BolsaCompraNet where Descripcion = 'Bahía Blanca') BEGIN insert into BolsaCompraNet (Descripcion,CodigoSap) values ('Bahía Blanca','07'); END


--BoletoCompraNet
IF NOT EXISTS (select 1 from BoletoCompraNet where Descripcion = 'Confirma') BEGIN insert into BoletoCompraNet (Descripcion) values ('Confirma'); END
IF NOT EXISTS (select 1 from BoletoCompraNet where Descripcion = 'Físico') BEGIN insert into BoletoCompraNet (Descripcion) values ('Físico'); END
IF NOT EXISTS (select 1 from BoletoCompraNet where Descripcion = 'Ninguno') BEGIN insert into BoletoCompraNet (Descripcion) values ('Ninguno'); END
IF NOT EXISTS (select 1 from BoletoCompraNet where Descripcion = 'Carta Oferta') BEGIN insert into BoletoCompraNet (Descripcion) values ('Carta Oferta'); END

--ClasificacionCompraNet
IF NOT EXISTS (select 1 from ClasificacionCompraNet where Descripcion = 'Productor') BEGIN insert into ClasificacionCompraNet (Descripcion) values ('Productor'); END
IF NOT EXISTS (select 1 from ClasificacionCompraNet where Descripcion = 'Acopiador') BEGIN insert into ClasificacionCompraNet (Descripcion) values ('Acopiador'); END
IF NOT EXISTS (select 1 from ClasificacionCompraNet where Descripcion = 'Otros') BEGIN insert into ClasificacionCompraNet (Descripcion) values ('Otros'); END

--Centro
IF NOT EXISTS (select 1 from Centro where Descripcion = 'San Lorenzo') BEGIN insert into Centro(Descripcion,CodigoSap) values ('San Lorenzo', '1029'); END
IF NOT EXISTS (select 1 from Centro where Descripcion = 'Pergamino') BEGIN insert into Centro(Descripcion,CodigoSap) values ('Pergamino', '1035'); END
IF NOT EXISTS (select 1 from Centro where Descripcion = 'Bandera') BEGIN insert into Centro(Descripcion,CodigoSap) values ('Bandera', '1127'); END
IF NOT EXISTS (select 1 from Centro where Descripcion = 'La Cautiva') BEGIN insert into Centro(Descripcion,CodigoSap) values ('La Cautiva', '1126'); END
IF NOT EXISTS (select 1 from Centro where Descripcion = 'Lincoln') BEGIN insert into Centro(Descripcion,CodigoSap) values ('Lincoln', '1036'); END

--EstadoContrato
IF NOT EXISTS (select 1 from EstadoContrato where Descripcion = 'Pendiente') BEGIN insert into EstadoContrato (Descripcion, Orden) values ('Pendiente', 1); END
IF NOT EXISTS (select 1 from EstadoContrato where Descripcion = 'Confirmado') BEGIN insert into EstadoContrato (Descripcion, Orden) values ('Confirmado', 2); END
IF NOT EXISTS (select 1 from EstadoContrato where Descripcion = 'Oferta') BEGIN insert into EstadoContrato (Descripcion, Orden) values ('Oferta', 4); END
IF NOT EXISTS (select 1 from EstadoContrato where Descripcion = 'Con Error') BEGIN insert into EstadoContrato (Descripcion, Orden) values ('Con Error', 3); END
IF NOT EXISTS (select 1 from EstadoContrato where Descripcion = 'Finalizado') BEGIN insert into EstadoContrato (Descripcion, Orden) values ('Finalizado', 5); END
IF NOT EXISTS (select 1 from EstadoContrato where Descripcion = 'Rechazado') BEGIN insert into EstadoContrato (Descripcion, Orden) values ('Rechazado', 6); END

--StandardDeCalidad
IF NOT EXISTS (select 1 from StandardDeCalidad where Descripcion = 'Camara') BEGIN insert into StandardDeCalidad (Descripcion,CodigoSap) values ('Camara','03'); END
IF NOT EXISTS (select 1 from StandardDeCalidad where Descripcion = 'Especial') BEGIN insert into StandardDeCalidad (Descripcion,CodigoSap) values ('Especial','04'); END

--CondicionFijacion
IF NOT EXISTS (select 1 from CondicionFijacion where Descripcion = 'HASTA 12 HS. POR PIZARRA CIEGA') BEGIN insert into CondicionFijacion(Descripcion,CodigoSap) values ('HASTA 12 HS. POR PIZARRA CIEGA', '01'); END 
IF NOT EXISTS (select 1 from CondicionFijacion where Descripcion = 'HASTA 13 HS. POR PIZARRA CIEGA') BEGIN insert into CondicionFijacion(Descripcion,CodigoSap) values ('HASTA 13 HS. POR PIZARRA CIEGA', '02'); END 
IF NOT EXISTS (select 1 from CondicionFijacion where Descripcion = 'H 1/2 HORA AP CBOT X PIZ Ó MOA') BEGIN insert into CondicionFijacion(Descripcion,CodigoSap) values ('H 1/2 HORA AP CBOT X PIZ Ó MOA', '03'); END 
IF NOT EXISTS (select 1 from CondicionFijacion where Descripcion = 'HASTA 12 HS. X PIZ.CIEGA Ó MOA') BEGIN insert into CondicionFijacion(Descripcion,CodigoSap) values ('HASTA 12 HS. X PIZ.CIEGA Ó MOA', '04'); END 
IF NOT EXISTS (select 1 from CondicionFijacion where Descripcion = 'HASTA 13 HS. X PIZ.CIEGA Ó MOA') BEGIN insert into CondicionFijacion(Descripcion,CodigoSap) values ('HASTA 13 HS. X PIZ.CIEGA Ó MOA', '05'); END 
IF NOT EXISTS (select 1 from CondicionFijacion where Descripcion = 'HASTA 14.30 HS POR PIZ / MERCADERIA') BEGIN insert into CondicionFijacion(Descripcion,CodigoSap) values ('HASTA 14.30 HS POR PIZ / MERCADERIA', '06'); END 
IF NOT EXISTS (select 1 from CondicionFijacion where Descripcion = 'MERCADO MOA') BEGIN insert into CondicionFijacion(Descripcion,CodigoSap) values ('MERCADO MOA', '07'); END 

--CalidadesEspeciales
IF NOT EXISTS (select 1 from CalidadEspecial where Descripcion = 'Dañados') BEGIN insert into CalidadEspecial (Descripcion, CodigoSap, MaterialId) values ('Dañados','MPSOJGDA',3); END
IF NOT EXISTS (select 1 from CalidadEspecial where Descripcion = 'Granos verdes') BEGIN insert into CalidadEspecial (Descripcion, CodigoSap, MaterialId) values ('Granos verdes','MPSOJGVE',3); END
--IF NOT EXISTS (select 1 from CalidadEspecial where Descripcion = 'Cuerpos extraños') BEGIN insert into CalidadEspecial (Descripcion, CodigoSap, MaterialId) values ('Cuerpos extraños','MPSOJCEX',3); END
IF NOT EXISTS (select 1 from CalidadEspecial where CodigoSap = 'MPMAZGRA') BEGIN insert into CalidadEspecial (Descripcion, CodigoSap, MaterialId) values ('Grado','MPMAZGRA',1); END
IF NOT EXISTS (select 1 from CalidadEspecial where CodigoSap = 'MPTRPGRA') BEGIN insert into CalidadEspecial (Descripcion, CodigoSap, MaterialId) values ('Grado','MPTRPGRA',2); END

--TipoDB
IF NOT EXISTS (select 1 from TipoDB where Descripcion = 'Sobre el precio') BEGIN insert into TipoDB (Descripcion, CodigoSap) values ('Sobre el precio','S'); END
IF NOT EXISTS (select 1 from TipoDB where Descripcion = 'Por Fuera del Precio') BEGIN insert into TipoDB (Descripcion, CodigoSap) values ('Por Fuera del Precio','A'); END

--TipoPeriodoDB
IF NOT EXISTS (select 1 from TipoPeriodoDB where Descripcion = 'Generales') BEGIN insert into TipoPeriodoDB (Descripcion, CodigoSap) values ('Generales','G'); END
--IF NOT EXISTS (select 1 from TipoPeriodoDB where Descripcion = 'Por Fecha de Entrega') BEGIN insert into TipoPeriodoDB (Descripcion, CodigoSap) values ('Por Fecha de Entrega','E'); END
--IF NOT EXISTS (select 1 from TipoPeriodoDB where Descripcion = 'Por Fecha de Fijación') BEGIN insert into TipoPeriodoDB (Descripcion, CodigoSap) values ('Por Fecha de Fijación','F'); END