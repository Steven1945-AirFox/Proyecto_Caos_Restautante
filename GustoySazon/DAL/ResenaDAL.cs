using GustoySazon.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace GustoySazon.DAL
{
    public class ResenaDAL
    {
        private string connectionString = ConfigurationManager.ConnectionStrings["GustoySazonDB"].ConnectionString;

        public List<ResenaModel> ObtenerResenas()
        {
            var lista = new List<ResenaModel>();
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = "SELECT Id, UsuarioId, NombreUsuario, Calificacion, ResenaTexto, Fecha FROM GustoySazonDB.Resenas ORDER BY Fecha DESC";
                SqlCommand cmd = new SqlCommand(query, conn);
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    lista.Add(new ResenaModel
                    {
                        Id = Convert.ToInt32(reader["Id"]),
                        UsuarioId = Convert.ToInt32(reader["UsuarioId"]),
                        NombreUsuario = reader["NombreUsuario"].ToString(),
                        Calificacion = reader["Calificacion"] == DBNull.Value ? null : (int?)Convert.ToInt32(reader["Calificacion"]),
                        ResenaTexto = reader["ResenaTexto"]?.ToString(),
                        Fecha = Convert.ToDateTime(reader["Fecha"])
                    });
                }
                reader.Close();
            }
            return lista;
        }
    }
}