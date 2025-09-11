using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.ClausulasBoleto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Molinos.DataAgro.Entities.Entities;
namespace Molinos.DataAgro.Business
{
    public interface IProcesadorClausulaConfirma
    {
        ResultadoClausula DevolverClausulas(ClausulaConfirma comando);
    }
    public interface IProcesadorClausulaConfirma<in TClausula> : IProcesadorClausulaConfirma
    {
        ResultadoClausula DevolverClausulas(TClausula comando);
    }
}
