using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GustoySazon.Models
{
    public class ValorMetricaModel
    {
        public int Id { get; set; }
        public int SimulacionId { get; set; }
        public string MetricaId { get; set; }  // o int si es un ID numérico
        public int Valor { get; set; }
        public DateTime FechaRegistro { get; set; }
    }
}
