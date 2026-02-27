--BolsaCompraNet
IF NOT EXISTS (select 1 from BolsaCompraNet where Descripcion = 'Buenos Aires') BEGIN insert into BolsaCompraNet (Descripcion,CodigoSap) values ('Buenos Aires','01'); END
IF NOT EXISTS (select 1 from BolsaCompraNet where Descripcion = 'Rosario') BEGIN insert into BolsaCompraNet (Descripcion,CodigoSap) values ('Rosario','02'); END
IF NOT EXISTS (select 1 from BolsaCompraNet where Descripcion = 'Santa Fe') BEGIN insert into BolsaCompraNet (Descripcion,CodigoSap) values ('Santa Fe','03'); END
IF NOT EXISTS (select 1 from BolsaCompraNet where Descripcion = 'Cordoba') BEGIN insert into BolsaCompraNet (Descripcion,CodigoSap) values ('Cordoba','04'); END
IF NOT EXISTS (select 1 from BolsaCompraNet where Descripcion = 'Entre Ríos') BEGIN insert into BolsaCompraNet (Descripcion,CodigoSap) values ('Entre Ríos','05'); END
IF NOT EXISTS (select 1 from BolsaCompraNet where Descripcion = 'Bahía Blanca') BEGIN insert into BolsaCompraNet (Descripcion,CodigoSap) values ('Bahía Blanca','07'); END
IF NOT EXISTS (select 1 from BolsaCompraNet where Descripcion = 'Chaco') BEGIN insert into BolsaCompraNet (Descripcion,CodigoSap) values ('Chaco','08'); END

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
IF NOT EXISTS (select 1 from CalidadEspecial where CodigoSap = 'MPMAZGRA') BEGIN insert into CalidadEspecial (Descripcion, CodigoSap, MaterialId) values ('Grado','MPMAZGRA',1); END
IF NOT EXISTS (select 1 from CalidadEspecial where CodigoSap = 'MPTRPGRA') BEGIN insert into CalidadEspecial (Descripcion, CodigoSap, MaterialId) values ('Grado','MPTRPGRA',2); END
IF NOT EXISTS (select 1 from CalidadEspecial where CodigoSap = 'MPGIRCEX'AND MaterialId=4) BEGIN insert into CalidadEspecial (Descripcion, CodigoSap, MaterialId) values ('Materia Extraña','MPGIRCEX',4); END
IF NOT EXISTS (select 1 from CalidadEspecial where CodigoSap = 'MPGIRCEX' AND MaterialId=5) BEGIN insert into CalidadEspecial (Descripcion, CodigoSap, MaterialId) values ('Materia Extraña','MPGIRCEX',5); END

--TipoDB
IF NOT EXISTS (select 1 from TipoDB where Descripcion = 'Sobre el precio') BEGIN insert into TipoDB (Descripcion, CodigoSap) values ('Sobre el precio','S'); END
IF NOT EXISTS (select 1 from TipoDB where Descripcion = 'Por Fuera del Precio') BEGIN insert into TipoDB (Descripcion, CodigoSap) values ('Por Fuera del Precio','A'); END

--TipoPeriodoDB
IF NOT EXISTS (select 1 from TipoPeriodoDB where Descripcion = 'Generales') BEGIN insert into TipoPeriodoDB (Descripcion, CodigoSap) values ('Generales','G'); END

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
IF NOT EXISTS (select 1 from Clausula where Discriminator = 'ClausulaCuarentaYOcho') BEGIN insert into Clausula(Discriminator, Orden, Estado) values ('ClausulaCuarentaYOcho',48 , 1); END
IF NOT EXISTS (select 1 from Clausula where Discriminator = 'ClausulaCuarentaYNueve') BEGIN insert into Clausula(Discriminator, Orden, Estado) values ('ClausulaCuarentaYNueve',49 , 1); END
IF NOT EXISTS (select 1 from Clausula where Discriminator = 'ClausulaCincuenta') BEGIN insert into Clausula(Discriminator, Orden, Estado) values ('ClausulaCincuenta',50 , 1); END
IF NOT EXISTS (select 1 from Clausula where Discriminator = 'ClausulaCincuentaYUno') BEGIN insert into Clausula(Discriminator, Orden, Estado) values ('ClausulaCincuentaYUno',51 , 1); END
IF NOT EXISTS (select 1 from Clausula where Discriminator = 'ClausulaCincuentaYDos') BEGIN insert into Clausula(Discriminator, Orden, Estado) values ('ClausulaCincuentaYDos',52 , 1); END
IF NOT EXISTS (select 1 from Clausula where Discriminator = 'ClausulaCincuentaYTres') BEGIN insert into Clausula(Discriminator, Orden, Estado) values ('ClausulaCincuentaYTres',53 , 1); END
IF NOT EXISTS (select 1 from Clausula where Discriminator = 'ClausulaCincuentaYCuatro') BEGIN insert into Clausula(Discriminator, Orden, Estado) values ('ClausulaCincuentaYCuatro',54 , 1); END
IF NOT EXISTS (select 1 from Clausula where Discriminator = 'ClausulaCincuentaYCinco') BEGIN insert into Clausula(Discriminator, Orden, Estado) values ('ClausulaCincuentaYCinco',55 , 1); END
IF NOT EXISTS (select 1 from Clausula where Discriminator = 'ClausulaCincuentaYSeis') BEGIN insert into Clausula(Discriminator, Orden, Estado) values ('ClausulaCincuentaYSeis',56 , 1); END
IF NOT EXISTS (select 1 from Clausula where Discriminator = 'ClausulaCincuentaYSiete') BEGIN insert into Clausula(Discriminator, Orden, Estado) values ('ClausulaCincuentaYSiete',57 , 1); END
IF NOT EXISTS (select 1 from Clausula where Discriminator = 'ClausulaCincuentaYOcho') BEGIN insert into Clausula(Discriminator, Orden, Estado) values ('ClausulaCincuentaYOcho',58 , 1); END
IF NOT EXISTS (select 1 from Clausula where Discriminator = 'ClausulaCincuentaYNueve') BEGIN insert into Clausula(Discriminator, Orden, Estado) values ('ClausulaCincuentaYNueve',59 , 1); END

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
IF NOT EXISTS (select 1 from RolPermiso where RolId = (select Id from Rol where Descripcion ='Recibir Sugerencia FAQ') and Permiso = 50) BEGIN insert into RolPermiso (RolId, Permiso) values ((select Id from Rol where Descripcion ='Recibir Sugerencia FAQ'), 50); END
IF NOT EXISTS (select 1 from RolPermiso where RolId = (select Id from Rol where Descripcion ='Algoritmo de Cupos') and Permiso = 735) BEGIN insert into RolPermiso (RolId, Permiso) values ((select Id from Rol where Descripcion ='Algoritmo de Cupos'), 735); END
IF NOT EXISTS (select 1 from RolPermiso where RolId = (select Id from Rol where Descripcion ='Administrador') and Permiso = -1) BEGIN insert into RolPermiso (RolId, Permiso) values ((select Id from Rol where Descripcion ='Administrador'), -1); END
IF NOT EXISTS (select 1 from RolPermiso where RolId = (select Id from Rol where Descripcion ='BoletoAdmin') and Permiso = 920) BEGIN insert into RolPermiso (RolId, Permiso) values ((select Id from Rol where Descripcion ='BoletoAdmin'), 920); END

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
IF NOT EXISTS (select 1 from ResearchHumedadSuelo where Descripcion = 'Humedo en superficie') BEGIN insert into ResearchHumedadSuelo (Descripcion) values('Húmedo en superficie'); END
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
IF NOT EXISTS (select 1 from ResearchTipoMuestra where Descripcion = 'Espigas en la hilera (1 m)') BEGIN insert into ResearchTipoMuestra (Descripcion) values('Espigas en la hilera (1 m)'); END
IF NOT EXISTS (select 1 from ResearchTipoMuestra where Descripcion = 'Plantas en la hilera (1 m)') BEGIN insert into ResearchTipoMuestra (Descripcion) values('Plantas en la hilera (1 m)'); END
IF NOT EXISTS (select 1 from ResearchTipoMuestra where Descripcion = 'Espigas en la hilera (10 mts)') BEGIN insert into ResearchTipoMuestra (Descripcion) values('Espigas en la hilera (10 mts)'); END
IF NOT EXISTS (select 1 from ResearchTipoMuestra where Descripcion = 'Diámetro del Capítulo (cm)') BEGIN insert into ResearchTipoMuestra (Descripcion) values('Diámetro del Capítulo (cm)'); END
IF NOT EXISTS (select 1 from ResearchTipoMuestra where Descripcion = 'Granos/espiga') BEGIN insert into ResearchTipoMuestra (Descripcion) values('Granos/espiga'); END
IF NOT EXISTS (select 1 from ResearchTipoMuestra where Descripcion = 'Vainas por planta') BEGIN insert into ResearchTipoMuestra (Descripcion) values('Vainas por planta'); END
IF NOT EXISTS (select 1 from ResearchTipoMuestra where Descripcion = 'Largo de la espiga (granos)') BEGIN insert into ResearchTipoMuestra (Descripcion) values('Largo de la espiga (granos)'); END
IF NOT EXISTS (select 1 from ResearchTipoMuestra where Descripcion = 'Granos alrededor') BEGIN insert into ResearchTipoMuestra (Descripcion) values('Granos alrededor'); END

--PuestoApoderado
IF NOT EXISTS (select 1 from PuestoApoderado where Descripcion = 'Presidente') BEGIN insert into PuestoApoderado (Descripcion) values ('Presidente'); END
IF NOT EXISTS (select 1 from PuestoApoderado where Descripcion = 'Apoderado') BEGIN insert into PuestoApoderado (Descripcion) values ('Apoderado'); END
IF NOT EXISTS (select 1 from PuestoApoderado where Descripcion = 'Otros') BEGIN insert into PuestoApoderado (Descripcion) values ('Otros'); END

--TipoDeCambio
IF NOT EXISTS (select 1 from TipoDeCambio where Descripcion = 'BNA') BEGIN insert into TipoDeCambio (Descripcion) values ('BNA'); END
IF NOT EXISTS (select 1 from TipoDeCambio where Descripcion = 'BLEND') BEGIN insert into TipoDeCambio (Descripcion) values ('BLEND'); END

--TipoDeCambio
insert into Log (Fecha, Xml) values (CURRENT_TIMESTAMP, 'Prueba deploy');

-- CREACION DE NUEVO ROL Y PERMISO POR ROL DE VISUALIZAR CONTRATO FASON
IF NOT EXISTS(SELECT 1 FROM Rol WHERE Descripcion = 'Visualizar Contratos Fason')
   BEGIN
		INSERT INTO Rol (Descripcion)VALUES('Visualizar Contratos Fason')
   END

DECLARE @RolVisCtoFasonId int
 SELECT @RolVisCtoFasonId = Id from Rol 
  WHERE Descripcion = 'Visualizar Contratos Fason'

IF NOT EXISTS(SELECT 1 FROM RolPermiso WHERE RolId = @RolVisCtoFasonId and Permiso = 312)
   BEGIN
		INSERT INTO RolPermiso (RolId,Permiso)VALUES(@RolVisCtoFasonId, 312)
   END

-- INICIO - Índices provistos por Algeiba
IF NOT EXISTS (
    SELECT 1 
    FROM sys.indexes 
    WHERE name = 'ProveedorId_CampanaId_MaterialId'
    AND object_id = OBJECT_ID('[dbo].[CampanaMaterialDetallePorMes]')
)
BEGIN
    CREATE NONCLUSTERED INDEX ProveedorId_CampanaId_MaterialId
    ON [dbo].[CampanaMaterialDetallePorMes] ([ProveedorId],[CampanaId],[MaterialId])
    WITH (SORT_IN_TEMPDB = ON, ONLINE = OFF, FILLFACTOR = 90) ON [PRIMARY];
END;

IF NOT EXISTS (
    SELECT 1 
    FROM sys.indexes 
    WHERE name = 'ndx_ID_FechaOperacion_AnulaYReemplazaContratoId'
    AND object_id = OBJECT_ID('[dbo].[Negocio]')
)
BEGIN
    CREATE NONCLUSTERED INDEX ndx_ID_FechaOperacion_AnulaYReemplazaContratoId
    ON [dbo].[Negocio] ([Id],[FechaOperacion],[AnulaYReemplazaContratoId],[DestinoId],[EstadoId],[TipoNegocioId],[Discriminator],[MaterialId],[OcultarEnTablero],[Pizarra],[PrestamoDevolucion],[TipoPosicionCBOTId],[TipoAgenteCompraId],[Venta])
    INCLUDE ([Cantidad], [Precio], [MonedaId], [PrecioNeto], [PrecioNetoPonderado], [Condicional])
    WITH (SORT_IN_TEMPDB = ON, ONLINE = OFF, FILLFACTOR = 90) ON [PRIMARY];
END;

IF NOT EXISTS (
    SELECT 1 
    FROM sys.indexes 
    WHERE name = 'ndx_TipoAgenteCompraId_OcultarEnTablero'
    AND object_id = OBJECT_ID('[dbo].[Negocio]')
)
BEGIN
    CREATE NONCLUSTERED INDEX ndx_TipoAgenteCompraId_OcultarEnTablero
    ON [dbo].[Negocio] ([TipoAgenteCompraId],[OcultarEnTablero],[AnulaYReemplazaContratoId],[MaterialId],[EstadoId])
    INCLUDE ([TipoNegocioId],[Cantidad],[Precio],[MonedaId],[DestinoId],[ContratoAcuerdoId],[Pizarra],[PrecioNeto],[Discriminator],[FechaOperacion],[PrestamoDevolucion],[Venta],[TipoPosicionCBOTId],[PrecioNetoPonderado],[Condicional]);
END;

IF NOT EXISTS (
    SELECT 1 
    FROM sys.indexes 
    WHERE name = 'ndx_ProveedorId_EsPrincipal'
    AND object_id = OBJECT_ID('[dbo].[ContactoComercial]')
)
BEGIN
    CREATE NONCLUSTERED INDEX ndx_ProveedorId_EsPrincipal
    ON [dbo].[ContactoComercial] ([ProveedorId],[EsPrincipal])
    INCLUDE ([Telefono1],[Telefono2],[Telefono3],[Email1],[Email2],[Email3]);
END;

---- SE COMENTA POR ERROR: "Lock request time out period exceeded."
--IF NOT EXISTS (
--    SELECT 1 
--    FROM sys.indexes 
--    WHERE name = 'IX_FechaIngreso_CentroId_MaterialId_ConDescarga_NegocioId_EstadoCupoId'
--    AND object_id = OBJECT_ID('[dbo].[Cupo]')
--)
--BEGIN
--    CREATE NONCLUSTERED INDEX IX_FechaIngreso_CentroId_MaterialId_ConDescarga_NegocioId_EstadoCupoId 
--    ON Cupo ( FechaIngreso, CentroId, MaterialId, ConDescarga, NegocioId, EstadoCupoId )
--END;

---- SE COMENTA POR ERROR: "Lock request time out period exceeded."
--IF NOT EXISTS (
--    SELECT 1 
--    FROM sys.indexes 
--    WHERE name = 'IX_FechaIngreso_CentroId_MaterialId_EstadoCupoId'
--    AND object_id = OBJECT_ID('[dbo].[Cupo]')
--)
--BEGIN
--    CREATE NONCLUSTERED INDEX IX_FechaIngreso_CentroId_MaterialId_EstadoCupoId 
--    ON Cupo ( FechaIngreso, CentroId, MaterialId, EstadoCupoId )
--END;

-- FIN - Índices provistos por Algeiba

IF NOT EXISTS (
    SELECT 1 
    FROM sys.indexes 
    WHERE name = 'NDX_ComercialId'
    AND object_id = OBJECT_ID('[dbo].[InformeComercialApertura]')
)
BEGIN
    CREATE NONCLUSTERED INDEX [NDX_ComercialId]
    ON [dbo].[InformeComercialApertura] ([ComercialId])
    INCLUDE ([Id],[FechaApertura])
END;

IF NOT EXISTS (
    SELECT 1 
    FROM sys.indexes 
    WHERE name = 'NDX_OcultarEnTablero_EstadoId'
    AND object_id = OBJECT_ID('[dbo].[Negocio]')
)
BEGIN
    CREATE NONCLUSTERED INDEX NDX_OcultarEnTablero_EstadoId
    ON [dbo].[Negocio] ([OcultarEnTablero],[EstadoId])
    INCLUDE ([MaterialId],[TipoNegocioId],[Cantidad],[Precio],[MonedaId],[Fecha],[ComercialId],[ComercialCreadorId],[ContratoAcuerdoId],[Pizarra],[PrecioNeto],[TipoAgenteCompraId],[Discriminator],[FechaOperacion],[Canje],[TipoPosicionCBOTId],[AnulaYReemplazaContratoId],[PrecioNetoPonderado],[Condicional]);
END;

IF NOT EXISTS (
    SELECT 1 
    FROM sys.indexes 
    WHERE name = 'NDX_AperturaPrecio_NegocioId_Porcentaje'
    AND object_id = OBJECT_ID('[dbo].[AperturaPrecio]')
)
BEGIN
    CREATE NONCLUSTERED INDEX NDX_AperturaPrecio_NegocioId_Porcentaje
    ON [dbo].[AperturaPrecio] ([ConceptoAperturaPrecioId],[NegocioId],[Porcentaje])
END;

IF NOT EXISTS (
    SELECT 1 
    FROM sys.indexes 
    WHERE name = 'NDX_fecha'
    AND object_id = OBJECT_ID('[dbo].[log]')
)
BEGIN
    CREATE NONCLUSTERED INDEX NDX_fecha
    ON [dbo].[log] ([fecha])
END;

IF NOT EXISTS (
    SELECT 1 
    FROM sys.indexes 
    WHERE name = 'NDX_Discriminator_EstadoId_ContratoSAP'
    AND object_id = OBJECT_ID('[dbo].[Negocio]')
)
BEGIN
    CREATE NONCLUSTERED INDEX NDX_Discriminator_EstadoId_ContratoSAP
    ON [dbo].[Negocio] ([Discriminator],[EstadoId],[ContratoSAP])
    INCLUDE ([ConfirmadoSAP],[FechaConfirmadoSAP])
END;

IF NOT EXISTS (
    SELECT 1 
    FROM sys.indexes 
    WHERE name = 'NDX_EstadoCupoId'
    AND object_id = OBJECT_ID('[dbo].[Cupo]')
)
BEGIN
    CREATE NONCLUSTERED INDEX NDX_EstadoCupoId
    ON [dbo].[Cupo] ([EstadoCupoId])
    INCLUDE ([CentroId],[FechaIngreso])
END;

IF NOT EXISTS (
    SELECT 1 
    FROM sys.indexes 
    WHERE name = 'NDX_CentroId'
    AND object_id = OBJECT_ID('[dbo].[Cupo]')
)
BEGIN
    CREATE NONCLUSTERED INDEX NDX_CentroId
    ON [dbo].[Cupo] ([CentroId])
    INCLUDE ([FechaIngreso],[EstadoCupoId])
END;

IF NOT EXISTS (
    SELECT 1 
    FROM sys.indexes 
    WHERE name = 'NDX_Pizarra_Discriminator_OcultarEnTablero_MaterialId_EstadoId_FechaOperacion'
    AND object_id = OBJECT_ID('[dbo].[Negocio]')
)
BEGIN
    CREATE NONCLUSTERED INDEX NDX_Pizarra_Discriminator_OcultarEnTablero_MaterialId_EstadoId_FechaOperacion
    ON [dbo].[Negocio] ([Pizarra],[Discriminator],[OcultarEnTablero],[MaterialId],[EstadoId],[FechaOperacion])
    INCLUDE ([Canje])
END;

IF NOT EXISTS (
    SELECT 1 
    FROM sys.indexes 
    WHERE name = 'NDX_NegocioId'
    AND object_id = OBJECT_ID('[dbo].[Servicio]')
)
BEGIN
    CREATE NONCLUSTERED INDEX NDX_NegocioId
    ON [dbo].[Servicio] ([NegocioId])
END;

IF NOT EXISTS (
    SELECT 1 
    FROM sys.indexes 
    WHERE name = 'NDX_NegocioId'
    AND object_id = OBJECT_ID('[dbo].[Calidad]')
)
BEGIN
    CREATE NONCLUSTERED INDEX NDX_NegocioId
    ON [dbo].[Calidad] ([NegocioId])
END;

IF NOT EXISTS (
    SELECT 1 
    FROM sys.indexes 
    WHERE name = 'NDX_FechaAlta'
    AND object_id = OBJECT_ID('[dbo].[Proveedor]')
)
BEGIN
    CREATE NONCLUSTERED INDEX NDX_FechaAlta
    ON [dbo].[Proveedor] ([FechaAlta])
    INCLUDE ([CUIT])
END;

IF NOT EXISTS (
    SELECT 1 
    FROM sys.indexes 
    WHERE name = 'NDX_Discriminator'
    AND object_id = OBJECT_ID('[dbo].[Negocio]')
)
BEGIN
    CREATE NONCLUSTERED INDEX NDX_Discriminator
    ON [dbo].[Negocio] ([Discriminator])
    INCLUDE ([Fecha],[ComercialId],[ComercialCreadorId])
END;

IF NOT EXISTS (
    SELECT 1 
    FROM sys.indexes 
    WHERE name = 'NDX_SugerenciaCupoId'
    AND object_id = OBJECT_ID('[dbo].[AdministracionCupo]')
)
BEGIN
    CREATE NONCLUSTERED INDEX NDX_SugerenciaCupoId
    ON [dbo].[AdministracionCupo] ([SugerenciaCupoId])
END;

IF NOT EXISTS (
    SELECT 1 
    FROM sys.indexes 
    WHERE name = 'NDX_MaterialId_EstadoId_DestinoId_Discriminator_EPA_EUDR'
    AND object_id = OBJECT_ID('[dbo].[Negocio]')
)
BEGIN
    CREATE NONCLUSTERED INDEX NDX_MaterialId_EstadoId_DestinoId_Discriminator_EPA_EUDR
    ON [dbo].[Negocio] ([MaterialId],[EstadoId],[DestinoId],[Discriminator],[EPA],[EUDR])
    INCLUDE ([TipoNegocioId],[Cantidad],[Precio],[FechaDesde],[FechaHasta],[ProveedorId],[MonedaId],[ComercialId],[ContratoSAP],[CD],[Warrant],[MercsDeposito],[CorredorId],[StandardDeCalidadId],[Sustentable],[TipoAgenteCompraId],[EsFason],[CaratulaMAT],[Canje],[FechaHastaOriginal],[ConDescarga])
END;

IF NOT EXISTS (
    SELECT 1 
    FROM sys.indexes 
    WHERE name = 'NDX_Discriminator_AnulaYReemplazaContratoId'
    AND object_id = OBJECT_ID('[dbo].[Negocio]')
)
BEGIN
    CREATE NONCLUSTERED INDEX NDX_Discriminator_AnulaYReemplazaContratoId
    ON [dbo].[Negocio] ([Discriminator],[AnulaYReemplazaContratoId])
END;

IF NOT EXISTS (
    SELECT 1 
    FROM sys.indexes 
    WHERE name = 'NDX_MaterialId_Discriminator_OcultarEnTablero_EstadoId_FechaOperacion'
    AND object_id = OBJECT_ID('[dbo].[Negocio]')
)
BEGIN
    CREATE NONCLUSTERED INDEX NDX_MaterialId_Discriminator_OcultarEnTablero_EstadoId_FechaOperacion
    ON [dbo].[Negocio] ([MaterialId],[Discriminator],[OcultarEnTablero],[EstadoId],[FechaOperacion])
    INCLUDE ([Cantidad],[CampanaId],[Posicion])
END;

IF NOT EXISTS (
    SELECT 1 
    FROM sys.indexes 
    WHERE name = 'NDX_TipoAgenteCompraId_OcultarEnTablero_TipoNegocioId_EstadoId_Discriminator'
    AND object_id = OBJECT_ID('[dbo].[Negocio]')
)
BEGIN
    CREATE NONCLUSTERED INDEX NDX_TipoAgenteCompraId_OcultarEnTablero_TipoNegocioId_EstadoId_Discriminator
    ON [dbo].[Negocio] ([TipoAgenteCompraId],[OcultarEnTablero],[TipoNegocioId],[EstadoId],[Discriminator])
    INCLUDE ([ContratoId],[MaterialId],[Cantidad],[Precio],[CampanaId],[FechaDesde],[FechaHasta],[MonedaId],[Fecha],[TrigoEspecial],[ContratoSAP],[DestinoId],[ContratoAcuerdoId],[Pizarra],[StandardDeCalidadId],[Posicion],[EsFason],[FechaOperacion],[Canje],[PrestamoDevolucion],[Virtual],[TipoPosicionCBOTId],[AnulaYReemplazaContratoId],[PrecioNetoPonderado],[CantidadDeposito])
END;

IF NOT EXISTS (
    SELECT 1 
    FROM sys.indexes 
    WHERE name = 'NDX_Job_GrabarDatosReporteCompraNet_01'
    AND object_id = OBJECT_ID('[dbo].[Negocio]')
)
BEGIN
    CREATE NONCLUSTERED INDEX NDX_Job_GrabarDatosReporteCompraNet_01
    ON [dbo].[Negocio] ([ContratoAcuerdoId],[TipoAgenteCompraId],[Discriminator],[OcultarEnTablero],[AnulaYReemplazaContratoId],[MaterialId],[EstadoId],[FechaOperacion])
    INCLUDE ([TipoNegocioId],[Cantidad],[CampanaId],[DestinoId],[TipoPosicionCBOTId])
END;

IF NOT EXISTS (
    SELECT 1 
    FROM sys.indexes 
    WHERE name = 'NDX_ConceptoAperturaPrecioId'
    AND object_id = OBJECT_ID('[dbo].[AperturaPrecio]')
)
BEGIN
    CREATE NONCLUSTERED INDEX NDX_ConceptoAperturaPrecioId
    ON [dbo].[AperturaPrecio] ([ConceptoAperturaPrecioId])
    INCLUDE ([Importe],[NegocioId])
END;

IF NOT EXISTS (
    SELECT 1 
    FROM sys.indexes 
    WHERE name = 'NDX_NegocioId'
    AND object_id = OBJECT_ID('[dbo].[AperturaPrecio]')
)
BEGIN
    CREATE NONCLUSTERED INDEX NDX_NegocioId
    ON [dbo].[AperturaPrecio] ([NegocioId])
    INCLUDE ([ConceptoAperturaPrecioId],[Importe])
END;

IF NOT EXISTS (
    SELECT 1 
    FROM sys.indexes 
    WHERE name = 'NDX_TipoAgenteCompraId_Discriminator_OcultarEnTablero_AnulaYReemplazaContratoId_MaterialId_EstadoId_FechaOperacion'
    AND object_id = OBJECT_ID('[dbo].[Negocio]')
)
BEGIN
    CREATE NONCLUSTERED INDEX NDX_TipoAgenteCompraId_Discriminator_OcultarEnTablero_AnulaYReemplazaContratoId_MaterialId_EstadoId_FechaOperacion
    ON [dbo].[Negocio] ([TipoAgenteCompraId],[Discriminator],[OcultarEnTablero],[AnulaYReemplazaContratoId],[MaterialId],[EstadoId],[FechaOperacion])
    INCLUDE ([TipoNegocioId],[Cantidad],[Precio],[MonedaId],[DestinoId],[ContratoAcuerdoId],[Pizarra],[PrecioNeto],[PrestamoDevolucion],[Venta],[TipoPosicionCBOTId],[PrecioNetoPonderado],[Condicional])
END;

IF NOT EXISTS (
    SELECT 1 
    FROM sys.indexes 
    WHERE name = 'NDX_Excedente'
    AND object_id = OBJECT_ID('[dbo].[ComercialId]')
)
BEGIN
    CREATE NONCLUSTERED INDEX NDX_Excedente
    ON [dbo].[AdministracionCupo] ([Excedente])
    INCLUDE ([ComercialId])
END;

IF NOT EXISTS (
    SELECT 1 
    FROM sys.indexes 
    WHERE name = 'NDX_Excedente_ComercialId_FechaCreacion'
    AND object_id = OBJECT_ID('[dbo].[AdministracionCupo]')
)
BEGIN
    CREATE NONCLUSTERED INDEX NDX_Excedente_ComercialId_FechaCreacion
    ON [dbo].[AdministracionCupo] ([Excedente])
    INCLUDE ([ComercialId],[FechaCreacion])
END;

IF NOT EXISTS (
    SELECT 1 
    FROM sys.indexes 
    WHERE name = 'NDX_MaterialId_Aceptado_ComercialId_FechaSugerida'
    AND object_id = OBJECT_ID('[dbo].[SugerenciaCupo]')
)
BEGIN
    CREATE NONCLUSTERED INDEX NDX_MaterialId_Aceptado_ComercialId_FechaSugerida
    ON [dbo].[SugerenciaCupo] ([MaterialId],[Aceptado],[ComercialId],[FechaSugerida])
    INCLUDE ([CentroId],[CantidadDeCupos],[TipoNegocioId],[Puntuacion],[MonedaId],[Precio],[ProveedorId],[ZonaCupoId],[Puntuaciones],[ContratoSAP],[NegocioId],[KgNegocio],[KgPendienteAplicar])
END;

IF NOT EXISTS (
    SELECT 1 
    FROM sys.indexes 
    WHERE name = 'NDX_MaterialId_Fecha'
    AND object_id = OBJECT_ID('[dbo].[ConfiguracionCupo]')
)
BEGIN
    CREATE NONCLUSTERED INDEX NDX_MaterialId_Fecha
    ON [dbo].[ConfiguracionCupo] ([MaterialId],[Fecha])
END;

IF NOT EXISTS (
    SELECT 1 
    FROM sys.indexes 
    WHERE name = 'NDX_ProveedorId_EstadoId_MaterialId_NegocioId'
    AND object_id = OBJECT_ID('[dbo].[AdministracionCupo]')
)
BEGIN
    CREATE NONCLUSTERED INDEX NDX_ProveedorId_EstadoId_MaterialId_NegocioId
    ON [dbo].[AdministracionCupo] ([ProveedorId],[EstadoId],[MaterialId],[NegocioId])
END;

IF NOT EXISTS (
    SELECT 1 
    FROM sys.indexes 
    WHERE name = 'NDX_MaterialId_ProveedorId_TipoNegocioId_Cantidad_Discriminator'
    AND object_id = OBJECT_ID('[dbo].[Negocio]')
)
BEGIN
    CREATE NONCLUSTERED INDEX NDX_MaterialId_ProveedorId_TipoNegocioId_Cantidad_Discriminator
    ON [dbo].[Negocio] ([MaterialId],[ProveedorId],[TipoNegocioId],[Cantidad],[Discriminator])
    INCLUDE ([FechaHasta],[EstadoId],[ContratoSAP],[Sustentable],[FechaHastaOriginal],[EPA],[EUDR])
END;

IF NOT EXISTS (
    SELECT 1 
    FROM sys.indexes 
    WHERE name = 'NDX_CentroId_MaterialId_Fecha'
    AND object_id = OBJECT_ID('[dbo].[ConfiguracionCupo]')
)
BEGIN
    CREATE NONCLUSTERED INDEX NDX_CentroId_MaterialId_Fecha
    ON [dbo].[ConfiguracionCupo] ([CentroId],[MaterialId],[Fecha])
    INCLUDE ([LimiteCupo],[CierreCupera],[LimiteAlgoritmo],[LiberarCupera],[LimiteAnterior],[LimiteDescarga])
END;

IF NOT EXISTS (
    SELECT 1 
    FROM sys.indexes 
    WHERE name = 'NDX_MaterialId_FechaIngreso'
    AND object_id = OBJECT_ID('[dbo].[Cupo]')
)
BEGIN
    CREATE NONCLUSTERED INDEX NDX_MaterialId_FechaIngreso
    ON [dbo].[Cupo] ([MaterialId],[FechaIngreso])
    INCLUDE ([ComercialId],[UsuarioCreador])
END;

IF NOT EXISTS (
    SELECT 1 
    FROM sys.indexes 
    WHERE name = 'NDX_FechaIngreso'
    AND object_id = OBJECT_ID('[dbo].[Cupo]')
)
BEGIN
    CREATE NONCLUSTERED INDEX NDX_FechaIngreso
    ON [dbo].[Cupo] ([FechaIngreso])
    INCLUDE ([ComercialId],[UsuarioCreador])
END;

IF NOT EXISTS (
    SELECT 1 
    FROM sys.indexes 
    WHERE name = 'NDX_CupoSap'
    AND object_id = OBJECT_ID('[dbo].[CupoSap]')
)
BEGIN
    CREATE NONCLUSTERED INDEX NDX_CupoSap
    ON [dbo].[Cupo] ([CupoSap])
END;

IF NOT EXISTS (
    SELECT 1 
    FROM sys.indexes 
    WHERE name = 'NDX_Excedente_Fecha'
    AND object_id = OBJECT_ID('[dbo].[AdministracionCupo]')
)
BEGIN
    CREATE NONCLUSTERED INDEX NDX_Excedente_Fecha
    ON [dbo].[AdministracionCupo] ([Excedente],[Fecha])
    INCLUDE ([ComercialId])
END;
-- CREACION DE NUEVO ROL Y PERMISO PARA INFORME COMERCIAL ADMINISTRADOR
IF NOT EXISTS(SELECT 1 FROM Rol WHERE Descripcion = 'Ver todos los contactos por proveedor')
   BEGIN
		INSERT INTO Rol (Descripcion)VALUES('Ver todos los contactos por proveedor')
   END

DECLARE @RolTodosContactos int
 SELECT @RolTodosContactos = Id from Rol 
  WHERE Descripcion = 'Ver todos los contactos por proveedor'

IF NOT EXISTS(SELECT 1 FROM RolPermiso WHERE RolId = @RolTodosContactos and Permiso = 921)
   BEGIN
		INSERT INTO RolPermiso (RolId,Permiso)VALUES(@RolTodosContactos, 921)
   END


--Habilitacion Job HangFire
IF NOT EXISTS (SELECT TOP 1 1 FROM HabilitacionJob WHERE Nombre = 'ActualizarCumplimientoCuposHangfireJob') BEGIN INSERT INTO HabilitacionJob VALUES ('ActualizarCumplimientoCuposHangfireJob', 0) END
IF NOT EXISTS (SELECT TOP 1 1 FROM HabilitacionJob WHERE Nombre = 'ActualizarEstadoDeContratosHangfireJob') BEGIN INSERT INTO HabilitacionJob VALUES ('ActualizarEstadoDeContratosHangfireJob', 0) END
IF NOT EXISTS (SELECT TOP 1 1 FROM HabilitacionJob WHERE Nombre = 'ActualizarFechaUltimaActualizacionManualesFAQHangfireJob') BEGIN INSERT INTO HabilitacionJob VALUES ('ActualizarFechaUltimaActualizacionManualesFAQHangfireJob', 0) END
IF NOT EXISTS (SELECT TOP 1 1 FROM HabilitacionJob WHERE Nombre = 'ActualizarMailProveedorHangfireJob') BEGIN INSERT INTO HabilitacionJob VALUES ('ActualizarMailProveedorHangfireJob', 0) END
IF NOT EXISTS (SELECT TOP 1 1 FROM HabilitacionJob WHERE Nombre = 'ActualizarPrecioPizarraHangfireJob') BEGIN INSERT INTO HabilitacionJob VALUES ('ActualizarPrecioPizarraHangfireJob', 0) END
IF NOT EXISTS (SELECT TOP 1 1 FROM HabilitacionJob WHERE Nombre = 'ActualizarProveedoresHomeHangfireJob') BEGIN INSERT INTO HabilitacionJob VALUES ('ActualizarProveedoresHomeHangfireJob', 0) END
IF NOT EXISTS (SELECT TOP 1 1 FROM HabilitacionJob WHERE Nombre = 'ActualizarRazonSocialHangfireJob') BEGIN INSERT INTO HabilitacionJob VALUES ('ActualizarRazonSocialHangfireJob', 0) END
IF NOT EXISTS (SELECT TOP 1 1 FROM HabilitacionJob WHERE Nombre = 'AnularAcuerdosHangfireJob') BEGIN INSERT INTO HabilitacionJob VALUES ('AnularAcuerdosHangfireJob', 0) END
IF NOT EXISTS (SELECT TOP 1 1 FROM HabilitacionJob WHERE Nombre = 'BorradoContratosHangfireJob') BEGIN INSERT INTO HabilitacionJob VALUES ('BorradoContratosHangfireJob', 0) END
IF NOT EXISTS (SELECT TOP 1 1 FROM HabilitacionJob WHERE Nombre = 'CerrarDiaHangfireJob') BEGIN INSERT INTO HabilitacionJob VALUES ('CerrarDiaHangfireJob', 0) END
IF NOT EXISTS (SELECT TOP 1 1 FROM HabilitacionJob WHERE Nombre = 'ConfirmacionAutomaticaPizarra13HrsHangfireJob') BEGIN INSERT INTO HabilitacionJob VALUES ('ConfirmacionAutomaticaPizarra13HrsHangfireJob', 0) END
IF NOT EXISTS (SELECT TOP 1 1 FROM HabilitacionJob WHERE Nombre = 'ConsultarCuposDiariosHangfireJob') BEGIN INSERT INTO HabilitacionJob VALUES ('ConsultarCuposDiariosHangfireJob', 0) END
IF NOT EXISTS (SELECT TOP 1 1 FROM HabilitacionJob WHERE Nombre = 'ConsultarMisturnosActivosHangfireJob') BEGIN INSERT INTO HabilitacionJob VALUES ('ConsultarMisturnosActivosHangfireJob', 0) END
IF NOT EXISTS (SELECT TOP 1 1 FROM HabilitacionJob WHERE Nombre = 'CrearSugerenciaCupoHangfireJob') BEGIN INSERT INTO HabilitacionJob VALUES ('CrearSugerenciaCupoHangfireJob', 0) END
IF NOT EXISTS (SELECT TOP 1 1 FROM HabilitacionJob WHERE Nombre = 'EnviarMailConfirmaHangfireJob') BEGIN INSERT INTO HabilitacionJob VALUES ('EnviarMailConfirmaHangfireJob', 0) END
IF NOT EXISTS (SELECT TOP 1 1 FROM HabilitacionJob WHERE Nombre = 'EnviarMailSugerenciasPendientesPorComercialHangfireJob') BEGIN INSERT INTO HabilitacionJob VALUES ('EnviarMailSugerenciasPendientesPorComercialHangfireJob', 0) END
IF NOT EXISTS (SELECT TOP 1 1 FROM HabilitacionJob WHERE Nombre = 'EnvioMailNegociosAnulaYReemplazaHangfireJob') BEGIN INSERT INTO HabilitacionJob VALUES ('EnvioMailNegociosAnulaYReemplazaHangfireJob', 0) END
IF NOT EXISTS (SELECT TOP 1 1 FROM HabilitacionJob WHERE Nombre = 'EnvioMailNegociosConDiaAnteriorHangfireJob') BEGIN INSERT INTO HabilitacionJob VALUES ('EnvioMailNegociosConDiaAnteriorHangfireJob', 0) END
IF NOT EXISTS (SELECT TOP 1 1 FROM HabilitacionJob WHERE Nombre = 'EnvioMailPendientesHangfireJob') BEGIN INSERT INTO HabilitacionJob VALUES ('EnvioMailPendientesHangfireJob', 0) END
IF NOT EXISTS (SELECT TOP 1 1 FROM HabilitacionJob WHERE Nombre = 'EnvioMailSinCTGHangfireJob') BEGIN INSERT INTO HabilitacionJob VALUES ('EnvioMailSinCTGHangfireJob', 0) END
IF NOT EXISTS (SELECT TOP 1 1 FROM HabilitacionJob WHERE Nombre = 'FinalizacionContratosHangfireJob') BEGIN INSERT INTO HabilitacionJob VALUES ('FinalizacionContratosHangfireJob', 0) END
IF NOT EXISTS (SELECT TOP 1 1 FROM HabilitacionJob WHERE Nombre = 'GrabarDatosReporteCompraNetHangfireJob') BEGIN INSERT INTO HabilitacionJob VALUES ('GrabarDatosReporteCompraNetHangfireJob', 0) END
IF NOT EXISTS (SELECT TOP 1 1 FROM HabilitacionJob WHERE Nombre = 'MigrarContratosPrimaryHangfireJob') BEGIN INSERT INTO HabilitacionJob VALUES ('MigrarContratosPrimaryHangfireJob', 0) END
IF NOT EXISTS (SELECT TOP 1 1 FROM HabilitacionJob WHERE Nombre = 'PesificadosHangfireJob') BEGIN INSERT INTO HabilitacionJob VALUES ('PesificadosHangfireJob', 0) END
IF NOT EXISTS (SELECT TOP 1 1 FROM HabilitacionJob WHERE Nombre = 'RechazarSolicitudesVencidasHangfireJob') BEGIN INSERT INTO HabilitacionJob VALUES ('RechazarSolicitudesVencidasHangfireJob', 0) END
IF NOT EXISTS (SELECT TOP 1 1 FROM HabilitacionJob WHERE Nombre = 'ReportePagosDiferidosHangfireJob') BEGIN INSERT INTO HabilitacionJob VALUES ('ReportePagosDiferidosHangfireJob', 0) END
IF NOT EXISTS (SELECT TOP 1 1 FROM HabilitacionJob WHERE Nombre = 'SincronizarResearchHangfireJob') BEGIN INSERT INTO HabilitacionJob VALUES ('SincronizarResearchHangfireJob', 0) END
IF NOT EXISTS (SELECT TOP 1 1 FROM HabilitacionJob WHERE Nombre = 'TransmitirCupoStopHangfireJob') BEGIN INSERT INTO HabilitacionJob VALUES ('TransmitirCupoStopHangfireJob', 0) END
IF NOT EXISTS (SELECT TOP 1 1 FROM HabilitacionJob WHERE Nombre = 'VerificarSolicitudesExtraordinariasPendientesHangfireJob') BEGIN INSERT INTO HabilitacionJob VALUES ('VerificarSolicitudesExtraordinariasPendientesHangfireJob', 0) END
IF NOT EXISTS (SELECT TOP 1 1 FROM HabilitacionJob WHERE Nombre = 'ProcessComprasAyerHangfireJob') BEGIN INSERT INTO HabilitacionJob VALUES ('ProcessComprasAyerHangfireJob', 0) END
IF NOT EXISTS (SELECT TOP 1 1 FROM HabilitacionJob WHERE Nombre = 'ProcessRg2300HangfireJob') BEGIN INSERT INTO HabilitacionJob VALUES ('ProcessRg2300HangfireJob', 0) END
IF NOT EXISTS (SELECT TOP 1 1 FROM HabilitacionJob WHERE Nombre = 'ProcessSisaHangfireJob') BEGIN INSERT INTO HabilitacionJob VALUES ('ProcessSisaHangfireJob', 0) END
IF NOT EXISTS (SELECT TOP 1 1 FROM HabilitacionJob WHERE Nombre = 'ProcessFacacopHangfireJob') BEGIN INSERT INTO HabilitacionJob VALUES ('ProcessFacacopHangfireJob', 0) END
IF NOT EXISTS (SELECT TOP 1 1 FROM HabilitacionJob WHERE Nombre = 'ProcessEstadoHangfireJob') BEGIN INSERT INTO HabilitacionJob VALUES ('ProcessEstadoHangfireJob', 0) END
IF NOT EXISTS (SELECT TOP 1 1 FROM HabilitacionJob WHERE Nombre = 'ProcessComprasHangfireJob') BEGIN INSERT INTO HabilitacionJob VALUES ('ProcessComprasHangfireJob', 0) END
IF NOT EXISTS (SELECT TOP 1 1 FROM HabilitacionJob WHERE Nombre = 'ProcessComprasDetalleHangfireJob') BEGIN INSERT INTO HabilitacionJob VALUES ('ProcessComprasDetalleHangfireJob', 0) END
IF NOT EXISTS (SELECT TOP 1 1 FROM HabilitacionJob WHERE Nombre = 'ProcessCapacidadProductivaHangfireJob') BEGIN INSERT INTO HabilitacionJob VALUES ('ProcessCapacidadProductivaHangfireJob', 0) END
IF NOT EXISTS (SELECT TOP 1 1 FROM HabilitacionJob WHERE Nombre = 'EliminarLogsAntiguosHangfireJob') BEGIN INSERT INTO HabilitacionJob VALUES ('EliminarLogsAntiguosHangfireJob', 0) END
IF NOT EXISTS (SELECT TOP 1 1 FROM HabilitacionJob WHERE Nombre = 'ActualizarScoringCuposDeProveedoresHangfireJob') BEGIN INSERT INTO HabilitacionJob VALUES ('ActualizarScoringCuposDeProveedoresHangfireJob', 0) END

--Clausula Carta Oferta
IF NOT EXISTS (select 1 from Clausula where Discriminator = 'ClausulaSesenta') BEGIN insert into Clausula(Discriminator, Orden, Estado) values ('ClausulaSesenta',60,1); END
IF NOT EXISTS (select 1 from Clausula where Discriminator = 'ClausulaSesentaYUno') BEGIN insert into Clausula(Discriminator, Orden, Estado) values ('ClausulaSesentaYUno',61,1); END

--Boleto Fisico
IF NOT EXISTS (select 1 from ClausulaBoletoFisico where Discriminator = 'ClausulaBoletoFisicoUno'          )BEGIN insert into ClausulaBoletoFisico(Discriminator, Orden, Estado, Clausula) values ('ClausulaBoletoFisicoUno'          ,1 ,1,'ClausulaUno'); END
IF NOT EXISTS (select 1 from ClausulaBoletoFisico where Discriminator = 'ClausulaBoletoFisicoDos'          )BEGIN insert into ClausulaBoletoFisico(Discriminator, Orden, Estado, Clausula) values ('ClausulaBoletoFisicoDos'          ,2 ,1,'ClausulaDos'); END
IF NOT EXISTS (select 1 from ClausulaBoletoFisico where Discriminator = 'ClausulaBoletoFisicoTres'         )BEGIN insert into ClausulaBoletoFisico(Discriminator, Orden, Estado, Clausula) values ('ClausulaBoletoFisicoTres'         ,3 ,1,'ClausulaTres'); END
IF NOT EXISTS (select 1 from ClausulaBoletoFisico where Discriminator = 'ClausulaBoletoFisicoCuatro'       )BEGIN insert into ClausulaBoletoFisico(Discriminator, Orden, Estado, Clausula) values ('ClausulaBoletoFisicoCuatro'       ,4 ,1,'ClausulaCuatro'); END
IF NOT EXISTS (select 1 from ClausulaBoletoFisico where Discriminator = 'ClausulaBoletoFisicoCinco'        )BEGIN insert into ClausulaBoletoFisico(Discriminator, Orden, Estado, Clausula) values ('ClausulaBoletoFisicoCinco'        ,5 ,1,'ClausulaCinco'); END
IF NOT EXISTS (select 1 from ClausulaBoletoFisico where Discriminator = 'ClausulaBoletoFisicoSeis'         )BEGIN insert into ClausulaBoletoFisico(Discriminator, Orden, Estado, Clausula) values ('ClausulaBoletoFisicoSeis'         ,6 ,1,'ClausulaSeis'); END
IF NOT EXISTS (select 1 from ClausulaBoletoFisico where Discriminator = 'ClausulaBoletoFisicoSiete'        )BEGIN insert into ClausulaBoletoFisico(Discriminator, Orden, Estado, Clausula) values ('ClausulaBoletoFisicoSiete'        ,7 ,1,'ClausulaSiete'); END
IF NOT EXISTS (select 1 from ClausulaBoletoFisico where Discriminator = 'ClausulaBoletoFisicoOcho'         )BEGIN insert into ClausulaBoletoFisico(Discriminator, Orden, Estado, Clausula) values ('ClausulaBoletoFisicoOcho'         ,8 ,1,'ClausulaOcho'); END
IF NOT EXISTS (select 1 from ClausulaBoletoFisico where Discriminator = 'ClausulaBoletoFisicoNueve'        )BEGIN insert into ClausulaBoletoFisico(Discriminator, Orden, Estado, Clausula) values ('ClausulaBoletoFisicoNueve'        ,9 ,1,'ClausulaNueve'); END
IF NOT EXISTS (select 1 from ClausulaBoletoFisico where Discriminator = 'ClausulaBoletoFisicoDiez'         )BEGIN insert into ClausulaBoletoFisico(Discriminator, Orden, Estado, Clausula) values ('ClausulaBoletoFisicoDiez'         ,10,1,'ClausulaDiez'); END
IF NOT EXISTS (select 1 from ClausulaBoletoFisico where Discriminator = 'ClausulaBoletoFisicoOnce'         )BEGIN insert into ClausulaBoletoFisico(Discriminator, Orden, Estado, Clausula) values ('ClausulaBoletoFisicoOnce'         ,11,1,'ClausulaOnce'); END
IF NOT EXISTS (select 1 from ClausulaBoletoFisico where Discriminator = 'ClausulaBoletoFisicoDoce'         )BEGIN insert into ClausulaBoletoFisico(Discriminator, Orden, Estado, Clausula) values ('ClausulaBoletoFisicoDoce'         ,12,1,'ClausulaDoce'); END
IF NOT EXISTS (select 1 from ClausulaBoletoFisico where Discriminator = 'ClausulaBoletoFisicoTrece'        )BEGIN insert into ClausulaBoletoFisico(Discriminator, Orden, Estado, Clausula) values ('ClausulaBoletoFisicoTrece'        ,13,1,'ClausulaTrece'); END
IF NOT EXISTS (select 1 from ClausulaBoletoFisico where Discriminator = 'ClausulaBoletoFisicoCatorce'      )BEGIN insert into ClausulaBoletoFisico(Discriminator, Orden, Estado, Clausula) values ('ClausulaBoletoFisicoCatorce'      ,14,1,'ClausulaCatorce'); END
IF NOT EXISTS (select 1 from ClausulaBoletoFisico where Discriminator = 'ClausulaBoletoFisicoQuince'       )BEGIN insert into ClausulaBoletoFisico(Discriminator, Orden, Estado, Clausula) values ('ClausulaBoletoFisicoQuince'       ,15,1,'ClausulaQuince'); END
IF NOT EXISTS (select 1 from ClausulaBoletoFisico where Discriminator = 'ClausulaBoletoFisicoDiesiseis'    )BEGIN insert into ClausulaBoletoFisico(Discriminator, Orden, Estado, Clausula) values ('ClausulaBoletoFisicoDiesiseis'    ,16,1,'ClausulaDiesiseis'); END
IF NOT EXISTS (select 1 from ClausulaBoletoFisico where Discriminator = 'ClausulaBoletoFisicoDiesisiete'   )BEGIN insert into ClausulaBoletoFisico(Discriminator, Orden, Estado, Clausula) values ('ClausulaBoletoFisicoDiesisiete'   ,17,1,'ClausulaDiesisiete'); END
IF NOT EXISTS (select 1 from ClausulaBoletoFisico where Discriminator = 'ClausulaBoletoFisicoDiesiocho'    )BEGIN insert into ClausulaBoletoFisico(Discriminator, Orden, Estado, Clausula) values ('ClausulaBoletoFisicoDiesiocho'    ,18,1,'ClausulaDiesiocho'); END
IF NOT EXISTS (select 1 from ClausulaBoletoFisico where Discriminator = 'ClausulaBoletoFisicoDiesiNueve'   )BEGIN insert into ClausulaBoletoFisico(Discriminator, Orden, Estado, Clausula) values ('ClausulaBoletoFisicoDiesiNueve'   ,19,1,'ClausulaDiesiNueve'); END
IF NOT EXISTS (select 1 from ClausulaBoletoFisico where Discriminator = 'ClausulaBoletoFisicoVeinte'       )BEGIN insert into ClausulaBoletoFisico(Discriminator, Orden, Estado, Clausula) values ('ClausulaBoletoFisicoVeinte'       ,20,1,'ClausulaVeinte'); END
IF NOT EXISTS (select 1 from ClausulaBoletoFisico where Discriminator = 'ClausulaBoletoFisicoVeinteYUno'   )BEGIN insert into ClausulaBoletoFisico(Discriminator, Orden, Estado, Clausula) values ('ClausulaBoletoFisicoVeinteYUno'   ,21,1,'ClausulaVeinteYUno'); END
IF NOT EXISTS (select 1 from ClausulaBoletoFisico where Discriminator = 'ClausulaBoletoFisicoVeinteYDos'   )BEGIN insert into ClausulaBoletoFisico(Discriminator, Orden, Estado, Clausula) values ('ClausulaBoletoFisicoVeinteYDos'   ,22,1,'ClausulaVeinteYDos'); END
IF NOT EXISTS (select 1 from ClausulaBoletoFisico where Discriminator = 'ClausulaBoletoFisicoVeinteYTres'  )BEGIN insert into ClausulaBoletoFisico(Discriminator, Orden, Estado, Clausula) values ('ClausulaBoletoFisicoVeinteYTres'  ,23,1,'ClausulaVeinteYTres'); END
IF NOT EXISTS (select 1 from ClausulaBoletoFisico where Discriminator = 'ClausulaBoletoFisicoVeinteYCuatro')BEGIN insert into ClausulaBoletoFisico(Discriminator, Orden, Estado, Clausula) values ('ClausulaBoletoFisicoVeinteYCuatro',24,1,'ClausulaVeinteYCuatro'); END

-- Boleto Carta Oferta
IF NOT EXISTS (select 1 from ClausulaCartaOferta where Discriminator = 'ClausulaCartaOfertaUno'       )BEGIN insert into ClausulaCartaOferta(Discriminator, Orden, Estado, Clausula) values ('ClausulaCartaOfertaUno',1,1,'ClausulaUno'); END
IF NOT EXISTS (select 1 from ClausulaCartaOferta where Discriminator = 'ClausulaCartaOfertaDos'       )BEGIN insert into ClausulaCartaOferta(Discriminator, Orden, Estado, Clausula) values ('ClausulaCartaOfertaDos',2,1,'ClausulaDos'); END
IF NOT EXISTS (select 1 from ClausulaCartaOferta where Discriminator = 'ClausulaCartaOfertaTres'      )BEGIN insert into ClausulaCartaOferta(Discriminator, Orden, Estado, Clausula) values ('ClausulaCartaOfertaTres',3,1,'ClausulaTres'); END
IF NOT EXISTS (select 1 from ClausulaCartaOferta where Discriminator = 'ClausulaCartaOfertaCuatro'    )BEGIN insert into ClausulaCartaOferta(Discriminator, Orden, Estado, Clausula) values ('ClausulaCartaOfertaCuatro',4,1,'ClausulaCuatro'); END
IF NOT EXISTS (select 1 from ClausulaCartaOferta where Discriminator = 'ClausulaCartaOfertaCinco'     )BEGIN insert into ClausulaCartaOferta(Discriminator, Orden, Estado, Clausula) values ('ClausulaCartaOfertaCinco',5,1,'ClausulaCinco'); END
IF NOT EXISTS (select 1 from ClausulaCartaOferta where Discriminator = 'ClausulaCartaOfertaSeis'      )BEGIN insert into ClausulaCartaOferta(Discriminator, Orden, Estado, Clausula) values ('ClausulaCartaOfertaSeis',6,1,'ClausulaSeis'); END
IF NOT EXISTS (select 1 from ClausulaCartaOferta where Discriminator = 'ClausulaCartaOfertaSiete'     )BEGIN insert into ClausulaCartaOferta(Discriminator, Orden, Estado, Clausula) values ('ClausulaCartaOfertaSiete',7,1,'ClausulaSiete'); END
IF NOT EXISTS (select 1 from ClausulaCartaOferta where Discriminator = 'ClausulaCartaOfertaOcho'      )BEGIN insert into ClausulaCartaOferta(Discriminator, Orden, Estado, Clausula) values ('ClausulaCartaOfertaOcho',8,1,'ClausulaOcho'); END
IF NOT EXISTS (select 1 from ClausulaCartaOferta where Discriminator = 'ClausulaCartaOfertaNueve'     )BEGIN insert into ClausulaCartaOferta(Discriminator, Orden, Estado, Clausula) values ('ClausulaCartaOfertaNueve',9,1,'ClausulaNueve'); END
IF NOT EXISTS (select 1 from ClausulaCartaOferta where Discriminator = 'ClausulaCartaOfertaDiez'      )BEGIN insert into ClausulaCartaOferta(Discriminator, Orden, Estado, Clausula) values ('ClausulaCartaOfertaDiez',10,1,'ClausulaDiez'); END
IF NOT EXISTS (select 1 from ClausulaCartaOferta where Discriminator = 'ClausulaCartaOfertaOnce'      )BEGIN insert into ClausulaCartaOferta(Discriminator, Orden, Estado, Clausula) values ('ClausulaCartaOfertaOnce',11,1,'ClausulaOnce'); END
IF NOT EXISTS (select 1 from ClausulaCartaOferta where Discriminator = 'ClausulaCartaOfertaDoce'      )BEGIN insert into ClausulaCartaOferta(Discriminator, Orden, Estado, Clausula) values ('ClausulaCartaOfertaDoce',12,1,'ClausulaDoce'); END
IF NOT EXISTS (select 1 from ClausulaCartaOferta where Discriminator = 'ClausulaCartaOfertaTrece'     )BEGIN insert into ClausulaCartaOferta(Discriminator, Orden, Estado, Clausula) values ('ClausulaCartaOfertaTrece',13,1,'ClausulaTrece'); END
IF NOT EXISTS (select 1 from ClausulaCartaOferta where Discriminator = 'ClausulaCartaOfertaCatorce'   )BEGIN insert into ClausulaCartaOferta(Discriminator, Orden, Estado, Clausula) values ('ClausulaCartaOfertaCatorce',14,1,'ClausulaCatorce'); END
IF NOT EXISTS (select 1 from ClausulaCartaOferta where Discriminator = 'ClausulaCartaOfertaQuince'    )BEGIN insert into ClausulaCartaOferta(Discriminator, Orden, Estado, Clausula) values ('ClausulaCartaOfertaQuince',15,1,'ClausulaQuince'); END
IF NOT EXISTS (select 1 from ClausulaCartaOferta where Discriminator = 'ClausulaCartaOfertaDiesiseis' )BEGIN insert into ClausulaCartaOferta(Discriminator, Orden, Estado, Clausula) values ('ClausulaCartaOfertaDiesiseis',16,1,'ClausulaDiesiseis'); END
IF NOT EXISTS (select 1 from ClausulaCartaOferta where Discriminator = 'ClausulaCartaOfertaDiesisiete')BEGIN insert into ClausulaCartaOferta(Discriminator, Orden, Estado, Clausula) values ('ClausulaCartaOfertaDiesisiete',17,1,'ClausulaDiesisiete'); END
IF NOT EXISTS (select 1 from ClausulaCartaOferta where Discriminator = 'ClausulaCartaOfertaDiesiocho' )BEGIN insert into ClausulaCartaOferta(Discriminator, Orden, Estado, Clausula) values ('ClausulaCartaOfertaDiesiocho',18,1,'ClausulaDiesiocho'); END

-- Boleto Confirma
IF NOT EXISTS (select 1 from ClausulaConfirma where Discriminator = 'ClausulaConfirmaUno'          ) BEGIN insert into ClausulaConfirma(Discriminator, Orden, Estado, Clausula) values ('ClausulaConfirmaUno',1,1,'ClausulaUno'); END
IF NOT EXISTS (select 1 from ClausulaConfirma where Discriminator = 'ClausulaConfirmaDos'          ) BEGIN insert into ClausulaConfirma(Discriminator, Orden, Estado, Clausula) values ('ClausulaConfirmaDos',2,1,'ClausulaDos'); END
IF NOT EXISTS (select 1 from ClausulaConfirma where Discriminator = 'ClausulaConfirmaTres'         ) BEGIN insert into ClausulaConfirma(Discriminator, Orden, Estado, Clausula) values ('ClausulaConfirmaTres',3,1,'ClausulaTres'); END
IF NOT EXISTS (select 1 from ClausulaConfirma where Discriminator = 'ClausulaConfirmaCuatro'       ) BEGIN insert into ClausulaConfirma(Discriminator, Orden, Estado, Clausula) values ('ClausulaConfirmaCuatro',4,1,'ClausulaCuatro'); END
IF NOT EXISTS (select 1 from ClausulaConfirma where Discriminator = 'ClausulaConfirmaCinco'        ) BEGIN insert into ClausulaConfirma(Discriminator, Orden, Estado, Clausula) values ('ClausulaConfirmaCinco',5,1,'ClausulaCinco'); END
IF NOT EXISTS (select 1 from ClausulaConfirma where Discriminator = 'ClausulaConfirmaSeis'         ) BEGIN insert into ClausulaConfirma(Discriminator, Orden, Estado, Clausula) values ('ClausulaConfirmaSeis',6,1,'ClausulaSeis'); END
IF NOT EXISTS (select 1 from ClausulaConfirma where Discriminator = 'ClausulaConfirmaSiete'        ) BEGIN insert into ClausulaConfirma(Discriminator, Orden, Estado, Clausula) values ('ClausulaConfirmaSiete',7,1,'ClausulaSiete'); END
IF NOT EXISTS (select 1 from ClausulaConfirma where Discriminator = 'ClausulaConfirmaOcho'         ) BEGIN insert into ClausulaConfirma(Discriminator, Orden, Estado, Clausula) values ('ClausulaConfirmaOcho',8,1,'ClausulaOcho'); END
IF NOT EXISTS (select 1 from ClausulaConfirma where Discriminator = 'ClausulaConfirmaNueve'        ) BEGIN insert into ClausulaConfirma(Discriminator, Orden, Estado, Clausula) values ('ClausulaConfirmaNueve',9,1,'ClausulaNueve'); END
IF NOT EXISTS (select 1 from ClausulaConfirma where Discriminator = 'ClausulaConfirmaDiez'         ) BEGIN insert into ClausulaConfirma(Discriminator, Orden, Estado, Clausula) values ('ClausulaConfirmaDiez',10,1,'ClausulaDiez'); END
IF NOT EXISTS (select 1 from ClausulaConfirma where Discriminator = 'ClausulaConfirmaOnce'         ) BEGIN insert into ClausulaConfirma(Discriminator, Orden, Estado, Clausula) values ('ClausulaConfirmaOnce',11,1,'ClausulaOnce'); END
IF NOT EXISTS (select 1 from ClausulaConfirma where Discriminator = 'ClausulaConfirmaDoce'         ) BEGIN insert into ClausulaConfirma(Discriminator, Orden, Estado, Clausula) values ('ClausulaConfirmaDoce',12,1,'ClausulaDoce'); END
IF NOT EXISTS (select 1 from ClausulaConfirma where Discriminator = 'ClausulaConfirmaTrece'        ) BEGIN insert into ClausulaConfirma(Discriminator, Orden, Estado, Clausula) values ('ClausulaConfirmaTrece',13,1,'ClausulaTrece'); END
IF NOT EXISTS (select 1 from ClausulaConfirma where Discriminator = 'ClausulaConfirmaCatorce'      ) BEGIN insert into ClausulaConfirma(Discriminator, Orden, Estado, Clausula) values ('ClausulaConfirmaCatorce',14,1,'ClausulaCatorce'); END
IF NOT EXISTS (select 1 from ClausulaConfirma where Discriminator = 'ClausulaConfirmaQuince'       ) BEGIN insert into ClausulaConfirma(Discriminator, Orden, Estado, Clausula) values ('ClausulaConfirmaQuince',15,1,'ClausulaQuince'); END
IF NOT EXISTS (select 1 from ClausulaConfirma where Discriminator = 'ClausulaConfirmaDiesiseis'    ) BEGIN insert into ClausulaConfirma(Discriminator, Orden, Estado, Clausula) values ('ClausulaConfirmaDiesiseis',16,1,'ClausulaDiesiseis'); END
IF NOT EXISTS (select 1 from ClausulaConfirma where Discriminator = 'ClausulaConfirmaDiesisiete'   ) BEGIN insert into ClausulaConfirma(Discriminator, Orden, Estado, Clausula) values ('ClausulaConfirmaDiesisiete',17,1,'ClausulaDiesisiete'); END
IF NOT EXISTS (select 1 from ClausulaConfirma where Discriminator = 'ClausulaConfirmaDiesiocho'    ) BEGIN insert into ClausulaConfirma(Discriminator, Orden, Estado, Clausula) values ('ClausulaConfirmaDiesiocho',18,1,'ClausulaDiesiocho'); END
IF NOT EXISTS (select 1 from ClausulaConfirma where Discriminator = 'ClausulaConfirmaDiesiNueve'   ) BEGIN insert into ClausulaConfirma(Discriminator, Orden, Estado, Clausula) values ('ClausulaConfirmaDiesiNueve',19,1,'ClausulaDiesiNueve'); END
IF NOT EXISTS (select 1 from ClausulaConfirma where Discriminator = 'ClausulaConfirmaVeinte'       ) BEGIN insert into ClausulaConfirma(Discriminator, Orden, Estado, Clausula) values ('ClausulaConfirmaVeinte',20,1,'ClausulaVeinte'); END
IF NOT EXISTS (select 1 from ClausulaConfirma where Discriminator = 'ClausulaConfirmaVeinteYUno'   ) BEGIN insert into ClausulaConfirma(Discriminator, Orden, Estado, Clausula) values ('ClausulaConfirmaVeinteYUno',21,1,'ClausulaVeinteYUno'); END
IF NOT EXISTS (select 1 from ClausulaConfirma where Discriminator = 'ClausulaConfirmaVeinteYDos'   ) BEGIN insert into ClausulaConfirma(Discriminator, Orden, Estado, Clausula) values ('ClausulaConfirmaVeinteYDos',22,1,'ClausulaVeinteYDos'); END
IF NOT EXISTS (select 1 from ClausulaConfirma where Discriminator = 'ClausulaConfirmaVeinteYTres'  ) BEGIN insert into ClausulaConfirma(Discriminator, Orden, Estado, Clausula) values ('ClausulaConfirmaVeinteYTres',23,1,'ClausulaVeinteYTres'); END
IF NOT EXISTS (select 1 from ClausulaConfirma where Discriminator = 'ClausulaConfirmaVeinteYCuatro') BEGIN insert into ClausulaConfirma(Discriminator, Orden, Estado, Clausula) values ('ClausulaConfirmaVeinteYCuatro',24,1,'ClausulaVeinteYCuatro'); END

-- Boleto Clausula Generico
IF NOT EXISTS (select 1 from ClausulaGenericos where Discriminator = 'ClausulaGenericosUno'            )BEGIN insert into ClausulaGenericos(Discriminator, Orden, Estado, Clausula) values ('ClausulaGenericosUno',1,1,'ClausulaUno'); END
IF NOT EXISTS (select 1 from ClausulaGenericos where Discriminator = 'ClausulaGenericosDos'            )BEGIN insert into ClausulaGenericos(Discriminator, Orden, Estado, Clausula) values ('ClausulaGenericosDos',2,1,'ClausulaDos'); END
IF NOT EXISTS (select 1 from ClausulaGenericos where Discriminator = 'ClausulaGenericosTres'           )BEGIN insert into ClausulaGenericos(Discriminator, Orden, Estado, Clausula) values ('ClausulaGenericosTres',3,1,'ClausulaTres'); END
IF NOT EXISTS (select 1 from ClausulaGenericos where Discriminator = 'ClausulaGenericosCuatro'         )BEGIN insert into ClausulaGenericos(Discriminator, Orden, Estado, Clausula) values ('ClausulaGenericosCuatro',4,1,'ClausulaCuatro'); END
IF NOT EXISTS (select 1 from ClausulaGenericos where Discriminator = 'ClausulaGenericosCinco'          )BEGIN insert into ClausulaGenericos(Discriminator, Orden, Estado, Clausula) values ('ClausulaGenericosCinco',5,1,'ClausulaCinco'); END
IF NOT EXISTS (select 1 from ClausulaGenericos where Discriminator = 'ClausulaGenericosSeis'           )BEGIN insert into ClausulaGenericos(Discriminator, Orden, Estado, Clausula) values ('ClausulaGenericosSeis',6,1,'ClausulaSeis'); END
IF NOT EXISTS (select 1 from ClausulaGenericos where Discriminator = 'ClausulaGenericosSiete'          )BEGIN insert into ClausulaGenericos(Discriminator, Orden, Estado, Clausula) values ('ClausulaGenericosSiete',7,1,'ClausulaSiete'); END
IF NOT EXISTS (select 1 from ClausulaGenericos where Discriminator = 'ClausulaGenericosOcho'           )BEGIN insert into ClausulaGenericos(Discriminator, Orden, Estado, Clausula) values ('ClausulaGenericosOcho',8,1,'ClausulaOcho'); END
IF NOT EXISTS (select 1 from ClausulaGenericos where Discriminator = 'ClausulaGenericosNueve'          )BEGIN insert into ClausulaGenericos(Discriminator, Orden, Estado, Clausula) values ('ClausulaGenericosNueve',9,1,'ClausulaNueve'); END
IF NOT EXISTS (select 1 from ClausulaGenericos where Discriminator = 'ClausulaGenericosDiez'           )BEGIN insert into ClausulaGenericos(Discriminator, Orden, Estado, Clausula) values ('ClausulaGenericosDiez',10,1,'ClausulaDiez'); END
IF NOT EXISTS (select 1 from ClausulaGenericos where Discriminator = 'ClausulaGenericosOnce'           )BEGIN insert into ClausulaGenericos(Discriminator, Orden, Estado, Clausula) values ('ClausulaGenericosOnce',11,1,'ClausulaOnce'); END
IF NOT EXISTS (select 1 from ClausulaGenericos where Discriminator = 'ClausulaGenericosDoce'           )BEGIN insert into ClausulaGenericos(Discriminator, Orden, Estado, Clausula) values ('ClausulaGenericosDoce',12,1,'ClausulaDoce'); END
IF NOT EXISTS (select 1 from ClausulaGenericos where Discriminator = 'ClausulaGenericosTrece'          )BEGIN insert into ClausulaGenericos(Discriminator, Orden, Estado, Clausula) values ('ClausulaGenericosTrece',13,1,'ClausulaTrece'); END
IF NOT EXISTS (select 1 from ClausulaGenericos where Discriminator = 'ClausulaGenericosCatorce'        )BEGIN insert into ClausulaGenericos(Discriminator, Orden, Estado, Clausula) values ('ClausulaGenericosCatorce',14,1,'ClausulaCatorce'); END
IF NOT EXISTS (select 1 from ClausulaGenericos where Discriminator = 'ClausulaGenericosQuince'         )BEGIN insert into ClausulaGenericos(Discriminator, Orden, Estado, Clausula) values ('ClausulaGenericosQuince',15,1,'ClausulaQuince'); END
IF NOT EXISTS (select 1 from ClausulaGenericos where Discriminator = 'ClausulaGenericosDiesiseis'      )BEGIN insert into ClausulaGenericos(Discriminator, Orden, Estado, Clausula) values ('ClausulaGenericosDiesiseis',16,1,'ClausulaDiesiseis'); END
IF NOT EXISTS (select 1 from ClausulaGenericos where Discriminator = 'ClausulaGenericosDiesisiete'     )BEGIN insert into ClausulaGenericos(Discriminator, Orden, Estado, Clausula) values ('ClausulaGenericosDiesisiete',17,1,'ClausulaDiesisiete'); END
IF NOT EXISTS (select 1 from ClausulaGenericos where Discriminator = 'ClausulaGenericosDiesiocho'      )BEGIN insert into ClausulaGenericos(Discriminator, Orden, Estado, Clausula) values ('ClausulaGenericosDiesiocho',18,1,'ClausulaDiesiocho'); END
IF NOT EXISTS (select 1 from ClausulaGenericos where Discriminator = 'ClausulaGenericosDiesiNueve'     )BEGIN insert into ClausulaGenericos(Discriminator, Orden, Estado, Clausula) values ('ClausulaGenericosDiesiNueve',19,1,'ClausulaDiesiNueve'); END
IF NOT EXISTS (select 1 from ClausulaGenericos where Discriminator = 'ClausulaGenericosVeinte'         )BEGIN insert into ClausulaGenericos(Discriminator, Orden, Estado, Clausula) values ('ClausulaGenericosVeinte',20,1,'ClausulaVeinte'); END
IF NOT EXISTS (select 1 from ClausulaGenericos where Discriminator = 'ClausulaGenericosVeinteYUno'     )BEGIN insert into ClausulaGenericos(Discriminator, Orden, Estado, Clausula) values ('ClausulaGenericosVeinteYUno',21,1,'ClausulaVeinteYUno'); END
IF NOT EXISTS (select 1 from ClausulaGenericos where Discriminator = 'ClausulaGenericosVeinteYDos'     )BEGIN insert into ClausulaGenericos(Discriminator, Orden, Estado, Clausula) values ('ClausulaGenericosVeinteYDos',22,1,'ClausulaVeinteYDos'); END
IF NOT EXISTS (select 1 from ClausulaGenericos where Discriminator = 'ClausulaGenericosVeinteYTres'    )BEGIN insert into ClausulaGenericos(Discriminator, Orden, Estado, Clausula) values ('ClausulaGenericosVeinteYTres',23,1,'ClausulaVeinteYTres'); END
IF NOT EXISTS (select 1 from ClausulaGenericos where Discriminator = 'ClausulaGenericosVeinteYCuatro'  )BEGIN insert into ClausulaGenericos(Discriminator, Orden, Estado, Clausula) values ('ClausulaGenericosVeinteYCuatro',24,1,'ClausulaVeinteYCuatro'); END
IF NOT EXISTS (select 1 from ClausulaGenericos where Discriminator = 'ClausulaGenericosVeinteYCinco'   )BEGIN insert into ClausulaGenericos(Discriminator, Orden, Estado, Clausula) values ('ClausulaGenericosVeinteYCinco',25,1,'ClausulaVeinteYCinco'); END
IF NOT EXISTS (select 1 from ClausulaGenericos where Discriminator = 'ClausulaGenericosVeinteYSeis'    )BEGIN insert into ClausulaGenericos(Discriminator, Orden, Estado, Clausula) values ('ClausulaGenericosVeinteYSeis',26,1,'ClausulaVeinteYSeis'); END
IF NOT EXISTS (select 1 from ClausulaGenericos where Discriminator = 'ClausulaGenericosVeinteYSiete'   )BEGIN insert into ClausulaGenericos(Discriminator, Orden, Estado, Clausula) values ('ClausulaGenericosVeinteYSiete',27,1,'ClausulaVeinteYSiete'); END
IF NOT EXISTS (select 1 from ClausulaGenericos where Discriminator = 'ClausulaGenericosVeinteYOcho'    )BEGIN insert into ClausulaGenericos(Discriminator, Orden, Estado, Clausula) values ('ClausulaGenericosVeinteYOcho',28,1,'ClausulaVeinteYOcho'); END
IF NOT EXISTS (select 1 from ClausulaGenericos where Discriminator = 'ClausulaGenericosVeinteYNueve'   )BEGIN insert into ClausulaGenericos(Discriminator, Orden, Estado, Clausula) values ('ClausulaGenericosVeinteYNueve',29,1,'ClausulaVeinteYNueve'); END
IF NOT EXISTS (select 1 from ClausulaGenericos where Discriminator = 'ClausulaGenericosTreinta'        )BEGIN insert into ClausulaGenericos(Discriminator, Orden, Estado, Clausula) values ('ClausulaGenericosTreinta',30,1,'ClausulaTreinta'); END
IF NOT EXISTS (select 1 from ClausulaGenericos where Discriminator = 'ClausulaGenericosTreintaYUno'    )BEGIN insert into ClausulaGenericos(Discriminator, Orden, Estado, Clausula) values ('ClausulaGenericosTreintaYUno',31,1,'ClausulaTreintaYUno'); END
IF NOT EXISTS (select 1 from ClausulaGenericos where Discriminator = 'ClausulaGenericosTreintaYDos'    )BEGIN insert into ClausulaGenericos(Discriminator, Orden, Estado, Clausula) values ('ClausulaGenericosTreintaYDos',32,1,'ClausulaTreintaYDos'); END
IF NOT EXISTS (select 1 from ClausulaGenericos where Discriminator = 'ClausulaGenericosTreintaYTres'   )BEGIN insert into ClausulaGenericos(Discriminator, Orden, Estado, Clausula) values ('ClausulaGenericosTreintaYTres',33,1,'ClausulaTreintaYTres'); END
IF NOT EXISTS (select 1 from ClausulaGenericos where Discriminator = 'ClausulaGenericosTreintaYCuatro' )BEGIN insert into ClausulaGenericos(Discriminator, Orden, Estado, Clausula) values ('ClausulaGenericosTreintaYCuatro',34,1,'ClausulaTreintaYCuatro'); END
IF NOT EXISTS (select 1 from ClausulaGenericos where Discriminator = 'ClausulaGenericosTreintaYCinco'  )BEGIN insert into ClausulaGenericos(Discriminator, Orden, Estado, Clausula) values ('ClausulaGenericosTreintaYCinco',35,1,'ClausulaTreintaYCinco'); END
IF NOT EXISTS (select 1 from ClausulaGenericos where Discriminator = 'ClausulaGenericosTreintaYSeis'   )BEGIN insert into ClausulaGenericos(Discriminator, Orden, Estado, Clausula) values ('ClausulaGenericosTreintaYSeis',36,1,'ClausulaTreintaYSeis'); END
IF NOT EXISTS (select 1 from ClausulaGenericos where Discriminator = 'ClausulaGenericosTreintaYSiete'  )BEGIN insert into ClausulaGenericos(Discriminator, Orden, Estado, Clausula) values ('ClausulaGenericosTreintaYSiete',37,1,'ClausulaTreintaYSiete'); END
IF NOT EXISTS (select 1 from ClausulaGenericos where Discriminator = 'ClausulaGenericosTreintaYOcho'   )BEGIN insert into ClausulaGenericos(Discriminator, Orden, Estado, Clausula) values ('ClausulaGenericosTreintaYOcho',38,1,'ClausulaTreintaYOcho'); END
IF NOT EXISTS (select 1 from ClausulaGenericos where Discriminator = 'ClausulaGenericosTreintaYNueve'  )BEGIN insert into ClausulaGenericos(Discriminator, Orden, Estado, Clausula) values ('ClausulaGenericosTreintaYNueve',39,1,'ClausulaTreintaYNueve'); END
IF NOT EXISTS (select 1 from ClausulaGenericos where Discriminator = 'ClausulaGenericosCuarenta'       )BEGIN insert into ClausulaGenericos(Discriminator, Orden, Estado, Clausula) values ('ClausulaGenericosCuarenta',40,1,'ClausulaCuarenta'); END
IF NOT EXISTS (select 1 from ClausulaGenericos where Discriminator = 'ClausulaGenericosCuarentaYUno'   )BEGIN insert into ClausulaGenericos(Discriminator, Orden, Estado, Clausula) values ('ClausulaGenericosCuarentaYUno',41,1,'ClausulaCuarentaYUno'); END
IF NOT EXISTS (select 1 from ClausulaGenericos where Discriminator = 'ClausulaGenericosCuarentaYDos'   )BEGIN insert into ClausulaGenericos(Discriminator, Orden, Estado, Clausula) values ('ClausulaGenericosCuarentaYDos',42,1,'ClausulaCuarentaYDos'); END
IF NOT EXISTS (select 1 from ClausulaGenericos where Discriminator = 'ClausulaGenericosCuarentaYTres'  )BEGIN insert into ClausulaGenericos(Discriminator, Orden, Estado, Clausula) values ('ClausulaGenericosCuarentaYTres',43,1,'ClausulaCuarentaYTres'); END
IF NOT EXISTS (select 1 from ClausulaGenericos where Discriminator = 'ClausulaGenericosCuarentaYCuatro')BEGIN insert into ClausulaGenericos(Discriminator, Orden, Estado, Clausula) values ('ClausulaGenericosCuarentaYCuatro',44,1,'ClausulaCuarentaYCuatro'); END
IF NOT EXISTS (select 1 from ClausulaGenericos where Discriminator = 'ClausulaGenericosCuarentaYCinco' )BEGIN insert into ClausulaGenericos(Discriminator, Orden, Estado, Clausula) values ('ClausulaGenericosCuarentaYCinco',45,1,'ClausulaCuarentaYCinco'); END
IF NOT EXISTS (select 1 from ClausulaGenericos where Discriminator = 'ClausulaGenericosCuarentaYSeis'  )BEGIN insert into ClausulaGenericos(Discriminator, Orden, Estado, Clausula) values ('ClausulaGenericosCuarentaYSeis',46,1,'ClausulaCuarentaYSeis'); END
IF NOT EXISTS (select 1 from ClausulaGenericos where Discriminator = 'ClausulaGenericosCuarentaYSiete' )BEGIN insert into ClausulaGenericos(Discriminator, Orden, Estado, Clausula) values ('ClausulaGenericosCuarentaYSiete',47,1,'ClausulaCuarentaYSiete'); END
IF NOT EXISTS (select 1 from ClausulaGenericos where Discriminator = 'ClausulaGenericosCuarentaYOcho'  )BEGIN insert into ClausulaGenericos(Discriminator, Orden, Estado, Clausula) values ('ClausulaGenericosCuarentaYOcho',48,1,'ClausulaCuarentaYOcho'); END
IF NOT EXISTS (select 1 from ClausulaGenericos where Discriminator = 'ClausulaGenericosCuarentaYNueve' )BEGIN insert into ClausulaGenericos(Discriminator, Orden, Estado, Clausula) values ('ClausulaGenericosCuarentaYNueve',49,1,'ClausulaCuarentaYNueve'); END


-- Correccion de descripciones de Destino
IF EXISTS(SELECT 1 FROM Centro WHERE Descripcion like 'Rosario Norte/Sur, opción comprador%') 
   BEGIN
    DECLARE @IdRosario int
	SELECT TOP 1 @IdRosario = Id FROM Centro WHERE Descripcion like 'Rosario Norte/Sur, opción comprador'
	UPDATE Centro SET Descripcion = 'Rosario Norte/Sur' 
	 WHERE Id = @IdRosario;
   END

IF EXISTS(SELECT 1 FROM Centro WHERE Descripcion like 'Rio del Valle (Planta Soto)%') 
   BEGIN
    DECLARE @IdRioValle int
	SELECT TOP 1 @IdRioValle = Id FROM Centro WHERE Descripcion like 'Rio del Valle (Planta Soto)'
	UPDATE Centro SET Descripcion = 'Rio del Valle' 
	 WHERE Id = @IdRioValle;
   END


-- ConfirmaAltaEstado
IF NOT EXISTS (SELECT 1 FROM ConfirmaAltaEstado WHERE Descripcion = 'Correcta')
    INSERT INTO ConfirmaAltaEstado (Descripcion, CodigoConfirmaAltaEstado)
    VALUES ('Correcta', 1);

IF NOT EXISTS (SELECT 1 FROM ConfirmaAltaEstado WHERE Descripcion = 'Permisos Insuficientes')
    INSERT INTO ConfirmaAltaEstado (Descripcion, CodigoConfirmaAltaEstado)
    VALUES ('Permisos Insuficientes', 2);

IF NOT EXISTS (SELECT 1 FROM ConfirmaAltaEstado WHERE Descripcion = 'Error')
    INSERT INTO ConfirmaAltaEstado (Descripcion, CodigoConfirmaAltaEstado)
    VALUES ('Error', 3);


-- ConfirmaAltaEstadoLote
IF NOT EXISTS (SELECT 1 FROM ConfirmaAltaEstadoLote WHERE Descripcion = 'Recepción con éxito')
    INSERT INTO ConfirmaAltaEstadoLote (Descripcion, CodigoConfirmaAltaEstadoLote)
    VALUES ('Recepción con éxito', 1);

IF NOT EXISTS (SELECT 1 FROM ConfirmaAltaEstadoLote WHERE Descripcion = 'Recepción con falla')
    INSERT INTO ConfirmaAltaEstadoLote (Descripcion, CodigoConfirmaAltaEstadoLote)
    VALUES ('Recepción con falla', 2);

IF NOT EXISTS (SELECT 1 FROM ConfirmaAltaEstadoLote WHERE Descripcion = 'En Proceso')
    INSERT INTO ConfirmaAltaEstadoLote (Descripcion, CodigoConfirmaAltaEstadoLote)
    VALUES ('En Proceso', 3);

IF NOT EXISTS (SELECT 1 FROM ConfirmaAltaEstadoLote WHERE Descripcion = 'Procesado')
    INSERT INTO ConfirmaAltaEstadoLote (Descripcion, CodigoConfirmaAltaEstadoLote)
    VALUES ('Procesado', 4);


-- ConfirmaAltaEstadoDocumento
IF NOT EXISTS (SELECT 1 FROM ConfirmaAltaEstadoDocumento WHERE Descripcion = 'Recepción con éxito')
    INSERT INTO ConfirmaAltaEstadoDocumento (Descripcion, CodigoConfirmaAltaEstadoDocumento)
    VALUES ('Recepción con éxito', 1);

IF NOT EXISTS (SELECT 1 FROM ConfirmaAltaEstadoDocumento WHERE Descripcion = 'Recepción con falla')
    INSERT INTO ConfirmaAltaEstadoDocumento (Descripcion, CodigoConfirmaAltaEstadoDocumento)
    VALUES ('Recepción con falla', 2);

IF NOT EXISTS (SELECT 1 FROM ConfirmaAltaEstadoDocumento WHERE Descripcion = 'En Proceso')
    INSERT INTO ConfirmaAltaEstadoDocumento (Descripcion, CodigoConfirmaAltaEstadoDocumento)
    VALUES ('En Proceso', 3);

IF NOT EXISTS (SELECT 1 FROM ConfirmaAltaEstadoDocumento WHERE Descripcion = 'Proceso con éxito')
    INSERT INTO ConfirmaAltaEstadoDocumento (Descripcion, CodigoConfirmaAltaEstadoDocumento)
    VALUES ('Proceso con éxito', 4);

IF NOT EXISTS (SELECT 1 FROM ConfirmaAltaEstadoDocumento WHERE Descripcion = 'Proceso con falla')
    INSERT INTO ConfirmaAltaEstadoDocumento (Descripcion, CodigoConfirmaAltaEstadoDocumento)
    VALUES ('Proceso con falla', 5);

-- CENTROS nuevos
IF NOT EXISTS (select 1 from Centro where Descripcion = 'Santa Clara') BEGIN insert into Centro (Descripcion,CodigoSap,Acopio,ValidaRedespacho,LocalidadId,CodigoPostal,Direccion,Comision,CargaNegocios,CargaCupos,NoPropio,CUIT,RazonSocial,CentroPropio) VALUES ('Santa Clara','9999','0','0',12699,2000,'Uriburu 3480','0','0','1','1','30500858628','MOLINOS RIO DE LA PLATA','0'); END

--Estado Confirma
if not exists(select 1 from EstadoConfirma where Codigo = 1)
begin
 insert into EstadoConfirma(Codigo,Descripcion) values (1, 'Contrato pendiente de Control')
end

if not exists(select 1 from EstadoConfirma where Codigo = 2)
begin
 insert into EstadoConfirma(Codigo,Descripcion) values (2, 'Controlado')
end

if not exists(select 1 from EstadoConfirma where Codigo = 3)
begin
 insert into EstadoConfirma(Codigo,Descripcion) values (3, 'En Firma')
end

if not exists(select 1 from EstadoConfirma where Codigo = 4)
begin
 insert into EstadoConfirma(Codigo,Descripcion) values (4, 'Pendiente de Registración')
end

if not exists(select 1 from EstadoConfirma where Codigo = 5)
begin
 insert into EstadoConfirma(Codigo,Descripcion) values (5, 'Registrado')
end

if not exists(select 1 from EstadoConfirma where Codigo = 7)
begin
 insert into EstadoConfirma(Codigo,Descripcion) values (7, 'Anulado')
end

if not exists(select 1 from EstadoConfirma where Codigo = 99)
begin
 insert into EstadoConfirma(Codigo,Descripcion) values (99, 'Anulado post-registración')
end

if not exists(select 1 from EstadoConfirma where Codigo = 9)
begin
 insert into EstadoConfirma(Codigo,Descripcion) values (9, 'Excluído porTiempo Excedido')
end

--Estado ControlDeBoletos
if not exists(select 1 from ControlDeBoletosEstado where Descripcion = 'Pendiente')
begin
 insert into ControlDeBoletosEstado(Descripcion) values ('Pendiente')
end

if not exists(select 1 from ControlDeBoletosEstado where Descripcion = 'En Proceso')
begin
 insert into ControlDeBoletosEstado(Descripcion) values ('En Proceso')
end

if not exists(select 1 from ControlDeBoletosEstado where Descripcion = 'En Oblea')
begin
 insert into ControlDeBoletosEstado(Descripcion) values ('En Oblea')
end

if not exists(select 1 from ControlDeBoletosEstado where Descripcion = 'En Certificacion')
begin
 insert into ControlDeBoletosEstado(Descripcion) values ('En Certificacion')
end

if not exists(select 1 from ControlDeBoletosEstado where Descripcion = 'Enviado Afip / Arca')
begin
 insert into ControlDeBoletosEstado(Descripcion) values ('Enviado Afip / Arca')
end

if not exists(select 1 from ControlDeBoletosEstado where Descripcion = 'Finalizado')
begin
 insert into ControlDeBoletosEstado(Descripcion) values ('Finalizado')
end

if not exists(select 1 from ControlDeBoletosEstado where Descripcion = 'Anulado')
begin
 insert into ControlDeBoletosEstado(Descripcion) values ('Anulado')
end

-- Insert TipoOblea
IF NOT EXISTS (SELECT 1 FROM TipoOblea WHERE Descripcion = 'Certificado AFIP')
BEGIN
    INSERT INTO TipoOblea (Codigo, Descripcion) 
    VALUES ('A', 'Certificado AFIP')
END

IF NOT EXISTS (SELECT 1 FROM TipoOblea WHERE Descripcion = 'Oblea Factura Plan Canje')
BEGIN
    INSERT INTO TipoOblea (Codigo, Descripcion) 
    VALUES ('F', 'Oblea Factura Plan Canje')
END

IF NOT EXISTS (SELECT 1 FROM TipoOblea WHERE Descripcion = 'Oblea Bolsa')
BEGIN
    INSERT INTO TipoOblea (Codigo, Descripcion) 
    VALUES ('O', 'Oblea Bolsa')
END

IF NOT EXISTS (SELECT 1 FROM TipoOblea WHERE Descripcion = 'Oblea Provisoria')
BEGIN
    INSERT INTO TipoOblea (Codigo, Descripcion) 
    VALUES ('P', 'Oblea Provisoria')
END