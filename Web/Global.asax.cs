using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using System.Web.Http;
using System.Web.Routing;

namespace Web
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        private static ILog Logger;
        protected void Application_Start()
        {
            log4net.Config.XmlConfigurator.Configure();
            Logger = LogManager.GetLogger("LogError");

            
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
            var clientController = new System.Web.Http.Controllers.ClientController();
            var errorMsg = errorCode == 404
                    ? "您访问的地址不存在"
                    : ex.GetType().Name == typeof(Common.InfoException).Name
                        ? ex.Message
                        : "服务器错误";

            routeData.Values.Add("controller", "Client");
            routeData.Values.Add("action", "Error");
            routeData.Values.Add("errorCode", errorCode);
            routeData.Values.Add("errorMsg", errorMsg);

            ((ApiController)clientController).Execute(new RequestContext(httpContext, routeData));
        }
    }
}
