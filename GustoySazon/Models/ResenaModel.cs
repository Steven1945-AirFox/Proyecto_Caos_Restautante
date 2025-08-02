using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GustoySazon.Models
{
    public class ResenaModel
    {
        public int Id { get; set; }
        public int UsuarioId { get; set; }
        public string NombreUsuario { get; set; }
        public int? Calificacion { get; set; }
        public string ResenaTexto { get; set; }
        public DateTime Fecha { get; set; }
    }
}


