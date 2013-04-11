﻿using System;
using System.ServiceModel.Activation;
using System.Web;
using System.Web.Routing;

namespace ServicePleaseService
{
    public class Global : HttpApplication
    {
        void Application_Start(object sender, EventArgs e)
        {
            RegisterRoutes();
        }

        private void RegisterRoutes()
        {
            RouteTable.Routes.Add(new ServiceRoute( "", 
                                                    new WebServiceHostFactory(), 
                                                    typeof(ServicePleaseServiceLibrary.ServicePleaseService)));
        }
    }
}
