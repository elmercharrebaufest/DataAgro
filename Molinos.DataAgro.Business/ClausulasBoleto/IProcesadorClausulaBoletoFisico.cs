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
    public interface IProcesadorClausulaBoletoFisico
    {
        ResultadoClausula DevolverClausulas(ClausulaBoletoFisico comando);
    }
    public interface IProcesadorClausulaBoletoFisico<in TClausula> : IProcesadorClausulaBoletoFisico
    {
        ResultadoClausula DevolverClausulas(TClausula comando);
    }
}
