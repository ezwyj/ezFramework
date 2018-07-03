using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace VueWebTemplate.Web.Models
{
    /// <summary>
    /// 登录认证属性
    /// </summary>
    public class AuthAttribute : AuthorizeAttribute
    {
        /// <summary>
        /// 验证授权
        /// 如果controller或action包含NoAuthAttribute属性，则不验证授权
        /// </summary>
        /// <param name="filterContext"></param>
        /// <returns></returns>
        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            if (filterContext == null)
            {
                throw new ArgumentNullException("filterContext");
            }

            // 如果controller或action包含MPAllowAnonymous属性，则不验证授权
            var actionFilter = filterContext.ActionDescriptor.GetCustomAttributes(typeof(AllowAnonymousAttribute), false);
            var controllerFilter = filterContext.ActionDescriptor.ControllerDescriptor.GetCustomAttributes(typeof(AllowAnonymousAttribute), false);
            if (actionFilter.Length == 1 || controllerFilter.Length == 1)
            {
                return;
            }

            // 最终执行默认的授权认证
            base.OnAuthorization(filterContext);
        }

        /// <summary>
        /// 授权失败处理
        /// 如果为Ajax请求，则返回登录失败信息
        /// 否则跳转到登录页
        /// </summary>
        /// <param name="filterContext"></param>
        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            filterContext.Result = new JsonResult { Data = new { State = false, Code = 401, Msg = "请登录" }, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }
    }
}