using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.ServiceModel.Activation;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Microsoft.IdentityModel.Web;
using Thinktecture.IdentityServer.Repositories;
using Thinktecture.IdentityServer.TokenService;
using Thinktecture.IdentityServer.Web.GlobalFilter;

namespace Thinktecture.IdentityServer.Web
{
    public class MvcApplication : HttpApplication
    {
        private const string ConfigurationName = "WebSite";
        
        [Import]
        public IConfigurationRepository ConfigurationRepository { get; set; }

        protected void Application_Start()
        {
            SetupCompositionContainer();
            Container.Current.SatisfyImportsOnce(this);
            
            FederatedAuthentication.ServiceConfigurationCreated += (s, e) =>
                {
                    if (!string.IsNullOrWhiteSpace(ConfigurationRepository.SigningCertificate.SubjectDistinguishedName))
                    {
                        e.ServiceConfiguration.SecurityTokenHandlers.AddOrReplace(
                            new X509CertificateSessionSecurityTokenHandler(ConfigurationRepository.SigningCertificate.Certificate));
                    }
                };

            AreaRegistration.RegisterAllAreas();
            RegisterGlobalFilters(GlobalFilters.Filters);
            RegisterRoutes(RouteTable.Routes);
        }

        private void SetupCompositionContainer()
        {
            Container.Current = new CompositionContainer(new RepositoryExportProvider());
        }

        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
            filters.Add(new GlobalViewModelFilter());
            filters.Add(new SslRedirectFilter());
            filters.Add(new InitialConfigurationFilter());
        }

        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                "FederationMetadata",
                "FederationMetadata/2007-06/FederationMetadata.xml",
                new { controller = "FederationMetadata", action = "Generate" }
            );

            routes.MapRoute(
                "RelyingPartiesAdmin",
                "admin/relyingparties/{action}/{id}",
                new { controller = "RelyingPartiesAdmin", action = "Index", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                "ClientCertificatesAdmin",
                "admin/clientcertificates/{action}/{userName}",
                new { controller = "ClientCertificatesAdmin", action = "Index", userName = UrlParameter.Optional }
            );

            routes.MapRoute(
                "DelegationAdmin",
                "admin/delegation/{action}/{userName}",
                new { controller = "DelegationAdmin", action = "Index", userName = UrlParameter.Optional }
            );

            routes.MapRoute(
                "Default", 
                "{controller}/{action}/{id}", 
                new { controller = "Home", action = "Index", id = UrlParameter.Optional },
                new { controller = "^(?!issue).*" }
            );

            // ws-federation (mvc)
            routes.MapRoute(
                "wsfederation",
                "issue/wsfed",
                new { controller = "WSFederation", action = "issue" }
            );

            // jsnotify (mvc)
            routes.MapRoute(
                "jsnotify",
                "issue/jsnotify",
                new { controller = "JSNotify", action = "issue" }
            );

            // simple http (mvc)
            routes.MapRoute(
                "simplehttp",
                "issue/simple",
                new { controller = "SimpleHttp", action = "issue" }
            );

            // oauth wrap (mvc)
            routes.MapRoute(
                "wrap",
                "issue/wrap",
                new { controller = "Wrap", action = "issue" }
            );

            // oauth2 (mvc)
            routes.MapRoute(
                "oauth2",
                "issue/oauth2/{action}",
                new { controller = "OAuth2", action = "token" }
            );

            // ws-trust (wcf)
            routes.Add(new ServiceRoute(
                "issue/wstrust",
                new TokenServiceHostFactory(),
                typeof(TokenServiceConfiguration)));
        }

        protected void Application_EndRequest(object sender, EventArgs e)
        {
            var context = (sender as HttpApplication).Context;

            if (context.Response.StatusCode == 401)
            {
                var noRedirect = context.Items["NoRedirect"];

                if (noRedirect == null)
                {
                    var route = new RouteValueDictionary(new Dictionary<string, object>
                        {
                            { "Controller", "Account" },
                            { "Action", "SignIn" },
                            { "ReturnUrl", HttpUtility.UrlEncode(context.Request.RawUrl, context.Request.ContentEncoding) }
                        });

                    Response.RedirectToRoute(route);
                }
            }
        }
    }
}