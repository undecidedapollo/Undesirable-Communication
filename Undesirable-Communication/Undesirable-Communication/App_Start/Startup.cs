using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(Undesirable_Communication.App_Start.Startup))]
namespace Undesirable_Communication.App_Start
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            
        }
    }
}