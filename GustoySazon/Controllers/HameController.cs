using GustoySazon.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Drawing.Printing;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Net.NetworkInformation;
using System.Net.Security;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.Web;
using System.Web.Mvc;
using static GustoySazon.Models.MeseroViewModel;





namespace GustoySazon.Controllers



{
    public class HameController : Controller
    {

        private string connectionString = ConfigurationManager.ConnectionStrings["GustoySazonDB"].ConnectionString;



        public ActionResult Index()
        {
            var modelo = new MeseroMesaViewModel();
            // Llenar el modelo con datos...
            return View(modelo);
        }

        public ActionResult Clientes()
        {
            return View();
        }


        public ActionResult Cocina()
        {
            var rnd = new Random();

            var model = new CocinaViewModel
            {
                Estres = rnd.Next(30, 100),
                Energia = rnd.Next(30, 100),
                Concentracion = rnd.Next(40, 100),
                RiesgoQuemado = rnd.Next(10, 90),
                Calidad = rnd.Next(50, 100),
                Temperatura = Math.Round(rnd.NextDouble() * 60, 1),
                Humo = Math.Round(rnd.NextDouble() * 100, 1),
                Ordenes = new List<OrdenModel>(),

                Equipos = new List<EquipoViewModel>
        {
            new EquipoViewModel { Nombre = "Parrilla Principal", Estado = "Funcionando", Capacidad = 2, Temperatura = 200 },
            new EquipoViewModel { Nombre = "Freidora", Estado = "Funcionando", Capacidad = 4, Temperatura = 180 },
            new EquipoViewModel { Nombre = "Parrilla Principal", Estado = "Roto", Capacidad = 0, Temperatura = 0 },
        }
            };

            return View(model);
        }

        public ActionResult GenCaos()
        {
            return View();
        }

        public ActionResult VistaGeneral()
        {
            return View();
        }

        public ActionResult analisis()
        {
            return View();
        }

        public ActionResult Personal()
        {
            return View();
        }

        public ActionResult Ordenes()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public ActionResult Login()
        {
            return View();
        }

        public ActionResult Autenticacion()
        {
            return View();
        }

        public ActionResult GerenteComunicacion()
        {
            return View();
        }

        public ActionResult GerenteCrisis()
        {
            return View();
        }

        public ActionResult GerenteFinanciero()
        {
            return View();
        }

        public ActionResult GerentePersonal()
        {
            return View();
        }

        public ActionResult GerenteResenas()
        {
            return View();
        }

        public ActionResult RegistroPersonal()
        {
            return View();
        }


        public ActionResult registro()
        {
            return View();
        }


        //Steven Clientes



        //ventana de filaespera

        //Para la pagina de Fila de espera
        //La idea es mostrar los clientes sentados X mesa al presionar el boton info
        public JsonResult ObtenerClientesMesa(int mesaId)
        {
            var clientes = new List<ClienteEnEspera>();

            using (SqlConnection conexion = new SqlConnection(connectionString))
            {
                conexion.Open();

                string comandoDatosClienteMesa = "SELECT Id, Nombre, HoraRegistro FROM Usuarios WHERE MesaId = @MesaId";

                using (SqlCommand ComandoAsignarMesa = new SqlCommand(comandoDatosClienteMesa, conexion))
                {
                    ComandoAsignarMesa.Parameters.AddWithValue("@MesaId", mesaId);

                    using (SqlDataReader lectorDatos = ComandoAsignarMesa.ExecuteReader())
                    {
                        while (lectorDatos.Read())
                        {
                            clientes.Add(new ClienteEnEspera
                            {
                                Id = (int)lectorDatos["Id"],
                                Nombre = lectorDatos["Nombre"].ToString(),
                                HoraRegistro = (DateTime)lectorDatos["HoraRegistro"]
                            });
                        }
                    }
                }
            }

            //Envia informacion en forma de Json para que la pagina Fila de espera la lea
            return Json(clientes, JsonRequestBehavior.AllowGet);
        }

        //Funciones de la fila de espera
        public ActionResult FilaEspera()
        {



            // Verificar si hay usuario en sesión y lo manda directamente a la mesa
            if (Session["UsuarioId"] != null)
            {
                int usuarioIdSesion = (int)Session["UsuarioId"];

                using (SqlConnection conexion = new SqlConnection(connectionString))
                {
                    conexion.Open();

                    // Verificar si el usuario ya tiene mesa asignada
                    string comandoComprobarUsuario = "SELECT MesaId FROM Usuarios WHERE Id = @UsuarioId";
                    using (SqlCommand ComandoComprobarMesa = new SqlCommand(comandoComprobarUsuario, conexion))
                    {
                        ComandoComprobarMesa.Parameters.AddWithValue("@UsuarioId", usuarioIdSesion);
                        var mesaId = ComandoComprobarMesa.ExecuteScalar();

                        if (mesaId != null && mesaId != DBNull.Value)
                        {
                            // Si ya tiene mesa, redirigir directamente a la mesa
                            return RedirectToAction("Mesa");
                        }

                    }
                }
            }



            var modelo = new FilaEsperaViewModel
            {
                ClientesEnEspera = new List<ClienteEnEspera>(),
                MesasDisponibles = new List<MesaDisponible>()
            };



            //Para obtener los usuarios activos
            using (SqlConnection conexion = new SqlConnection(connectionString))
            {
                conexion.Open();


                int usuarioId = 0;
                int sillasRequeridas = 2;
                string comandoUsuariosActuales;

                if (Session["UsuarioId"] != null)
                {
                    //si cliente tiene sesion activa que se ordene en la lista de espera
                    usuarioId = (int)Session["UsuarioId"];
                    comandoUsuariosActuales = "SELECT Id, Sillas FROM Usuarios WHERE Id = @UsuarioId";
                }
                else
                {
                    //si el usuario es nuevo que se posicione de ultimo en la fila
                    comandoUsuariosActuales = "SELECT TOP 1 Id, Sillas FROM Usuarios ORDER BY HoraRegistro DESC";
                }





                //Para preparar datos del cliente para la fila de espera
                using (SqlCommand comandoSillasPedidas = new SqlCommand(comandoUsuariosActuales, conexion))
                {
                    if (Session["UsuarioId"] != null)
                    {
                        comandoSillasPedidas.Parameters.AddWithValue("@UsuarioId", usuarioId);
                    }

                    using (SqlDataReader lector = comandoSillasPedidas.ExecuteReader())
                    {
                        if (lector.Read())
                        {
                            usuarioId = (int)lector["Id"];
                            sillasRequeridas = (int)lector["Sillas"];
                        }
                        else
                        {
                            conexion.Close();
                            return RedirectToAction("registro");
                        }
                    }
                }


                // Guardar sesion del usuario
                //tanto usuario como identificacion
                if (Session["UsuarioId"] == null)
                {
                    Session["UsuarioId"] = usuarioId;
                }

                modelo.UsuarioIdActual = usuarioId;

                // Clientes en espera sin mesa asignada
                string comandoClientes = @"SELECT Id, Nombre, HoraRegistro FROM Usuarios WHERE MesaId IS NULL";

                using (SqlCommand comandoClientesEspera = new SqlCommand(comandoClientes, conexion))
                using (SqlDataReader lector = comandoClientesEspera.ExecuteReader())
                {
                    while (lector.Read())
                    {
                        modelo.ClientesEnEspera.Add(new ClienteEnEspera
                        {
                            Id = (int)lector["Id"],
                            Nombre = lector["Nombre"].ToString(),
                            HoraRegistro = (DateTime)lector["HoraRegistro"]
                        });
                    }
                }


                // Obtener mesas con cantidad de sillas que el cliente elije
                string comandoMesas = @"SELECT m.Id, m.NumeroMesa, m.Sillas, m.Ubicacion, m.Estado, (SELECT COUNT(*) FROM Usuarios u WHERE u.MesaId = m.Id) AS Ocupantes
                                    FROM Mesas m WHERE m.Sillas = @Sillas";

                using (SqlCommand comandoMesasPorSillas = new SqlCommand(comandoMesas, conexion))
                {
                    comandoMesasPorSillas.Parameters.AddWithValue("@Sillas", sillasRequeridas);

                    using (SqlDataReader lector = comandoMesasPorSillas.ExecuteReader())
                    {
                        while (lector.Read())
                        {
                            modelo.MesasDisponibles.Add(new MesaDisponible
                            {
                                Id = (int)lector["Id"],
                                NumeroMesa = (int)lector["NumeroMesa"],
                                Sillas = (int)lector["Sillas"],
                                Ubicacion = lector["Ubicacion"].ToString(),
                                Estado = lector["Estado"].ToString(),

                                //para ver las personas en X mesa
                                Ocupantes = (int)lector["Ocupantes"]
                            });
                        }
                    }
                }

                conexion.Close();
            }

            return View("FilaEspera", modelo);

        }


        //Funciones para asignar mesa en Fila Espera
        [HttpPost]
        public ActionResult AsignarMesa(int usuarioId, int mesaId)
        {
            using (SqlConnection conexion = new SqlConnection(connectionString))
            {
                conexion.Open();

                // Verificar que el usuario no tenga ya mesa asignada
                string comandoComprobarMesa = "SELECT MesaId FROM Usuarios WHERE Id = @UsuarioId";
                using (SqlCommand comandoMesaInfo = new SqlCommand(comandoComprobarMesa, conexion))

                {
                    comandoMesaInfo.Parameters.AddWithValue("@UsuarioId", usuarioId);
                    var MesaActual = comandoMesaInfo.ExecuteScalar();

                    //si devuelve info de mesa que me redirija a la mesa asociada
                    if (MesaActual != null && MesaActual != DBNull.Value)
                    {
                        conexion.Close();
                        return RedirectToAction("Mesa");
                    }
                }

                Session["UsuarioId"] = usuarioId;
                Session["MesaId"] = mesaId;


                // Colocar la mesa al usuario en la BD
                string comandoActualizarMesaUsuario = "UPDATE Usuarios SET MesaId = @MesaId WHERE Id = @UsuarioId";
                using (SqlCommand comandoMesaUsuario = new SqlCommand(comandoActualizarMesaUsuario, conexion))
                {
                    comandoMesaUsuario.Parameters.AddWithValue("@MesaId", mesaId);
                    comandoMesaUsuario.Parameters.AddWithValue("@UsuarioId", usuarioId);
                    comandoMesaUsuario.ExecuteNonQuery();
                }



                // Verificar si la mesa está llena, cuantos clientes tiene
                string comandoContarClienteMesa = "SELECT COUNT(*) FROM Usuarios WHERE MesaId = @MesaId";
                int ocupantes = 0;
                using (SqlCommand comandoContarOcupantes = new SqlCommand(comandoContarClienteMesa, conexion))
                {
                    comandoContarOcupantes.Parameters.AddWithValue("@MesaId", mesaId);
                    ocupantes = (int)comandoContarOcupantes.ExecuteScalar();
                }




                //verificar la capacidad de sillas de la mesa
                string comandoSillas = "SELECT Sillas FROM Mesas WHERE Id = @MesaId";
                int capacidad = 0;
                using (SqlCommand comandoContarSillas = new SqlCommand(comandoSillas, conexion))
                {
                    comandoContarSillas.Parameters.AddWithValue("@MesaId", mesaId);
                    capacidad = (int)comandoContarSillas.ExecuteScalar();
                }


                //si hay igual cantidad de ocupantes que sillas cambia el estado y no me deja entrar
                if (ocupantes == capacidad)
                {
                    string comandoActualizarEstadoMesa = "UPDATE Mesas SET Estado = 'Ocupada' WHERE Id = @MesaId";
                    using (SqlCommand comandoEstadoMesa = new SqlCommand(comandoActualizarEstadoMesa, conexion))
                    {
                        comandoEstadoMesa.Parameters.AddWithValue("@MesaId", mesaId);
                        comandoEstadoMesa.ExecuteNonQuery();
                    }
                }

                conexion.Close();
            }

            return RedirectToAction("Menu", "Home");
        }


        //Funciones de la mesa
        //funcion para poder llevar la orden del Cliente
        [HttpGet]
        public JsonResult ObtenerEstadoMesa()
        {
            try
            {
                var estadoMesas = new List<object>();

                using (var conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    // Obtener todas las mesas con su estado actual
                    string query = @"SELECT m.Id AS MesaId, m.NumeroMesa, m.Estado, 
                            (SELECT COUNT(*) FROM Usuarios u WHERE u.MesaId = m.Id) AS Ocupantes,
                            (SELECT TOP 1 o.Estado FROM Ordenes o 
                             JOIN Usuarios u ON o.UsuarioId = u.Id 
                             WHERE u.MesaId = m.Id ORDER BY o.Fecha DESC) AS EstadoPedido
                            FROM Mesas m";

                    using (var cmd = new SqlCommand(query, conn))
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            estadoMesas.Add(new
                            {
                                Id = (int)reader["MesaId"],
                                NumeroMesa = (int)reader["NumeroMesa"],
                                Estado = reader["Estado"].ToString(),
                                EstadoPedido = reader["EstadoPedido"] != DBNull.Value ?
                                                 reader["EstadoPedido"].ToString() : "Sin pedido",
                                Ocupantes = (int)reader["Ocupantes"]
                            });
                        }
                    }
                }

                return Json(new
                {
                    success = true,
                    ClientesEnMesa = estadoMesas
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    message = ex.Message
                }, JsonRequestBehavior.AllowGet);
            }
        }








        //Funciones para la mesa
        public ActionResult Mesa()
        {
            var usuarioId = Session["UsuarioId"] as int?;
            if (usuarioId == null) return RedirectToAction("Registro");

            var modelo = new MesaViewModel()
            {
                Pedidos = new List<PedidoDetalle>(),
                ClientesEnMesa = new List<ClienteMesaInfo>()
            };

            using (var conexion = new SqlConnection(connectionString))
            {
                conexion.Open();

                // Obtener mesa y estado de atención del usuario actual
                var comandoCargarEstadoMesaGeneral = @"SELECT m.Id AS MesaId, m.NumeroMesa, o.Estado AS EstadoAtencion FROM Usuarios u JOIN Mesas m ON u.MesaId = m.Id 
                                                     LEFT JOIN Ordenes o ON o.UsuarioId = u.Id WHERE u.Id = @Uid ORDER BY o.Fecha DESC";

                using (var comandoMesaEstado = new SqlCommand(comandoCargarEstadoMesaGeneral, conexion))
                {
                    comandoMesaEstado.Parameters.AddWithValue("@Uid", usuarioId.Value);
                    using (var lector = comandoMesaEstado.ExecuteReader())
                    {
                        if (lector.Read())
                        {
                            modelo.MesaId = (int)lector["MesaId"];
                            modelo.NumeroMesa = (int)lector["NumeroMesa"];
                            modelo.EstadoAtencion = lector["EstadoAtencion"] != DBNull.Value ? lector["EstadoAtencion"].ToString() : "Desconocido";
                        }
                    }
                }







                // Obtener pedidos de la orden actual para el usuario
                var comandoCargarOrdenesMesaGeneral = @"SELECT Cantidad, PrecioUnitario, PrecioTotal, Estado, NombreComida, MenuId FROM Ordenes WHERE UsuarioId = @Uid";

                using (var comandoOrdenes = new SqlCommand(comandoCargarOrdenesMesaGeneral, conexion))
                {
                    comandoOrdenes.Parameters.AddWithValue("@Uid", usuarioId.Value);
                    using (var lector = comandoOrdenes.ExecuteReader())
                    {
                        while (lector.Read())
                        {
                            modelo.Pedidos.Add(new PedidoDetalle
                            {
                                MenuId = lector["MenuId"] != DBNull.Value ? (int)lector["MenuId"] : 0,
                                NombreComida = lector["NombreComida"] != DBNull.Value ? lector["NombreComida"].ToString() : "Sin nombre",
                                Cantidad = lector["Cantidad"] != DBNull.Value ? (int)lector["Cantidad"] : 0,
                                PrecioUnitario = lector["PrecioUnitario"] != DBNull.Value ? (decimal)lector["PrecioUnitario"] : 0m,
                                PrecioTotal = lector["PrecioTotal"] != DBNull.Value ? (decimal)lector["PrecioTotal"] : 0m,
                                Estado = lector["Estado"] != DBNull.Value ? lector["Estado"].ToString() : "Desconocido"
                            });
                        }
                    }
                }






                // Obtener info de todos los clientes en la misma mesa y ver estado del pedido
                var comandoClientesdeMesa = @"SELECT u.Nombre, CASE WHEN EXISTS ( SELECT 1 FROM Ordenes o WHERE o.UsuarioId = u.Id AND o.Estado NOT IN ('Entregado', 'Pagado')
                         ) THEN 1 ELSE 0 END AS TienePedido, CASE WHEN EXISTS (SELECT 1 FROM Ordenes o WHERE o.UsuarioId = u.Id AND o.Estado = 'Pagado') THEN 'Pagado'
                         WHEN EXISTS (SELECT 1 FROM Ordenes o WHERE o.UsuarioId = u.Id AND o.Estado = 'Entregado') THEN 'Entregado'
                         WHEN EXISTS (SELECT 1 FROM Ordenes o WHERE o.UsuarioId = u.Id AND o.Estado NOT IN ('Entregado', 'Pagado')) THEN 'Activo' ELSE 'Esperando' END AS EstadoPedido
                         FROM Usuarios u WHERE u.MesaId = @MesaId AND u.Id != @UsuarioId";

                using (var comandoMetricasClientes = new SqlCommand(comandoClientesdeMesa, conexion))
                {
                    comandoMetricasClientes.Parameters.AddWithValue("@MesaId", modelo.MesaId);
                    comandoMetricasClientes.Parameters.AddWithValue("@UsuarioId", usuarioId.Value);
                    using (var lector = comandoMetricasClientes.ExecuteReader())
                    {
                        while (lector.Read())
                        {
                            modelo.ClientesEnMesa.Add(new ClienteMesaInfo
                            {
                                Nombre = lector["Nombre"].ToString(),
                                TienePedido = Convert.ToBoolean(lector["TienePedido"]),
                                EstadoPedido = lector["EstadoPedido"].ToString()
                            });
                        }
                    }
                }

                conexion.Close();
            }

            return View(modelo);
        }








        [HttpPost]
        public ActionResult Mesa(MesaViewModel modelo)
        {
            if (Session["UsuarioId"] == null)
            {
                return RedirectToAction("Registro");
            }

            int usuarioId = (int)Session["UsuarioId"];

            using (SqlConnection conexion = new SqlConnection(connectionString))
            {
                conexion.Open();

                // Obtener las ordenes relacionadas con la id del usuario en sesion
                int ordenId = 0;
                string comandoObtenerOrden = "SELECT TOP 1 Id FROM Ordenes WHERE UsuarioId = @UsuarioId ORDER BY Fecha DESC";

                using (SqlCommand comandoObtenerOrdeneCliente = new SqlCommand(comandoObtenerOrden, conexion))
                {
                    comandoObtenerOrdeneCliente.Parameters.AddWithValue("@UsuarioId", usuarioId);
                    var result = comandoObtenerOrdeneCliente.ExecuteScalar();
                    if (result == null)
                    {
                        TempData["Error"] = "No se encontró orden para este usuario.";
                        return RedirectToAction("Menu");
                    }

                    ordenId = (int)result;
                }


                //insertar las ordenes relacionadas al cliente a la tabla Ordenes

                if (modelo?.Pedidos != null && modelo.Pedidos.Any())
                {
                    int mesaId = modelo.MesaId;

                    foreach (var menuplatillos in modelo.Pedidos)
                    {
                        string comandoInsertarDetalle = @"INSERT INTO Ordenes (UsuarioId, MesaId, MenuId, Cantidad, PrecioUnitario, PrecioTotal, NombreComida, Estado)
                                                 VALUES (@UsuarioId, @MesaId, @MenuId, @Cantidad, @PrecioUnitario, @PrecioTotal, @NombreComida, 'Pendiente')";

                        using (SqlCommand comandoDetalleOrden = new SqlCommand(comandoInsertarDetalle, conexion))
                        {
                            comandoDetalleOrden.Parameters.AddWithValue("@UsuarioId", usuarioId);
                            comandoDetalleOrden.Parameters.AddWithValue("@MesaId", mesaId);
                            comandoDetalleOrden.Parameters.AddWithValue("@MenuId", menuplatillos.MenuId);
                            comandoDetalleOrden.Parameters.AddWithValue("@Cantidad", menuplatillos.Cantidad);
                            comandoDetalleOrden.Parameters.AddWithValue("@PrecioUnitario", menuplatillos.PrecioUnitario);
                            comandoDetalleOrden.Parameters.AddWithValue("@PrecioTotal", menuplatillos.PrecioTotal);
                            comandoDetalleOrden.Parameters.AddWithValue("@NombreComida", menuplatillos.NombreComida);

                            comandoDetalleOrden.ExecuteNonQuery();
                        }
                    }
                }

                conexion.Close();
            }

            return RedirectToAction("Mesa");
        }





        //Funcion de salir del restaurante
        [HttpPost]
        public ActionResult LiberarMesa()
        {
            var usuarioId = Session["UsuarioId"] as int?;
            if (usuarioId == null) return Json(new { success = false });

            using (SqlConnection conexion = new SqlConnection(connectionString))
            {
                conexion.Open();

                // Obtener mesa actual del usuario
                int mesaId = 0;
                string comandoMesa = "SELECT MesaId FROM Usuarios WHERE Id = @UsuarioId";
                using (SqlCommand comandoMesaActual = new SqlCommand(comandoMesa, conexion))
                {

                    //Si se obtiene mesa obtener la identificacion
                    comandoMesaActual.Parameters.AddWithValue("@UsuarioId", usuarioId);
                    var resultadoComando = comandoMesaActual.ExecuteScalar();
                    if (resultadoComando != null && resultadoComando != DBNull.Value)
                    {
                        mesaId = (int)resultadoComando;
                    }
                }




                if (mesaId > 0)
                {
                    // Liberar al usuario de la mesa
                    string comandoBorrarMesaUsuario = "UPDATE Usuarios SET MesaId = NULL WHERE Id = @UsuarioId";
                    using (SqlCommand comandoBorrar = new SqlCommand(comandoBorrarMesaUsuario, conexion))
                    {
                        comandoBorrar.Parameters.AddWithValue("@UsuarioId", usuarioId);
                        comandoBorrar.ExecuteNonQuery();
                    }




                    // Actualizar estado de la mesa si se queda sin personas sentadas
                    string comandoContarPersonasMesa = "SELECT COUNT(*) FROM Usuarios WHERE MesaId = @MesaId";
                    int ocupantes = 0;
                    using (SqlCommand comandoPersonasMesa = new SqlCommand(comandoContarPersonasMesa, conexion))
                    {
                        comandoPersonasMesa.Parameters.AddWithValue("@MesaId", mesaId);
                        ocupantes = (int)comandoPersonasMesa.ExecuteScalar();
                    }

                    if (ocupantes == 0)
                    {
                        string comandoActualizarMesa = "UPDATE Mesas SET Estado = 'Libre' WHERE Id = @MesaId";
                        using (SqlCommand comandoActualizar = new SqlCommand(comandoActualizarMesa, conexion))
                        {
                            comandoActualizar.Parameters.AddWithValue("@MesaId", mesaId);
                            comandoActualizar.ExecuteNonQuery();
                        }
                    }
                }

                // Eliminar la orden actual si existe por id
                string comandoBorrarOrdenUsuario = "DELETE FROM Ordenes WHERE UsuarioId = @UsuarioId";
                using (SqlCommand comandoBorrarOrden = new SqlCommand(comandoBorrarOrdenUsuario, conexion))
                {
                    comandoBorrarOrden.Parameters.AddWithValue("@UsuarioId", usuarioId);
                    comandoBorrarOrden.ExecuteNonQuery();
                }

                conexion.Close();
            }

            // Mantener la sesión pero limpiar mesa y orden
            return Json(new { success = true });
        }








        //Funciones de la ventana del menu
        //mostrar platillos y llamar info cliente
        public ActionResult Menu()
        {
            var usuarioId = Session["UsuarioId"] as int?;
            if (usuarioId == null) return RedirectToAction("Registro");

            var menu = new List<Platillo>();
            int mesaId = 0;

            using (SqlConnection conexion = new SqlConnection(connectionString))
            {
                conexion.Open();

                // Obtener el menú
                string comandoInfoMenu = "SELECT Id, NombreComida, Precio, Tipo, Estado, ImagenUrl FROM Menu WHERE Estado = 'Disponible'";
                using (SqlCommand comandoMenu = new SqlCommand(comandoInfoMenu, conexion))
                {
                    using (SqlDataReader lector = comandoMenu.ExecuteReader())
                    {
                        while (lector.Read())
                        {
                            menu.Add(new Platillo
                            {
                                Id = (int)lector["Id"],
                                NombreComida = lector["NombreComida"].ToString(),
                                Precio = (decimal)lector["Precio"],
                                Tipo = lector["Tipo"].ToString(),
                                Estado = lector["Estado"].ToString(),
                                ImagenUrl = lector["ImagenUrl"].ToString()
                            });
                        }
                    }
                }






                // Obtener la mesa del usuario para asociar la orden
                string comandoMesaUsuario = "SELECT MesaId FROM Usuarios WHERE Id = @UsuarioId";
                using (SqlCommand comandoMesa = new SqlCommand(comandoMesaUsuario, conexion))
                {
                    comandoMesa.Parameters.AddWithValue("@UsuarioId", usuarioId.Value);
                    var resultado = comandoMesa.ExecuteScalar();
                    if (resultado != DBNull.Value)
                        mesaId = (int)resultado;
                }

                conexion.Close();
            }

            var model = new MesaViewModel
            {
                Menu = menu,
                MesaId = mesaId,
                Pedidos = new List<PedidoDetalle>()
            };

            return View(model);
        }









        //Funciones del menu
        //realizar orden, crear codigo orden
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Menu(MesaViewModel modelo)
        {
            if (Session["UsuarioId"] == null)
            {
                TempData["Error"] = "Sesión expirada. Por favor regístrese nuevamente.";
                return RedirectToAction("Login");
            }

            int usuarioId = (int)Session["UsuarioId"];
            int mesaId = modelo.MesaId;

            using (SqlConnection conexion = new SqlConnection(connectionString))
            {
                conexion.Open();

                // Validar mesa asignada
                if (mesaId <= 0)
                {
                    string comandoMesaInfo = "SELECT MesaId FROM Usuarios WHERE Id = @UsuarioId";
                    using (SqlCommand comandoMesa = new SqlCommand(comandoMesaInfo, conexion))
                    {
                        comandoMesa.Parameters.AddWithValue("@UsuarioId", usuarioId);
                        var resultado = comandoMesa.ExecuteScalar();
                        if (resultado == null || resultado == DBNull.Value)
                        {
                            TempData["Error"] = "No se encontró una mesa asignada para este usuario.";
                            return RedirectToAction("Mesa");
                        }
                        mesaId = (int)resultado;
                    }
                }

                // Validar que tenemos pedidos
                if (modelo.Pedidos == null || !modelo.Pedidos.Any())
                {
                    return RedirectToAction("Menu");
                }



                // Insertar cada pedido en una fila de la tabla
                using (var transaccion = conexion.BeginTransaction())
                {
                    try
                    {
                        int codigoDeOrden = new Random().Next(100000, 999999);

                        foreach (var item in modelo.Pedidos)
                        {
                            string comandoInsertarOrden = @"INSERT INTO Ordenes(GrupoOrdenId, UsuarioId, MesaId, MenuId, NombreComida, Cantidad, PrecioUnitario, PrecioTotal, Estado, Fecha)
                                                 VALUES(@GrupoOrdenId, @UsuarioId, @MesaId, @MenuId, @NombreComida, @Cantidad, @PrecioUnitario, @PrecioTotal, 'Pendiente', GETDATE());";

                            using (SqlCommand comandoOrden = new SqlCommand(comandoInsertarOrden, conexion, transaccion))
                            {
                                comandoOrden.Parameters.AddWithValue("@GrupoOrdenId", codigoDeOrden);
                                comandoOrden.Parameters.AddWithValue("@UsuarioId", usuarioId);
                                comandoOrden.Parameters.AddWithValue("@MesaId", mesaId);
                                comandoOrden.Parameters.AddWithValue("@MenuId", item.MenuId);
                                comandoOrden.Parameters.AddWithValue("@NombreComida", item.NombreComida);
                                comandoOrden.Parameters.AddWithValue("@Cantidad", item.Cantidad);
                                comandoOrden.Parameters.AddWithValue("@PrecioUnitario", item.PrecioUnitario);
                                comandoOrden.Parameters.AddWithValue("@PrecioTotal", item.PrecioTotal);

                                comandoOrden.ExecuteNonQuery();
                            }

                            string comandoAgregarTablaPagos = @"INSERT INTO Pagos (GrupoOrdenId, UsuarioId, MesaId, NomComida, Cantidad, PrecioUnitario, IVA, Subtotal, Propina, Estado, Fecha)
                                                 VALUES (@GrupoOrdenId, @UsuarioId, @MesaId, @NomComida, @Cantidad, @PrecioUnitario, @IVA, @Subtotal, @Propina, 'Por Pagar', GETDATE());";

                            using (SqlCommand comandoTablaPagos = new SqlCommand(comandoAgregarTablaPagos, conexion, transaccion))
                            {
                                comandoTablaPagos.Parameters.AddWithValue("@GrupoOrdenId", codigoDeOrden);
                                comandoTablaPagos.Parameters.AddWithValue("@UsuarioId", usuarioId);
                                comandoTablaPagos.Parameters.AddWithValue("@MesaId", mesaId);
                                comandoTablaPagos.Parameters.AddWithValue("@NomComida", item.NombreComida);
                                comandoTablaPagos.Parameters.AddWithValue("@Cantidad", item.Cantidad);
                                comandoTablaPagos.Parameters.AddWithValue("@PrecioUnitario", item.PrecioUnitario);
                                comandoTablaPagos.Parameters.AddWithValue("@IVA", 13);
                                comandoTablaPagos.Parameters.AddWithValue("@Subtotal", item.PrecioTotal);
                                comandoTablaPagos.Parameters.AddWithValue("@Propina", 0);

                                comandoTablaPagos.ExecuteNonQuery();
                            }
                        }

                        transaccion.Commit();
                        TempData["Success"] = "¡Pedido realizado con éxito!";
                    }
                    catch (Exception ex)
                    {
                        transaccion.Rollback();
                        TempData["Error"] = $"Error al procesar el pedido: {ex.Message}";
                        return RedirectToAction("Menu");
                    }



                }
            }

            return RedirectToAction("Mesa");
        }














        [HttpGet]
        public JsonResult ObtenerOrdenesPorMesa(int mesaId)
        {
            try
            {
                // Validar parámetro de entrada
                if (mesaId <= 0)
                {
                    return Json(new { success = false, message = "ID de mesa inválido" });
                }

                using (var conn = new SqlConnection(connectionString))
                {
                    var pedidos = new List<object>();
                    conn.Open();

                    // Query optimizada para incluir solo estados relevantes
                    string query = @"SELECT o.Id, o.NombreComida, o.Cantidad, 
                           o.PrecioTotal, o.Estado, o.MeseroId
                           FROM Ordenes o
                           WHERE o.MesaId = @MesaId 
                           AND o.Estado IN ('Pendiente', 'Preparando', 'Platillo Listo')
                           ORDER BY o.Fecha DESC";

                    using (var cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@MesaId", mesaId);

                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                pedidos.Add(new
                                {
                                    Id = Convert.ToInt32(reader["Id"]),
                                    NombreComida = reader["NombreComida"].ToString(),
                                    Cantidad = Convert.ToInt32(reader["Cantidad"]),
                                    PrecioTotal = Convert.ToDecimal(reader["PrecioTotal"]),
                                    Estado = reader["Estado"].ToString(),
                                    MeseroId = reader["MeseroId"] != DBNull.Value ?
                                              Convert.ToInt32(reader["MeseroId"]) : (int?)null
                                });
                            }
                        }
                    }

                    return Json(new
                    {
                        success = true,
                        data = pedidos.Count > 0 ? pedidos : null
                    }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                // Log del error real
                System.Diagnostics.Debug.WriteLine($"Error en ObtenerOrdenesPorMesa: {ex}");

                return Json(new
                {
                    success = false,
                    message = "Error al obtener órdenes",
                    error = ex.Message
                }, JsonRequestBehavior.AllowGet);
            }
        }











        //Funciones de la ventana pagos
        //calcular montos de la pagina
        public ActionResult Pagos()
        {
            var usuarioId = ObtenerUsuarioIdDeSesion();
            var nombreUsuario = ObtenerNombreUsuarioDeSesion();

            var modelo = new PagosViewModel
            {
                UsuarioId = usuarioId,
                NombreUsuario = nombreUsuario,
                Tarjetas = ObtenerTarjetasDelUsuario(usuarioId, nombreUsuario),
                ItemsOrden = ObtenerItemsOrden(usuarioId),
            };

            // Calcular subtotal, iva, propina y total
            modelo.Subtotal = CalcularSubtotal(modelo.ItemsOrden);
            modelo.IVA = modelo.Subtotal * 0.13m;
            modelo.Propina = 0;
            modelo.Total = modelo.Subtotal + modelo.IVA + modelo.Propina;

            return View(modelo);
        }

        //obtener tarjetas para mostrar en pagos
        private List<TarjetaViewModel> ObtenerTarjetasDelUsuario(int usuarioId, string nombreTitular)
        {
            var tarjetas = new List<TarjetaViewModel>();

            using (var conexion = new SqlConnection(connectionString))
            {
                conexion.Open();

                string comandollamarTarjeta = @"SELECT Id, TipoTarjeta, NumTarjeta, FechaVenc FROM Tarjetas WHERE UsuarioId = @UsuarioId";

                using (var comandoTarjeta = new SqlCommand(comandollamarTarjeta, conexion))
                {
                    comandoTarjeta.Parameters.AddWithValue("@UsuarioId", usuarioId);

                    using (var lector = comandoTarjeta.ExecuteReader())
                    {
                        while (lector.Read())
                        {
                            var numeroCompleto = lector["NumTarjeta"].ToString();
                            var enmascarado = EnmascararNumeroTarjeta(numeroCompleto);

                            tarjetas.Add(new TarjetaViewModel
                            {
                                Id = Convert.ToInt32(lector["Id"]),
                                TipoTarjeta = lector["TipoTarjeta"].ToString(),
                                NumeroTarjeta = numeroCompleto,
                                NumeroTarjetaEnmascarado = enmascarado,
                                FechaVencimiento = Convert.ToDateTime(lector["FechaVenc"]).ToString("MM/yyyy"),
                                NombreTitular = nombreTitular
                            });
                        }
                    }
                }
            }

            return tarjetas;
        }

        //llamar los elementos de la tabla para ordenar en pagos
        public List<ItemOrden> ObtenerItemsOrden(int usuarioId)
        {
            var items = new List<ItemOrden>();
            string comandollamarOrdenPorPagar = @"SELECT Id AS GrupoOrdenId, NomComida AS NombreComida, Cantidad, PrecioUnitario
                            FROM Pagos WHERE UsuarioId = @UsuarioId AND Estado = 'Por Pagar'";

            using (var connection = new SqlConnection(connectionString))
            using (var comandoOrdenPorPagar = new SqlCommand(comandollamarOrdenPorPagar, connection))
            {
                comandoOrdenPorPagar.Parameters.AddWithValue("@UsuarioId", usuarioId);
                connection.Open();

                using (var lector = comandoOrdenPorPagar.ExecuteReader())
                {
                    while (lector.Read())
                    {
                        items.Add(new ItemOrden
                        {
                            OrdenId = lector.GetInt32(lector.GetOrdinal("GrupoOrdenId")),
                            NombreComida = lector.GetString(lector.GetOrdinal("NombreComida")),
                            Cantidad = lector.GetInt32(lector.GetOrdinal("Cantidad")),
                            PrecioUnitario = lector.GetDecimal(lector.GetOrdinal("PrecioUnitario"))
                        });
                    }
                }
            }

            return items;
        }

        private decimal CalcularSubtotal(List<ItemOrden> items)
        {
            decimal subtotal = 0;
            foreach (var item in items)
            {
                subtotal += item.Subtotal;
            }
            return subtotal;
        }




        //Para obtener las credenciales del usuario tanto registro como login
        private int ObtenerUsuarioIdDeSesion()
        {
            if (Session["UsuarioId"] != null)
                return (int)Session["UsuarioId"];

            else
                throw new Exception("Usuario no autenticado");
        }



        private string ObtenerNombreUsuarioDeSesion()
        {
            if (Session["NombreUsuario"] != null)
                return Session["NombreUsuario"].ToString();

            if (Session["Nombre"] != null)
                return Session["Nombre"].ToString();

            return "Anonimo";
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ProcesarPago(PagosViewModel modelo)
        {
            if (!ModelState.IsValid)
            {
                modelo.NombreUsuario = ObtenerNombreUsuarioDeSesion();
                modelo.Tarjetas = ObtenerTarjetasDelUsuario(modelo.UsuarioId, modelo.NombreUsuario);
                modelo.ItemsOrden = ObtenerItemsOrden(modelo.UsuarioId);
                return View("Pagos", modelo);
            }

            using (SqlConnection conexion = new SqlConnection(connectionString))


            {
                conexion.Open();


                using (var accion = conexion.BeginTransaction())
                {

                    // eliminar las ordenes entregadas al cliente
                    string eliminarOrdenesEntregadas = @"delete from Ordenes where UsuarioId = @UsuarioId and Estado = 'Entregado'";


                    using (var comando = new SqlCommand(eliminarOrdenesEntregadas, conexion, accion))
                    {
                        comando.Parameters.AddWithValue("@UsuarioId", modelo.UsuarioId);
                        comando.ExecuteNonQuery();
                    }

                    // asignar al platillo como pagado
                    string actualizarPagos = @" UPDATE Pagos SET Estado = 'Pagado' WHERE UsuarioId = @UsuarioId AND GrupoOrdenId IN ( SELECT DISTINCT GrupoOrdenId FROM Pagos
                                                 WHERE UsuarioId = @UsuarioId AND Estado = 'Por Pagar' )";
                    using (var comando2 = new SqlCommand(actualizarPagos, conexion, accion))
                    {
                        comando2.Parameters.AddWithValue("@UsuarioId", modelo.UsuarioId);
                        comando2.ExecuteNonQuery();
                    }

                    accion.Commit();

                }



                // sacar al cliente de la mesa despues de pagar


                // Quitar la mesa al usuario
                string actualizarMesaUsuario = "UPDATE Usuarios SET MesaId = NULL WHERE Id = @UsuarioId";
                using (SqlCommand comandoQuitarMesa = new SqlCommand(actualizarMesaUsuario, conexion))
                {
                    comandoQuitarMesa.Parameters.AddWithValue("@UsuarioId", modelo.UsuarioId);
                    comandoQuitarMesa.ExecuteNonQuery();
                }

                // Ver si hay otros clientes
                string contarOcupantes = "SELECT COUNT(*) FROM Usuarios WHERE MesaId = @MesaId";
                int ocupantes = 0;
                using (SqlCommand comandoContarPersonas = new SqlCommand(contarOcupantes, conexion))
                {
                    comandoContarPersonas.Parameters.AddWithValue("@MesaId", modelo.MesaId);
                    ocupantes = (int)comandoContarPersonas.ExecuteScalar();
                }

                // Liberar mesa si ya esta vacia
                if (ocupantes == 0)
                {
                    string actualizarMesa = "UPDATE Mesas SET Estado = 'Libre' WHERE Id = @MesaId";
                    using (SqlCommand comandoactualizarmesa = new SqlCommand(actualizarMesa, conexion))
                    {
                        comandoactualizarmesa.Parameters.AddWithValue("@MesaId", modelo.MesaId);
                        comandoactualizarmesa.ExecuteNonQuery();
                    }
                }



                conexion.Close();



            }

            return RedirectToAction("Index");
        }

        private string EnmascararNumeroTarjeta(string tarjetaNumero)
        {
            if (string.IsNullOrEmpty(tarjetaNumero) || tarjetaNumero.Length < 4)
                return tarjetaNumero;

            return new string('*', tarjetaNumero.Length - 4) + tarjetaNumero.Substring(tarjetaNumero.Length - 4);
        }

        //una vez completado los espacios de la reseña se hace un insert a la tabla reseñas
        [HttpPost]
        public ActionResult EnviarResena(ResenaRequest request)
        {
            string NombreUsuario = ObtenerNombreUsuarioDeSesion();
            try
            {
                using (SqlConnection conexion = new SqlConnection(connectionString))
                {
                    conexion.Open();
                    string comandoCrearResena = @"INSERT INTO Resenas (UsuarioId, NombreUsuario, Calificacion, ResenaTexto, Fecha)
                                                VALUES (@UsuarioId, @NombreUsuario, @Calificacion, @ResenaTexto, GETDATE())";

                    using (SqlCommand contenidoResena = new SqlCommand(comandoCrearResena, conexion))
                    {
                        contenidoResena.Parameters.AddWithValue("@UsuarioId", request.UsuarioId);
                        contenidoResena.Parameters.AddWithValue("@NombreUsuario", NombreUsuario);
                        contenidoResena.Parameters.AddWithValue("@Calificacion", request.Calificacion);
                        contenidoResena.Parameters.AddWithValue("@ResenaTexto", string.IsNullOrEmpty(request.Comentario) ? DBNull.Value : (object)request.Comentario);

                        contenidoResena.ExecuteNonQuery();
                    }
                }
                TempData["Mensaje"] = "¡Gracias por tu reseña!";
                return RedirectToAction("Pagos");
            }
            catch (Exception)
            {
                TempData["Error"] = "Ocurrió un error al guardar la reseña.";
                return RedirectToAction("Pagos");
            }
        }


        //parte Login y Autenticacion

        [HttpPost]
        public ActionResult Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();


                //Tomar datos de la tabla de clientes
                string queryUsuario = @"SELECT Id, Nombre, Rol, Correo FROM Usuarios WHERE Correo = @Correo AND Contrasena = @Contrasena";
                using (SqlCommand cmdUsuario = new SqlCommand(queryUsuario, conn))
                {
                    cmdUsuario.Parameters.AddWithValue("@Correo", model.Correo);
                    cmdUsuario.Parameters.AddWithValue("@Contrasena", model.Contrasena);

                    using (SqlDataReader reader = cmdUsuario.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            Session["UsuarioId"] = reader["Id"];
                            Session["Nombre"] = reader["Nombre"];
                            Session["Rol"] = reader["Rol"];
                            Session["Correo"] = reader["Correo"];

                            string codigo = GenerarCodigo();
                            Session["CodigoVerificacion"] = codigo;
                            EnviarCodigoPorCorreo(model.Correo, codigo);

                            return RedirectToAction("Autenticacion");
                        }
                    }
                }


                //Tomar datos de la tabla de Empleados
                string queryEmpleado = @"SELECT Id, Nombre, Rol, Correo FROM Empleados WHERE Correo = @Correo AND Contrasena = @Contrasena";
                using (SqlCommand cmdEmpleado = new SqlCommand(queryEmpleado, conn))
                {
                    cmdEmpleado.Parameters.AddWithValue("@Correo", model.Correo);
                    cmdEmpleado.Parameters.AddWithValue("@Contrasena", model.Contrasena);

                    using (SqlDataReader reader = cmdEmpleado.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            Session["EmpleadoId"] = reader["Id"];
                            Session["Nombre"] = reader["Nombre"];
                            Session["Rol"] = reader["Rol"];
                            Session["Correo"] = reader["Correo"];

                            string codigo = GenerarCodigo();
                            Session["CodigoVerificacion"] = codigo;
                            EnviarCodigoPorCorreo(model.Correo, codigo);

                            return RedirectToAction("Autenticacion");
                        }
                    }
                }
            }

            ViewBag.Error = "Correo o contraseña incorrectos.";
            return View(model);
        }

        //metodo de Autenticacion
        [HttpPost]
        public ActionResult ValidarCodigo(string codigoIngresado)
        {
            string codigoGenerado = Session["CodigoVerificacion"]?.ToString();

            if (codigoIngresado == codigoGenerado)
            {
                string rol = Session["Rol"]?.ToString();

                switch (rol)
                {
                    case "Chef":
                        return RedirectToAction("Cocina", "Home");
                    case "Mesero":
                        return RedirectToAction("Mesero", "Home");
                    case "Gerente":
                        return RedirectToAction("VistaGeneral", "Home");
                    case "Cliente":
                        return RedirectToAction("filaespera", "Home");
                    default:
                        return RedirectToAction("Index", "Home");
                }
            }

            TempData["Error"] = "Código incorrecto. Intente nuevamente.";
            return RedirectToAction("Autenticacion");
        }

        private string GenerarCodigo()
        {
            const string caracteres = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789!@#$%?¿&*";
            var random = new Random();
            return new string(Enumerable.Repeat(caracteres, 6)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        private void EnviarCodigoPorCorreo(string correoDestino, string codigo)
        {
            ServicePointManager.ServerCertificateValidationCallback =
                delegate (object s, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
                {
                    return true;
                };

            var mensaje = new MailMessage();
            mensaje.From = new MailAddress("gustosazon8@gmail.com", "Gusto y Sazón");
            mensaje.To.Add(correoDestino);
            mensaje.Subject = "Código de Verificación";
            mensaje.Body = $"Tu código de verificación es: {codigo}";
            mensaje.IsBodyHtml = false;

            var smtp = new SmtpClient("smtp.gmail.com")
            {
                Port = 587,
                Credentials = new NetworkCredential("gustosazon8@gmail.com", "asiv yjjj ixtj nqch"),
                EnableSsl = true
            };

            smtp.Send(mensaje);
        }




        //Parte del mesero

        public ActionResult Mesero()
        {
            if (Session["EmpleadoId"] == null)
                return RedirectToAction("Login");

            int empleadoId = Convert.ToInt32(Session["EmpleadoId"]);
            var model = new MeseroMesaViewModel
            {
                Id = empleadoId,
                Nombre = Session["Nombre"]?.ToString(),
                Correo = Session["Correo"]?.ToString(),
                Rol = Session["Rol"]?.ToString(),
                Turno = "Diurno",
                HoraInicio = new TimeSpan(14, 30, 0),
                HoraFin = new TimeSpan(22, 30, 0),
                NivelEstres = 0m,
                NivelEnergia = 100m,
                Eficiencia = 100m
            };

            using (var conn = new SqlConnection(connectionString))
            {
                conn.Open();

                // 1. Obtener información del mesero
                string queryMesero = @"SELECT TOP 1 Turno, PropinasTotales
                             FROM MeseroActividad 
                             WHERE EmpleadoId = @EmpleadoId 
                             ORDER BY Id DESC";

                using (var cmd = new SqlCommand(queryMesero, conn))
                {
                    cmd.Parameters.AddWithValue("@EmpleadoId", empleadoId);
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            model.Turno = reader["Turno"]?.ToString() ?? model.Turno;
                            model.PropinasTurno = reader["PropinasTotales"] != DBNull.Value ?
                                Convert.ToDecimal(reader["PropinasTotales"]) : 0m;
                        }
                    }
                }

                // 2. Obtener todas las mesas
                string queryMesas = @"SELECT m.Id AS MesaId, m.NumeroMesa, m.Sillas, m.Ubicacion, m.Estado,
                            (SELECT COUNT(*) FROM Usuarios u WHERE u.MesaId = m.Id) AS Ocupantes
                            FROM Mesas m
                            ORDER BY m.NumeroMesa";

                using (var cmd = new SqlCommand(queryMesas, conn))
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        model.Mesas.Add(new MesaInfo
                        {
                            MesaId = (int)reader["MesaId"],
                            NumeroMesa = (int)reader["NumeroMesa"],
                            Sillas = (int)reader["Sillas"],
                            Ubicacion = reader["Ubicacion"].ToString(),
                            EstadoAtencion = reader["Estado"].ToString(),
                            Ocupantes = (int)reader["Ocupantes"]
                        });
                    }
                }


            }

            return View(model);
        }

        private void CargarDetallesMesa(MesaInfo mesa, SqlConnection conexion)
        {
            // Implementación para cargar pedidos y clientes de la mesa
            // Similar a la que tenías anteriormente
        }








        //Steven
        [HttpPost]
        public JsonResult ActualizarEstadoPedido(int ordenId, string nuevoEstado)
        {
            try
            {
                using (var conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    string query = "UPDATE Ordenes SET Estado = @NuevoEstado WHERE Id = @OrdenId";

                    using (var cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@NuevoEstado", nuevoEstado);
                        cmd.Parameters.AddWithValue("@OrdenId", ordenId);

                        int affectedRows = cmd.ExecuteNonQuery();

                        return Json(new
                        {
                            success = affectedRows > 0,
                            message = affectedRows > 0 ? "Estado actualizado" : "No se encontró la orden"
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    message = ex.Message
                });
            }
        }





        [HttpPost]
        public JsonResult ActualizarEstadoOrden(int ordenId, string nuevoEstado, int meseroId)
        {
            try
            {
                // Validar estados permitidos
                var estadosPermitidos = new[] { "Preparando", "Entregado" };
                if (!estadosPermitidos.Contains(nuevoEstado))
                {
                    return Json(new { success = false, message = "Estado no válido" });
                }

                using (var conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    // Verificar estado actual
                    string queryVerificar = "SELECT Estado FROM Ordenes WHERE Id = @OrdenId";
                    string estadoActual;

                    using (var cmd = new SqlCommand(queryVerificar, conn))
                    {
                        cmd.Parameters.AddWithValue("@OrdenId", ordenId);
                        estadoActual = cmd.ExecuteScalar()?.ToString();
                    }

                    // Validar transición de estados
                    if (nuevoEstado == "Preparando" && estadoActual != "Pendiente")
                    {
                        return Json(new
                        {
                            success = false,
                            message = "Solo se puede 'Tomar Orden' cuando el estado es 'Pendiente'"
                        });
                    }

                    if (nuevoEstado == "Entregado" && estadoActual != "Platillo Listo")
                    {
                        return Json(new
                        {
                            success = false,
                            message = "Solo se puede 'Entregar Orden' cuando el estado es 'Platillo Listo'"
                        });
                    }

                    // Actualizar el estado
                    string queryActualizar = @"UPDATE Ordenes 
                                     SET Estado = @NuevoEstado, 
                                         MeseroId = @MeseroId,
                                         Fecha = GETDATE()
                                     WHERE Id = @OrdenId";

                    using (var cmd = new SqlCommand(queryActualizar, conn))
                    {
                        cmd.Parameters.AddWithValue("@NuevoEstado", nuevoEstado);
                        cmd.Parameters.AddWithValue("@OrdenId", ordenId);
                        cmd.Parameters.AddWithValue("@MeseroId", meseroId);

                        int affectedRows = cmd.ExecuteNonQuery();

                        return Json(new
                        {
                            success = affectedRows > 0,
                            message = affectedRows > 0 ? "Estado actualizado" : "No se encontró la orden"
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    message = ex.Message
                });
            }
        }

        //Fin Steven




    }
}