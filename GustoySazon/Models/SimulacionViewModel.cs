using System;
using System.Collections.Generic;

namespace GustoySazon.Models
{
    public class SimulacionViewModel
    {
        public int? SimulacionId { get; set; }
        public string Estado { get; set; }
        public DateTime? FechaInicio { get; set; }
        public List<MetricaViewModel> Metricas { get; set; }
        public List<EventoViewModel> EventosDisponibles { get; set; }

        public SimulacionViewModel()
        {
            Metricas = new List<MetricaViewModel>();
            EventosDisponibles = new List<EventoViewModel>();
        }
    }

    public class MetricaViewModel
    {
        public int Id { get; set; }
        public string Titulo { get; set; }
        public int Valor { get; set; }
        public int ValorMaximo { get; set; }
        public string Icono { get; set; }
        public string Color { get; set; }
        public string Unidad { get; set; }
    }

    public class EventoViewModel
    {
        public string Id { get; set; }
        public string Titulo { get; set; }
        public string Descripcion { get; set; }
        public string Icono { get; set; }
        public string Categoria { get; set; }
        public bool Activo { get; set; }
        public List<SolucionViewModel> Soluciones { get; set; }
    }

    public class SolucionViewModel
    {
        public string Severidad { get; set; }
        public string Texto { get; set; }
    }
}
