using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using GustoySazon.Models;
using System.Data.SqlClient;
using System.Configuration;
using System.Diagnostics; // Para usar Debug.WriteLine

namespace GustoySazon.DAL
{
    //public class SimulacionDAL
    //{
    //    private readonly string connectionString = ConfigurationManager.ConnectionStrings["GustoySazonDB"].ConnectionString;



    //    public int CrearSimulacion()
    //    {
    //        int simulacionId = 0;
    //        simulacionId = CrearSimulacionIntro();

    //        return simulacionId;
    //    }
    //    public int CrearSimulacionIntro()
    //    {
    //        int simulacionId = 0;
    //        Debug.WriteLine(" DAL: Intentando crear simulación...");
    //        try
    //        {
    //            using (SqlConnection conn = new SqlConnection(connectionString))
    //            {
    //                string query = @"INSERT INTO GC_Simulaciones (FechaInicio, Estado) 
    //                             OUTPUT INSERTED.Id
    //                             VALUES (GETDATE(), 'Activa')";

    //                SqlCommand cmd = new SqlCommand(query, conn);
    //                Debug.WriteLine($"📡 DAL: Conexión string: {connectionString}");
    //                conn.Open();
    //                Debug.WriteLine(" DAL: Conexión abierta exitosamente");
    //                simulacionId = (int)cmd.ExecuteScalar();
    //                Debug.WriteLine($" DAL: Simulación creada con ID: {simulacionId}");
    //            }
    //        }
    //        catch (Exception ex)
    //        {
    //            Debug.WriteLine(" DAL: Error al crear simulación: " + ex.Message);
    //        }
    //        return simulacionId;
    //    }

    public class SimulacionDAL
    {
        private readonly string connectionString = ConfigurationManager.ConnectionStrings["GustoySazonDB"].ConnectionString;

        public int CrearSimulacion()
        {
            int simulacionId = 0;

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    string query = @"INSERT INTO GC_Simulaciones (FechaInicio, Estado) 
                                     OUTPUT INSERTED.Id
                                     VALUES (GETDATE(), 'Activa')";

                    SqlCommand cmd = new SqlCommand(query, conn);
                    conn.Open();
                    simulacionId = (int)cmd.ExecuteScalar();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("DAL Error al crear simulación: " + ex.Message);
            }

            return simulacionId;
        }

        // De aca para arriba no tocar

        public void RegistrarMetrica(ValorMetricaModel metrica)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = @"INSERT INTO GC_ValoresMetricas 
                         (SimulacionId, MetricaId, Valor, FechaRegistro) 
                         VALUES (@SimulacionId, @MetricaId, @Valor, @FechaRegistro)";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@SimulacionId", metrica.SimulacionId);
                cmd.Parameters.AddWithValue("@MetricaId", metrica.MetricaId);
                cmd.Parameters.AddWithValue("@Valor", metrica.Valor);
                cmd.Parameters.AddWithValue("@FechaRegistro", metrica.FechaRegistro);

                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }


        public void GuardarValorMetrica(int simulacionId, int metricaId, int valor)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = @"INSERT INTO GC_ValoresMetricas 
                         (SimulacionId, MetricaId, Valor, FechaRegistro) 
                         VALUES (@SimulacionId, @MetricaId, @Valor, @Fecha)";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@SimulacionId", simulacionId);
                cmd.Parameters.AddWithValue("@MetricaId", metricaId); // ahora INT
                cmd.Parameters.AddWithValue("@Valor", valor);
                cmd.Parameters.AddWithValue("@Fecha", DateTime.Now);

                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }


        public void InsertarMetrica(int simulacionId, int metricaId, int valor)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["GustoySazonDB"].ConnectionString))
                {
                    string query = "INSERT INTO GC_Metricas (SimulacionId, MetricaId, Valor) VALUES (@SimulacionId, @MetricaId, @Valor)";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@SimulacionId", simulacionId);
                    cmd.Parameters.AddWithValue("@MetricaId", metricaId);
                    cmd.Parameters.AddWithValue("@Valor", valor);
                    conn.Open();
                    cmd.ExecuteNonQuery();
                    Debug.WriteLine("✅ DAL: Métrica insertada correctamente");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("❌ DAL: Error al insertar métrica: " + ex.Message);
            }
        }


        public void GuardarSolucionAplicada(SolucionAplicadaModel solucion)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = @"INSERT INTO GC_SolucionesAplicadas 
            (SimulacionId, EventoId, Solucion, Severidad, FechaAplicacion)
            VALUES (@SimulacionId, @EventoId, @Solucion, @Severidad, @Fecha)";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@SimulacionId", solucion.SimulacionId);
                cmd.Parameters.AddWithValue("@EventoId", solucion.EventoId);
                cmd.Parameters.AddWithValue("@Solucion", solucion.Solucion);
                cmd.Parameters.AddWithValue("@Severidad", solucion.Severidad);
                cmd.Parameters.AddWithValue("@Fecha", solucion.FechaAplicacion);
                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }
    }
}


