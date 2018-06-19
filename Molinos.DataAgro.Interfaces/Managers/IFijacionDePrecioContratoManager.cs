
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Mastersoft.Framework.Standard;
using Mastersoft.Framework.DataRepository;
using Mastersoft.Framework.Interfaces;

using Molinos.DataAgro.Entities;

namespace Molinos.DataAgro.Interfaces
{
    public interface IFijacionDePrecioContratoManager
    {
        void Inicializar(MSContext oContexto);

        void Inicializar(MSContext oContexto, IUnitOfWorkAsync oUnitOfWork);

        Task<DatosIniAbmFijacionDePrecioContrato> TraerDatosInicialesAsync();

        Task<ResultIniFijacionDePrecioContrato> TraerTodoFijacionDePrecioAsync();

        Task<FijacionDePrecioContrato> TraerFijacionDePrecioAsync(int intFijacionId);

        Task<GrabarContratoResult> GrabarAmpliacionFijacion(FijacionDePrecioContrato oFijacion);

        Task<ResultIniFijacionDePrecioContrato> TraerFijacionDePrecioContratoAsync(int ContratoId);

        Task<GrabarFijacionResult> GrabarFijacionDePrecioAsync(FijacionDePrecioContrato oFijacionDePrecio);

        Task<GrabarFijacionResult> ConfirmarFijacion(FijacionDePrecioContrato oFijacionDePrecio);

        Task<EntityErrors> EliminarFijacionDePrecioAsync(int intFijacionId);

        Task<GrabarFijacionResult> FinalizarFijacion(FijacionDePrecioContrato oParam, string activeDiretoryId);
    }
}


