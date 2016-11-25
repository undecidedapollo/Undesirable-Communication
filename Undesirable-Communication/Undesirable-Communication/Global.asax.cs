using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Web;
using System.Web.Http;
using System.Web.Security;
using System.Web.SessionState;
using Undesirable_Communication.App_Start;

namespace Undesirable_Communication
{
    public class Global : System.Web.HttpApplication
    {

        protected void Application_Start(object sender, EventArgs e)
        {
            GlobalConfiguration.Configure(WebApiConfig.Register);
            GlobalConfiguration.Configuration.Formatters.XmlFormatter.SupportedMediaTypes.Clear();
            GlobalConfiguration.Configuration.Formatters.JsonFormatter.SupportedMediaTypes.Add(new MediaTypeHeaderValue("text/html"));

        }

        protected void Session_Start(object sender, EventArgs e)
        {

        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {

        }

        protected void Application_AuthenticateRequest(object sender, EventArgs e)
        {

        }

        protected void Application_Error(object sender, EventArgs e)
        {

        }

        protected void Session_End(object sender, EventArgs e)
        {

        }

        protected void Application_End(object sender, EventArgs e)
        {

        }
    }
}