using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace GustoySazon.Models
{
    public class ClienteViewModel
    {
        public string Nombre { get; set; }
        public string Cedula { get; set; }
        public int Sillas { get; set; }

        public string TipoTarjeta { get; set; }
        public string NumTarjeta { get; set; }
        public string FechaVenc { get; set; }
        public string Correo { get; set; }      
        public string Contrasena { get; set; }  
    }
}