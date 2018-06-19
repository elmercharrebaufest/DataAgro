using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Mastersoft.Framework.Standard;
using Mastersoft.Framework.DataRepository;
using Mastersoft.Framework.Interfaces;

using Molinos.DataAgro.Entities;

namespace Molinos.DataAgro.Interfaces {

    public interface ICompraNetManager {

        void Inicializar(MSContext oContexto);

        void Inicializar(MSContext oContexto, IUnitOfWorkAsync oUnitOfWork); 
        
        Task<DatosIniCompraNet> TraerDatosInicialesAsync(string ActiveDirectory);
        /*
        ParamCompraNet TraerParam();

        EntityErrors ValidarContrato(ParamCompraNet oParam);

        Task<ResultCompraNet> TraerDatosAsyncContrato(ParamCompraNet oParam);
        
        Task<DatosIniCompraNet> TraerDatosInicialesAsyncFijacion(string ActiveDirectory);

        EntityErrors ValidarFijacion(ParamCompraNet oParam);

        Task<ResultCompraNet> TraerDatosAsyncFijacion(ParamCompraNet oParam);
        */

        //void ActualizarProveedoresCubo(int? ComercialId);

    }
}