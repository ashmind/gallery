using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Web;
using System.Web.Compilation;
using System.Web.Mvc;
using System.Web.Routing;
using System.IO;

using AshMind.Extensions;

using AshMind.Web.Gallery.Core;
using AshMind.Web.Gallery.Site.Models;

using Autofac;
using Autofac.Builder;
using Autofac.Modules;
using Autofac.Integration.Web;
using Autofac.Integration.Web.Mvc;
using AshMind.Web.Gallery.Core.Access;
using System.Security.Principal;
using System.Configuration;

namespace AshMind.Web.Gallery.Site {
    public class MvcApplication : System.Web.HttpApplication {
        private static ContainerProvider containerProvider;
        private static Exception startFailure;

        public static IContainer RequestContainer {
            get { return containerProvider.RequestContainer; }
        }

        protected void Application_Start() {
            Start();
        }

        protected void Start() {
            try {
                AreaRegistration.RegisterAllAreas();

                RegisterRoutes(RouteTable.Routes);
                this.RegisterContainer();

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
            
            routes.MapRoute(
                "Login",
                "login/{action}",
                new { controller = "Access", action = "Login" }
            );

            routes.MapRoute(
                "Image",
                "{album}/{item}/{size}",
                new { controller = "Image", action = "Get", size = ImageSize.Original }
            );

            routes.MapRoute(
                "Home",
                "{album}",
                new { controller = "Gallery", action = "Home", album = "" }
            );

            routes.MapRoute(
                "OpenIdDiscover",
                "openiddiscover",
                new { controller = "Access", action = "Discover" }
            );
        }

        protected virtual void RegisterContainer() {
            var builder = new ContainerBuilder();

            builder.RegisterModule(new ImplicitCollectionSupportModule());
            builder.RegisterModule(new AutofacControllerModule(
                BuildManager.GetReferencedAssemblies().Cast<Assembly>().ToArray()
            ));

            var albumsRoot = ConfigurationManager.AppSettings["Albums"]
                          ?? Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);

            var dataRoot = Path.Combine(Server.MapPath("~"), ".Store");
            Directory.CreateDirectory(dataRoot);

            builder.RegisterModule(new CoreModule(albumsRoot, dataRoot));
            builder.RegisterModule(new WebModule());

            containerProvider = new ContainerProvider(builder.Build());
            ControllerBuilder.Current.SetControllerFactory(new AutofacControllerFactory(containerProvider));
        }

        private void DiscoverAllModules(ContainerBuilder builder) {
            var path = Server.MapPath("~/bin");

            foreach (var file in Directory.GetFiles(path, "*.dll")) {
                if (!file.Contains("Web.Gallery."))
                    continue;

                var assembly = Assembly.LoadFrom(file);
                var modules = from type in assembly.GetTypes()
                              where typeof(IModule).IsAssignableFrom(type)
                              select (IModule)Activator.CreateInstance(type);

                modules.ForEach(builder.RegisterModule);
            }
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

        protected void Application_AuthenticateRequest(object sender, EventArgs e) {
            var principal = HttpContext.Current.User;
            if (principal == null || !principal.Identity.IsAuthenticated)
                return;

            var userRepository = RequestContainer.Resolve<IUserRepository>();
            var user = userRepository.Load(principal.Identity.Name);
            HttpContext.Current.User = new GenericPrincipal(principal.Identity, new string[0]);
        }

        protected void Application_EndRequest(object sender, EventArgs e) {
            if (startFailure != null)
                return;

            if (containerProvider == null)
                return;

            containerProvider.DisposeRequestContainer();
        }   
    }
}