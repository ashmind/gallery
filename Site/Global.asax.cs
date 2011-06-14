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

using AshMind.Extensions;

using AshMind.IO.Abstraction.DefaultImplementation;

using AshMind.Gallery.Core;
using AshMind.Gallery.Core.Security;
using AshMind.Gallery.Imaging.GdiPlus;
using AshMind.Gallery.Integration.Picasa;
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
                new { controller = "album" },
                new { action = "(standardnames)" }
            );

            routes.MapRoute(
                "Album Action",
                "{albumID}/{action}",
                new { controller = "album" },
                new { action = "download" }
            );

            routes.MapRoute(
                "Item Action",
                "{album}/{item}/{action}",
                new { controller = "albumitem" },
                new { action = "(view|proposedelete|revertdelete)" }
            );

            DependencyResolver.Current.GetService<Logic.IImageRequestStrategy>()
                              .MapRoute(routes, "Image", "Get");

            routes.MapRoute(
                "Home",
                "{album}",
                new { controller = "album", action = "gallery", album = "" }
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

            var fileSystem = new FileSystem();
            var albumLocation = fileSystem.GetLocation(albumsRoot);
            var dataLocation = fileSystem.GetLocation(dataRoot);
            var picasaContactsXmlFile = picasaContactsXmlPath.IsNotNullOrEmpty() ? fileSystem.GetFile(picasaContactsXmlPath) : null;

            // TODO: enable autodiscovery
            builder.RegisterModule(new CoreModule(albumLocation, dataLocation, () => new WebCache()));
            builder.RegisterModule(new SecurityModule(dataLocation));
            builder.RegisterModule(new PicasaModule(picasaContactsXmlFile));
            builder.RegisterModule(new GdiImagingModule());
            builder.RegisterModule(new WebModule());

            builder.RegisterInstance(dataLocation);
            
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