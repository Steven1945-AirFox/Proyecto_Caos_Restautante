using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;               // Para Controller, JsonResult, Json()
using System.Diagnostics;           // Para Debug.WriteLine()
using GustoySazon.BLL;              // Para SimulacionBLL
using GustoySazon.Models;

namespace GustoySazon.Controllers
{
    public class SimulacionController : Controller
    {
        private SimulacionBLL bll = new SimulacionBLL();

        [HttpPost]
        public JsonResult Iniciar()
        {
            try
            {
                int id = bll.IniciarNuevaSimulacion();
                Debug.WriteLine($"Simulación iniciada con ID: {id}");
                return Json(new { success = true, id = id });
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error en controlador al iniciar simulación: " + ex.Message);
                return Json(new { success = false, message = ex.Message });
            }
        }

        // de aca para arriba no tocar

        [HttpPost]
        public JsonResult RegistrarMetrica(int simulacionId, string metricaId, int valor)
        {
            try
            {
                ValorMetricaModel metrica = new ValorMetricaModel
                {
                    SimulacionId = simulacionId,
                    MetricaId = metricaId,
                    Valor = valor,
                    FechaRegistro = DateTime.Now
                };

                bll.RegistrarMetrica(metrica);

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        // Registra una solución aplicada
        [HttpPost]
        public JsonResult RegistrarSolucion(SolucionAplicadaModel model)
        {
            try
            {
                model.FechaAplicacion = DateTime.Now;
                Debug.WriteLine($"➡️ Recibido Solución: SimulaciónID={model.SimulacionId}, EventoID={model.EventoId}, Solución={model.Solucion}, Severidad={model.Severidad}");

                bll.RegistrarSolucionAplicada(model);
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error al registrar solución aplicada: " + ex.Message);
                return Json(new { success = false, message = ex.Message });
            }
        }
    }
}

