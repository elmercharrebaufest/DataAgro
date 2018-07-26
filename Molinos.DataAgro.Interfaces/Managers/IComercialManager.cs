
using Mastersoft.Framework.Standard;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Molinos.DataAgro.Interfaces
{
    public interface IComercialManager
    {
        Task<DatosIniAbmComercial> TraerDatosInicialesAsync();

        Task<ResultIniComercial> TraerTodoComercialAsync();

        Task<Comercial> TraerComercialAsync(int intComercialId);

        Task<List<ComercialCombo>> ObtenerComerciales(int intComercialId);

        Task<EntityErrors> GrabarComercialAsync(Comercial oComercial);

        Task<EntityErrors> EliminarComercialAsync(int intComercialId);

        List<Comercial> ListarComercial(string comercial, List<int> comerciales);
        
        bool ComercialExiste(string ActiveDirectoryId);

        bool ComercialPerteneceProveedor(string ActiveDirectory_Id, int Proveedor_Id);

        Task<int> VerificarGrupoComercial(string grupoDeCompra);

    }
}


