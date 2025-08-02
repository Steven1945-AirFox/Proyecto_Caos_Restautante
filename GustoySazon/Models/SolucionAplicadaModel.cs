using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GustoySazon.Models
{
    public class SolucionAplicadaModel
    {
        public int Id { get; set; }
        public int SimulacionId { get; set; }
        public string EventoId { get; set; }
        public string Solucion { get; set; }
        public string Severidad { get; set; }
        public DateTime FechaAplicacion { get; set; }
    }
}