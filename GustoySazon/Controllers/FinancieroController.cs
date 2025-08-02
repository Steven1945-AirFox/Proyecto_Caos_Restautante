using GustoySazon.BLL;
using GustoySazon.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace GustoySazon.Controllers
{
    public class FinancieroController : Controller
    {
        private FinancieroBLL bll = new FinancieroBLL();

        [HttpGet]
        public JsonResult ObtenerResumenDiario()
        {
            try
            {
                ResumenPagoModel resumen = bll.ObtenerUltimaVenta();

                if (resumen != null)
                {
                    return Json(new
                    {
                        success = true,
                        totalColones = resumen.TotalPagado
                    }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { success = true, totalColones = 0 }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, error = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}