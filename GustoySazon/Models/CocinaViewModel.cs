using System.Collections.Generic;

namespace GustoySazon.Models
{
    public class CocinaViewModel
    {
        // Indicadores del chef
        public int Estres { get; set; }
        public int Energia { get; set; }
        public int Concentracion { get; set; }
        public int RiesgoQuemado { get; set; }
        public int Calidad { get; set; }
        public double Temperatura { get; set; }
        public double Humo { get; set; }

        // Datos del rendimiento
        public int OrdenesCompletadas { get; set; }
        public int CalidadPromedio { get; set; }
        public int HorasTrabajadas { get; set; }
        public int OrdenesPendientes { get; set; }

        // Listas para la vista
        public List<OrdenModel> Ordenes { get; set; } = new List<OrdenModel>();
        public List<EquipoViewModel> Equipos { get; set; } = new List<EquipoViewModel>();
    }

    public class EquipoViewModel
    {
        public string Nombre { get; set; }
        public string Estado { get; set; }
        public int Capacidad { get; set; } // 0 a 5
        public int Temperatura { get; set; } // en °C
    }
}
