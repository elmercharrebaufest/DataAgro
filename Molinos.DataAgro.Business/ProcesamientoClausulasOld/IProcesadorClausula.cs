using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;

namespace Molinos.DataAgro.Business
{
    public interface IProcesadorClausula
    {
        ResultadoClausula DevolverClausulas(Clausula comando);
    }
    public interface IProcesadorClausula<in TClausula> : IProcesadorClausula
    {
        ResultadoClausula DevolverClausulas(TClausula comando);
    }
}
