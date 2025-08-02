using GustoySazon.BLL;
using GustoySazon.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace GustoySazon.Controllers
{
    public class RegistroPersonalController : Controller
    {
        private EmpleadoBLL bll = new EmpleadoBLL();

        [HttpPost]
        public JsonResult AgregarEmpleado(string nombre, string rol)
        {
            try
            {
                var nuevoEmpleado = new EmpleadoModel
                {
                    Nombre = nombre,
                    Cedula = new Random().Next(100000000, 999999999),
                    Correo = $"{nombre.Replace(" ", "").ToLower()}@restaurante.com",
                    Contrasena = "123456",
                    Rol = rol
                };

                bll.RegistrarEmpleado(nuevoEmpleado);
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpGet]
        public JsonResult Listar()
        {
            var empleados = new EmpleadoBLL().ObtenerEmpleados();
            return Json(empleados, JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        public JsonResult ActualizarEmpleado(EmpleadoModel emp)
        {
            try
            {
                new EmpleadoBLL().ActualizarEmpleado(emp);
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public JsonResult EliminarEmpleado(int id)
        {
            try
            {
                new EmpleadoBLL().EliminarEmpleado(id);
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
    }
}