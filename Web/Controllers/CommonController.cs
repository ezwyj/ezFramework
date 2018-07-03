using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;


namespace Web.Controllers
{
    public class CommonController : Controller
    {
        /// <summary>
        /// 登录
        /// 开发阶段模拟用
        /// </summary>
        /// <param name="badge"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost]
        public ActionResult Login(string badge)
        {
            System.Web.Security.FormsAuthentication.SetAuthCookie(badge, false);
            return Json(new { State = true });
        }

        /// <summary>
        /// 登出
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Logout()
        {
            Session.Clear();
            System.Web.Security.FormsAuthentication.SignOut();
            return Json(new { State = true });
        }

        /// <summary>
        /// 获取用户信息
        /// </summary>
        /// <returns></returns>
        public ActionResult GetUserInfo()
        {
            var badge = HttpContext.User.Identity.Name;
            if (string.IsNullOrEmpty(badge))
            {
                return Json(new { State = false, Code = 401, Msg = "请登录" }, JsonRequestBehavior.AllowGet);
            }
            //var user = UserEntity.GetSingle(HttpContext.User.Identity.Name);
            // var rights = UserEntity.GetAllRightList(badge); 这里可以将用户的权限和用户信息一起返回到页面，这样页面可以做权限管理
            return Json(new { State = true, Data = new { User = ""/*, Rights = rights*/ } }, JsonRequestBehavior.AllowGet);
        }
    }
}