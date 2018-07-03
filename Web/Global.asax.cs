using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;
using System.Web.Routing;
using log4net;

namespace VueWebTemplate.Web
{
    public class MvcApplication : System.Web.HttpApplication
    {
        private static ILog Logger;

        /// <summary>
        /// 应用启动
        /// </summary>
        protected void Application_Start()
        {
            // 配置log4net
            log4net.Config.XmlConfigurator.Configure();
            Logger = LogManager.GetLogger("LogError");

            AreaRegistration.RegisterAllAreas();
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            // 登录控制
            GlobalFilters.Filters.Add(new Models.AuthAttribute());
        }

        /// <summary>
        /// 应用出错处理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Application_Error(Object sender, EventArgs e)
        {
            Exception ex = Server.GetLastError();
            if (ex == null) return;

            var errorCode = (ex is HttpException) ? (ex as HttpException).GetHttpCode() : 500;
            if (errorCode != 404)
            {
                Logger.Error(ex);
            }

            // 不显示自定义错误信息
            var customError = System.Configuration.ConfigurationManager.GetSection("system.web/customErrors") as CustomErrorsSection;
            if (customError.Mode != CustomErrorsMode.Off) return;

            Server.ClearError();

            var httpContext = new HttpContextWrapper(Context);
            var routeData = new RouteData();
            var clientController = new Controllers.ClientController();
            var errorMsg = errorCode == 404
                    ? "您访问的地址不存在"
                    : ex.GetType().Name == typeof(Common.InfoException).Name
                        ? ex.Message
                        : "服务器错误";

            routeData.Values.Add("controller", "Page");
            routeData.Values.Add("action", "Error");
            routeData.Values.Add("errorCode", errorCode);
            routeData.Values.Add("errorMsg", errorMsg);

            ((IController)clientController).Execute(new RequestContext(httpContext, routeData));
        }
    }
}
