using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GustoySazon.Models
{
    public class LoginViewModel
    {
        public string Correo { get; set; }
        public string Contrasena { get; set; }
    }




    public class Usuario
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public int Cedula { get; set; }
        public string Correo { get; set; }
        public string Contrasena { get; set; }
        public string Rol { get; set; }
    }

    public class Empleado
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public int Cedula { get; set; }
        public string Correo { get; set; }
        public string Contrasena { get; set; }
        public string Rol { get; set; }
    }
}
