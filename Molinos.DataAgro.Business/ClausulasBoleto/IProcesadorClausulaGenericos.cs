using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.ClausulasBoleto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Molinos.DataAgro.Entities.Entities;

namespace Molinos.DataAgro.Business.ClausulasBoleto
{
    public interface IProcesadorClausulaGenericos
    {
        ResultadoClausula DevolverClausulas(ClausulaGenericos comando);
    }
    public interface IProcesadorClausulaGenericos<in TClausula> : IProcesadorClausulaGenericos
    {
        ResultadoClausula DevolverClausulas(TClausula comando);
    }
}
