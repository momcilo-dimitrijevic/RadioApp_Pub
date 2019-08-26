using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RadioApp.Services.Api;
using RadioApp.ViewModels;
using RadioApp.Views;
using ReactiveUI;
using Splat;
using IShoutCastService = RadioApp.Services.Api.IShoutCastService;
using RadioApp.Helpers;
using RadioApp.Services;

namespace RadioApp
{
    public class AppBootstrapper : ReactiveObject, IScreen
    {
        public RoutingState Router { get; protected set; }

        public AppBootstrapper()
        {
            Router = new RoutingState();

            Locator.CurrentMutable.RegisterConstant(this, typeof(IScreen));

            RegisterServices();
            RegisterPages();
        }

        private void RegisterServices()
        {
            Locator.CurrentMutable.Register(() => ApiServiceFactory<IShoutCastService>.Instance, typeof(IShoutCastService));
            Locator.CurrentMutable.Register(() => new StationService(), typeof(IStationService));
        }

        private void RegisterPages()
        {
            Locator.CurrentMutable.Register(() => new HomePage(), typeof(IViewFor<HomeViewModel>));
        }
    }
}
