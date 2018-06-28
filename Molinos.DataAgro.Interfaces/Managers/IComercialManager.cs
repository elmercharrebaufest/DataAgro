
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Mastersoft.Framework.Standard;
using Mastersoft.Framework.DataRepository;
using Mastersoft.Framework.Interfaces;

using Molinos.DataAgro.Entities;
using Molinos.DataAgro.Entities.Common.Enums;

namespace Molinos.DataAgro.Interfaces
{
    public interface IComercialManager
    {
        void Inicializar(MSContext oContexto);

        void Inicializar(MSContext oContexto, IUnitOfWorkAsync oUnitOfWork);

        Task<DatosIniAbmComercial> TraerDatosInicialesAsync();

        Task<ResultIniComercial> TraerTodoComercialAsync();

        Task<Comercial> TraerComercialAsync(int intComercialId);

        Task<List<ComercialCombo>> ObtenerComerciales(int intComercialId);

        Task<EntityErrors> GrabarComercialAsync(Comercial oComercial);

        Task<EntityErrors> EliminarComercialAsync(int intComercialId);

        List<Comercial> ListarComercial(string comercial);

        bool EsAdmin(string ActiveDirectoryId);

        EnumPerfil ObtenerPerfil(string activeDirectoryId);

        bool EsPerfilAdministrativo(string activeDirectoryId);

        bool EsPerfilVisualizador(string activeDirectoryId);

        bool ComercialExiste(string ActiveDirectoryId);

        bool ComercialPerteneceProveedor(string ActiveDirectory_Id, int Proveedor_Id);

        Task<int> VerificarGrupoComercial(string grupoDeCompra);

        //void ActualizarNumeroSAPGrupoDeCompras(int GrupoCompraId, string NumeroSAP);
    }
}


