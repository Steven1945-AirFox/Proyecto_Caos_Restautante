using GustoySazon.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace GustoySazon.DAL
{
    public class FinancieroDAL
    {
        private readonly string connectionString = ConfigurationManager.ConnectionStrings["GustoySazonDB"].ConnectionString;

        public ResumenPagoModel ObtenerUltimaVentaRegistrada()
        {
            ResumenPagoModel resumen = null;

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = @"
            SELECT TOP 1 Fecha, TotalPagado
            FROM GustoySazonDB.ResumenPagosPorDia
            ORDER BY Fecha DESC";  // ← La más reciente

                SqlCommand cmd = new SqlCommand(query, conn);
                conn.Open();

                SqlDataReader reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    resumen = new ResumenPagoModel
                    {
                        Fecha = Convert.ToDateTime(reader["Fecha"]),
                        TotalPagado = Convert.ToDecimal(reader["TotalPagado"])
                    };
                }

                reader.Close();
            }

            return resumen;
        }
    }
}