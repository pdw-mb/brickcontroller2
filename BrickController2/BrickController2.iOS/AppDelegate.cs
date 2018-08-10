﻿using Autofac;
using BrickController2.CreationManagement.DI;
using BrickController2.DeviceManagement.DI;
using BrickController2.iOS.HardwareServices.DI;
using BrickController2.UI.DI;
using Foundation;
using UIKit;

namespace BrickController2.iOS
{
    [Register("AppDelegate")]
    public partial class AppDelegate : global::Xamarin.Forms.Platform.iOS.FormsApplicationDelegate
    {
        public override bool FinishedLaunching(UIApplication uiApp, NSDictionary options)
        {
            global::Xamarin.Forms.Forms.Init();

            var container = InitDI();
            var app = container.Resolve<App>();
            LoadApplication(app);

            return base.FinishedLaunching(uiApp, options);
        }

        private IContainer InitDI()
        {
            var builder = new ContainerBuilder();

            builder.RegisterModule(new HardwareServicesModule());

            builder.RegisterModule(new CreationManagementModule());
            builder.RegisterModule(new DeviceManagementModule());
            builder.RegisterModule(new UiModule());

            return builder.Build();
        }
    }
}