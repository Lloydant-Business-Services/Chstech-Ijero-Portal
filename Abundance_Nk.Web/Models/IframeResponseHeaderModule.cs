using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Abundance_Nk.Web.Models
{
    public class IframeResponseHeaderModule : IHttpModule
    {
        public void Dispose()
        {
            this.Dispose();
        }

        public void Init(HttpApplication context)
        {
            context.PreSendRequestHeaders += Context_PreSendRequestHeaders;
        }

        private void Context_PreSendRequestHeaders(object sender, EventArgs e)
        {
            HttpContext context = ((HttpApplication)sender).Context;
            context.Response.AddHeader("X-Frame-Options", "SAMEORIGIN");
        }
    }
}