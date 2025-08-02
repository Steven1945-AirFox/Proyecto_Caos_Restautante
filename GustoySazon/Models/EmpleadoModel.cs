using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GustoySazon.Models
{
    public class EmpleadoModel
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public int Cedula { get; set; }
        public string Correo { get; set; }
        public string Contrasena { get; set; }
        public string Rol { get; set; }
    }
}