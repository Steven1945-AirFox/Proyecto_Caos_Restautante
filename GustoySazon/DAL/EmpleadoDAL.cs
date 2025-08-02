using GustoySazon.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace GustoySazon.DAL
{
    public class EmpleadoDAL
    {
        private string connectionString = ConfigurationManager.ConnectionStrings["GustoySazonDB"].ConnectionString;

        public void InsertarEmpleado(EmpleadoModel emp)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = "INSERT INTO Empleados (Nombre, Cedula, Correo, Contrasena, Rol) VALUES (@Nombre, @Cedula, @Correo, @Contrasena, @Rol)";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@Nombre", emp.Nombre);
                cmd.Parameters.AddWithValue("@Cedula", emp.Cedula);
                cmd.Parameters.AddWithValue("@Correo", emp.Correo);
                cmd.Parameters.AddWithValue("@Contrasena", emp.Contrasena);
                cmd.Parameters.AddWithValue("@Rol", emp.Rol);

                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }


        public void ActualizarEmpleado(EmpleadoModel emp)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = "UPDATE Empleados SET Nombre=@Nombre, Cedula=@Cedula, Correo=@Correo, Contrasena=@Contrasena, Rol=@Rol WHERE Id=@Id";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@Nombre", emp.Nombre);
                cmd.Parameters.AddWithValue("@Cedula", emp.Cedula);
                cmd.Parameters.AddWithValue("@Correo", emp.Correo);
                cmd.Parameters.AddWithValue("@Contrasena", emp.Contrasena);
                cmd.Parameters.AddWithValue("@Rol", emp.Rol);
                cmd.Parameters.AddWithValue("@Id", emp.Id);

                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        public void EliminarEmpleado(int id)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = "DELETE FROM Empleados WHERE Id=@Id";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@Id", id);

                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }


        public List<EmpleadoModel> ObtenerEmpleados()
        {
            List<EmpleadoModel> lista = new List<EmpleadoModel>();

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = "SELECT * FROM Empleados";
                SqlCommand cmd = new SqlCommand(query, conn);
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    lista.Add(new EmpleadoModel
                    {
                        Id = Convert.ToInt32(reader["Id"]),
                        Nombre = reader["Nombre"].ToString(),
                        Cedula = Convert.ToInt32(reader["Cedula"]),
                        Correo = reader["Correo"].ToString(),
                        Contrasena = reader["Contrasena"].ToString(),
                        Rol = reader["Rol"].ToString()
                    });
                }
            }

            return lista;
        }
    }
}
