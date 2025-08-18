using GustoySazon.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Drawing.Printing;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Mail;
using System.Net.NetworkInformation;
using System.Net.Security;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using static GustoySazon.Models.MeseroViewModel;







namespace GustoySazon.Controllers



{
    public class HomeController : Controller
    {

        private string connectionString = ConfigurationManager.ConnectionStrings["GustoySazonDB"].ConnectionString;



        public ActionResult Index()
        {
            return View();
        }
        

       


        public ActionResult Clientes()
        {
            return View();
        }

        public ActionResult ControlPersonal()
        {
            return View();
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
                MesasDisponibles = new List<MesaDisponible>(),
                PosicionEnFila = 0 // Inicializar posición
            };

            using (SqlConnection conexion = new SqlConnection(connectionString))
            {
                conexion.Open();

                int usuarioId = 0;
                string comandoUsuariosActuales;

                if (Session["UsuarioId"] != null)
                {
                    usuarioId = (int)Session["UsuarioId"];
                    comandoUsuariosActuales = "SELECT Id, Nombre, HoraRegistro FROM Usuarios WHERE Id = @UsuarioId"; // Agregado HoraRegistro
                }
                else
                {
                    comandoUsuariosActuales = "SELECT TOP 1 Id, Nombre, HoraRegistro FROM Usuarios ORDER BY HoraRegistro DESC"; // Agregado HoraRegistro
                }

                // Obtener usuario actual
                using (SqlCommand comandoUsuario = new SqlCommand(comandoUsuariosActuales, conexion))
                {
                    if (Session["UsuarioId"] != null)
                    {
                        comandoUsuario.Parameters.AddWithValue("@UsuarioId", usuarioId);
                    }

                    using (SqlDataReader lector = comandoUsuario.ExecuteReader())
                    {
                        if (lector.Read())
                        {
                            usuarioId = (int)lector["Id"];
                            modelo.NombreUsuarioActual = lector["Nombre"].ToString();
                            modelo.HoraRegistroUsuario = (DateTime)lector["HoraRegistro"]; // Asignar hora de registro
                        }
                        else
                        {
                            conexion.Close();
                            return RedirectToAction("index");
                        }
                    }
                }

                // Guardar sesion del usuario si es nuevo
                if (Session["UsuarioId"] == null)
                {
                    Session["UsuarioId"] = usuarioId;
                }

                modelo.UsuarioIdActual = usuarioId;

                // Calcular posición en fila
                string comandoPosicion = @"
            SELECT COUNT(*) + 1 AS Posicion
            FROM Usuarios
            WHERE MesaId IS NULL AND HoraRegistro < (SELECT HoraRegistro FROM Usuarios WHERE Id = @UsuarioId)";

                using (SqlCommand comandoPosicionFila = new SqlCommand(comandoPosicion, conexion))
                {
                    comandoPosicionFila.Parameters.AddWithValue("@UsuarioId", usuarioId);
                    modelo.PosicionEnFila = (int)comandoPosicionFila.ExecuteScalar();
                }

                // Obtener TODAS las mesas disponibles
                string comandoMesas = @"SELECT m.Id, m.NumeroMesa, m.Sillas, m.Ubicacion, m.Estado, 
                           (SELECT COUNT(*) FROM Usuarios u WHERE u.MesaId = m.Id) AS Ocupantes
                           FROM Mesas m";

                using (SqlCommand comandoMesasDisponibles = new SqlCommand(comandoMesas, conexion))
                using (SqlDataReader lector = comandoMesasDisponibles.ExecuteReader())
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
                            Ocupantes = (int)lector["Ocupantes"]
                        });
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
        public JsonResult ObtenerEstadoMesa()
        {
            var usuarioId = Session["UsuarioId"] as int?;
            if (usuarioId == null)
                return Json(new { success = false }, JsonRequestBehavior.AllowGet);

            int mesaId = 0;
            string estadoAtencionUsuario = "Desconocido";
            var pedidos = new List<object>();
            var clientesEnMesa = new List<object>();

            using (var conexion = new SqlConnection(connectionString))
            {
                conexion.Open();

                //obtener las ordener por fecha y estado
                string comandoEstadoOrdenes = @"SELECT TOP 1 m.Id AS MesaId, o.Estado FROM Usuarios u JOIN Mesas m ON u.MesaId = m.Id
                                     LEFT JOIN Ordenes o ON o.UsuarioId = u.Id WHERE u.Id = @UsuarioId ORDER BY o.Fecha DESC";

                using (var comandoOrdenPorID = new SqlCommand(comandoEstadoOrdenes, conexion))
                {
                    comandoOrdenPorID.Parameters.AddWithValue("@UsuarioId", usuarioId.Value);
                    using (var lector = comandoOrdenPorID.ExecuteReader())
                    {
                        if (lector.Read())
                        {
                            mesaId = (int)lector["MesaId"];
                            estadoAtencionUsuario = lector["Estado"] != DBNull.Value ? lector["Estado"].ToString() : "Desconocido";
                        }
                    }
                }





                // Obtener pedidos actuales para el usuario
                string comandoPedidosPorUsuario = @"SELECT NombreComida, Cantidad, PrecioUnitario, Estado FROM Ordenes WHERE UsuarioId = @UsuarioId";

                using (var comandoPedidosPorID = new SqlCommand(comandoPedidosPorUsuario, conexion))
                {
                    comandoPedidosPorID.Parameters.AddWithValue("@UsuarioId", usuarioId.Value);
                    using (var lector = comandoPedidosPorID.ExecuteReader())
                    {
                        while (lector.Read())
                        {
                            pedidos.Add(new
                            {
                                NombreComida = lector["NombreComida"].ToString(),
                                Cantidad = (int)lector["Cantidad"],
                                PrecioUnitario = lector["PrecioUnitario"] != DBNull.Value ? (decimal)lector["PrecioUnitario"] : 0m,
                                Estado = lector["Estado"].ToString()
                            });
                        }
                    }
                }



                // Obtener todos clientes en la mesa
                if (mesaId > 0)
                {
                    string comandoClientesEnMesa = @"SELECT u.Id, u.Nombre FROM Usuarios u WHERE u.MesaId = @MesaId";

                    var usuariosMesa = new List<(int Id, string Nombre)>();
                    using (var comandoClienteSentados = new SqlCommand(comandoClientesEnMesa, conexion))
                    {

                        //se sacan los clientes en la mesa por id
                        comandoClienteSentados.Parameters.AddWithValue("@MesaId", mesaId);
                        using (var lector = comandoClienteSentados.ExecuteReader())
                        {
                            while (lector.Read())
                            {
                                usuariosMesa.Add(((int)lector["Id"], lector["Nombre"].ToString()));
                            }
                        }
                    }






                    // Para la lista de clientes en mesa determinar un estado
                    foreach (var usuario in usuariosMesa)


                    {
                        //si no ha pedido nada
                        string estadoCliente = "No ha ordenado aun";

                        string comandoEstadoPedidoOtros = @" SELECT TOP 1 Estado FROM Ordenes WHERE UsuarioId = @Uid ORDER BY Fecha DESC";

                        using (var comandoPedidoOtros = new SqlCommand(comandoEstadoPedidoOtros, conexion))
                        {
                            comandoPedidoOtros.Parameters.AddWithValue("@Uid", usuario.Id);
                            var estadoPedidoObj = comandoPedidoOtros.ExecuteScalar();
                            if (estadoPedidoObj != null)
                            {
                                var estadoPedido = estadoPedidoObj.ToString();



                                //Existen 2 estados para la vista de los otros clientes despues de pedir
                                if (estadoPedido == "Entregado")
                                {
                                    estadoCliente = "Por pagar";
                                }
                                else
                                {
                                    estadoCliente = "Pedido activo";
                                }
                            }
                        }

                        clientesEnMesa.Add(new
                        {
                            Id = usuario.Id,
                            Nombre = usuario.Nombre,
                            EstadoPedido = estadoCliente
                        });
                    }
                }

                conexion.Close();
            }


            //al final se envia todo en un json
            return Json(new
            {
                success = true,
                EstadoAtencion = estadoAtencionUsuario,
                Pedidos = pedidos,
                ClientesEnMesa = clientesEnMesa
            }, JsonRequestBehavior.AllowGet);
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
            if (usuarioId == null) return RedirectToAction("index");

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



















        //Funciones de la ventana pagos
        //calcular montos de la pagina
        public ActionResult Pagos(bool pagoGrupal = false)
        {
            var usuarioId = ObtenerUsuarioIdDeSesion();
            var nombreUsuario = ObtenerNombreUsuarioDeSesion();
            int mesaId = 0;

            // Obtener mesa del usuario
            using (var conexion = new SqlConnection(connectionString))
            {
                conexion.Open();
                string comandoMesa = "SELECT MesaId FROM Usuarios WHERE Id = @UsuarioId";
                using (var cmd = new SqlCommand(comandoMesa, conexion))
                {
                    cmd.Parameters.AddWithValue("@UsuarioId", usuarioId);
                    var result = cmd.ExecuteScalar();
                    if (result != DBNull.Value) mesaId = (int)result;
                }
            }

            var modelo = new PagosViewModel
            {
                UsuarioId = usuarioId,
                NombreUsuario = nombreUsuario,
                Tarjetas = ObtenerTarjetasDelUsuario(usuarioId, nombreUsuario),
                MesaId = mesaId,
                EsPagoGrupal = pagoGrupal
            };

            if (pagoGrupal)
            {
                // Obtener todos los items de la mesa
                modelo.ItemsMesa = ObtenerItemsMesa(mesaId);
                modelo.ItemsOrden = modelo.ItemsMesa; // Mostrar todos los items en la vista
            }
            else
            {
                // Obtener solo los items del usuario (comportamiento actual)
                modelo.ItemsOrden = ObtenerItemsOrden(usuarioId);
            }

            // Calcular montos
            modelo.Subtotal = CalcularSubtotal(modelo.ItemsOrden);
            modelo.IVA = modelo.Subtotal * 0.13m;
            modelo.Propina = 0;
            modelo.Total = modelo.Subtotal + modelo.IVA + modelo.Propina;

            // Obtener usuarios en la mesa para pago grupal
            if (pagoGrupal)
            {
                modelo.UsuariosMesa = ObtenerUsuariosEnMesa(mesaId);
            }

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





        private List<int> ObtenerUsuariosEnMesa(int mesaId)
        {
            var usuarios = new List<int>();
            using (var conexion = new SqlConnection(connectionString))
            {
                conexion.Open();
                string comando = "SELECT Id FROM Usuarios WHERE MesaId = @MesaId";
                using (var cmd = new SqlCommand(comando, conexion))
                {
                    cmd.Parameters.AddWithValue("@MesaId", mesaId);
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            usuarios.Add((int)reader["Id"]);
                        }
                    }
                }
            }
            return usuarios;
        }







        //llamar los elementos de la tabla para ordenar en pagos
        private List<ItemOrden> ObtenerItemsMesa(int mesaId)
        {
            var items = new List<ItemOrden>();
            string comando = @"SELECT p.Id AS GrupoOrdenId, p.NomComida AS NombreComida, 
                      p.Cantidad, p.PrecioUnitario, p.UsuarioId
                      FROM Pagos p
                      JOIN Usuarios u ON p.UsuarioId = u.Id
                      WHERE u.MesaId = @MesaId AND p.Estado = 'Por Pagar'";

            using (var connection = new SqlConnection(connectionString))
            using (var cmd = new SqlCommand(comando, connection))
            {
                cmd.Parameters.AddWithValue("@MesaId", mesaId);
                connection.Open();

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        items.Add(new ItemOrden
                        {
                            OrdenId = reader.GetInt32(reader.GetOrdinal("GrupoOrdenId")),
                            NombreComida = reader.GetString(reader.GetOrdinal("NombreComida")),
                            Cantidad = reader.GetInt32(reader.GetOrdinal("Cantidad")),
                            PrecioUnitario = reader.GetDecimal(reader.GetOrdinal("PrecioUnitario")),
                            UsuarioId = reader.GetInt32(reader.GetOrdinal("UsuarioId"))
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
                throw new Exception("Usuario no autenticado, Vaya al Login y Registrese");
                

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
                modelo.ItemsOrden = modelo.EsPagoGrupal ?
                    ObtenerItemsMesa(modelo.MesaId) : ObtenerItemsOrden(modelo.UsuarioId);
                return View("Pagos", modelo);
            }

            using (SqlConnection conexion = new SqlConnection(connectionString))
            {
                conexion.Open();

                using (var transaccion = conexion.BeginTransaction())
                {
                    try
                    {
                        if (modelo.EsPagoGrupal)
                        {
                            // 1. Obtener todos los GrupoOrdenId únicos de la mesa que están por pagar
                            var grupoOrdenIds = new List<int>();
                            string queryGrupoOrden = @"SELECT DISTINCT GrupoOrdenId FROM Pagos 
                                        WHERE MesaId = @MesaId AND Estado = 'Por Pagar'";

                            using (var cmd = new SqlCommand(queryGrupoOrden, conexion, transaccion))
                            {
                                cmd.Parameters.AddWithValue("@MesaId", modelo.MesaId);
                                using (var reader = cmd.ExecuteReader())
                                {
                                    while (reader.Read())
                                    {
                                        grupoOrdenIds.Add(Convert.ToInt32(reader["GrupoOrdenId"]));
                                    }
                                }
                            }

                            // 2. Marcar como pagado en la tabla Pagos para TODOS los usuarios de la mesa
                            string actualizarPagosMesa = @"UPDATE Pagos SET Estado = 'Pagado' 
                                        WHERE MesaId = @MesaId AND Estado = 'Por Pagar'";
                            using (var cmd = new SqlCommand(actualizarPagosMesa, conexion, transaccion))
                            {
                                cmd.Parameters.AddWithValue("@MesaId", modelo.MesaId);
                                int affectedRows = cmd.ExecuteNonQuery();

                                if (affectedRows == 0)
                                {
                                    throw new Exception("No se encontraron pagos pendientes para esta mesa");
                                }
                            }

                            // 3. Eliminar TODAS las órdenes con estado "Entregado" de la mesa completa
                            if (grupoOrdenIds.Any())
                            {
                                string eliminarOrdenesMesa = @"DELETE FROM Ordenes 
                                            WHERE GrupoOrdenId IN ({0}) AND Estado = 'Entregado'";

                                string parametros = string.Join(",", grupoOrdenIds.Select((_, i) => $"@GrupoOrdenId{i}"));
                                eliminarOrdenesMesa = string.Format(eliminarOrdenesMesa, parametros);

                                using (var cmd = new SqlCommand(eliminarOrdenesMesa, conexion, transaccion))
                                {
                                    for (int i = 0; i < grupoOrdenIds.Count; i++)
                                    {
                                        cmd.Parameters.AddWithValue($"@GrupoOrdenId{i}", grupoOrdenIds[i]);
                                    }
                                    int ordenesEliminadas = cmd.ExecuteNonQuery();
                                    System.Diagnostics.Debug.WriteLine($"Se eliminaron {ordenesEliminadas} órdenes entregadas");
                                }
                            }

                            // 4. Liberar solo al usuario que está pagando
                            string liberarUsuario = "UPDATE Usuarios SET MesaId = NULL WHERE Id = @UsuarioId";
                            using (var cmd = new SqlCommand(liberarUsuario, conexion, transaccion))
                            {
                                cmd.Parameters.AddWithValue("@UsuarioId", modelo.UsuarioId);
                                cmd.ExecuteNonQuery();
                            }
                        }
                        else
                        {
                            // Pago individual
                            // 1. Eliminar órdenes entregadas del usuario
                            string eliminarOrdenes = @"DELETE FROM Ordenes 
                                    WHERE UsuarioId = @UsuarioId AND Estado = 'Entregado'";
                            using (var cmd = new SqlCommand(eliminarOrdenes, conexion, transaccion))
                            {
                                cmd.Parameters.AddWithValue("@UsuarioId", modelo.UsuarioId);
                                cmd.ExecuteNonQuery();
                            }

                            // 2. Marcar como pagado en la tabla Pagos
                            string actualizarPagos = @"UPDATE Pagos SET Estado = 'Pagado' 
                                    WHERE UsuarioId = @UsuarioId AND Estado = 'Por Pagar'";
                            using (var cmd = new SqlCommand(actualizarPagos, conexion, transaccion))
                            {
                                cmd.Parameters.AddWithValue("@UsuarioId", modelo.UsuarioId);
                                cmd.ExecuteNonQuery();
                            }

                            // 3. Liberar al usuario
                            string liberarUsuario = "UPDATE Usuarios SET MesaId = NULL WHERE Id = @UsuarioId";
                            using (var cmd = new SqlCommand(liberarUsuario, conexion, transaccion))
                            {
                                cmd.Parameters.AddWithValue("@UsuarioId", modelo.UsuarioId);
                                cmd.ExecuteNonQuery();
                            }

                            // 4. Verificar si la mesa quedó vacía
                            string contarOcupantes = "SELECT COUNT(*) FROM Usuarios WHERE MesaId = @MesaId";
                            int ocupantes = 0;
                            using (var cmd = new SqlCommand(contarOcupantes, conexion, transaccion))
                            {
                                cmd.Parameters.AddWithValue("@MesaId", modelo.MesaId);
                                ocupantes = (int)cmd.ExecuteScalar();
                            }

                            if (ocupantes == 0)
                            {
                                string actualizarMesa = "UPDATE Mesas SET Estado = 'Libre' WHERE Id = @MesaId";
                                using (var cmd = new SqlCommand(actualizarMesa, conexion, transaccion))
                                {
                                    cmd.Parameters.AddWithValue("@MesaId", modelo.MesaId);
                                    cmd.ExecuteNonQuery();
                                }
                            }
                        }

                        // Registrar en finanzas
                        string comandoFinanzas = @"
                    IF EXISTS (SELECT 1 FROM finanzas WHERE DiaRegistrado = CAST(GETDATE() AS DATE))
                    BEGIN
                        UPDATE finanzas 
                        SET ganancias_totales = ganancias_totales + @Total,
                            propinas_totales = propinas_totales + @Propina
                        WHERE DiaRegistrado = CAST(GETDATE() AS DATE);
                    END
                    ELSE
                    BEGIN
                        INSERT INTO finanzas (ganancias_totales, propinas_totales)
                        VALUES (@Total, @Propina);
                    END";

                        using (var cmdFinanzas = new SqlCommand(comandoFinanzas, conexion, transaccion))
                        {
                            cmdFinanzas.Parameters.AddWithValue("@Total", modelo.Total);
                            cmdFinanzas.Parameters.AddWithValue("@Propina", modelo.Propina);
                            cmdFinanzas.ExecuteNonQuery();
                        }

                        // Obtener número de mesa para la factura
                        int numeroMesa = 0;
                        string queryNumeroMesa = "SELECT NumeroMesa FROM Mesas WHERE Id = @MesaId";
                        using (var cmdNumeroMesa = new SqlCommand(queryNumeroMesa, conexion, transaccion))
                        {
                            cmdNumeroMesa.Parameters.AddWithValue("@MesaId", modelo.MesaId);
                            numeroMesa = (int)cmdNumeroMesa.ExecuteScalar();
                        }
                        modelo.NumeroMesa = numeroMesa;

                        transaccion.Commit();
                        TempData["Success"] = "Pago procesado exitosamente";

                        // Enviar factura por correo
                        string correoCliente = Session["Correo"]?.ToString();
                        if (!string.IsNullOrEmpty(correoCliente))
                        {
                            try
                            {
                                EnviarFacturaPorCorreo(correoCliente, modelo, modelo.EsPagoGrupal);
                            }
                            catch (Exception ex)
                            {
                                System.Diagnostics.Debug.WriteLine($"Error al enviar factura: {ex.Message}");
                                // No hacemos rollback por un error en el envío del correo
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        transaccion.Rollback();
                        TempData["Error"] = $"Error al procesar el pago: {ex.Message}";
                        return RedirectToAction("Pagos", new { pagoGrupal = modelo.EsPagoGrupal });
                    }
                }
            }

            return RedirectToAction("Index");
        }








        private void EnviarFacturaPorCorreo(string correoDestino, PagosViewModel modelo, bool esPagoGrupal)
        {
            try
            {
                ServicePointManager.ServerCertificateValidationCallback =
                    delegate (object s, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
                    {
                        return true;
                    };

                var mensaje = new MailMessage();
                mensaje.From = new MailAddress("gustosazon8@gmail.com", "Gusto y Sazón");
                mensaje.To.Add(correoDestino);
                mensaje.Subject = $"Factura Electrónica - Gusto y Sazón - {DateTime.Now:dd/MM/yyyy}";

                // Construir el cuerpo HTML de la factura
                string cuerpoFactura = ConstruirCuerpoFactura(modelo, esPagoGrupal);
                mensaje.Body = cuerpoFactura;
                mensaje.IsBodyHtml = true;

                var smtp = new SmtpClient("smtp.gmail.com")
                {
                    Port = 587,
                    Credentials = new NetworkCredential("gustosazon8@gmail.com", "asiv yjjj ixtj nqch"),
                    EnableSsl = true
                };

                smtp.Send(mensaje);
            }
            catch (Exception ex)
            {
                // Puedes loggear el error si lo deseas
                System.Diagnostics.Debug.WriteLine($"Error al enviar factura: {ex.Message}");
            }
        }

        private string ConstruirCuerpoFactura(PagosViewModel modelo, bool esPagoGrupal)
        {
            string tipoPago = esPagoGrupal ? "Pago Grupal (Mesa completa)" : "Pago Individual";
            string nombreCliente = Session["Nombre"]?.ToString() ?? "Cliente";
            string numeroFactura = DateTime.Now.ToString("yyyyMMddHHmmss");

            var sb = new StringBuilder();
            sb.AppendLine($@"
<!DOCTYPE html>
<html>
<head>
    <style>
        body {{ font-family: Arial, sans-serif; line-height: 1.6; }}
        .factura {{ max-width: 800px; margin: 0 auto; padding: 20px; border: 1px solid #ddd; }}
        .header {{ text-align: center; margin-bottom: 20px; }}
        .info {{ margin-bottom: 20px; }}
        .detalle {{ width: 100%; border-collapse: collapse; margin-bottom: 20px; }}
        .detalle th, .detalle td {{ border: 1px solid #ddd; padding: 8px; text-align: left; }}
        .detalle th {{ background-color: #f2f2f2; }}
        .totales {{ text-align: right; margin-top: 20px; }}
        .footer {{ margin-top: 30px; text-align: center; font-size: 0.9em; color: #666; }}
    </style>
</head>
<body>
    <div class='factura'>
        <div class='header'>
            <h1>Gusto y Sazón</h1>
            <h2>Factura Electrónica #{numeroFactura}</h2>
            <p>{DateTime.Now:dd/MM/yyyy HH:mm}</p>
        </div>

        <div class='info'>
            <p><strong>Cliente:</strong> {nombreCliente}</p>
            <p><strong>Tipo de pago:</strong> {tipoPago}</p>
            <p><strong>Mesa:</strong> {modelo.NumeroMesa}</p>
        </div>");

            // Solo agregar la tabla si hay items
            if (modelo.ItemsOrden != null && modelo.ItemsOrden.Any())
            {
                sb.AppendLine(@"
        <table class='detalle'>
            <thead>
                <tr>
                    <th>Descripción</th>
                    <th>Cantidad</th>
                    <th>Precio Unitario</th>
                    <th>Subtotal</th>
                </tr>
            </thead>
            <tbody>");

                foreach (var item in modelo.ItemsOrden)
                {
                    sb.AppendLine($@"
                <tr>
                    <td>{item.NombreComida}</td>
                    <td>{item.Cantidad}</td>
                    <td>₡{item.PrecioUnitario.ToString("N2")}</td>
                    <td>₡{item.Subtotal.ToString("N2")}</td>
                </tr>");
                }

                sb.AppendLine(@"
            </tbody>
        </table>");
            }
            else
            {
                sb.AppendLine("<p>No hay items en esta orden</p>");
            }

            sb.AppendLine($@"
        <div class='totales'>
            <p><strong>Subtotal:</strong> ₡{modelo.Subtotal.ToString("N2")}</p>
            <p><strong>IVA (13%):</strong> ₡{modelo.IVA.ToString("N2")}</p>
            <p><strong>Propina:</strong> ₡{modelo.Propina.ToString("N2")}</p>
            <p><strong>Total:</strong> ₡{modelo.Total.ToString("N2")}</p>
        </div>

        <div class='footer'>
            <p>¡Gracias por su visita!</p>
            <p>Gusto y Sazón - Tel: 1234-5678</p>
            <p>Esta factura es un comprobante electrónico</p>
        </div>
    </div>
</body>
</html>");

            return sb.ToString();
        }








        private List<ItemOrden> ObtenerItemsOrden(int usuarioId)
        {
            var items = new List<ItemOrden>();
            string comando = @"SELECT Id AS GrupoOrdenId, NomComida AS NombreComida, Cantidad, PrecioUnitario
                      FROM Pagos WHERE UsuarioId = @UsuarioId AND Estado = 'Por Pagar'";

            using (var connection = new SqlConnection(connectionString))
            using (var cmd = new SqlCommand(comando, connection))
            {
                cmd.Parameters.AddWithValue("@UsuarioId", usuarioId);
                connection.Open();

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        items.Add(new ItemOrden
                        {
                            OrdenId = reader.GetInt32(reader.GetOrdinal("GrupoOrdenId")),
                            NombreComida = reader.GetString(reader.GetOrdinal("NombreComida")),
                            Cantidad = reader.GetInt32(reader.GetOrdinal("Cantidad")),
                            PrecioUnitario = reader.GetDecimal(reader.GetOrdinal("PrecioUnitario")),
                            UsuarioId = usuarioId
                        });
                    }
                }
            }
            return items;
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
                Credentials = new NetworkCredential("gustosazon8@gmail.com", "amwt dziv ezmx heod"),
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

        }













        [HttpGet]
        public JsonResult ObtenerResumenFinanciero()
        {
            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    // Obtener el resumen del día actual
                    var query = @"SELECT 
                        SUM(ganancias_totales) as GananciasTotales,
                        SUM(propinas_totales) as PropinasTotales,
                        (SUM(ganancias_totales) + SUM(propinas_totales)) as VentasHoy
                      FROM finanzas 
                      WHERE DiaRegistrado = CAST(GETDATE() AS DATE)";

                    using (var cmd = new SqlCommand(query, connection))
                    {
                        using (var reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                var ganancias = reader["GananciasTotales"] != DBNull.Value ?
                                    Convert.ToDecimal(reader["GananciasTotales"]) : 0m;
                                var propinas = reader["PropinasTotales"] != DBNull.Value ?
                                    Convert.ToDecimal(reader["PropinasTotales"]) : 0m;
                                var ventas = reader["VentasHoy"] != DBNull.Value ?
                                    Convert.ToDecimal(reader["VentasHoy"]) : 0m;

                                return Json(new
                                {
                                    success = true,
                                    gananciasTotales = ganancias,
                                    propinasTotales = propinas,
                                    ventasHoy = ventas
                                }, JsonRequestBehavior.AllowGet);
                            }
                            else
                            {
                                return Json(new
                                {
                                    success = true,
                                    gananciasTotales = 0m,
                                    propinasTotales = 0m,
                                    ventasHoy = 0m
                                }, JsonRequestBehavior.AllowGet);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    error = ex.Message
                }, JsonRequestBehavior.AllowGet);
            }
        }






















        //Parte Ordenes Cocina


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
                Equipos = new List<EquipoViewModel>
        {
            new EquipoViewModel { Nombre = "Parrilla Principal", Estado = "Funcionando", Capacidad = 2, Temperatura = 200 },
            new EquipoViewModel { Nombre = "Freidora", Estado = "Funcionando", Capacidad = 4, Temperatura = 180 },
            new EquipoViewModel { Nombre = "Parrilla Principal", Estado = "Roto", Capacidad = 0, Temperatura = 0 },
        }
            };












            // Obtener las órdenes pendientes de la base de datos
            using (SqlConnection conexion = new SqlConnection(connectionString))
            {
                conexion.Open();

                string query = @"SELECT o.Id, m.NumeroMesa, o.NombreComida, o.Cantidad, o.PrecioTotal, o.Estado 
                        FROM Ordenes o
                        JOIN Mesas m ON o.MesaId = m.Id
                        WHERE o.Estado IN ('Preparando', 'Platillo Listo')
                        ORDER BY o.Fecha";

                using (SqlCommand cmd = new SqlCommand(query, conexion))
                {
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        model.Ordenescocina = new List<OrdenModelcocina>();
                        while (reader.Read())
                        {
                            model.Ordenescocina.Add(new OrdenModelcocina
                            {
                                Id = (int)reader["Id"],
                                MesaId = (int)reader["NumeroMesa"],
                                NombreComida = reader["NombreComida"].ToString(),
                                Cantidad = (int)reader["Cantidad"],
                                PrecioTotal = (decimal)reader["PrecioTotal"],
                                Estado = reader["Estado"].ToString()
                            });
                        }
                    }
                }
            }

            return View(model);
        }











        







        private readonly string _connectionString = ConfigurationManager.ConnectionStrings["GustoySazonDB"].ConnectionString;

        [HttpPost]
        public JsonResult MarcarOrdenComoLista(int id)
        {
            try
            {
                using (SqlConnection conexion = new SqlConnection(_connectionString))
                {
                    conexion.Open();

                    string query = "UPDATE Ordenes SET Estado = 'Platillo Listo' WHERE Id = @id";

                    using (SqlCommand cmd = new SqlCommand(query, conexion))
                    {
                        cmd.Parameters.AddWithValue("@id", id);
                        int rowsAffected = cmd.ExecuteNonQuery();

                        if (rowsAffected == 0)
                        {
                            return Json(new { success = false, message = "Orden no encontrada o ya actualizada." });
                        }
                    }
                }

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                // Para debug, puedes registrar ex.Message en logs si quieres
                return Json(new { success = false, message = ex.Message });
            }
        }










        [HttpPost]
        public ActionResult ActualizarEstado(int id)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["GustoySazonDB"].ConnectionString;

            using (SqlConnection conexion = new SqlConnection(connectionString))
            {
                conexion.Open();

                string updateQuery = "UPDATE Ordenes SET Estado = 'Listo para entregar' WHERE Id = @id";

                using (SqlCommand cmd = new SqlCommand(updateQuery, conexion))
                {
                    cmd.Parameters.AddWithValue("@id", id);
                    cmd.ExecuteNonQuery();
                }
            }

            // Retorna código 200 OK
            return new HttpStatusCodeResult(HttpStatusCode.OK);
        }















        // Diccionario de platillos y precios organizado por categorías
        private readonly Dictionary<string, Dictionary<string, decimal>> _menuCategorizado = new Dictionary<string, Dictionary<string, decimal>>
    {
        {
            "Comida", new Dictionary<string, decimal>
            {
                {"Casado con Pollo", 7200.00m},
                {"Arroz con Camarones", 8600.00m},
                {"Gallo Pinto", 3500.00m},
                {"Sopa Negra", 4000.00m},
                {"Olla de Carne", 9000.00m},
                {"Ceviche Tradicional", 6500.00m},
                {"Empanadas de Queso", 3000.00m},
                {"Chifrijo", 5800.00m},
                {"Tamal de Cerdo", 4500.00m},
                {"Picadillo de Verduras", 4000.00m}
            }
        },
        {
            "Bebida", new Dictionary<string, decimal>
            {
                {"Fresco de Guayaba", 2500.00m},
                {"Agua de Sapo", 2700.00m},
                {"Refresco Natural de Maracuyá", 3000.00m},
                {"Café Chorreado", 3200.00m},
                {"Batido de Guanábana", 4000.00m},
                {"Agua Mineral", 2500.00m},
                {"Limonada Natural", 2800.00m},
                {"Cerveza Nacional", 8500.00m},
                {"Mojito Tropical", 11000.00m},
                {"Refresco de Tamarindo", 3000.00m}
            }
        },
        {
            "Postre", new Dictionary<string, decimal>
            {
                {"Tres Leches", 5000.00m},
                {"Arroz con Leche", 4000.00m},
                {"Flan de Coco", 5200.00m},
                {"Cocada", 2800.00m},
                {"Helado Artesanal de Café", 6000.00m},
                {"Postre de Nance", 4500.00m},
                {"Bizcocho de Pan", 3000.00m},
                {"Bocadillo con Queso", 2700.00m},
                {"Pay de Limón", 5800.00m},
                {"Churros con Chocolate", 5500.00m}
            }
        }
    };

        // Detalles adicionales de los platillos
        private readonly Dictionary<string, string> _detallesPlatillos = new Dictionary<string, string>
    {
        {"Casado con Pollo", "Nuestro Casado lleva: arroz, frijoles, pollo asado, ensalada, plátano maduro y tortillas recién hechas"},
        {"Olla de Carne", "Cocción de 5 horas con 7 tipos de verduras y carne de res premium"},
        {"Tres Leches", "Preparado con leche entera, leche evaporada y leche condensada, bañado en almíbar casero"},
        {"Chifrijo", "Deliciosa combinación de chicharrón, frijoles, pico de gallo y toques de ají"},
        {"Mojito Tropical", "Preparado con ron, hierbabuena fresca, limón, azúcar y un toque de frutas tropicales"}
    };

        // Lista de respuestas aleatorias
        private readonly List<string> _saludos = new List<string>
    {
        "¡Hola! Soy el asistente virtual de Gusto y Sazón. ¿En qué puedo ayudarte?",
        "¡Buen día! ¿Qué deseas saber sobre nuestro restaurante?",
        "Hola :) Estoy aquí para ayudarte con cualquier consulta sobre el menú o servicios."
    };

        private readonly List<string> _despedidas = new List<string>
    {
        "¡Gracias por visitar Gusto y Sazón! Que tengas un excelente día.",
        "Fue un placer ayudarte. ¡Buen provecho!",
        "Hasta luego. ¡Esperamos verte pronto de nuevo!"
    };

        private readonly List<string> _recomendaciones = new List<string>
    {
        "Te recomiendo nuestro {0}, un plato exquisito por sólo ₡{1:#,###}",
        "No puedes perderte nuestro {0}, uno de los favoritos por sólo ₡{1:#,###}",
        "Prueba nuestro {0}, una delicia culinaria por ₡{1:#,###}"
    };

        private readonly List<string> _chistes = new List<string>
    {
        "¿Qué le dice un jamón a otro jamón? ¡Nos han cortado!",
        "¿Cómo se llama el hijo del cocinero? El recetario",
        "¿Qué hace un muelle en un restaurante? Nada, pero tiene mucho resorte"
    };

        // Juego de adivinanzas
        private readonly List<(string pregunta, string respuesta)> _adivinanzas = new List<(string, string)>
    {
        ("Soy redonda como el sol, con huevos y jamón, en el desayuno soy tradición. ¿Qué soy?", "Gallo Pinto"),
        ("Blanco por dentro, verde por fuera, si quieres que te lo diga, espera.", "Aguacate"),
        ("Tengo masa y soy relleno, en hojas me cocino lento, en Navidad no puedo faltar. ¿Quién soy?", "Tamal"),
        ("No soy postre ni soy pan, pero con café me comerán. ¿Qué soy?", "Bizcocho"),
        ("Bebida soy, pero no agua, de frutas tropicales tengo la magia.", "Refresco Natural")
    };

        // Curiosidades gastronómicas
        private readonly List<string> _curiosidades = new List<string>
    {
        "¿Sabías que el Gallo Pinto es considerado plato nacional en Costa Rica y Nicaragua?",
        "Nuestra Sopa Negra lleva 7 horas de cocción para lograr su sabor perfecto",
        "El secreto del Ceviche Tradicional está en macerar el pescado con limón criollo",
        "El 'Agua de Sapo' debe su nombre a su color característico, ¡pero no lleva sapos!",
        "Nuestros Churros con Chocolate siguen una receta española original de 1894"
    };

        // Trivia del restaurante
        private readonly List<(string pregunta, string respuesta)> _trivia = new List<(string, string)>
    {
        ("¿En qué año se fundó Gusto y Sazón?", "1999"),
        ("¿Cuál es nuestro plato más antiguo?", "Olla de Carne"),
        ("¿Qué ingrediente secreto lleva nuestra Sopa Negra?", "hojas de culantro coyote"),
        ("¿Cuántos tipos de leche lleva nuestro Tres Leches?", "tres"),
        ("¿De qué país es originaria la receta de nuestro Flan de Coco?", "Filipinas")
    };

        // Variables para mantener estado del juego
        private string _adivinanzaActual = "";
        private string _respuestaAdivinanza = "";
        private string _triviaActual = "";
        private string _respuestaTrivia = "";

        [HttpPost]
        public JsonResult Chatbot(string mensaje)
        {
            System.Threading.Thread.Sleep(new Random().Next(800, 1500));

            // Preprocesamiento del mensaje
            mensaje = PreprocesarMensaje(mensaje);

            string respuesta = "";

            // 1. Verificar herramientas útiles
            respuesta = ProporcionarHerramientas(mensaje);
            if (!string.IsNullOrEmpty(respuesta))
                return Json(new { respuesta });

            // 2. Verificar entretenimiento
            respuesta = ProporcionarEntretenimiento(mensaje);
            if (!string.IsNullOrEmpty(respuesta))
                return Json(new { respuesta });

            // 3. Manejo de contexto
            if (Session["ultimoContexto"] != null)
            {
                var contexto = Session["ultimoContexto"].ToString();

                // Respuesta a adivinanza
                if (contexto == "esperandoRespuestaAdivinanza")
                {
                    if (mensaje.Contains(_respuestaAdivinanza.ToLower()) ||
                        _adivinanzas.Any(a => a.respuesta.ToLower() == mensaje))
                    {
                        respuesta = $"¡Correcto! Era {_respuestaAdivinanza}. ¿Quieres otra adivinanza?";
                    }
                    else
                    {
                        respuesta = $"Casi... la respuesta era {_respuestaAdivinanza}. ¿Otra adivinanza?";
                    }
                    Session.Remove("ultimoContexto");
                    return Json(new { respuesta });
                }

                // Selección de categoría de menú
                if (contexto == "esperandoCategoriaMenu")
                {
                    string categoria = "";
                    if (mensaje == "1" || mensaje == "comida" || mensaje == "comidas" || mensaje.Contains("plato")) categoria = "Comida";
                    else if (mensaje == "2" || mensaje == "bebida" || mensaje == "bebidas" || mensaje.Contains("tomar")) categoria = "Bebida";
                    else if (mensaje == "3" || mensaje == "postre" || mensaje == "postres" || mensaje.Contains("dulce")) categoria = "Postre";

                    if (!string.IsNullOrEmpty(categoria))
                    {
                        respuesta = FormatMenuCategory(categoria);
                        Session["ultimoContexto"] = "esperandoDetallePlatillo";
                        return Json(new { respuesta });
                    }
                }

                // Detalles de platillo
                if (contexto == "esperandoDetallePlatillo")
                {
                    var platillo = BuscarPlatillo(mensaje);
                    if (platillo != null)
                    {
                        var (nombre, precio, categoria) = platillo.Value;
                        respuesta = $"🔹 {nombre} (₡{precio:#,###})";

                        if (_detallesPlatillos.ContainsKey(nombre))
                        {
                            respuesta += $"\n{_detallesPlatillos[nombre]}";
                        }

                        respuesta += $"\n\n¿Quieres agregar este {categoria.ToLower()} a tu pedido?";
                        Session["ultimoContexto"] = "esperandoConfirmacionPedido";
                        Session["platilloActual"] = nombre;
                        return Json(new { respuesta });
                    }
                }

                // Confirmación de pedido
                if (contexto == "esperandoConfirmacionPedido")
                {
                    if (mensaje.Contains("si") || mensaje.Contains("claro") || mensaje.Contains("ok"))
                    {
                        var platillo = Session["platilloActual"].ToString();
                        respuesta = $"¡Perfecto! He anotado {platillo} para tu pedido. ¿Deseas algo más?";
                    }
                    else
                    {
                        respuesta = "Entendido. ¿Te interesa otro platillo o en qué más puedo ayudarte?";
                    }
                    Session.Remove("ultimoContexto");
                    Session.Remove("platilloActual");
                    return Json(new { respuesta });
                }
            }

            // Intenciones principales
            if (EsSaludo(mensaje))
            {
                respuesta = _saludos[new Random().Next(_saludos.Count)];
            }
            else if (EsDespedida(mensaje))
            {
                respuesta = _despedidas[new Random().Next(_despedidas.Count)];
            }
            else if (EsMenu(mensaje))
            {
                respuesta = "Nuestro menú se divide en:\n1. Comidas 🍛\n2. Bebidas 🍹\n3. Postres 🍰\n\n¿Qué categoría deseas? (Di el número o nombre)";
                Session["ultimoContexto"] = "esperandoCategoriaMenu";
            }
            else if (EsRecomendacion(mensaje))
            {
                if (mensaje.Contains("comida") || mensaje.Contains("plato"))
                {
                    respuesta = RecomendarPlatillo("Comida");
                }
                else if (mensaje.Contains("bebida") || mensaje.Contains("tomar"))
                {
                    respuesta = RecomendarPlatillo("Bebida");
                }
                else if (mensaje.Contains("postre") || mensaje.Contains("dulce"))
                {
                    respuesta = RecomendarPlatillo("Postre");
                }
                else
                {
                    respuesta = $"Te recomiendo:\n{RecomendarPlatillo(RecomendarPorHora())}";
                }
            }
            else if (EsPago(mensaje))
            {
                respuesta = "💳 Aceptamos Visa/Mastercard. Solicita la cuenta al mesero cuando termines.";
            }
            else if (EsAdivinanza(mensaje))
            {
                var adivinanza = _adivinanzas[new Random().Next(_adivinanzas.Count)];
                _respuestaAdivinanza = adivinanza.respuesta;
                Session["ultimoContexto"] = "esperandoRespuestaAdivinanza";
                respuesta = $"🧩 ADIVINANZA:\n{adivinanza.pregunta}";
            }
            else if (EsHora(mensaje))
            {
                respuesta = $"Son las {DateTime.Now.ToString("hh:mm tt")}. ¿En qué más te ayudo?";
            }
            else if (EsQueja(mensaje))
            {
                respuesta = "Lamentamos escuchar eso. Por favor comunícate con nuestro gerente: 5555-1234";
            }
            else if (EsGracias(mensaje))
            {
                respuesta = "¡Gracias a ti! ¿Necesitas algo más?";
            }
            else if (EsConsultaRestaurante(mensaje))
            {
                respuesta = ResponderConsultaRestaurante(mensaje);
            }
            else
            {
                // Respuesta por defecto mejorada
                var respuestas = new List<string>
            {
                "¿Te gustaría ver el menú, una recomendación o jugar una adivinanza?",
                "Puedo ayudarte con: nuestro menú, métodos de pago o recomendaciones",
                "¿Quieres información sobre comidas, bebidas o postres?"
            };
                respuesta = respuestas[new Random().Next(respuestas.Count)];
            }

            return Json(new { respuesta });
        }

        #region Métodos de ayuda
        private string PreprocesarMensaje(string mensaje)
        {
            return mensaje.ToLower()
                .Replace("?", "")
                .Replace("¿", "")
                .Replace("!", "")
                .Replace("¡", "")
                .Replace(".", "")
                .Replace(",", "")
                .Trim();
        }

        private bool EsSaludo(string mensaje) =>
            mensaje.Contains("hola") || mensaje == "hi" || mensaje.Contains("buen") || mensaje.Contains("buenas");

        private bool EsDespedida(string mensaje) =>
            mensaje.Contains("adios") || mensaje.Contains("chao") || mensaje.Contains("hasta luego") ||
            mensaje.Contains("gracias") || mensaje.Contains("listo");

        private bool EsMenu(string mensaje) =>
            mensaje.Contains("menu") || mensaje.Contains("carta") || mensaje.Contains("que ofreces") ||
            mensaje.Contains("platillos") || mensaje.Contains("que tienen");

        private bool EsRecomendacion(string mensaje) =>
            mensaje.Contains("recomi") || mensaje.Contains("sugiere") ||
            mensaje.Contains("que me aconsejas") || mensaje.Contains("que es bueno");

        private bool EsPago(string mensaje) =>
            mensaje.Contains("pago") || mensaje.Contains("pagar") || mensaje.Contains("cuenta") ||
            mensaje.Contains("metodo") || mensaje.Contains("tarjeta") || mensaje.Contains("efectivo");

        private bool EsAdivinanza(string mensaje) =>
            mensaje.Contains("adivina") || mensaje.Contains("juego") || mensaje.Contains("jugar");

        private bool EsHora(string mensaje) =>
            mensaje.Contains("hora") || mensaje.Contains("fecha");

        private bool EsQueja(string mensaje) =>
            mensaje.Contains("queja") || mensaje.Contains("reclamo") || mensaje.Contains("problema");

        private bool EsGracias(string mensaje) =>
            mensaje.Contains("gracias") || mensaje == "thx" || mensaje.Contains("agradezco");

        private bool EsConsultaRestaurante(string mensaje) =>
            mensaje.Contains("horario") || mensaje.Contains("abren") || mensaje.Contains("cierran") ||
            mensaje.Contains("ubicacion") || mensaje.Contains("direccion") || mensaje.Contains("telefono");

        private string ResponderConsultaRestaurante(string mensaje)
        {
            if (mensaje.Contains("horario") || mensaje.Contains("abren") || mensaje.Contains("cierran"))
                return "🕒 Nuestros horarios son:\nLunes a Viernes: 11am - 10pm\nSábados y Domingos: 9am - 11pm";

            if (mensaje.Contains("ubicacion") || mensaje.Contains("direccion"))
                return "📍 Estamos en Avenida Central, 200m este del Parque Central, San José";

            if (mensaje.Contains("telefono"))
                return "📞 Puedes llamarnos al: 2222-5555";

            return "ℹ️ Más información:\nHorarios: L-V 11am-10pm\nDirección: Av. Central, SJ\nTeléfono: 2222-5555";
        }

        private string ProporcionarHerramientas(string mensaje)
        {
            if (mensaje.Contains("clima"))
                return "☀️ El clima actual en San José es cálido, 27°C con posibilidad de lluvias por la tarde.";

            if (mensaje.Contains("trafico") || mensaje.Contains("transito"))
                return "🚗 El tráfico alrededor del restaurante es fluido actualmente.";

            if (mensaje.Contains("wifi"))
                return "📶 Nuestra red WiFi es 'GustoSazon_Guest' con contraseña 'Bienvenidos2023'";

            if (mensaje.Contains("evento") || mensaje.Contains("fiesta"))
                return "🎉 Ofrecemos servicio de banquetes para eventos. Contáctanos al 2222-5555 ext. 3 para más información.";

            return null;
        }

        private string ProporcionarEntretenimiento(string mensaje)
        {
            if (mensaje.Contains("chiste") || mensaje.Contains("broma"))
                return _chistes[new Random().Next(_chistes.Count)];

            if (mensaje.Contains("trivia"))
                return IniciarTrivia();

            if (mensaje.Contains("adivina") || mensaje.Contains("jugar"))
                return IniciarAdivinanza();

            if (mensaje.Contains("curiosidad") || mensaje.Contains("dato"))
                return _curiosidades[new Random().Next(_curiosidades.Count)];

            return null;
        }

        private string IniciarTrivia()
        {
            var pregunta = _trivia[new Random().Next(_trivia.Count)];
            _triviaActual = pregunta.pregunta;
            _respuestaTrivia = pregunta.respuesta;
            Session["ultimoContexto"] = "esperandoRespuestaTrivia";
            return $"❓ TRIVIA:\n{pregunta.pregunta}";
        }

        private string IniciarAdivinanza()
        {
            var adivinanza = _adivinanzas[new Random().Next(_adivinanzas.Count)];
            _adivinanzaActual = adivinanza.pregunta;
            _respuestaAdivinanza = adivinanza.respuesta;
            Session["ultimoContexto"] = "esperandoRespuestaAdivinanza";
            return $"🧩 ADIVINANZA:\n{adivinanza.pregunta}";
        }

        private string RecomendarPorHora()
        {
            var hora = DateTime.Now.Hour;
            if (hora < 11) return "Comida";
            if (hora < 16) return "Comida";
            if (hora < 19) return "Bebida";
            return "Postre";
        }

        private string FormatMenuCategory(string categoria)
        {
            var items = _menuCategorizado[categoria]
                .Select(item => $"• {item.Key}: ₡{item.Value:#,###}")
                .OrderBy(x => x)
                .ToArray();

            return $"{categoria.ToUpper()} 🍴:\n" + string.Join("\n", items) +
                   $"\n\n¿Quieres más detalles de algún {categoria.ToLower()} en especial?";
        }

        private string RecomendarPlatillo(string categoria)
        {
            var platillos = _menuCategorizado[categoria];
            var platillo = platillos.ElementAt(new Random().Next(platillos.Count));

            var recomendacion = _recomendaciones[new Random().Next(_recomendaciones.Count)];
            return string.Format(recomendacion, platillo.Key, platillo.Value) +
                   (_detallesPlatillos.ContainsKey(platillo.Key) ?
                    $"\n{_detallesPlatillos[platillo.Key]}" : "");
        }

        private (string nombre, decimal precio, string categoria)? BuscarPlatillo(string mensaje)
        {
            foreach (var categoria in _menuCategorizado)
            {
                foreach (var platillo in categoria.Value)
                {
                    if (mensaje.Contains(platillo.Key.ToLower()))
                    {
                        return (platillo.Key, platillo.Value, categoria.Key);
                    }
                }
            }
            return null;
        }
    #endregion

























        private const string RecoveryEmailKey = "RecoveryEmail";
        private readonly string _apiBaseUrl = "http://localhost:3004/api/recuperar";

        // GET: Home/ContrasenaOlvidada
        public ActionResult ContrasenaOlvidada()
        {
            return View();
        }

        // GET: Home/CambiarContrasena
        public ActionResult CambiarContrasena()
        {
            // Recuperar email de múltiples fuentes
            var email = Session[RecoveryEmailKey] as string ??
                       TempData[RecoveryEmailKey] as string ??
                       Request.Cookies[RecoveryEmailKey]?.Value;

            if (string.IsNullOrEmpty(email))
            {
                return RedirectToAction("ContrasenaOlvidada");
            }

            // Guardar nuevamente para la próxima petición POST
            TempData[RecoveryEmailKey] = email;
            ViewBag.RecoveryEmail = email;
            return View();
        }






        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> SolicitarCodigo(string email)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(email))
                {
                    return Json(new { success = false, message = "Debe proporcionar un correo electrónico" });
                }

                using (var client = new HttpClient())
                {
                    var response = await client.PostAsync($"{_apiBaseUrl}/recuperar",
                        new StringContent(
                            JsonConvert.SerializeObject(new { correo = email }),
                            System.Text.Encoding.UTF8,
                            "application/json"));

                    var content = await response.Content.ReadAsStringAsync();
                    var result = JsonConvert.DeserializeObject<dynamic>(content);

                    if (response.IsSuccessStatusCode && result.success == true)
                    {
                        // Almacenar en múltiples lugares para persistencia
                        Session[RecoveryEmailKey] = email;
                        TempData[RecoveryEmailKey] = email;
                        Response.Cookies.Add(new HttpCookie(RecoveryEmailKey, email)
                        {
                            Expires = DateTime.Now.AddMinutes(30)
                        });

                        // Devolver URL de redirección
                        return Json(new
                        {
                            success = true,
                            redirectUrl = Url.Action("CambiarContrasena", "Home")
                        });
                    }

                    string errorMessage = result.message ?? "Error al enviar el código";
                    return Json(new { success = false, message = errorMessage });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Error de conexión: {ex.Message}" });
            }
        }











        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> CambiarContrasena(string codigo, string nuevaContrasena, string confirmarContrasena)
        {
            // Recuperar email de múltiples fuentes
            var email = Session[RecoveryEmailKey] as string ??
                       TempData[RecoveryEmailKey] as string ??
                       Request.Cookies[RecoveryEmailKey]?.Value;

            if (string.IsNullOrEmpty(email))
            {
                return Json(new
                {
                    success = false,
                    message = "Sesión expirada, por favor inicie el proceso nuevamente"
                });
            }

            if (nuevaContrasena != confirmarContrasena)
            {
                return Json(new
                {
                    success = false,
                    message = "Las contraseñas no coinciden"
                });
            }

            try
            {
                using (var client = new HttpClient())
                {
                    var response = await client.PostAsync($"{_apiBaseUrl}/cambiar",
                        new StringContent(
                            JsonConvert.SerializeObject(new
                            {
                                correo = email,
                                codigo = codigo,
                                nuevaContrasena = nuevaContrasena
                            }),
                            System.Text.Encoding.UTF8,
                            "application/json"));

                    var content = await response.Content.ReadAsStringAsync();
                    var result = JsonConvert.DeserializeObject<dynamic>(content);

                    if (response.IsSuccessStatusCode && result.success == true)
                    {
                        // Limpiar todos los datos de recuperación
                        Session.Remove(RecoveryEmailKey);
                        TempData.Remove(RecoveryEmailKey);
                        Response.Cookies.Add(new HttpCookie(RecoveryEmailKey) { Expires = DateTime.Now.AddDays(-1) });

                        return Json(new
                        {
                            success = true,
                            redirectUrl = Url.Action("Login", "Home"),
                            message = "Contraseña cambiada exitosamente"
                        });
                    }

                    string errorMessage = result.message ?? "Error al cambiar la contraseña";
                    return Json(new { success = false, message = errorMessage });
                }
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    message = $"Error de conexión: {ex.Message}"
                });
            }
        }




















    }
}