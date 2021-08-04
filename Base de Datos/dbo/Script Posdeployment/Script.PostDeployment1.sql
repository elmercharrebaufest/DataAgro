
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

