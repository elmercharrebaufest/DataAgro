
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
IF NOT EXISTS (select 1 from BoletoCompraNet where Descripcion = 'Sin Boleto') BEGIN insert into BoletoCompraNet (Descripcion) values ('Sin Boleto'); END

--ClasificacionCompraNet
IF NOT EXISTS (select 1 from ClasificacionCompraNet where Descripcion = 'Productor') BEGIN insert into ClasificacionCompraNet (Descripcion) values ('Productor'); END
IF NOT EXISTS (select 1 from ClasificacionCompraNet where Descripcion = 'Acopiador') BEGIN insert into ClasificacionCompraNet (Descripcion) values ('Acopiador'); END
IF NOT EXISTS (select 1 from ClasificacionCompraNet where Descripcion = 'Otros') BEGIN insert into ClasificacionCompraNet (Descripcion) values ('Otros'); END

--Centro
IF NOT EXISTS (select 1 from Centro where Descripcion = 'S. Lorenzo') BEGIN insert into Centro(Descripcion,CodigoSap) values ('S. Lorenzo', '1029'); END
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
IF NOT EXISTS (select 1 from EstadoContrato where Descripcion = 'Reconfirmar') BEGIN insert into EstadoContrato (Descripcion, Orden) values ('Reconfirmar', 7); END
IF NOT EXISTS (select 1 from EstadoContrato where Descripcion = 'Eliminado') BEGIN insert into EstadoContrato (Descripcion, Orden) values ('Eliminado', 8); END
IF NOT EXISTS (select 1 from EstadoContrato where Descripcion = 'Carga') BEGIN insert into EstadoContrato (Descripcion, Orden) values ('Carga', 9); END
IF NOT EXISTS (select 1 from EstadoContrato where Descripcion = 'PreAnulado') BEGIN insert into EstadoContrato (Descripcion, Orden) values ('PreAnulado', 10); END
IF NOT EXISTS (select 1 from EstadoContrato where Descripcion = 'ReconfirmarFinalizado') BEGIN insert into EstadoContrato (Descripcion, Orden) values ('ReconfirmarFinalizado', 11); END

--StandardDeCalidad
IF NOT EXISTS (select 1 from StandardDeCalidad where Descripcion = 'Camara' AND CodigoSap = '3') BEGIN insert into StandardDeCalidad (Descripcion,CodigoSap) values ('Camara','3'); END
IF NOT EXISTS (select 1 from StandardDeCalidad where Descripcion = 'Camara' AND CodigoSap = '1') BEGIN insert into StandardDeCalidad (Descripcion,CodigoSap) values ('Camara','1'); END
IF NOT EXISTS (select 1 from StandardDeCalidad where Descripcion = 'Especial') BEGIN insert into StandardDeCalidad (Descripcion,CodigoSap) values ('Especial','4'); END
IF NOT EXISTS (select 1 from StandardDeCalidad where Descripcion = 'Fabrica') BEGIN insert into StandardDeCalidad (Descripcion,CodigoSap) values ('Fabrica','3'); END

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
IF NOT EXISTS (select 1 from CalidadEspecial where CodigoSap = 'MPGIRCEX'AND MaterialId=4) BEGIN insert into CalidadEspecial (Descripcion, CodigoSap, MaterialId) values ('Materia Extraña','MPGIRCEX',4); END
IF NOT EXISTS (select 1 from CalidadEspecial where CodigoSap = 'MPGIRCEX' AND MaterialId=5) BEGIN insert into CalidadEspecial (Descripcion, CodigoSap, MaterialId) values ('Materia Extraña','MPGIRCEX',5); END

--TipoDB
IF NOT EXISTS (select 1 from TipoDB where Descripcion = 'Sobre el precio') BEGIN insert into TipoDB (Descripcion, CodigoSap) values ('Sobre el precio','S'); END
IF NOT EXISTS (select 1 from TipoDB where Descripcion = 'Por Fuera del Precio') BEGIN insert into TipoDB (Descripcion, CodigoSap) values ('Por Fuera del Precio','A'); END

--TipoPeriodoDB
IF NOT EXISTS (select 1 from TipoPeriodoDB where Descripcion = 'Generales') BEGIN insert into TipoPeriodoDB (Descripcion, CodigoSap) values ('Generales','G'); END
--IF NOT EXISTS (select 1 from TipoPeriodoDB where Descripcion = 'Por Fecha de Entrega') BEGIN insert into TipoPeriodoDB (Descripcion, CodigoSap) values ('Por Fecha de Entrega','E'); END
--IF NOT EXISTS (select 1 from TipoPeriodoDB where Descripcion = 'Por Fecha de Fijación') BEGIN insert into TipoPeriodoDB (Descripcion, CodigoSap) values ('Por Fecha de Fijación','F'); END

--Tipo Negocio
IF NOT EXISTS (select 1 from TipoNegocio where Descripcion = 'FASON') BEGIN insert into TipoNegocio (Descripcion) values ('FASON'); END
IF NOT EXISTS (select 1 from TipoNegocio where Descripcion = 'AGENTE DE COMPRAS') BEGIN insert into TipoNegocio (Descripcion) values ('AGENTE DE COMPRAS'); END
IF NOT EXISTS (select 1 from TipoNegocio where Descripcion = 'CONTRATO ACUERDO') BEGIN insert into TipoNegocio (Descripcion) values ('CONTRATO ACUERDO'); END
IF NOT EXISTS (select 1 from TipoNegocio where Descripcion = 'ESPACIO DINAMICO') BEGIN insert into TipoNegocio (Descripcion) values ('ESPACIO DINAMICO'); END

--Tipo Fason
IF NOT EXISTS (select 1 from TipoFason where Descripcion = 'FAS') BEGIN insert into TipoFason (Descripcion) values ('FAS'); END
IF NOT EXISTS (select 1 from TipoFason where Descripcion = 'FOB') BEGIN insert into TipoFason (Descripcion) values ('FOB'); END

--Tipo Hedge Material
IF NOT EXISTS (select 1 from TipoHedgeMaterial where Descripcion = 'Disponible') BEGIN insert into TipoHedgeMaterial (Descripcion) values ('Disponible'); END
IF NOT EXISTS (select 1 from TipoHedgeMaterial where Descripcion = 'Forward') BEGIN insert into TipoHedgeMaterial (Descripcion) values ('Forward'); END
IF NOT EXISTS (select 1 from TipoHedgeMaterial where Descripcion = 'New Crop') BEGIN insert into TipoHedgeMaterial (Descripcion) values ('New Crop'); END

--Tipo Objetivo
IF NOT EXISTS (select 1 from TipoObjetivo where Descripcion = 'Pricing') BEGIN insert into TipoObjetivo (Descripcion) values ('Pricing'); END
IF NOT EXISTS (select 1 from TipoObjetivo where Descripcion = 'A Remitir') BEGIN insert into TipoObjetivo (Descripcion) values ('A Remitir'); END

--Tipo Agente de Compra
IF NOT EXISTS (select 1 from TipoAgenteCompra where Descripcion = 'MAT') BEGIN insert into TipoAgenteCompra (Descripcion) values ('MAT'); END
IF NOT EXISTS (select 1 from TipoAgenteCompra where Descripcion = 'Rofex') BEGIN insert into TipoAgenteCompra (Descripcion) values ('Rofex'); END
--Tipo Research
IF NOT EXISTS (select 1 from TipoResearch where Descripcion = 'Avance Siembra') BEGIN insert into TipoResearch (Descripcion) values ('Avance Siembra'); END
IF NOT EXISTS (select 1 from TipoResearch where Descripcion = 'Avance Cosecha') BEGIN insert into TipoResearch (Descripcion) values ('Avance Cosecha'); END
IF NOT EXISTS (select 1 from TipoResearch where Descripcion = 'Situación de cultivos') BEGIN insert into TipoResearch (Descripcion) values ('Situación de cultivos'); END
IF NOT EXISTS (select 1 from TipoResearch where Descripcion = 'Ventas y Stock') BEGIN insert into TipoResearch (Descripcion) values ('Ventas y Stock'); END

--Nivel Tarifa
IF NOT EXISTS (select 1 from NivelTarifa where Descripcion = 'Estimado provisorio') BEGIN insert into NivelTarifa (Descripcion,CodigoSap) values ('Estimado provisorio','Z01'); END
IF NOT EXISTS (select 1 from NivelTarifa where Descripcion = 'Estimado confirmado') BEGIN insert into NivelTarifa (Descripcion,CodigoSap) values ('Estimado confirmado','Z02'); END
IF NOT EXISTS (select 1 from NivelTarifa where Descripcion = 'Real') BEGIN insert into NivelTarifa (Descripcion,CodigoSap) values ('Real','Z03'); END

--Estadío
IF NOT EXISTS (select 1 from Estadio where Descripcion = 'Emergencia' AND MaterialId = 1) BEGIN insert into Estadio (Descripcion, MaterialId) values ('Emergencia', 1); END
IF NOT EXISTS (select 1 from Estadio where Descripcion = 'Desarrollo de hojas' AND MaterialId = 1) BEGIN insert into Estadio (Descripcion, MaterialId) values ('Desarrollo de hojas', 1); END
IF NOT EXISTS (select 1 from Estadio where Descripcion = 'Panojamiento' AND MaterialId = 1) BEGIN insert into Estadio (Descripcion, MaterialId) values ('Panojamiento', 1); END
IF NOT EXISTS (select 1 from Estadio where Descripcion = 'Floración' AND MaterialId = 1) BEGIN insert into Estadio (Descripcion, MaterialId) values ('Floración', 1); END
IF NOT EXISTS (select 1 from Estadio where Descripcion = 'Llenado de granos' AND MaterialId = 1) BEGIN insert into Estadio (Descripcion, MaterialId) values ('Llenado de granos', 1); END
IF NOT EXISTS (select 1 from Estadio where Descripcion = 'Madurez' AND MaterialId = 1) BEGIN insert into Estadio (Descripcion, MaterialId) values ('Madurez', 1); END
IF NOT EXISTS (select 1 from Estadio where Descripcion = 'Cosecha' AND MaterialId = 1) BEGIN insert into Estadio (Descripcion, MaterialId) values ('Cosecha', 1); END

IF NOT EXISTS (select 1 from Estadio where Descripcion = 'Emergencia' AND MaterialId = 2) BEGIN insert into Estadio (Descripcion, MaterialId) values ('Emergencia', 2); END
IF NOT EXISTS (select 1 from Estadio where Descripcion = 'Desarrollo de hojas' AND MaterialId = 2) BEGIN insert into Estadio (Descripcion, MaterialId) values ('Desarrollo de hojas', 2); END
IF NOT EXISTS (select 1 from Estadio where Descripcion = 'Macollaje' AND MaterialId = 2) BEGIN insert into Estadio (Descripcion, MaterialId) values ('Macollaje', 2); END
IF NOT EXISTS (select 1 from Estadio where Descripcion = 'Encañazón' AND MaterialId = 2) BEGIN insert into Estadio (Descripcion, MaterialId) values ('Encañazón', 2); END
IF NOT EXISTS (select 1 from Estadio where Descripcion = 'Espiga Embuchada' AND MaterialId = 2) BEGIN insert into Estadio (Descripcion, MaterialId) values ('Espiga Embuchada', 2); END
IF NOT EXISTS (select 1 from Estadio where Descripcion = 'Floración' AND MaterialId = 2) BEGIN insert into Estadio (Descripcion, MaterialId) values ('Floración', 2); END
IF NOT EXISTS (select 1 from Estadio where Descripcion = 'Llenado de granos' AND MaterialId = 2) BEGIN insert into Estadio (Descripcion, MaterialId) values ('Llenado de granos', 2); END
IF NOT EXISTS (select 1 from Estadio where Descripcion = 'Madurez' AND MaterialId = 2) BEGIN insert into Estadio (Descripcion, MaterialId) values ('Madurez', 2); END
IF NOT EXISTS (select 1 from Estadio where Descripcion = 'Cosecha' AND MaterialId = 2) BEGIN insert into Estadio (Descripcion, MaterialId) values ('Cosecha', 2); END

IF NOT EXISTS (select 1 from Estadio where Descripcion = 'Emergencia' AND MaterialId = 3) BEGIN insert into Estadio (Descripcion, MaterialId) values ('Emergencia', 3); END
IF NOT EXISTS (select 1 from Estadio where Descripcion = 'Desarrollo de hojas' AND MaterialId = 3) BEGIN insert into Estadio (Descripcion, MaterialId) values ('Desarrollo de hojas', 3); END
IF NOT EXISTS (select 1 from Estadio where Descripcion = 'Inicio de Floración' AND MaterialId = 3) BEGIN insert into Estadio (Descripcion, MaterialId) values ('Inicio de Floración', 3); END
IF NOT EXISTS (select 1 from Estadio where Descripcion = 'Floración' AND MaterialId = 3) BEGIN insert into Estadio (Descripcion, MaterialId) values ('Floración', 3); END
IF NOT EXISTS (select 1 from Estadio where Descripcion = 'Formación de Vainas (R3)' AND MaterialId = 3) BEGIN insert into Estadio (Descripcion, MaterialId) values ('Formación de Vainas (R3)', 3); END
IF NOT EXISTS (select 1 from Estadio where Descripcion = 'Llenado de granos (R5)' AND MaterialId = 3) BEGIN insert into Estadio (Descripcion, MaterialId) values ('Llenado de granos (R5)', 3); END
IF NOT EXISTS (select 1 from Estadio where Descripcion = 'Madurez' AND MaterialId = 3) BEGIN insert into Estadio (Descripcion, MaterialId) values ('Madurez', 3); END
IF NOT EXISTS (select 1 from Estadio where Descripcion = 'Cosecha' AND MaterialId = 3) BEGIN insert into Estadio (Descripcion, MaterialId) values ('Cosecha', 3); END

IF NOT EXISTS (select 1 from Estadio where Descripcion = 'Emergencia' AND MaterialId = 4) BEGIN insert into Estadio (Descripcion, MaterialId) values ('Emergencia', 4); END
IF NOT EXISTS (select 1 from Estadio where Descripcion = 'Desarrollo de hojas' AND MaterialId = 4) BEGIN insert into Estadio (Descripcion, MaterialId) values ('Desarrollo de hojas', 4); END
IF NOT EXISTS (select 1 from Estadio where Descripcion = 'Botón Floral' AND MaterialId = 4) BEGIN insert into Estadio (Descripcion, MaterialId) values ('Botón Floral', 4); END
IF NOT EXISTS (select 1 from Estadio where Descripcion = 'Floración' AND MaterialId = 4) BEGIN insert into Estadio (Descripcion, MaterialId) values ('Floración', 4); END
IF NOT EXISTS (select 1 from Estadio where Descripcion = 'Llenado de granos' AND MaterialId = 4) BEGIN insert into Estadio (Descripcion, MaterialId) values ('Llenado de granos', 4); END
IF NOT EXISTS (select 1 from Estadio where Descripcion = 'Madurez' AND MaterialId = 4) BEGIN insert into Estadio (Descripcion, MaterialId) values ('Madurez', 4); END
IF NOT EXISTS (select 1 from Estadio where Descripcion = 'Cosecha' AND MaterialId = 4) BEGIN insert into Estadio (Descripcion, MaterialId) values ('Cosecha', 4); END

IF NOT EXISTS (select 1 from Estadio where Descripcion = 'Emergencia' AND MaterialId = 5) BEGIN insert into Estadio (Descripcion, MaterialId) values ('Emergencia', 5); END
IF NOT EXISTS (select 1 from Estadio where Descripcion = 'Desarrollo de hojas' AND MaterialId = 5) BEGIN insert into Estadio (Descripcion, MaterialId) values ('Desarrollo de hojas', 5); END
IF NOT EXISTS (select 1 from Estadio where Descripcion = 'Botón Floral' AND MaterialId = 5) BEGIN insert into Estadio (Descripcion, MaterialId) values ('Botón Floral', 5); END
IF NOT EXISTS (select 1 from Estadio where Descripcion = 'Floración' AND MaterialId = 5) BEGIN insert into Estadio (Descripcion, MaterialId) values ('Floración', 5); END
IF NOT EXISTS (select 1 from Estadio where Descripcion = 'Llenado de granos' AND MaterialId = 5) BEGIN insert into Estadio (Descripcion, MaterialId) values ('Llenado de granos', 5); END
IF NOT EXISTS (select 1 from Estadio where Descripcion = 'Madurez' AND MaterialId = 5) BEGIN insert into Estadio (Descripcion, MaterialId) values ('Madurez', 5); END
IF NOT EXISTS (select 1 from Estadio where Descripcion = 'Cosecha' AND MaterialId = 5) BEGIN insert into Estadio (Descripcion, MaterialId) values ('Cosecha', 5); END

--Zona Cupo
IF NOT EXISTS (select 1 from ZonaCupo where CodigoSap = 'CBA' ) BEGIN insert into ZonaCupo (Descripcion, CodigoSap) values ('CORREDOR BS AS', 'CBA'); END
IF NOT EXISTS (select 1 from ZonaCupo where CodigoSap = 'CRO' ) BEGIN insert into ZonaCupo (Descripcion, CodigoSap) values ('CORREDOR ROSARIO', 'CRO'); END
IF NOT EXISTS (select 1 from ZonaCupo where CodigoSap = 'FAS' ) BEGIN insert into ZonaCupo (Descripcion, CodigoSap) values ('Fasones CAGSA/MOLCA, YPF y AMAGGI', 'FAS'); END
IF NOT EXISTS (select 1 from ZonaCupo where CodigoSap = 'MAT' ) BEGIN insert into ZonaCupo (Descripcion, CodigoSap) values ('MAT-ROFEX', 'MAT'); END
IF NOT EXISTS (select 1 from ZonaCupo where CodigoSap = 'OIC' ) BEGIN insert into ZonaCupo (Descripcion, CodigoSap) values ('ORIG INTERIOR CENTRO', 'OIC'); END
IF NOT EXISTS (select 1 from ZonaCupo where CodigoSap = 'OIN' ) BEGIN insert into ZonaCupo (Descripcion, CodigoSap) values ('ORIG INTERIOR NORTE', 'OIN'); END
IF NOT EXISTS (select 1 from ZonaCupo where CodigoSap = 'OIS' ) BEGIN insert into ZonaCupo (Descripcion, CodigoSap) values ('ORIG INTERIOR SUR', 'OIS'); END
IF NOT EXISTS (select 1 from ZonaCupo where CodigoSap = 'PPR' ) BEGIN insert into ZonaCupo (Descripcion, CodigoSap) values ('PRODUCCION PROPIA', 'PPR'); END
IF NOT EXISTS (select 1 from ZonaCupo where CodigoSap = 'RED' ) BEGIN insert into ZonaCupo (Descripcion, CodigoSap) values ('REDESPACHOS', 'RED'); END
IF NOT EXISTS (select 1 from ZonaCupo where CodigoSap = 'SOL' ) BEGIN insert into ZonaCupo (Descripcion, CodigoSap) values ('SOLIDARIDAD', 'SOL'); END
--Pizarra

IF NOT EXISTS (select 1 from Pizarra where Descripcion = 'ROSARIO') BEGIN insert into Pizarra(Descripcion, Codigo) values ('ROSARIO', 'ROS'); END
IF NOT EXISTS (select 1 from Pizarra where Descripcion = 'BAHIA BLANCA') BEGIN insert into Pizarra(Descripcion, Codigo) values ('BAHIA BLANCA', 'BBA'); END
IF NOT EXISTS (select 1 from Pizarra where Descripcion = 'DARSENA') BEGIN insert into Pizarra(Descripcion, Codigo) values ('DARSENA', 'DAR'); END
IF NOT EXISTS (select 1 from Pizarra where Descripcion = 'QUEQUEN') BEGIN insert into Pizarra(Descripcion, Codigo) values ('QUEQUEN', 'QQ'); END

--EstadoContrato
IF NOT EXISTS (select 1 from EstadoCupo where Descripcion = 'Sin CTG') BEGIN insert into EstadoCupo (Descripcion, Orden) values ('Sin CTG', 1); END
IF NOT EXISTS (select 1 from EstadoCupo where Descripcion = 'Activado') BEGIN insert into EstadoCupo (Descripcion, Orden) values ('Activado', 2); END
IF NOT EXISTS (select 1 from EstadoCupo where Descripcion = 'Descargado') BEGIN insert into EstadoCupo (Descripcion, Orden) values ('Descargado', 3); END
IF NOT EXISTS (select 1 from EstadoCupo where Descripcion = 'Anulado') BEGIN insert into EstadoCupo (Descripcion, Orden) values ('Anulado', 4); END
IF NOT EXISTS (select 1 from EstadoCupo where Descripcion = 'Arribado') BEGIN insert into EstadoCupo (Descripcion, Orden) values ('Arribado', 5); END
IF NOT EXISTS (select 1 from EstadoCupo where Descripcion = 'Sin STOP') BEGIN insert into EstadoCupo (Descripcion, Orden) values ('Sin STOP', 6); END
IF NOT EXISTS (select 1 from EstadoCupo where Descripcion = 'Error STOP') BEGIN insert into EstadoCupo (Descripcion, Orden) values ('Error STOP', 7); END
IF NOT EXISTS (select 1 from EstadoCupo where Descripcion = 'Disponible') BEGIN insert into EstadoCupo (Descripcion, Orden) values ('Disponible', 8); END
IF NOT EXISTS (select 1 from EstadoCupo where Descripcion = 'Rechazado') BEGIN insert into EstadoCupo (Descripcion, Orden) values ('Rechazado', 9); END

--CondicionFijacion
IF NOT EXISTS (select 1 from CondicionPago where Descripcion = '3 DÍAS HÁBILES DE FECHA DE FIJACIÓN') BEGIN insert into CondicionPago(Descripcion,CodigoSap) values ('3 DÍAS HÁBILES DE FECHA DE FIJACIÓN', '03'); END 
IF NOT EXISTS (select 1 from CondicionPago where Descripcion = '4 DÍAS HÁBILES DE FECHA DE FIJACIÓN') BEGIN insert into CondicionPago(Descripcion,CodigoSap) values ('4 DÍAS HÁBILES DE FECHA DE FIJACIÓN', '04'); END 
IF NOT EXISTS (select 1 from CondicionPago where Descripcion = '5 DÍAS HÁBILES DE FECHA DE FIJACIÓN') BEGIN insert into CondicionPago(Descripcion,CodigoSap) values ('5 DÍAS HÁBILES DE FECHA DE FIJACIÓN', '05'); END 
IF NOT EXISTS (select 1 from CondicionPago where Descripcion = '6 DÍAS HÁBILES DE FECHA DE FIJACIÓN') BEGIN insert into CondicionPago(Descripcion,CodigoSap) values ('6 DÍAS HÁBILES DE FECHA DE FIJACIÓN', '06'); END 
IF NOT EXISTS (select 1 from CondicionPago where Descripcion = '10 DÍAS HÁBILES DE FECHA DE FIJACIÓN') BEGIN insert into CondicionPago(Descripcion,CodigoSap) values ('10 DÍAS HÁBILES DE FECHA DE FIJACIÓN', '10'); END 
IF NOT EXISTS (select 1 from CondicionPago where Descripcion = '10 DIAS CORRIDOS DE FIJACION') BEGIN insert into CondicionPago(Descripcion,CodigoSap) values ('10 DIAS CORRIDOS DE FIJACION', '1T'); END 
IF NOT EXISTS (select 1 from CondicionPago where Descripcion = '15 DIAS CORRIDOS DE FIJACION') BEGIN insert into CondicionPago(Descripcion,CodigoSap) values ('15 DIAS CORRIDOS DE FIJACION', '2T'); END 

--MotivoAnterior
IF NOT EXISTS (select 1 from MotivoAnterior where Descripcion = 'Otro') BEGIN insert into MotivoAnterior(Descripcion) values ('Otro'); END
IF NOT EXISTS (select 1 from MotivoAnterior where Descripcion = 'Error en la carga') BEGIN insert into MotivoAnterior(Descripcion) values ('Error en la carga'); END
IF NOT EXISTS (select 1 from MotivoAnterior where Descripcion = 'Me olvidé de cargar') BEGIN insert into MotivoAnterior(Descripcion) values ('Me olvidé de cargar'); END

--ClasificacionCompraNet
IF NOT EXISTS (select 1 from TipoRangoConfirmacionAutomatica where Descripcion = 'Confirmación y Reconfirmación') BEGIN insert into TipoRangoConfirmacionAutomatica (Descripcion) values ('Confirmación y Reconfirmación'); END
IF NOT EXISTS (select 1 from TipoRangoConfirmacionAutomatica where Descripcion = 'Confirmación') BEGIN insert into TipoRangoConfirmacionAutomatica (Descripcion) values ('Confirmación'); END
IF NOT EXISTS (select 1 from TipoRangoConfirmacionAutomatica where Descripcion = 'Reconfirmación') BEGIN insert into TipoRangoConfirmacionAutomatica (Descripcion) values ('Reconfirmación'); END
IF NOT EXISTS (select 1 from MotivoAnterior where Descripcion = 'Me olvidé de cargar') BEGIN insert into MotivoAnterior(Descripcion) values ('Me olvidé de cargar'); END
IF NOT EXISTS (select 1 from MotivoAnterior where Descripcion = 'Me olvidé de cargar') BEGIN insert into MotivoAnterior(Descripcion) values ('Me olvidé de cargar'); END

--ConceptoAperturaPrecio
IF NOT EXISTS (select 1 from ConceptoAperturaPrecio where Descripcion = 'Basis') BEGIN insert into ConceptoAperturaPrecio (Descripcion,CodigoSap) values ('Basis','BA'); END

--Configuracion precio Moa
IF NOT EXISTS (select 1 from EstadoPrecioMOA where MaterialId = 1 and TipoNegocioId = 1) BEGIN insert into EstadoPrecioMOA (MaterialId, TipoNegocioId, Habilitado) values (1,1, 1); END
IF NOT EXISTS (select 1 from EstadoPrecioMOA where MaterialId = 2 and TipoNegocioId = 1) BEGIN insert into EstadoPrecioMOA (MaterialId, TipoNegocioId, Habilitado) values (2,1, 1); END
IF NOT EXISTS (select 1 from EstadoPrecioMOA where MaterialId = 3 and TipoNegocioId = 1) BEGIN insert into EstadoPrecioMOA (MaterialId, TipoNegocioId, Habilitado) values (3,1, 1); END
IF NOT EXISTS (select 1 from EstadoPrecioMOA where MaterialId = 4 and TipoNegocioId = 1) BEGIN insert into EstadoPrecioMOA (MaterialId, TipoNegocioId, Habilitado) values (4,1, 1); END
IF NOT EXISTS (select 1 from EstadoPrecioMOA where MaterialId = 5 and TipoNegocioId = 1) BEGIN insert into EstadoPrecioMOA (MaterialId, TipoNegocioId, Habilitado) values (5,1, 1); END

IF NOT EXISTS (select 1 from EstadoPrecioMOA where MaterialId = 1 and TipoNegocioId = 2) BEGIN insert into EstadoPrecioMOA (MaterialId, TipoNegocioId, Habilitado) values (1,1, 2); END
IF NOT EXISTS (select 1 from EstadoPrecioMOA where MaterialId = 2 and TipoNegocioId = 2) BEGIN insert into EstadoPrecioMOA (MaterialId, TipoNegocioId, Habilitado) values (2,1, 2); END
IF NOT EXISTS (select 1 from EstadoPrecioMOA where MaterialId = 3 and TipoNegocioId = 2) BEGIN insert into EstadoPrecioMOA (MaterialId, TipoNegocioId, Habilitado) values (3,1, 2); END
IF NOT EXISTS (select 1 from EstadoPrecioMOA where MaterialId = 4 and TipoNegocioId = 2) BEGIN insert into EstadoPrecioMOA (MaterialId, TipoNegocioId, Habilitado) values (4,1, 2); END
IF NOT EXISTS (select 1 from EstadoPrecioMOA where MaterialId = 5 and TipoNegocioId = 2) BEGIN insert into EstadoPrecioMOA (MaterialId, TipoNegocioId, Habilitado) values (5,1, 2); END

IF NOT EXISTS (select 1 from EstadoPrecioMOA where MaterialId = 1 and TipoNegocioId = 3) BEGIN insert into EstadoPrecioMOA (MaterialId, TipoNegocioId, Habilitado) values (1,1, 3); END
IF NOT EXISTS (select 1 from EstadoPrecioMOA where MaterialId = 2 and TipoNegocioId = 3) BEGIN insert into EstadoPrecioMOA (MaterialId, TipoNegocioId, Habilitado) values (2,1, 3); END
IF NOT EXISTS (select 1 from EstadoPrecioMOA where MaterialId = 3 and TipoNegocioId = 3) BEGIN insert into EstadoPrecioMOA (MaterialId, TipoNegocioId, Habilitado) values (3,1, 3); END
IF NOT EXISTS (select 1 from EstadoPrecioMOA where MaterialId = 4 and TipoNegocioId = 3) BEGIN insert into EstadoPrecioMOA (MaterialId, TipoNegocioId, Habilitado) values (4,1, 3); END
IF NOT EXISTS (select 1 from EstadoPrecioMOA where MaterialId = 5 and TipoNegocioId = 3) BEGIN insert into EstadoPrecioMOA (MaterialId, TipoNegocioId, Habilitado) values (5,1, 3); END


--TipoPosicionCBOT
IF NOT EXISTS (select 1 from TipoPosicionCBOT where Descripcion = 'CBOT') BEGIN insert into TipoPosicionCBOT (Descripcion) values ('CBOT'); END
IF NOT EXISTS (select 1 from TipoPosicionCBOT where Descripcion = 'MAT') BEGIN insert into TipoPosicionCBOT (Descripcion) values ('MAT'); END
IF NOT EXISTS (select 1 from TipoPosicionCBOT where Descripcion = 'PASE') BEGIN insert into TipoPosicionCBOT (Descripcion) values ('PASE'); END

--Camara
IF NOT EXISTS (select 1 from Camara where Descripcion = 'Rosario') BEGIN insert into Camara (Descripcion) values ('Rosario'); END
IF NOT EXISTS (select 1 from Camara where Descripcion = 'Santa Fe') BEGIN insert into Camara (Descripcion) values ('Santa Fe'); END
IF NOT EXISTS (select 1 from Camara where Descripcion = 'Bahía Blanca') BEGIN insert into Camara (Descripcion) values ('Bahía Blanca'); END
IF NOT EXISTS (select 1 from Camara where Descripcion = 'Buenos Aires') BEGIN insert into Camara (Descripcion) values ('Buenos Aires'); END

--ComisionAFavor
IF NOT EXISTS (select 1 from ComisionAFavor where Descripcion = 'Cliente') BEGIN insert into ComisionAFavor (Descripcion) values ('Cliente'); END
IF NOT EXISTS (select 1 from ComisionAFavor where Descripcion = 'MOA') BEGIN insert into ComisionAFavor (Descripcion) values ('MOA'); END
IF NOT EXISTS (select 1 from ComisionAFavor where Descripcion = 'Corredor') BEGIN insert into ComisionAFavor (Descripcion) values ('Corredor'); END

--CondicionDePagoFijacionVenta
IF NOT EXISTS (select 1 from CondicionDePagoVenta where Descripcion = 'A partir de la fecha de Entrega') BEGIN insert into CondicionDePagoVenta (Descripcion, CondicionFijacion, CondicionPesificado) values ('A partir de la fecha de entrega', 1, 1); END
IF NOT EXISTS (select 1 from CondicionDePagoVenta where Descripcion = 'A partir de la fecha de Pesificación') BEGIN insert into CondicionDePagoVenta (Descripcion, CondicionFijacion) values ('A partir de la fecha de Pesificación', 1); END
IF NOT EXISTS (select 1 from CondicionDePagoVenta where Descripcion = 'A partir de la fecha de Liquidación') BEGIN insert into CondicionDePagoVenta (Descripcion, CondicionFijacion) values ('A partir de la fecha de Liquidación', 1); END
IF NOT EXISTS (select 1 from CondicionDePagoVenta where Descripcion = 'A partir de la fecha de Fijación') BEGIN insert into CondicionDePagoVenta (Descripcion, CondicionFijacion, CondicionPesificado) values ('A partir de la fecha de Fijación', 1, 1); END
IF NOT EXISTS (select 1 from CondicionDePagoVenta where Descripcion = 'Anteriores al pago') BEGIN insert into CondicionDePagoVenta (Descripcion, CondicionPesificado) values ('Anteriores al pago', 1); END
--Configuracion
Update Configuracion set RedespachoMaximoARP = isnull(RedespachoMaximoARP, 1000), RedespachoMaximoUSDM = isnull(RedespachoMaximoUSDM, 60)

--BoletoVenta

--CondicionDePagoFijacionVenta
IF NOT EXISTS (select 1 from BoletoVenta where Descripcion = 'Confirma') BEGIN insert into BoletoVenta (Descripcion) values ('Confirma'); END
IF NOT EXISTS (select 1 from BoletoVenta where Descripcion = 'Físico') BEGIN insert into BoletoVenta (Descripcion) values ('Físico'); END
IF NOT EXISTS (select 1 from BoletoVenta where Descripcion = 'Carta Oferta') BEGIN insert into BoletoVenta (Descripcion) values ('Carta Oferta'); END
IF NOT EXISTS (select 1 from BoletoVenta where Descripcion = 'Confirmación de Negocio') BEGIN insert into BoletoVenta (Descripcion) values ('Confirmación de Negocio'); END
IF NOT EXISTS (select 1 from BoletoVenta where Descripcion = 'A Convenir') BEGIN insert into BoletoVenta (Descripcion) values ('A Convenir'); END


--TipoAdministracionCupo
IF NOT EXISTS (select 1 from TipoAdministracionCupo where Descripcion = 'Algoritmo') BEGIN insert into TipoAdministracionCupo(Descripcion) values ('Algoritmo'); END
IF NOT EXISTS (select 1 from TipoAdministracionCupo where Descripcion = 'Extraordinaria') BEGIN insert into TipoAdministracionCupo(Descripcion) values ('Extraordinaria'); END
--TipoNegocioRangoConfirmacionAutomatica
IF NOT EXISTS (select 1 from TipoNegocioRangoConfirmacionAutomatica where Descripcion = 'A PRECIO Y FIJACION') BEGIN insert into TipoNegocioRangoConfirmacionAutomatica (Descripcion) values ('A PRECIO Y FIJACION'); END
IF NOT EXISTS (select 1 from TipoNegocioRangoConfirmacionAutomatica where Descripcion = 'A PRECIO') BEGIN insert into TipoNegocioRangoConfirmacionAutomatica (Descripcion) values ('A PRECIO'); END
IF NOT EXISTS (select 1 from TipoNegocioRangoConfirmacionAutomatica where Descripcion = 'FIJACION') BEGIN insert into TipoNegocioRangoConfirmacionAutomatica (Descripcion) values ('FIJACION'); END

--TipoAdministracionCupo
IF NOT EXISTS (select 1 from TipoAdministracionCupo where Descripcion = 'Algoritmo') BEGIN insert into TipoAdministracionCupo(Descripcion) values ('Algoritmo'); END
IF NOT EXISTS (select 1 from TipoAdministracionCupo where Descripcion = 'Extraordinaria') BEGIN insert into TipoAdministracionCupo(Descripcion) values ('Extraordinaria'); END

--Comisionista
IF NOT EXISTS (select 1 from Segmentacion where Descripcion = 'Comisionista') BEGIN insert into Segmentacion(Descripcion, Grupo) values ('Comisionista','Comisionistas'); END
IF NOT EXISTS (select 1 from TipoAdministracionCupo where Descripcion = 'Extraordinaria') BEGIN insert into TipoAdministracionCupo(Descripcion) values ('Extraordinaria'); END

-- TipoNegocioDetalle

IF NOT EXISTS (select 1 from TipoNegocioDetalle where Descripcion = 'A FIJAR') BEGIN insert into TipoNegocioDetalle(Descripcion, TipoNegocioId) values ('A FIJAR', 1); END
IF NOT EXISTS (select 1 from TipoNegocioDetalle where Descripcion = 'A FIJAR PASE') BEGIN insert into TipoNegocioDetalle(Descripcion, TipoNegocioId) values ('A FIJAR PASE', 1); END
IF NOT EXISTS (select 1 from TipoNegocioDetalle where Descripcion = 'CANJE') BEGIN insert into TipoNegocioDetalle(Descripcion, TipoNegocioId) values ('CANJE', 1); END
IF NOT EXISTS (select 1 from TipoNegocioDetalle where Descripcion = 'A PRECIO') BEGIN insert into TipoNegocioDetalle(Descripcion, TipoNegocioId) values ('A PRECIO', 2); END
IF NOT EXISTS (select 1 from TipoNegocioDetalle where Descripcion = 'FIJACION') BEGIN insert into TipoNegocioDetalle(Descripcion, TipoNegocioId) values ('FIJACION', 3); END
IF NOT EXISTS (select 1 from TipoNegocioDetalle where Descripcion = 'CONVENIO') BEGIN insert into TipoNegocioDetalle(Descripcion, TipoNegocioId) values ('CONVENIO', 1); END
IF NOT EXISTS (select 1 from TipoNegocioDetalle where Descripcion = 'FIJ. CONVENIO') BEGIN insert into TipoNegocioDetalle(Descripcion, TipoNegocioId) values ('FIJ. CONVENIO', 2); END
IF NOT EXISTS (select 1 from TipoNegocioDetalle where Descripcion = 'FASON') BEGIN insert into TipoNegocioDetalle(Descripcion, TipoNegocioId) values ('FASON', 4); END
IF NOT EXISTS (select 1 from TipoNegocioDetalle where Descripcion = 'FASON MP') BEGIN insert into TipoNegocioDetalle(Descripcion, TipoNegocioId) values ('FASON MP', 1); END
IF NOT EXISTS (select 1 from TipoNegocioDetalle where Descripcion = 'PRESTAMO DEVOLUCION') BEGIN insert into TipoNegocioDetalle(Descripcion, TipoNegocioId) values ('PRESTAMO DEVOLUCION', 1); END
IF NOT EXISTS (select 1 from TipoNegocioDetalle where Descripcion = 'VENTA') BEGIN insert into TipoNegocioDetalle(Descripcion, TipoNegocioId) values ('VENTA', 2); END
IF NOT EXISTS (select 1 from TipoNegocioDetalle where Descripcion = 'FIJACION VIRTUAL') BEGIN insert into TipoNegocioDetalle(Descripcion, TipoNegocioId) values ('FIJACION VIRTUAL', 3); END
IF NOT EXISTS (select 1 from TipoNegocioDetalle where Descripcion = 'FIJACION CANJE') BEGIN insert into TipoNegocioDetalle(Descripcion, TipoNegocioId) values ('FIJACION CANJE', 3); END
IF NOT EXISTS (select 1 from TipoNegocioDetalle where Descripcion = 'FIJACION PASE') BEGIN insert into TipoNegocioDetalle(Descripcion, TipoNegocioId) values ('FIJACION PASE', 3); END

-- Clausula

IF NOT EXISTS (select 1 from Clausula where Discriminator = 'ClausulaUno') BEGIN insert into Clausula(Discriminator, Orden, Estado) values ('ClausulaUno', 1, 1); END
IF NOT EXISTS (select 1 from Clausula where Discriminator = 'ClausulaDos') BEGIN insert into Clausula(Discriminator, Orden, Estado) values ('ClausulaDos', 2, 1); END
IF NOT EXISTS (select 1 from Clausula where Discriminator = 'ClausulaTres') BEGIN insert into Clausula(Discriminator, Orden, Estado) values ('ClausulaTres', 3, 1); END
IF NOT EXISTS (select 1 from Clausula where Discriminator = 'ClausulaCuatro') BEGIN insert into Clausula(Discriminator, Orden, Estado) values ('ClausulaCuatro', 4 , 1); END
IF NOT EXISTS (select 1 from Clausula where Discriminator = 'ClausulaCinco') BEGIN insert into Clausula(Discriminator, Orden, Estado) values ('ClausulaCinco', 5 , 1); END
IF NOT EXISTS (select 1 from Clausula where Discriminator = 'ClausulaSeis') BEGIN insert into Clausula(Discriminator, Orden, Estado) values ('ClausulaSeis', 6 , 1); END
IF NOT EXISTS (select 1 from Clausula where Discriminator = 'ClausulaSiete') BEGIN insert into Clausula(Discriminator, Orden, Estado) values ('ClausulaSiete', 7 , 1); END
IF NOT EXISTS (select 1 from Clausula where Discriminator = 'ClausulaOcho') BEGIN insert into Clausula(Discriminator, Orden, Estado) values ('ClausulaOcho', 8 , 1); END
IF NOT EXISTS (select 1 from Clausula where Discriminator = 'ClausulaNueve') BEGIN insert into Clausula(Discriminator, Orden, Estado) values ('ClausulaNueve', 9 , 1); END
IF NOT EXISTS (select 1 from Clausula where Discriminator = 'ClausulaDiez') BEGIN insert into Clausula(Discriminator, Orden, Estado) values ('ClausulaDiez', 10 , 1); END
IF NOT EXISTS (select 1 from Clausula where Discriminator = 'ClausulaOnce') BEGIN insert into Clausula(Discriminator, Orden, Estado) values ('ClausulaOnce', 11 , 1); END
IF NOT EXISTS (select 1 from Clausula where Discriminator = 'ClausulaDoce') BEGIN insert into Clausula(Discriminator, Orden, Estado) values ('ClausulaDoce', 12 , 1); END
IF NOT EXISTS (select 1 from Clausula where Discriminator = 'ClausulaTrece') BEGIN insert into Clausula(Discriminator, Orden, Estado) values ('ClausulaTrece', 13 , 1); END
IF NOT EXISTS (select 1 from Clausula where Discriminator = 'ClausulaCatorce') BEGIN insert into Clausula(Discriminator, Orden, Estado) values ('ClausulaCatorce', 14 , 1); END
IF NOT EXISTS (select 1 from Clausula where Discriminator = 'ClausulaQuince') BEGIN insert into Clausula(Discriminator, Orden, Estado) values ('ClausulaQuince', 15 , 1); END
IF NOT EXISTS (select 1 from Clausula where Discriminator = 'ClausulaDieciseis') BEGIN insert into Clausula(Discriminator, Orden, Estado) values ('ClausulaDieciseis',16 , 1); END
IF NOT EXISTS (select 1 from Clausula where Discriminator = 'ClausulaDiecisiete') BEGIN insert into Clausula(Discriminator, Orden, Estado) values ('ClausulaDiecisiete',17 , 1); END
IF NOT EXISTS (select 1 from Clausula where Discriminator = 'ClausulaDieciocho') BEGIN insert into Clausula(Discriminator, Orden, Estado) values ('ClausulaDieciocho',18 , 1); END
IF NOT EXISTS (select 1 from Clausula where Discriminator = 'ClausulaDiecinueve') BEGIN insert into Clausula(Discriminator, Orden, Estado) values ('ClausulaDiecinueve',19 , 1); END
IF NOT EXISTS (select 1 from Clausula where Discriminator = 'ClausulaVeinte') BEGIN insert into Clausula(Discriminator, Orden, Estado) values ('ClausulaVeinte',20 , 1); END
IF NOT EXISTS (select 1 from Clausula where Discriminator = 'ClausulaVeintiuno') BEGIN insert into Clausula(Discriminator, Orden, Estado) values ('ClausulaVeintiuno',21 , 1); END
IF NOT EXISTS (select 1 from Clausula where Discriminator = 'ClausulaVeintidos') BEGIN insert into Clausula(Discriminator, Orden, Estado) values ('ClausulaVeintidos',22 , 1); END
IF NOT EXISTS (select 1 from Clausula where Discriminator = 'ClausulaVeintitres') BEGIN insert into Clausula(Discriminator, Orden, Estado) values ('ClausulaVeintitres',23 , 1); END
IF NOT EXISTS (select 1 from Clausula where Discriminator = 'ClausulaVeinticuatro') BEGIN insert into Clausula(Discriminator, Orden, Estado) values ('ClausulaVeinticuatro',24 , 1); END
IF NOT EXISTS (select 1 from Clausula where Discriminator = 'ClausulaVeinticinco') BEGIN insert into Clausula(Discriminator, Orden, Estado) values ('ClausulaVeinticinco',25 , 1); END
IF NOT EXISTS (select 1 from Clausula where Discriminator = 'ClausulaVeintiseis') BEGIN insert into Clausula(Discriminator, Orden, Estado) values ('ClausulaVeintiseis',26 , 1); END
IF NOT EXISTS (select 1 from Clausula where Discriminator = 'ClausulaVeintisiete') BEGIN insert into Clausula(Discriminator, Orden, Estado) values ('ClausulaVeintisiete',27 , 1); END
IF NOT EXISTS (select 1 from Clausula where Discriminator = 'ClausulaVeintiocho') BEGIN insert into Clausula(Discriminator, Orden, Estado) values ('ClausulaVeintiocho',28 , 1); END
IF NOT EXISTS (select 1 from Clausula where Discriminator = 'ClausulaVeintinueve') BEGIN insert into Clausula(Discriminator, Orden, Estado) values ('ClausulaVeintinueve',29 , 1); END
IF NOT EXISTS (select 1 from Clausula where Discriminator = 'ClausulaTreinta') BEGIN insert into Clausula(Discriminator, Orden, Estado) values ('ClausulaTreinta',30 , 1); END
IF NOT EXISTS (select 1 from Clausula where Discriminator = 'ClausulaTreintaYUno') BEGIN insert into Clausula(Discriminator, Orden, Estado) values ('ClausulaTreintaYUno',31 , 1); END
IF NOT EXISTS (select 1 from Clausula where Discriminator = 'ClausulaTreintaYDos') BEGIN insert into Clausula(Discriminator, Orden, Estado) values ('ClausulaTreintaYDos',32 , 1); END
IF NOT EXISTS (select 1 from Clausula where Discriminator = 'ClausulaTreintaYTres') BEGIN insert into Clausula(Discriminator, Orden, Estado) values ('ClausulaTreintaYTres',33 , 1); END
IF NOT EXISTS (select 1 from Clausula where Discriminator = 'ClausulaTreintaYCuatro') BEGIN insert into Clausula(Discriminator, Orden, Estado) values ('ClausulaTreintaYCuatro',34 , 1); END
IF NOT EXISTS (select 1 from Clausula where Discriminator = 'ClausulaTreintaYCinco') BEGIN insert into Clausula(Discriminator, Orden, Estado) values ('ClausulaTreintaYCinco',35 , 1); END
IF NOT EXISTS (select 1 from Clausula where Discriminator = 'ClausulaTreintaYSeis') BEGIN insert into Clausula(Discriminator, Orden, Estado) values ('ClausulaTreintaYSeis',36 , 1); END
IF NOT EXISTS (select 1 from Clausula where Discriminator = 'ClausulaTreintaYSiete') BEGIN insert into Clausula(Discriminator, Orden, Estado) values ('ClausulaTreintaYSiete',37 , 1); END
IF NOT EXISTS (select 1 from Clausula where Discriminator = 'ClausulaTreintaYOcho') BEGIN insert into Clausula(Discriminator, Orden, Estado) values ('ClausulaTreintaYOcho',38 , 1); END
IF NOT EXISTS (select 1 from Clausula where Discriminator = 'ClausulaTreintaYNueve') BEGIN insert into Clausula(Discriminator, Orden, Estado) values ('ClausulaTreintaYNueve',39 , 1); END
IF NOT EXISTS (select 1 from Clausula where Discriminator = 'ClausulaCuarenta') BEGIN insert into Clausula(Discriminator, Orden, Estado)values ('ClausulaCuarenta',40 , 1); END
IF NOT EXISTS (select 1 from Clausula where Discriminator = 'ClausulaCuarentaYUno') BEGIN insert into Clausula(Discriminator, Orden, Estado) values ('ClausulaCuarentaYUno',41 , 1); END
IF NOT EXISTS (select 1 from Clausula where Discriminator = 'ClausulaCuarentaYDos') BEGIN insert into Clausula(Discriminator, Orden, Estado)values ('ClausulaCuarentaYDos',42 , 1); END
IF NOT EXISTS (select 1 from Clausula where Discriminator = 'ClausulaCuarentaYTres') BEGIN insert into Clausula(Discriminator, Orden, Estado) values ('ClausulaCuarentaYTres',43 , 1); END
IF NOT EXISTS (select 1 from Clausula where Discriminator = 'ClausulaCuarentaYCuatro') BEGIN insert into Clausula(Discriminator, Orden, Estado) values ('ClausulaCuarentaYCuatro',44 , 1); END
IF NOT EXISTS (select 1 from Clausula where Discriminator = 'ClausulaCuarentaYCinco') BEGIN insert into Clausula(Discriminator, Orden, Estado) values ('ClausulaCuarentaYCinco',45 , 1); END
IF NOT EXISTS (select 1 from Clausula where Discriminator = 'ClausulaCuarentaYSeis') BEGIN insert into Clausula(Discriminator, Orden, Estado) values ('ClausulaCuarentaYSeis',46, 1); END
IF NOT EXISTS (select 1 from Clausula where Discriminator = 'ClausulaCuarentaYSiete') BEGIN insert into Clausula(Discriminator, Orden, Estado) values ('ClausulaCuarentaYSiete',47 , 1); END


--EstadoHome
IF NOT EXISTS (select 1 from EstadoHome where Descripcion = 'Habilitado') BEGIN insert into EstadoHome (Descripcion, Color) values ('Habilitado', 'green'); END
IF NOT EXISTS (select 1 from EstadoHome where Descripcion = 'Legajo irregular') BEGIN insert into EstadoHome (Descripcion, Color) values ('Legajo irregular', 'yellow'); END
IF NOT EXISTS (select 1 from EstadoHome where Descripcion = 'No habilitado') BEGIN insert into EstadoHome (Descripcion, Color) values ('No habilitado', 'red'); END

--TipoActividad
IF NOT EXISTS (select 1 from TipoActividad where Descripcion = 'Visita') BEGIN insert into TipoActividad (Descripcion) values ('Visita'); END

IF NOT EXISTS (select 1 from CierreCupera where MaterialId = (select top 1 MaterialId from Material where Descripcion ='Trigo')) BEGIN insert into CierreCupera (MaterialId, Cierre) values ((select top 1 MaterialId from Material where Descripcion ='Trigo'), 0); END
IF NOT EXISTS (select 1 from CierreCupera where MaterialId = (select top 1 MaterialId from Material where Descripcion ='Soja')) BEGIN insert into CierreCupera (MaterialId, Cierre) values ((select top 1 MaterialId from Material where Descripcion ='Soja'), 0); END
IF NOT EXISTS (select 1 from CierreCupera where MaterialId = (select top 1 MaterialId from Material where Descripcion ='Girasol')) BEGIN insert into CierreCupera (MaterialId, Cierre) values ((select top 1 MaterialId from Material where Descripcion ='Girasol'), 0); END
IF NOT EXISTS (select 1 from CierreCupera where MaterialId = (select top 1 MaterialId from Material where Descripcion ='Girasol AO')) BEGIN insert into CierreCupera (MaterialId, Cierre) values ((select top 1 MaterialId from Material where Descripcion ='Girasol AO'), 0); END

IF NOT EXISTS (select 1 from Rol where Descripcion = 'Recibir Sugerencia FAQ') BEGIN insert into Rol (Descripcion) values ('Recibir Sugerencia FAQ'); END
IF NOT EXISTS (select 1 from RolPermiso where RolId = (select RolId from Rol where Descripcion ='Recibir Sugerencia FAQ') and Permiso = 50) BEGIN insert into RolPermiso (RolId, Permiso) values ((select Id from Rol where Descripcion ='Recibir Sugerencia FAQ'), 50); END

--ResearchCoeficienteCultivo
IF NOT EXISTS (select 1 from ResearchCoeficienteCultivo where MaterialId = (select MaterialId from Material where Descripcion='Trigo') and Coeficiente='0.9') BEGIN insert into ResearchCoeficienteCultivo (MaterialId, Coeficiente) values((select MaterialId from Material where Descripcion='Trigo'),'0.9'); END
IF NOT EXISTS (select 1 from ResearchCoeficienteCultivo where MaterialId = (select MaterialId from Material where Descripcion='Maiz') and Coeficiente='0.9') BEGIN insert into ResearchCoeficienteCultivo (MaterialId, Coeficiente) values((select MaterialId from Material where Descripcion='Maiz'),'0.9'); END
IF NOT EXISTS (select 1 from ResearchCoeficienteCultivo where MaterialId = (select MaterialId from Material where Descripcion='Soja') and Coeficiente='0.9') BEGIN insert into ResearchCoeficienteCultivo (MaterialId, Coeficiente) values((select MaterialId from Material where Descripcion='Soja'),'0.9'); END
IF NOT EXISTS (select 1 from ResearchCoeficienteCultivo where MaterialId = (select MaterialId from Material where Descripcion='Girasol') and Coeficiente='0.8') BEGIN insert into ResearchCoeficienteCultivo (MaterialId, Coeficiente) values((select MaterialId from Material where Descripcion='Girasol'),'0.8'); END

--ResearchCondicion
IF NOT EXISTS (select 1 from ResearchCondicion where Descripcion = 'Excelente') BEGIN insert into ResearchCondicion (Descripcion) values('Excelente'); END
IF NOT EXISTS (select 1 from ResearchCondicion where Descripcion = 'Muy buena') BEGIN insert into ResearchCondicion (Descripcion) values('Muy buena'); END
IF NOT EXISTS (select 1 from ResearchCondicion where Descripcion = 'Buena') BEGIN insert into ResearchCondicion (Descripcion) values('Buena'); END
IF NOT EXISTS (select 1 from ResearchCondicion where Descripcion = 'Regular') BEGIN insert into ResearchCondicion (Descripcion) values('Regular'); END
IF NOT EXISTS (select 1 from ResearchCondicion where Descripcion = 'Mala') BEGIN insert into ResearchCondicion (Descripcion) values('Mala'); END
IF NOT EXISTS (select 1 from ResearchCondicion where Descripcion = 'Muy mala') BEGIN insert into ResearchCondicion (Descripcion) values('Muy mala'); END

--ResearchCondicionCultivo
IF NOT EXISTS (select 1 from ResearchCondicionCultivo where MaterialId=(select MaterialId from Material where Descripcion='Trigo') and CondicionId=(select CondicionId from ResearchCondicion where Descripcion='Excelente')) BEGIN insert into ResearchCondicionCultivo (MaterialId, CondicionId, Valor) values((select MaterialId from Material where Descripcion='Trigo'),(select CondicionId from ResearchCondicion where Descripcion='Excelente'),'42'); END
IF NOT EXISTS (select 1 from ResearchCondicionCultivo where MaterialId=(select MaterialId from Material where Descripcion='Trigo') and CondicionId=(select CondicionId from ResearchCondicion where Descripcion='Muy buena')) BEGIN insert into ResearchCondicionCultivo (MaterialId, CondicionId, Valor) values((select MaterialId from Material where Descripcion='Trigo'),(select CondicionId from ResearchCondicion where Descripcion='Muy buena'),'40'); END
IF NOT EXISTS (select 1 from ResearchCondicionCultivo where MaterialId=(select MaterialId from Material where Descripcion='Trigo') and CondicionId=(select CondicionId from ResearchCondicion where Descripcion='Buena')) BEGIN insert into ResearchCondicionCultivo (MaterialId, CondicionId, Valor) values((select MaterialId from Material where Descripcion='Trigo'),(select CondicionId from ResearchCondicion where Descripcion='Buena'),'36'); END
IF NOT EXISTS (select 1 from ResearchCondicionCultivo where MaterialId=(select MaterialId from Material where Descripcion='Trigo') and CondicionId=(select CondicionId from ResearchCondicion where Descripcion='Regular')) BEGIN insert into ResearchCondicionCultivo (MaterialId, CondicionId, Valor) values((select MaterialId from Material where Descripcion='Trigo'),(select CondicionId from ResearchCondicion where Descripcion='Regular'),'32'); END
IF NOT EXISTS (select 1 from ResearchCondicionCultivo where MaterialId=(select MaterialId from Material where Descripcion='Trigo') and CondicionId=(select CondicionId from ResearchCondicion where Descripcion='Mala')) BEGIN insert into ResearchCondicionCultivo (MaterialId, CondicionId, Valor) values((select MaterialId from Material where Descripcion='Trigo'),(select CondicionId from ResearchCondicion where Descripcion='Mala'),'26'); END
IF NOT EXISTS (select 1 from ResearchCondicionCultivo where MaterialId=(select MaterialId from Material where Descripcion='Soja') and CondicionId=(select CondicionId from ResearchCondicion where Descripcion='Excelente')) BEGIN insert into ResearchCondicionCultivo (MaterialId, CondicionId, Valor) values((select MaterialId from Material where Descripcion='Soja'),(select CondicionId from ResearchCondicion where Descripcion='Excelente'),'190'); END
IF NOT EXISTS (select 1 from ResearchCondicionCultivo where MaterialId=(select MaterialId from Material where Descripcion='Soja') and CondicionId=(select CondicionId from ResearchCondicion where Descripcion='Muy buena')) BEGIN insert into ResearchCondicionCultivo (MaterialId, CondicionId, Valor) values((select MaterialId from Material where Descripcion='Soja'),(select CondicionId from ResearchCondicion where Descripcion='Muy buena'),'180'); END
IF NOT EXISTS (select 1 from ResearchCondicionCultivo where MaterialId=(select MaterialId from Material where Descripcion='Soja') and CondicionId=(select CondicionId from ResearchCondicion where Descripcion='Buena')) BEGIN insert into ResearchCondicionCultivo (MaterialId, CondicionId, Valor) values((select MaterialId from Material where Descripcion='Soja'),(select CondicionId from ResearchCondicion where Descripcion='Buena'),'160'); END
IF NOT EXISTS (select 1 from ResearchCondicionCultivo where MaterialId=(select MaterialId from Material where Descripcion='Soja') and CondicionId=(select CondicionId from ResearchCondicion where Descripcion='Regular')) BEGIN insert into ResearchCondicionCultivo (MaterialId, CondicionId, Valor) values((select MaterialId from Material where Descripcion='Soja'),(select CondicionId from ResearchCondicion where Descripcion='Regular'),'140'); END
IF NOT EXISTS (select 1 from ResearchCondicionCultivo where MaterialId=(select MaterialId from Material where Descripcion='Soja') and CondicionId=(select CondicionId from ResearchCondicion where Descripcion='Mala')) BEGIN insert into ResearchCondicionCultivo (MaterialId, CondicionId, Valor) values((select MaterialId from Material where Descripcion='Soja'),(select CondicionId from ResearchCondicion where Descripcion='Mala'),'120'); END
IF NOT EXISTS (select 1 from ResearchCondicionCultivo where MaterialId=(select MaterialId from Material where Descripcion='Maiz') and CondicionId=(select CondicionId from ResearchCondicion where Descripcion='Excelente')) BEGIN insert into ResearchCondicionCultivo (MaterialId, CondicionId, Valor) values((select MaterialId from Material where Descripcion='Maiz'),(select CondicionId from ResearchCondicion where Descripcion='Excelente'),'340'); END
IF NOT EXISTS (select 1 from ResearchCondicionCultivo where MaterialId=(select MaterialId from Material where Descripcion='Maiz') and CondicionId=(select CondicionId from ResearchCondicion where Descripcion='Muy buena')) BEGIN insert into ResearchCondicionCultivo (MaterialId, CondicionId, Valor) values((select MaterialId from Material where Descripcion='Maiz'),(select CondicionId from ResearchCondicion where Descripcion='Muy buena'),'320'); END
IF NOT EXISTS (select 1 from ResearchCondicionCultivo where MaterialId=(select MaterialId from Material where Descripcion='Maiz') and CondicionId=(select CondicionId from ResearchCondicion where Descripcion='Buena')) BEGIN insert into ResearchCondicionCultivo (MaterialId, CondicionId, Valor) values((select MaterialId from Material where Descripcion='Maiz'),(select CondicionId from ResearchCondicion where Descripcion='Buena'),'300'); END
IF NOT EXISTS (select 1 from ResearchCondicionCultivo where MaterialId=(select MaterialId from Material where Descripcion='Maiz') and CondicionId=(select CondicionId from ResearchCondicion where Descripcion='Regular')) BEGIN insert into ResearchCondicionCultivo (MaterialId, CondicionId, Valor) values((select MaterialId from Material where Descripcion='Maiz'),(select CondicionId from ResearchCondicion where Descripcion='Regular'),'265'); END
IF NOT EXISTS (select 1 from ResearchCondicionCultivo where MaterialId=(select MaterialId from Material where Descripcion='Maiz') and CondicionId=(select CondicionId from ResearchCondicion where Descripcion='Mala')) BEGIN insert into ResearchCondicionCultivo (MaterialId, CondicionId, Valor) values((select MaterialId from Material where Descripcion='Maiz'),(select CondicionId from ResearchCondicion where Descripcion='Mala'),'240'); END

--ResearchHumedadSuelo
IF NOT EXISTS (select 1 from ResearchHumedadSuelo where Descripcion = 'Muy seco, grietas profundas') BEGIN insert into ResearchHumedadSuelo (Descripcion) values('Muy seco, grietas profundas'); END
IF NOT EXISTS (select 1 from ResearchHumedadSuelo where Descripcion = 'Seco, grietas pequeñas') BEGIN insert into ResearchHumedadSuelo (Descripcion) values('Seco, grietas pequeñas'); END
IF NOT EXISTS (select 1 from ResearchHumedadSuelo where Descripcion = 'Seco en superficie, pero aparece humedad en los primeros cm') BEGIN insert into ResearchHumedadSuelo (Descripcion) values('Seco en superficie, pero aparece humedad en los primeros cm'); END
IF NOT EXISTS (select 1 from ResearchHumedadSuelo where Descripcion = 'Húmedo en superficie') BEGIN insert into ResearchHumedadSuelo (Descripcion) values('Húmedo en superficie'); END
IF NOT EXISTS (select 1 from ResearchHumedadSuelo where Descripcion = 'Suelo saturado') BEGIN insert into ResearchHumedadSuelo (Descripcion) values('Suelo saturado'); END

--ResearchEstadio
IF NOT EXISTS (select 1 from ResearchEstadio where Descripcion = 'Emergencia') BEGIN insert into ResearchEstadio (Descripcion) values ('Emergencia'); END
IF NOT EXISTS (select 1 from ResearchEstadio where Descripcion = 'Desarrollo de hojas') BEGIN insert into ResearchEstadio (Descripcion) values ('Desarrollo de hojas'); END
IF NOT EXISTS (select 1 from ResearchEstadio where Descripcion = 'Floración') BEGIN insert into ResearchEstadio (Descripcion) values ('Floración'); END
IF NOT EXISTS (select 1 from ResearchEstadio where Descripcion = 'Llenado de granos') BEGIN insert into ResearchEstadio (Descripcion) values ('Llenado de granos'); END
IF NOT EXISTS (select 1 from ResearchEstadio where Descripcion = 'Madurez') BEGIN insert into ResearchEstadio (Descripcion) values ('Madurez'); END
IF NOT EXISTS (select 1 from ResearchEstadio where Descripcion = 'Cosecha') BEGIN insert into ResearchEstadio (Descripcion) values ('Cosecha'); END
IF NOT EXISTS (select 1 from ResearchEstadio where Descripcion = 'Macollaje') BEGIN insert into ResearchEstadio (Descripcion) values ('Macollaje'); END
IF NOT EXISTS (select 1 from ResearchEstadio where Descripcion = 'Encañazón') BEGIN insert into ResearchEstadio (Descripcion) values ('Encañazón'); END
IF NOT EXISTS (select 1 from ResearchEstadio where Descripcion = 'Espiga embuchada') BEGIN insert into ResearchEstadio (Descripcion) values ('Espiga embuchada'); END
IF NOT EXISTS (select 1 from ResearchEstadio where Descripcion = 'Inicio de Floración (R1)') BEGIN insert into ResearchEstadio (Descripcion) values ('Inicio de Floración (R1)'); END
IF NOT EXISTS (select 1 from ResearchEstadio where Descripcion = 'Formación de vainas (R3)') BEGIN insert into ResearchEstadio (Descripcion) values ('Formación de vainas (R3)'); END
IF NOT EXISTS (select 1 from ResearchEstadio where Descripcion = 'Llenado de granos (R5)') BEGIN insert into ResearchEstadio (Descripcion) values ('Llenado de granos (R5)'); END
IF NOT EXISTS (select 1 from ResearchEstadio where Descripcion = 'Panojamiento') BEGIN insert into ResearchEstadio (Descripcion) values ('Panojamiento'); END
IF NOT EXISTS (select 1 from ResearchEstadio where Descripcion = 'Polinización/Floración') BEGIN insert into ResearchEstadio (Descripcion) values ('Polinización/Floración'); END
IF NOT EXISTS (select 1 from ResearchEstadio where Descripcion = 'Cuaje') BEGIN insert into ResearchEstadio (Descripcion) values ('Cuaje'); END
IF NOT EXISTS (select 1 from ResearchEstadio where Descripcion = 'Grano Lechoso') BEGIN insert into ResearchEstadio (Descripcion) values ('Grano Lechoso'); END
IF NOT EXISTS (select 1 from ResearchEstadio where Descripcion = 'Grano Pastoso') BEGIN insert into ResearchEstadio (Descripcion) values ('Grano Pastoso'); END
IF NOT EXISTS (select 1 from ResearchEstadio where Descripcion = 'Dentición') BEGIN insert into ResearchEstadio (Descripcion) values ('Dentición'); END
IF NOT EXISTS (select 1 from ResearchEstadio where Descripcion = 'Botón floral') BEGIN insert into ResearchEstadio (Descripcion) values ('Botón floral'); END

--ResearchEstadioFenologico
IF NOT EXISTS (select 1 from ResearchEstadioFenologico where MaterialId=(select MaterialId from Material where Descripcion='Trigo') and EstadioId=(select EstadioId from ResearchEstadio where Descripcion='Emergencia')) BEGIN insert into ResearchEstadioFenologico (MaterialId, EstadioId, ConRendimiento) values((select MaterialId from Material where Descripcion='Trigo'),(select EstadioId from ResearchEstadio where Descripcion='Emergencia'),'0'); END
IF NOT EXISTS (select 1 from ResearchEstadioFenologico where MaterialId=(select MaterialId from Material where Descripcion='Trigo') and EstadioId=(select EstadioId from ResearchEstadio where Descripcion='Desarrollo de hojas')) BEGIN insert into ResearchEstadioFenologico (MaterialId, EstadioId, ConRendimiento) values((select MaterialId from Material where Descripcion='Trigo'),(select EstadioId from ResearchEstadio where Descripcion='Desarrollo de hojas'),'0'); END
IF NOT EXISTS (select 1 from ResearchEstadioFenologico where MaterialId=(select MaterialId from Material where Descripcion='Trigo') and EstadioId=(select EstadioId from ResearchEstadio where Descripcion='Macollaje')) BEGIN insert into ResearchEstadioFenologico (MaterialId, EstadioId, ConRendimiento) values((select MaterialId from Material where Descripcion='Trigo'),(select EstadioId from ResearchEstadio where Descripcion='Macollaje'),'0'); END
IF NOT EXISTS (select 1 from ResearchEstadioFenologico where MaterialId=(select MaterialId from Material where Descripcion='Trigo') and EstadioId=(select EstadioId from ResearchEstadio where Descripcion='Encañazón')) BEGIN insert into ResearchEstadioFenologico (MaterialId, EstadioId, ConRendimiento) values((select MaterialId from Material where Descripcion='Trigo'),(select EstadioId from ResearchEstadio where Descripcion='Encañazón'),'0'); END
IF NOT EXISTS (select 1 from ResearchEstadioFenologico where MaterialId=(select MaterialId from Material where Descripcion='Trigo') and EstadioId=(select EstadioId from ResearchEstadio where Descripcion='Espiga embuchada')) BEGIN insert into ResearchEstadioFenologico (MaterialId, EstadioId, ConRendimiento) values((select MaterialId from Material where Descripcion='Trigo'),(select EstadioId from ResearchEstadio where Descripcion='Espiga embuchada'),'0'); END
IF NOT EXISTS (select 1 from ResearchEstadioFenologico where MaterialId=(select MaterialId from Material where Descripcion='Trigo') and EstadioId=(select EstadioId from ResearchEstadio where Descripcion='Floración')) BEGIN insert into ResearchEstadioFenologico (MaterialId, EstadioId, ConRendimiento) values((select MaterialId from Material where Descripcion='Trigo'),(select EstadioId from ResearchEstadio where Descripcion='Floración'),'0'); END
IF NOT EXISTS (select 1 from ResearchEstadioFenologico where MaterialId=(select MaterialId from Material where Descripcion='Trigo') and EstadioId=(select EstadioId from ResearchEstadio where Descripcion='Llenado de granos')) BEGIN insert into ResearchEstadioFenologico (MaterialId, EstadioId, ConRendimiento) values((select MaterialId from Material where Descripcion='Trigo'),(select EstadioId from ResearchEstadio where Descripcion='Llenado de granos'),'1'); END
IF NOT EXISTS (select 1 from ResearchEstadioFenologico where MaterialId=(select MaterialId from Material where Descripcion='Trigo') and EstadioId=(select EstadioId from ResearchEstadio where Descripcion='Madurez')) BEGIN insert into ResearchEstadioFenologico (MaterialId, EstadioId, ConRendimiento) values((select MaterialId from Material where Descripcion='Trigo'),(select EstadioId from ResearchEstadio where Descripcion='Madurez'),'1'); END
IF NOT EXISTS (select 1 from ResearchEstadioFenologico where MaterialId=(select MaterialId from Material where Descripcion='Trigo') and EstadioId=(select EstadioId from ResearchEstadio where Descripcion='Cosecha')) BEGIN insert into ResearchEstadioFenologico (MaterialId, EstadioId, ConRendimiento) values((select MaterialId from Material where Descripcion='Trigo'),(select EstadioId from ResearchEstadio where Descripcion='Cosecha'),'1'); END

IF NOT EXISTS (select 1 from ResearchEstadioFenologico where MaterialId=(select MaterialId from Material where Descripcion='Soja') and EstadioId=(select EstadioId from ResearchEstadio where Descripcion='Emergencia')) BEGIN insert into ResearchEstadioFenologico (MaterialId, EstadioId, ConRendimiento) values((select MaterialId from Material where Descripcion='Soja'),(select EstadioId from ResearchEstadio where Descripcion='Emergencia'),'0'); END
IF NOT EXISTS (select 1 from ResearchEstadioFenologico where MaterialId=(select MaterialId from Material where Descripcion='Soja') and EstadioId=(select EstadioId from ResearchEstadio where Descripcion='Desarrollo de hojas')) BEGIN insert into ResearchEstadioFenologico (MaterialId, EstadioId, ConRendimiento) values((select MaterialId from Material where Descripcion='Soja'),(select EstadioId from ResearchEstadio where Descripcion='Desarrollo de hojas'),'0'); END
IF NOT EXISTS (select 1 from ResearchEstadioFenologico where MaterialId=(select MaterialId from Material where Descripcion='Soja') and EstadioId=(select EstadioId from ResearchEstadio where Descripcion='Inicio de Floración (R1)')) BEGIN insert into ResearchEstadioFenologico (MaterialId, EstadioId, ConRendimiento) values((select MaterialId from Material where Descripcion='Soja'),(select EstadioId from ResearchEstadio where Descripcion='Inicio de Floración (R1)'),'0'); END
IF NOT EXISTS (select 1 from ResearchEstadioFenologico where MaterialId=(select MaterialId from Material where Descripcion='Soja') and EstadioId=(select EstadioId from ResearchEstadio where Descripcion='Formación de vainas (R3)')) BEGIN insert into ResearchEstadioFenologico (MaterialId, EstadioId, ConRendimiento) values((select MaterialId from Material where Descripcion='Soja'),(select EstadioId from ResearchEstadio where Descripcion='Formación de vainas (R3)'),'0'); END
IF NOT EXISTS (select 1 from ResearchEstadioFenologico where MaterialId=(select MaterialId from Material where Descripcion='Soja') and EstadioId=(select EstadioId from ResearchEstadio where Descripcion='Llenado de granos (R5)')) BEGIN insert into ResearchEstadioFenologico (MaterialId, EstadioId, ConRendimiento) values((select MaterialId from Material where Descripcion='Soja'),(select EstadioId from ResearchEstadio where Descripcion='Llenado de granos (R5)'),'1'); END
IF NOT EXISTS (select 1 from ResearchEstadioFenologico where MaterialId=(select MaterialId from Material where Descripcion='Soja') and EstadioId=(select EstadioId from ResearchEstadio where Descripcion='Madurez')) BEGIN insert into ResearchEstadioFenologico (MaterialId, EstadioId, ConRendimiento) values((select MaterialId from Material where Descripcion='Soja'),(select EstadioId from ResearchEstadio where Descripcion='Madurez'),'1'); END
IF NOT EXISTS (select 1 from ResearchEstadioFenologico where MaterialId=(select MaterialId from Material where Descripcion='Soja') and EstadioId=(select EstadioId from ResearchEstadio where Descripcion='Cosecha')) BEGIN insert into ResearchEstadioFenologico (MaterialId, EstadioId, ConRendimiento) values((select MaterialId from Material where Descripcion='Soja'),(select EstadioId from ResearchEstadio where Descripcion='Cosecha'),'1'); END

IF NOT EXISTS (select 1 from ResearchEstadioFenologico where MaterialId=(select MaterialId from Material where Descripcion='Maiz') and EstadioId=(select EstadioId from ResearchEstadio where Descripcion='Emergencia')) BEGIN insert into ResearchEstadioFenologico (MaterialId, EstadioId, ConRendimiento) values((select MaterialId from Material where Descripcion='Maiz'),(select EstadioId from ResearchEstadio where Descripcion='Emergencia'),'0'); END
IF NOT EXISTS (select 1 from ResearchEstadioFenologico where MaterialId=(select MaterialId from Material where Descripcion='Maiz') and EstadioId=(select EstadioId from ResearchEstadio where Descripcion='Desarrollo de hojas')) BEGIN insert into ResearchEstadioFenologico (MaterialId, EstadioId, ConRendimiento) values((select MaterialId from Material where Descripcion='Maiz'),(select EstadioId from ResearchEstadio where Descripcion='Desarrollo de hojas'),'0'); END
IF NOT EXISTS (select 1 from ResearchEstadioFenologico where MaterialId=(select MaterialId from Material where Descripcion='Maiz') and EstadioId=(select EstadioId from ResearchEstadio where Descripcion='Panojamiento')) BEGIN insert into ResearchEstadioFenologico (MaterialId, EstadioId, ConRendimiento) values((select MaterialId from Material where Descripcion='Maiz'),(select EstadioId from ResearchEstadio where Descripcion='Panojamiento'),'0'); END
IF NOT EXISTS (select 1 from ResearchEstadioFenologico where MaterialId=(select MaterialId from Material where Descripcion='Maiz') and EstadioId=(select EstadioId from ResearchEstadio where Descripcion='Polinización/Floración')) BEGIN insert into ResearchEstadioFenologico (MaterialId, EstadioId, ConRendimiento) values((select MaterialId from Material where Descripcion='Maiz'),(select EstadioId from ResearchEstadio where Descripcion='Polinización/Floración'),'0'); END
IF NOT EXISTS (select 1 from ResearchEstadioFenologico where MaterialId=(select MaterialId from Material where Descripcion='Maiz') and EstadioId=(select EstadioId from ResearchEstadio where Descripcion='Cuaje')) BEGIN insert into ResearchEstadioFenologico (MaterialId, EstadioId, ConRendimiento) values((select MaterialId from Material where Descripcion='Maiz'),(select EstadioId from ResearchEstadio where Descripcion='Cuaje'),'0'); END
IF NOT EXISTS (select 1 from ResearchEstadioFenologico where MaterialId=(select MaterialId from Material where Descripcion='Maiz') and EstadioId=(select EstadioId from ResearchEstadio where Descripcion='Grano Lechoso')) BEGIN insert into ResearchEstadioFenologico (MaterialId, EstadioId, ConRendimiento) values((select MaterialId from Material where Descripcion='Maiz'),(select EstadioId from ResearchEstadio where Descripcion='Grano Lechoso'),'1'); END
IF NOT EXISTS (select 1 from ResearchEstadioFenologico where MaterialId=(select MaterialId from Material where Descripcion='Maiz') and EstadioId=(select EstadioId from ResearchEstadio where Descripcion='Grano Pastoso')) BEGIN insert into ResearchEstadioFenologico (MaterialId, EstadioId, ConRendimiento) values((select MaterialId from Material where Descripcion='Maiz'),(select EstadioId from ResearchEstadio where Descripcion='Grano Pastoso'),'1'); END
IF NOT EXISTS (select 1 from ResearchEstadioFenologico where MaterialId=(select MaterialId from Material where Descripcion='Maiz') and EstadioId=(select EstadioId from ResearchEstadio where Descripcion='Dentición')) BEGIN insert into ResearchEstadioFenologico (MaterialId, EstadioId, ConRendimiento) values((select MaterialId from Material where Descripcion='Maiz'),(select EstadioId from ResearchEstadio where Descripcion='Dentición'),'1'); END
IF NOT EXISTS (select 1 from ResearchEstadioFenologico where MaterialId=(select MaterialId from Material where Descripcion='Maiz') and EstadioId=(select EstadioId from ResearchEstadio where Descripcion='Madurez')) BEGIN insert into ResearchEstadioFenologico (MaterialId, EstadioId, ConRendimiento) values((select MaterialId from Material where Descripcion='Maiz'),(select EstadioId from ResearchEstadio where Descripcion='Madurez'),'1'); END
IF NOT EXISTS (select 1 from ResearchEstadioFenologico where MaterialId=(select MaterialId from Material where Descripcion='Maiz') and EstadioId=(select EstadioId from ResearchEstadio where Descripcion='Cosecha')) BEGIN insert into ResearchEstadioFenologico (MaterialId, EstadioId, ConRendimiento) values((select MaterialId from Material where Descripcion='Maiz'),(select EstadioId from ResearchEstadio where Descripcion='Cosecha'),'1'); END

IF NOT EXISTS (select 1 from ResearchEstadioFenologico where MaterialId=(select MaterialId from Material where Descripcion='Girasol') and EstadioId=(select EstadioId from ResearchEstadio where Descripcion='Emergencia')) BEGIN insert into ResearchEstadioFenologico (MaterialId, EstadioId, ConRendimiento) values((select MaterialId from Material where Descripcion='Girasol'),(select EstadioId from ResearchEstadio where Descripcion='Emergencia'),'0'); END
IF NOT EXISTS (select 1 from ResearchEstadioFenologico where MaterialId=(select MaterialId from Material where Descripcion='Girasol') and EstadioId=(select EstadioId from ResearchEstadio where Descripcion='Desarrollo de hojas')) BEGIN insert into ResearchEstadioFenologico (MaterialId, EstadioId, ConRendimiento) values((select MaterialId from Material where Descripcion='Girasol'),(select EstadioId from ResearchEstadio where Descripcion='Desarrollo de hojas'),'0'); END
IF NOT EXISTS (select 1 from ResearchEstadioFenologico where MaterialId=(select MaterialId from Material where Descripcion='Girasol') and EstadioId=(select EstadioId from ResearchEstadio where Descripcion='Botón floral')) BEGIN insert into ResearchEstadioFenologico (MaterialId, EstadioId, ConRendimiento) values((select MaterialId from Material where Descripcion='Girasol'),(select EstadioId from ResearchEstadio where Descripcion='Botón floral'),'0'); END
IF NOT EXISTS (select 1 from ResearchEstadioFenologico where MaterialId=(select MaterialId from Material where Descripcion='Girasol') and EstadioId=(select EstadioId from ResearchEstadio where Descripcion='Floración')) BEGIN insert into ResearchEstadioFenologico (MaterialId, EstadioId, ConRendimiento) values((select MaterialId from Material where Descripcion='Girasol'),(select EstadioId from ResearchEstadio where Descripcion='Floración'),'0'); END
IF NOT EXISTS (select 1 from ResearchEstadioFenologico where MaterialId=(select MaterialId from Material where Descripcion='Girasol') and EstadioId=(select EstadioId from ResearchEstadio where Descripcion='Llenado de granos')) BEGIN insert into ResearchEstadioFenologico (MaterialId, EstadioId, ConRendimiento) values((select MaterialId from Material where Descripcion='Girasol'),(select EstadioId from ResearchEstadio where Descripcion='Llenado de granos'),'1'); END
IF NOT EXISTS (select 1 from ResearchEstadioFenologico where MaterialId=(select MaterialId from Material where Descripcion='Girasol') and EstadioId=(select EstadioId from ResearchEstadio where Descripcion='Madurez')) BEGIN insert into ResearchEstadioFenologico (MaterialId, EstadioId, ConRendimiento) values((select MaterialId from Material where Descripcion='Girasol'),(select EstadioId from ResearchEstadio where Descripcion='Madurez'),'1'); END
IF NOT EXISTS (select 1 from ResearchEstadioFenologico where MaterialId=(select MaterialId from Material where Descripcion='Girasol') and EstadioId=(select EstadioId from ResearchEstadio where Descripcion='Cosecha')) BEGIN insert into ResearchEstadioFenologico (MaterialId, EstadioId, ConRendimiento) values((select MaterialId from Material where Descripcion='Girasol'),(select EstadioId from ResearchEstadio where Descripcion='Cosecha'),'1'); END

--ResearchTipoCarga
IF NOT EXISTS (select 1 from ResearchTipoCarga where Descripcion = 'Completa') BEGIN insert into ResearchTipoCarga (Descripcion) values('Completa'); END
IF NOT EXISTS (select 1 from ResearchTipoCarga where Descripcion = 'Express') BEGIN insert into ResearchTipoCarga (Descripcion) values('Express'); END

--ResearchTipoMuestra
IF NOT EXISTS (select 1 from ResearchTipoMuestra where Descripcion = 'Espigas en 1 m') BEGIN insert into ResearchTipoMuestra (Descripcion) values('Espigas en 1 m'); END
IF NOT EXISTS (select 1 from ResearchTipoMuestra where Descripcion = 'Granos por espiga') BEGIN insert into ResearchTipoMuestra (Descripcion) values('Granos por espiga'); END
IF NOT EXISTS (select 1 from ResearchTipoMuestra where Descripcion = 'Plantas en 1 m') BEGIN insert into ResearchTipoMuestra (Descripcion) values('Plantas en 1 m'); END
IF NOT EXISTS (select 1 from ResearchTipoMuestra where Descripcion = 'Vainas por planta') BEGIN insert into ResearchTipoMuestra (Descripcion) values('Vainas por planta'); END
IF NOT EXISTS (select 1 from ResearchTipoMuestra where Descripcion = 'Espigas en 10 mts') BEGIN insert into ResearchTipoMuestra (Descripcion) values('Espigas en 10 mts'); END
IF NOT EXISTS (select 1 from ResearchTipoMuestra where Descripcion = 'Granos por hilera (largo)') BEGIN insert into ResearchTipoMuestra (Descripcion) values('Granos por hilera (largo)'); END
IF NOT EXISTS (select 1 from ResearchTipoMuestra where Descripcion = 'Hileras por espiga') BEGIN insert into ResearchTipoMuestra (Descripcion) values('Hileras por espiga'); END
IF NOT EXISTS (select 1 from ResearchTipoMuestra where Descripcion = 'Capítulos en 10 mts') BEGIN insert into ResearchTipoMuestra (Descripcion) values('Capítulos en 10 mts'); END

