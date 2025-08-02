//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Web;
//using GustoySazon.Models;    // Para acceder a SimulacionModel y ValorMetricaModel
//using GustoySazon.DAL;
//using System.Diagnostics;


//namespace GustoySazon.BLL
//{
//    public class SimulacionBLL
//    {
//        private SimulacionDAL dal = new SimulacionDAL();

//        public int IniciarNuevaSimulacion()
//        {
//            return dal.CrearSimulacion();
//        }

//        public void RegistrarMetrica(ValorMetricaModel model)
//        {
//            dal.RegistrarMetrica(model);  // Llama a DAL con el modelo completo
//        }

//        public void RegistrarSolucionAplicada(SolucionAplicadaModel model)
//        {
//            dal.GuardarSolucionAplicada(model);  // Usa el nombre correcto del método en tu DAL
//        }
//    }
//}