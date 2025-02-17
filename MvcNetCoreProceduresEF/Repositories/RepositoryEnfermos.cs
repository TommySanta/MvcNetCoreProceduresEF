using System.Data.Common;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using MvcNetCoreProceduresEF.Data;
using MvcNetCoreProceduresEF.Models;

namespace MvcNetCoreProceduresEF.Repositories
{
    public class RepositoryEnfermos
    {
        private EnfermosContext context;

        public RepositoryEnfermos(EnfermosContext context)
        {
            this.context = context;
        }

        public async Task<List<Enfermo>> GetEnfermosAsync() 
        {
            using (DbCommand com = this.context.Database.GetDbConnection().CreateCommand())
            {
                string sql = "SP_TODOS_ENFERMOS";
                com.CommandType = System.Data.CommandType.StoredProcedure;
                com.CommandText = sql;
                await com.Connection.OpenAsync();

                DbDataReader reader = await com.ExecuteReaderAsync();
                List<Enfermo> enfermos = new List<Enfermo>();
                while (await reader.ReadAsync())
                {
                    Enfermo enfermo = new Enfermo
                    {
                        Inscripcion = reader["INSCRIPCION"].ToString(),
                        Apellido = reader["APELLIDO"].ToString(),
                        Direccion = reader["DIRECCION"].ToString(),
                        FechaNacimiento = DateTime.Parse(reader["FECHA_NAC"].ToString()),
                        Genero = reader["S"].ToString()
                    };
                    enfermos.Add(enfermo);
                }
                await reader.CloseAsync();
                await com.Connection.CloseAsync();
                return enfermos;
            }
        }

        public Enfermo FindEnfermo(string inscripcion)
        {
            string sql = "SP_FIND_ENFERMO @INSCRIPCION";
            SqlParameter pamInscripcion =
                new SqlParameter("@INSCRIPCION", inscripcion);
            var consulta = this.context.Enfermos.FromSqlRaw(sql, pamInscripcion);

            Enfermo enfermo = consulta.AsEnumerable().FirstOrDefault();
            return enfermo;
        }

        public async Task DeleteEnfermoAsync(string inscripcion)
        {
            string sql = "SP_DELETE_ENFERMO @INSCRIPCION";
            SqlParameter pamInscripcion =
                new SqlParameter("@INSCRIPCION", inscripcion);

            await this.context.Database.ExecuteSqlRawAsync(sql, pamInscripcion);


            #region Una Forma de hacerlo
            //string sql = "SP_DELETE_ENFERMO";
            //SqlParameter pamInscripcion =
            //    new SqlParameter("@INSCRIPCION", inscripcion);
            //using (DbCommand com = this.context.Database.GetDbConnection().CreateCommand())
            //{
            //    com.CommandType = System.Data.CommandType.StoredProcedure;
            //    com.CommandText = sql;
            //    com.Parameters.Add(pamInscripcion);
            //    com.Connection.Open();
            //    com.ExecuteNonQuery();
            //    com.Connection.Close();
            //    com.Parameters.Clear();
            //}
            #endregion

        }

        public async Task InsertEnfermoAsync(string apellido, string direccion,
            DateTime fechaNacimiento, string genero, string nss)
        {
            string sql = "SP_INSERT_ENFERMO @APELLIDO, @DIRECCION, @FECHA_NAC, @GENERO, @NSS";

            
            SqlParameter pamApellido = new SqlParameter("@APELLIDO", apellido);
            SqlParameter pamDireccion = new SqlParameter("@DIRECCION", direccion);
            SqlParameter pamFechaNac = new SqlParameter("@FECHA_NAC", fechaNacimiento);
            SqlParameter pamGenero = new SqlParameter("@GENERO", genero);
            SqlParameter pamNss = new SqlParameter("@NSS", nss);

            await this.context.Database.ExecuteSqlRawAsync(sql,
                pamNss, pamApellido, pamDireccion, pamFechaNac, pamGenero);
        }
    }
}
