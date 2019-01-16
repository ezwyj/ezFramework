using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Web.Controllers
{
    public class DefaultController : Controller
    {
        /// <summary>
        /// 页面
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            return View("~/Views/index.cshtml");
        }

        /// <summary>
        /// 错误返回
        /// </summary>
        /// <returns></returns>
        public ActionResult Error(string errorCode, string errorMsg)
        {
            return Json(new { State = false, Code = errorCode, Msg = errorMsg }, JsonRequestBehavior.AllowGet);
        }
    }
}