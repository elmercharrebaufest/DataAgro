using Autofac.Extras.NLog;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Repository;
using Molinos.DataAgro.Interfaces;
using Molinos.DataAgro.Entities.Common.Enums;

namespace Molinos.DataAgro.Business.Procesamiento
{
    public class ProcesadorClausulaCuarentaYDos : ProcesadorClausula<ClausulaCuarentaYDos>
    {
        public ProcesadorClausulaCuarentaYDos(IRepositorio repositorio, ILogger log, IConsultarEstadoBoletoAgent estadoBoleto)
           : base(repositorio, log, estadoBoleto)
        {

        }

        public override ResultadoClausula DevolverClausulas(ClausulaCuarentaYDos clausula)
        {
            var res = new ResultadoClausula();
            if (clausula.Basico.MaterialId == (int)EnumMateriales.SOJA && (clausula.Basico.EUDR || clausula.Basico.EPA))
            {
                if (clausula.Basico.CorredorId > 0)
                {
                    res.Texto = "Dado que el comprador adquiere por el presente el poroto de soja vendido por el vendedor con destino de exportación (destino que incluye a cualquier subproducto resultante), y a que dicho destino puede ser cualquier país miembro de la Unión Europea o a que los potenciales adquirentes de la mercadería o de sus subproductos pueden tener su domicilio allí, el vendedor declara en este acto conocer y aceptar las disposiciones del Reglamento 1115/2023 del Parlamento Europeo y del Consejo (el \"Reglamento\"), relativo a la comercialización en el mercado de la Unión Europea y a la exportación desde la Unión Europea de determinadas materias primas y productos asociados a la deforestación y la degradación forestal, " +
                    "y que tales disposiciones alcanzan expresamente a la mercadería objeto de este contrato y a sus subproductos. Especialmente, el vendedor declara con la debida diligencia exigida por el Reglamento, conforme a las condiciones establecidas en los artículos 3, 2 inciso 40 y concordantes de dicho Reglamento, que el poroto de soja objeto del presente contrato proviene de lotes bajo su explotación o explotación de terceros que ya se cultivaban antes del 31 de diciembre de 2020, y que desde ese momento y hasta la fecha de celebración de este contrato, en ninguno de esos lotes o parte de dichos lotes se han desarrollado o reimplantado bosques nativos, ni se han desarrollado pastizales naturales, encontrándose libres de deforestación en los términos del Reglamento. " +
                    "Del mismo modo, el vendedor declara con la debida diligencia exigida por el Reglamento, que la explotación de tales lotes o partes de lotes que han dado origen a la mercadería objeto de este contrato  no ha incumplido desde el 31 de diciembre de 2020 al momento de celebración de este contrato con normativa vigente en la República Argentina relativa a (a) derechos de uso de la tierra; (b) protección del medio ambiente; (c) normas relacionadas con los bosques, incluida la gestión forestal y la conservación de la biodiversidad; (d) derechos de terceros; (e) derechos laborales; (f) derechos humanos protegidos por el derecho internacional; (g) el principio de consentimiento libre, previo e informado (CLPI), incluido lo establecido en la Declaración de las Naciones Unidas sobre los Derechos de los Pueblos Indígenas; ni (h) regulaciones tributarias, anticorrupción, comerciales y aduaneras. " +
                    "En caso de que alguna de las declaraciones contenidas en la presente cláusula no sea verdadera, o que por cualquier otro motivo no imputable al comprador el poroto de soja entregado no fuese apto para obtener la certificación relativa a que se encuentra en cumplimiento del Reglamento (otorgada por Visec o quien en cada momento certifique dichas circunstancias), las partes acuerdan que dicho evento se considerará un incumplimiento a una obligación principal por parte del vendedor, y el comprador tendrá el derecho de modificar las condiciones de entrega acordadas, rechazar la mercadería o aplicar cualquier otra medida o recurso que pudiera corresponder.";
                }
                else
                {
                    res.Texto = "Dado que Molinos Agro S.A. adquiere por el presente el poroto de soja vendido por el vendedor con destino de exportación (destino que incluye a cualquier subproducto resultante), y a que dicho destino puede ser cualquier país miembro de la Unión Europea o a que los potenciales adquirentes de la mercadería o de sus subproductos pueden tener su domicilio allí, el vendedor declara en este acto conocer y aceptar las disposiciones del Reglamento 1115/2023 del Parlamento Europeo y del Consejo (el \"Reglamento\"), relativo a la comercialización en el mercado de la Unión Europea y a la exportación desde la Unión Europea de determinadas materias primas y productos asociados a la deforestación y la degradación forestal, " +
                        "y que tales disposiciones alcanzan expresamente a la mercadería objeto de este contrato y a sus subproductos. Especialmente, el vendedor declara con la debida diligencia exigida por el Reglamento, conforme a las condiciones establecidas en los artículos 3, 2 inciso 40 y concordantes de dicho Reglamento, que el poroto de soja objeto del presente contrato proviene de lotes bajo su explotación o explotación de terceros que ya se cultivaban antes del 31 de diciembre de 2020, y que desde ese momento y hasta la fecha de celebración de este contrato, en ninguno de esos lotes o parte de dichos lotes se han desarrollado o reimplantado bosques nativos, ni se han desarrollado pastizales naturales, encontrándose libres de deforestación en los términos del Reglamento. " +
                        "Del mismo modo, el vendedor declara con la debida diligencia exigida por el Reglamento, que la explotación de tales lotes o partes de lotes que han dado origen a la mercadería objeto de este contrato no ha incumplido desde el 31 de diciembre de 2020 al momento de celebración de este contrato con normativa vigente en la República Argentina relativa a (a) derechos de uso de la tierra; (b) protección del medio ambiente; (c) normas relacionadas con los bosques, incluida la gestión forestal y la conservación de la biodiversidad; (d) derechos de terceros; (e) derechos laborales; (f) derechos humanos protegidos por el derecho internacional; (g) el principio de consentimiento libre, previo e informado (CLPI), incluido lo establecido en la Declaración de las Naciones Unidas sobre los Derechos de los Pueblos Indígenas; ni (h) regulaciones tributarias, anticorrupción, comerciales y aduaneras. " +
                        "En caso de que alguna de las declaraciones contenidas en la presente cláusula no sea verdadera, o que por cualquier otro motivo no imputable a Molinos Agro S.A. el poroto de soja entregado no fuese apto para obtener la certificación relativa a que se encuentra en cumplimiento del Reglamento (otorgada por Visec o quien en cada momento certifique dichas circunstancias), las partes acuerdan que dicho evento se considerará un incumplimiento a una obligación principal por parte del vendedor, y Molinos Agro S.A. tendrá el derecho de modificar las condiciones de entrega acordadas, rechazar la mercadería o aplicar cualquier otra medida o recurso que pudiera corresponder.";
                }
            }
            return res;
        }
    }
}