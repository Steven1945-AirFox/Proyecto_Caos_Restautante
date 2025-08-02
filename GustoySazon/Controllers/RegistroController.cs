using GustoySazon.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace GustoySazon.Controllers
{
    public class RegistroController : Controller
    {
        private string connectionString = ConfigurationManager.ConnectionStrings["GustoySazonDB"].ConnectionString;

        [HttpGet]
        public ActionResult Index()
        {
            // Verificar si ya hay un usuario en sesión
            if (Session["UsuarioId"] != null)
            {
                return RedirectToAction("FilaEspera", "Home");
            }
            return View();
        }




        [HttpPost]
        public ActionResult Index(ClienteViewModel modelo)
        {
            if (ModelState.IsValid)
            {
                using (SqlConnection conexion = new SqlConnection(connectionString))
                {
                    conexion.Open();

                    // Verificar si el cliente ya existe por cédula
                    string comandoIdentificarUsuarioCedula = "SELECT Id FROM Usuarios WHERE Cedula = @Cedula";
                    int usuarioId = 0;

                    using (SqlCommand comandoUsuarioCedula = new SqlCommand(comandoIdentificarUsuarioCedula, conexion))
                    {
                        comandoUsuarioCedula.Parameters.AddWithValue("@Cedula", modelo.Cedula);
                        var result = comandoUsuarioCedula.ExecuteScalar();

                        if (result != null) 
                        {
                            usuarioId = (int)result;

                            // Actualizar datos del cliente incluyendo Correo y Contrasena
                            string comandoActualizarDatos = "UPDATE Usuarios SET Nombre = @Nombre, Sillas = @Sillas, Correo = @Correo, Contrasena = @Contrasena WHERE Id = @Id";
                            using (SqlCommand comandoActualizar = new SqlCommand(comandoActualizarDatos, conexion))
                            {
                                comandoActualizar.Parameters.AddWithValue("@Nombre", modelo.Nombre);
                                comandoActualizar.Parameters.AddWithValue("@Sillas", modelo.Sillas);
                                comandoActualizar.Parameters.AddWithValue("@Correo", modelo.Correo);
                                comandoActualizar.Parameters.AddWithValue("@Contrasena", modelo.Contrasena);
                                comandoActualizar.Parameters.AddWithValue("@Id", usuarioId);
                                comandoActualizar.ExecuteNonQuery();
                            }
                        }
                        else 
                        {
                            string comandoEnviarUsuario = @"INSERT INTO Usuarios (Nombre, Cedula, Sillas, HoraRegistro, Correo, Contrasena)
                                                OUTPUT INSERTED.Id VALUES (@Nombre, @Cedula, @Sillas, GETDATE(), @Correo, @Contrasena)";

                            using (SqlCommand comandoUsuario = new SqlCommand(comandoEnviarUsuario, conexion))
                            {
                                comandoUsuario.Parameters.AddWithValue("@Nombre", modelo.Nombre);
                                comandoUsuario.Parameters.AddWithValue("@Cedula", modelo.Cedula);
                                comandoUsuario.Parameters.AddWithValue("@Sillas", modelo.Sillas);
                                comandoUsuario.Parameters.AddWithValue("@Correo", modelo.Correo);
                                comandoUsuario.Parameters.AddWithValue("@Contrasena", modelo.Contrasena);
                                usuarioId = (int)comandoUsuario.ExecuteScalar();
                            }
                        }
                    }

                    Session["UsuarioId"] = usuarioId;
                    Session["NombreUsuario"] = modelo.Nombre;




                    // Actualizar o insertar tarjeta
                    string comandoEditarAgregarTarjeta = @" IF EXISTS (SELECT 1 FROM Tarjetas WHERE UsuarioId = @UsuarioId) UPDATE Tarjetas SET 
                                        TipoTarjeta = @TipoTarjeta, NumTarjeta = @NumTarjeta, FechaVenc = @FechaVenc WHERE UsuarioId = @UsuarioId
                                        ELSE INSERT INTO Tarjetas (UsuarioId, TipoTarjeta, NumTarjeta, FechaVenc) VALUES (@UsuarioId, @TipoTarjeta, @NumTarjeta, @FechaVenc)";

                    using (SqlCommand comandoTarjeta = new SqlCommand(comandoEditarAgregarTarjeta, conexion))
                    {
                        comandoTarjeta.Parameters.AddWithValue("@UsuarioId", usuarioId);
                        comandoTarjeta.Parameters.AddWithValue("@TipoTarjeta", modelo.TipoTarjeta);
                        comandoTarjeta.Parameters.AddWithValue("@NumTarjeta", modelo.NumTarjeta);
                        comandoTarjeta.Parameters.AddWithValue("@FechaVenc", DateTime.ParseExact(modelo.FechaVenc, "MM/yy", null));
                        comandoTarjeta.ExecuteNonQuery();
                    }

                    conexion.Close();
                }

                return RedirectToAction("Login", "Home");
            }

            return View(modelo);
        }








        //rellenar espacios del formulario automaticamente
        [HttpGet]
        public ActionResult GetClientePorCedula(string cedula)
        {
            using (SqlConnection conexion = new SqlConnection(connectionString))
            {
                conexion.Open();

                string comandoRellenarCliente = @"SELECT u.Nombre, u.Sillas, t.TipoTarjeta, t.NumTarjeta, 
                                FORMAT(t.FechaVenc, 'MM/yy') as FechaVenc, u.Correo, u.Contrasena
                                FROM Usuarios u LEFT JOIN Tarjetas t ON u.Id = t.UsuarioId WHERE u.Cedula = @Cedula";

                using (SqlCommand comandoRellenar = new SqlCommand(comandoRellenarCliente, conexion))
                {
                    comandoRellenar.Parameters.AddWithValue("@Cedula", cedula);

                    using (SqlDataReader lector = comandoRellenar.ExecuteReader())
                    {
                        if (lector.Read())
                        {
                            return Json(new
                            {
                                nombre = lector["Nombre"].ToString(),
                                sillas = (int)lector["Sillas"],
                                tipoTarjeta = lector["TipoTarjeta"] != DBNull.Value ? lector["TipoTarjeta"].ToString() : "",
                                numTarjeta = lector["NumTarjeta"] != DBNull.Value ? lector["NumTarjeta"].ToString() : "",
                                fechaVenc = lector["FechaVenc"] != DBNull.Value ? lector["FechaVenc"].ToString() : "",
                                correo = lector["Correo"] != DBNull.Value ? lector["Correo"].ToString() : "",
                                contrasena = lector["Contrasena"] != DBNull.Value ? lector["Contrasena"].ToString() : ""
                            }, JsonRequestBehavior.AllowGet);
                        }
                    }
                }
            }

            return Json(null, JsonRequestBehavior.AllowGet);
        }
    }
}
