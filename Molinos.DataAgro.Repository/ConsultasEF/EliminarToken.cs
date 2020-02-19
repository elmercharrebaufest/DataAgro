using System;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Transactions;

namespace Molinos.DataAgro.Repository.ConsultasEF
{
    public class EliminarToken: IConsultaEscalar<bool>
    {
        private int cuit;

        public EliminarToken(int cuit)
        {
            this.cuit = cuit;
        }

        private static bool Query(DbContext contexto, int cuit)
        {
            contexto.Database.SqlQuery<int>(@"
                    begin 
                        DELETE FROM TokenAuth WHERE Cuit = @cuit or Vencimiento < @fecha
		                select 1 
		            end
                "
                , new SqlParameter("@cuit", cuit)
                , new SqlParameter("@fecha", DateTime.Now)).First();
            return true;
        }

        public virtual bool Ejecutar(DbContext contexto)
        {
            using (new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = IsolationLevel.ReadUncommitted }))
            {
                return Query(contexto, cuit);
            }
        }
    }
}
