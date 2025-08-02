using GustoySazon.BLL;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace GustoySazon.Controllers
{
    public class ResenaController : Controller
    {
        private ResenaBLL bll = new ResenaBLL();

        [HttpGet]
        public JsonResult ListarResenas()
        {
            try
            {
                var resenas = bll.ObtenerResenas();
                return Json(new { success = true, data = resenas }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, error = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}