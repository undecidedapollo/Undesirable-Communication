using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net.Http.Headers;
using System.Web;
using System.Web.Http;
using System.Web.Routing;
using System.Web.Security;
using System.Web.SessionState;
using Undesirable_Communication.App_Start;
using Undesirable_Communication.Infrastructure;
using Undesirable_Communication.Queues;

namespace Undesirable_Communication
{
    public class Global : System.Web.HttpApplication
    {

        private BackgroundWorker connectionMakerThread;

        protected void Application_Start(object sender, EventArgs e)
        {
            GlobalConfiguration.Configure(WebApiConfig.Register);
            GlobalConfiguration.Configuration.Formatters.XmlFormatter.SupportedMediaTypes.Clear();
            GlobalConfiguration.Configuration.Formatters.JsonFormatter.SupportedMediaTypes.Add(new MediaTypeHeaderValue("text/html"));
            connectionMakerThread = new BackgroundWorker();
            connectionMakerThread.DoWork += ChatManager.ConnectionMakerThread_DoWork;
            connectionMakerThread.RunWorkerAsync();

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
            connectionMakerThread.CancelAsync();
        }
    }
}