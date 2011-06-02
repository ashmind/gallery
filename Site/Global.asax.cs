using System;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Web.Compilation;
using System.Web.Mvc;
using System.Web.Routing;
using System.IO;

using Autofac;
using Autofac.Integration.Mvc;

using AshMind.Gallery.Core;
using AshMind.Gallery.Site.Fixes;
using AshMind.Gallery.Site.Routing;

namespace AshMind.Gallery.Site {
    public class MvcApplication : System.Web.HttpApplication {
        private static Exception startFailure;

        protected void Application_Start() {
            Start();
        }

        protected void Start() {
            try {
                AreaRegistration.RegisterAllAreas();

                this.RegisterContainer();
                RegisterRoutes(RouteTable.Routes);

                startFailure = null;
            }
            catch (Exception ex) {
                startFailure = ex;
            }
        }

        private void Restart() {
            RouteTable.Routes.Clear();
            this.Start();
        }
        
        public static void RegisterRoutes(RouteCollection routes) {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            
            routes.MapLowerCaseRoute(
                "Access",
                "access/{action}/{key}",
                new { controller = "access" },
                new { action = "(grant|revoke|impersonate)", key = "" }
            );

            routes.MapLowerCaseRoute(
                "Login",
                "login/{action}",
                new { controller = "access", action = "login" }
            );

            routes.MapLowerCaseRoute(
                "Ajax",
                "ajax/{action}",
                new { controller = "gallery" },
                new { action = "(standardalbumnames)" }
            );

            routes.MapRoute(
                "Item Action",
                "{album}/{item}/{action}",
                new { controller = "gallery" },
                new { action = "(view|comment|proposedelete|revertdelete)" }
            );

            DependencyResolver.Current.GetService<Logic.IImageRequestStrategy>()
                              .MapRoute(routes, "Image", "Get");

            routes.MapRoute(
                "Home",
                "{album}",
                new { controller = "gallery", action = "home", album = "" }
            );

            routes.MapRoute(
                 "OpenIdDiscover",
                 "login/openiddiscover",
                 new { controller = "access", action = "discover" }
             );
        }

        protected virtual void RegisterContainer() {
            var builder = new ContainerBuilder();
            builder.RegisterControllers(BuildManager.GetReferencedAssemblies().Cast<Assembly>().ToArray());
            
            var albumsRoot = ConfigurationManager.AppSettings["Albums"]
                          ?? Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);

            var picasaContactsXmlPath = Server.MapPath(ConfigurationManager.AppSettings["contacts.xml"]);

            var dataRoot = Path.Combine(Server.MapPath("~"), ".Store");
            Directory.CreateDirectory(dataRoot);

            builder.RegisterModule(new CoreModule(albumsRoot, dataRoot, picasaContactsXmlPath, () => new WebCache()));
            builder.RegisterModule(new WebModule());
            
            var container = builder.Build();
            var dependencyResolver = new AutofacDependencyResolver(container);

            DependencyResolver.SetResolver(dependencyResolver);
        }
        
        protected void Application_BeginRequest(object sender, EventArgs e) {
            if (startFailure != null)
                this.Restart();

            Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
            Thread.CurrentThread.CurrentUICulture = CultureInfo.InvariantCulture;

            if (startFailure != null) {
                Response.ContentType = "text/plain";
                Response.Output.WriteLine("Critical failure while starting up the application.");
                Response.Output.WriteLine();
                Response.Output.Write(startFailure.ToString());
                Response.Flush();
                Response.End();
            }
        }
    }
}