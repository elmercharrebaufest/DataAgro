using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Entities.Helpers;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Transactions;

namespace Molinos.DataAgro.Repository
{
    public class RepositorioEF : IRepositorio
    {
        private const int SqlFkError = 547;

        private readonly DbContext context;

        public RepositorioEF(DbContext context)
        {
            this.context = context;
        }

        private IDbSet<TEntidad> Set<TEntidad>() where TEntidad : class
        {
            return context.Set<TEntidad>();
        }

        public TEntidad Obtener<TEntidad>(object id) where TEntidad : class
        {
            return Set<TEntidad>().Find(id);
        }

        public TEntidad ObtenerUnchanged<TEntidad>(object id) where TEntidad : class
        {
            var entity = Set<TEntidad>().Find(id);
            context.Entry(entity).State = EntityState.Unchanged;
            return entity;
        }

        public TEntidad Obtener<TEntidad>(Expression<Func<TEntidad, bool>> filtro) where TEntidad : class
        {
            return Set<TEntidad>().FirstOrDefault(filtro);
        }
        public TEntidad ObtenerNoTracking<TEntidad>(Expression<Func<TEntidad, bool>> filtro) where TEntidad : class
        {
            return Set<TEntidad>().AsNoTracking().FirstOrDefault(filtro);
        }
        public TEntidad Obtener<TEntidad>(IEnumerable<Expression<Func<TEntidad, object>>> includes, Expression<Func<TEntidad, bool>> filtro) where TEntidad : class
        {
            IQueryable<TEntidad> resultado = Set<TEntidad>();
            foreach (var i in includes)
            {
                resultado = resultado.Include(i);
            }
            return resultado.FirstOrDefault(filtro);
        }

        public TEntidad ObtenerPrimero<TEntidad>(Expression<Func<TEntidad, bool>> condicion) where TEntidad : class
        {
            return Set<TEntidad>().FirstOrDefault(condicion);
        }

        public TProyeccion Obtener<TEntidad, TProyeccion>(Expression<Func<TEntidad, bool>> filtro, Expression<Func<TEntidad, TProyeccion>> proyeccion)
            where TEntidad : class
        {
            return Set<TEntidad>().Where(filtro).Select(proyeccion).FirstOrDefault();
        }

        public TProyeccion ObtenerMayor<TEntidad, TOrden, TProyeccion>(Expression<Func<TEntidad, bool>> filtro, Expression<Func<TEntidad, TOrden>> columnaOrden, Expression<Func<TEntidad, TProyeccion>> proyeccion)
            where TEntidad : class
            where TOrden : IComparable
        {
            return Set<TEntidad>().Where(filtro).OrderByDescending(columnaOrden).Select(proyeccion).FirstOrDefault();
        }

        public TEntidad ObtenerMenor<TEntidad, TOrden>(Expression<Func<TEntidad, bool>> filtro, Expression<Func<TEntidad, TOrden>> columnaOrden)
            where TEntidad : class
            where TOrden : IComparable
        {
            return Set<TEntidad>().Where(filtro).OrderBy(columnaOrden).FirstOrDefault();
        }

        public TEntidad ObtenerMayor<TEntidad, TOrden>(Expression<Func<TEntidad, bool>> filtro, Expression<Func<TEntidad, TOrden>> columnaOrden)
            where TEntidad : class
            where TOrden : IComparable
        {
            return Set<TEntidad>().Where(filtro).OrderByDescending(columnaOrden).FirstOrDefault();
        }

        public TProyeccion ObtenerMenor<TEntidad, TOrden, TProyeccion>(Expression<Func<TEntidad, bool>> filtro, Expression<Func<TEntidad, TOrden>> columnaOrden, Expression<Func<TEntidad, TProyeccion>> proyeccion)
            where TEntidad : class
            where TOrden : IComparable
        {
            return Set<TEntidad>().Where(filtro).OrderBy(columnaOrden).Select(proyeccion).FirstOrDefault();
        }

        public List<TEntidad> Listar<TEntidad>(Expression<Func<TEntidad, bool>> filtro = null, int maxResultados = 0, string orden = null, DirOrden direccionOrden = DirOrden.Asc) where TEntidad : class
        {
            return ListarQueryable(Set<TEntidad>(), filtro, orden, direccionOrden, maxResultados).ToList();
        }

        public List<TEntidad> ListarNoTracking<TEntidad>(Expression<Func<TEntidad, bool>> filtro = null, int maxResultados = 0, string orden = null, DirOrden direccionOrden = DirOrden.Asc) where TEntidad : class
        {
            return ListarQueryable(Set<TEntidad>().AsNoTracking(), filtro, orden, direccionOrden, maxResultados).ToList();
        }

        public List<TEntidad> Listar<TEntidad>(IEnumerable<Expression<Func<TEntidad, object>>> includes, Expression<Func<TEntidad, bool>> filtro, int maxResultados = 0, string orden = null, DirOrden direccionOrden = DirOrden.Asc) where TEntidad : class
        {
            IQueryable<TEntidad> resultado = Set<TEntidad>();
            foreach (var i in includes)
            {
                resultado = resultado.Include(i);
            }

            return ListarQueryable(resultado, filtro, orden, direccionOrden, maxResultados).ToList();
        }

        public List<TProyeccion> Listar<TEntidad, TProyeccion>(Expression<Func<TEntidad, TProyeccion>> proyeccion, Expression<Func<TEntidad, bool>> filtro = null, int maxResultados = 0, string orden = null, DirOrden direccionOrden = DirOrden.Asc) where TEntidad : class
        {
            IQueryable<TEntidad> resultado = Set<TEntidad>();
            if (filtro != null)
            {
                resultado = resultado.Where(filtro);
            }

            var resultadoFinal = resultado.GroupBy(proyeccion).Select(g => g.Key);
            resultadoFinal = ListarProyeccionQueryable(resultadoFinal, orden, direccionOrden, maxResultados);
            return resultadoFinal.ToList();
        }

        public decimal Sumar<TEntidad>(Expression<Func<TEntidad, decimal>> proyeccion, Expression<Func<TEntidad, bool>> filtro = null) where TEntidad : class
        {
            IQueryable<TEntidad> resultado = Set<TEntidad>();
            if (filtro != null)
            {
                resultado = resultado.Where(filtro);
            }
            return resultado.Select(proyeccion).DefaultIfEmpty(0).Sum();
        }

        public List<TEntidad> ListarConsulta<TEntidad>(IConsulta<TEntidad> consulta) where TEntidad : class
        {
            return consulta.Ejecutar(context);
        }

        public TEntidad ObtenerConsultaEscalar<TEntidad>(IConsultaEscalar<TEntidad> consulta)
        {
            return consulta.Ejecutar(context);
        }

        public int Contar<TEntidad>() where TEntidad : class
        {
            return Set<TEntidad>().Count();
        }

        public int Contar<TEntidad>(Expression<Func<TEntidad, bool>> filtro) where TEntidad : class
        {
            return Set<TEntidad>().Count(filtro);
        }

        public bool Existe<TEntidad>(Expression<Func<TEntidad, bool>> filtro) where TEntidad : class
        {
            // Esto genera una query más optima que usar un Any()
            return Set<TEntidad>().Where(filtro).Select(x => 1).FirstOrDefault() != 0;
        }

        public TEntidad Agregar<TEntidad>(TEntidad entidad) where TEntidad : class
        {
            return Set<TEntidad>().Add(entidad);
        }

        public virtual void AgregarTodos<TEntidad>(IEnumerable<TEntidad> items, List<KeyValuePair<string, string>> properties = null) where TEntidad : class
        {
            var enumerable = items as IList<TEntidad> ?? items.ToList();
            if (enumerable.Any())
            {
                var dataTable = enumerable.ToDataTable(true, properties);
                context.SqlBulkInsert(dataTable, dataTable.TableName);
            }
        }

        public virtual void ActualizarTodos<TEntidad>(IEnumerable<TEntidad> items, List<KeyValuePair<string, string>> properties = null, string columnaJoin = "Id", string where = "") where TEntidad : class
        {
            var enumerable = items as IList<TEntidad> ?? items.ToList();
            if (enumerable.Any())
            {
                var dataTable = enumerable.ToDataTable(false, properties);
                context.SqlBulkUpdate(dataTable, dataTable.TableName, columnaJoin, where);
            }
        }

        public TEntidad Remover<TEntidad>(object id) where TEntidad : class
        {
            return Remover(Obtener<TEntidad>(id));
        }

        public TEntidad Remover<TEntidad>(TEntidad entidad) where TEntidad : class
        {
            return Set<TEntidad>().Remove(entidad);
        }

        public void RemoverTodos<TEntidad>(IEnumerable<TEntidad> entidades) where TEntidad : class
        {
            foreach (var entidad in entidades)
            {
                Set<TEntidad>().Remove(entidad);
            }
        }

        public void RemoverTodos<TEntidad>(Expression<Func<TEntidad, bool>> filter) where TEntidad : class
        {
            var query = context.Set<TEntidad>().Where(filter);

            string selectSql = query.ToString();
            string deleteSql = "DELETE [Extent1] " + selectSql.Substring(selectSql.IndexOf("FROM"));

            var internalQuery = query.GetType()
                .GetFields(BindingFlags.NonPublic | BindingFlags.Instance)
                .Where(field => field.Name == "_internalQuery")
                .Select(field => field.GetValue(query))
                .First();

            var objectQuery = internalQuery.GetType()
                .GetFields(BindingFlags.NonPublic | BindingFlags.Instance)
                .Where(field => field.Name == "_objectQuery")
                .Select(field => field.GetValue(internalQuery))
                .First() as ObjectQuery;

            var parameters = objectQuery.Parameters
                .Select(p => new SqlParameter(p.Name, p.Value))
                .ToArray();

            context.Database.ExecuteSqlCommand(deleteSql, parameters);
        }

        public void RemoverTodosConReseedCero<TEntidad>(Expression<Func<TEntidad, bool>> filter) where TEntidad : class
        {
            var query = context.Set<TEntidad>().Where(filter);

            string selectSql = query.ToString();
            string deleteSql = "DELETE [Extent1] " + selectSql.Substring(selectSql.IndexOf("FROM"));

            // Obtener parámetros de la query
            var internalQuery = query.GetType()
                .GetFields(BindingFlags.NonPublic | BindingFlags.Instance)
                .Where(field => field.Name == "_internalQuery")
                .Select(field => field.GetValue(query))
                .First();

            var objectQuery = internalQuery.GetType()
                .GetFields(BindingFlags.NonPublic | BindingFlags.Instance)
                .Where(field => field.Name == "_objectQuery")
                .Select(field => field.GetValue(internalQuery))
                .First() as ObjectQuery;

            var parameters = objectQuery.Parameters
                .Select(p => new SqlParameter(p.Name, p.Value))
                .ToArray();

            // Ejecutar el DELETE
            context.Database.ExecuteSqlCommand(deleteSql, parameters);

            // Extraer nombre de tabla y esquema del SELECT
            var fromIndex = selectSql.IndexOf("FROM", StringComparison.OrdinalIgnoreCase);
            var tableSegment = selectSql.Substring(fromIndex);

            var match = System.Text.RegularExpressions.Regex.Match(
                tableSegment,
                @"FROM\s+\[?(?<schema>\w+)\]?\.\[?(?<table>\w+)\]?",
                System.Text.RegularExpressions.RegexOptions.IgnoreCase
            );

            if (match.Success)
            {
                var schema = match.Groups["schema"].Value;
                var table = match.Groups["table"].Value;

                // Chequear si la tabla quedó vacía
                string countSql = $"SELECT COUNT(1) FROM [{schema}].[{table}]";
                int remainingRows = context.Database.SqlQuery<int>(countSql).FirstOrDefault();

                // Solo hacer RESEED si está vacía
                if (remainingRows == 0)
                {
                    string reseedSql = $"DBCC CHECKIDENT ('[{schema}].[{table}]', RESEED, 0)";
                    context.Database.ExecuteSqlCommand(reseedSql);
                }
            }
        }

        public TResultado EjecutarComando<TResultado>(IComando<TResultado> comando)
        {
            return comando.Ejecutar(context);
        }

        public int GuardarCambios()
        {
            try
            {
                return context.SaveChanges();
            }
            catch (DataException e)
            {
                if (ObtenerCodigoError(e) == SqlFkError)
                {
                    throw new EntidadReferenciadaException(string.Empty, e);
                }
                throw;
            }
        }



        private int ObtenerCodigoError(DataException e)
        {
            var code = 0;
            if (e.InnerException != null)
            {
                if (e.InnerException.InnerException is SqlException sqlEx)
                {
                    code = sqlEx.Number;
                }
            }
            return code;
        }

        public List<TEntidad> SelStorePaginado<TEntidad>(string store, int maxResultados, int pagina, params object[] parameters) where TEntidad : class
        {
            var parametros = context.Database.SqlQuery<string>($"select PARAMETER_NAME from information_schema.parameters where specific_name = '{store}'").ToList();

            var i = 0;
            var parametrosSql = new List<SqlParameter>();
            var parametrosStr = string.Empty;
            foreach (var parametro in parametros)
            {
                if (parametro != "@RETURN_VALUE")
                {
                    parametrosSql.Add(new SqlParameter(parametro, parameters[i] == null ? DBNull.Value : parameters[i]));
                    i++;
                    if (parametrosStr.Length > 0)
                    {
                        parametrosStr += ", ";
                    }
                    parametrosStr = parametrosStr + parametro;
                }
            }

            var resultado = context.Database
                .SqlQuery<TEntidad>(("exec " + store + " " + parametrosStr).Trim(), parametrosSql.ToArray());
            var resultadoPaginado = resultado.Select(x => x);
            if (pagina > 0)
            {
                resultadoPaginado = resultadoPaginado.Skip((pagina - 1) * maxResultados);
            }
            if (maxResultados > 0)
            {
                resultadoPaginado = resultadoPaginado.Take(maxResultados);
            }
            return resultadoPaginado.ToList();
        }

        public List<TEntidad> SelStore<TEntidad>(string store, int maxResultados, params object[] parameters) where TEntidad : class
        {
            return SelStorePaginado<TEntidad>(store, maxResultados, 0, parameters);
        }

        public void EliminarTokens(long cuit)
        {
            context.Database.SqlQuery<int>(@"
                    begin 
                        DELETE FROM TokenAuth WHERE Cuit = @cuit or Vencimiento < @fecha
                        select 1 
                    end
                "
                , new SqlParameter("@cuit", cuit)
                , new SqlParameter("@fecha", DateTime.Now)).First();
        }

        private static IQueryable<TProyeccion> ListarProyeccionQueryable<TProyeccion>(IQueryable<TProyeccion> resultadoFinal, string orden, DirOrden direccionOrden, int maxResultados)
        {

            if (orden != null)
            {
                var selectorOrden = Expresiones.Propiedad<TProyeccion>(orden);
                resultadoFinal = direccionOrden == DirOrden.Asc
                                 ? resultadoFinal.OrderBy(selectorOrden)
                                 : resultadoFinal.OrderByDescending(selectorOrden);
            }
            //CAMBIE ESTO ARA ACA ABAJO
            if (maxResultados != 0)
            {
                resultadoFinal = resultadoFinal.Take(maxResultados);
            }

            return resultadoFinal;
        }

        private IQueryable<TEntidad> ListarQueryable<TEntidad>(IQueryable<TEntidad> resultado, Expression<Func<TEntidad, bool>> filtro, string orden, DirOrden direccionOrden, int maxResultados) where TEntidad : class
        {
            if (filtro != null)
            {
                resultado = resultado.Where(filtro);
            }

            if (maxResultados != 0)
            {
                resultado = resultado.Take(maxResultados);
            }

            if (orden != null)
            {
                var selectorOrden = Expresiones.Propiedad<TEntidad>(orden);
                resultado = direccionOrden == DirOrden.Asc
                                 ? resultado.OrderBy(selectorOrden)
                                 : resultado.OrderByDescending(selectorOrden);
            }

            return resultado;
        }

        public void MigrarReporteCompraNetPosicionCompras()
        {
            context.Database.SqlQuery<int>(@"
                    begin 
                        exec MigrarReporteCompraNetPosicionCompras
                        select 1 
                    end
                "
                ).First();
        }

        public void TruncarTabla<TEntidad>() where TEntidad : class
        {
            var tabla = typeof(TEntidad).Name;
            context.Database.ExecuteSqlCommand("TRUNCATE TABLE [" + tabla + "]");
        }

        public List<Cupo> ListarCupoConsultaCuposDiarios(List<string> cuposSapStop)
        {
            var value = "'" + string.Join("','", cuposSapStop) + "'";
            var sql = string.Format(
    "SELECT * FROM Cupo WHERE CupoSap IN ({0})",
    value);

            var result = context.Set<Cupo>().SqlQuery(sql).ToList();
            return result;
            //context.Database.SqlQuery<Cupo>(@"
            //        begin 
            //            exec MigrarReporteCompraNetPosicionCompras
            //      select 1 
            //  end
            //    "
            //               ).First();
        }

        public List<TEntidad> ListarEntidadMasiva<TEntidad>(string campo, List<string> filtros) where TEntidad : class
        {
            var filtro = "'" + string.Join("','", filtros) + "'";
            var tabla = typeof(TEntidad).Name;
            var sql = string.Format(
            "SELECT * FROM {0} WHERE {1} IN ({2})",
            tabla, campo, filtro);

            var result = context.Set<TEntidad>().SqlQuery(sql).ToList();
            return result;
        }


    }
}
