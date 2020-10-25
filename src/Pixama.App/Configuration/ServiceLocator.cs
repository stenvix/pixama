using Autofac;
using Pixama.Logic.Services;
using Pixama.Logic.ViewModels.Photo;
using Pixama.Logic.ViewModels.Shell;
using System.Collections.Concurrent;
using Windows.UI.ViewManagement;
using Pixama.Logic.ViewModels.Pages;

namespace Pixama.App.Configuration
{
    public class ServiceLocator
    {
        private static readonly ConcurrentDictionary<int, ServiceLocator> ServiceLocators;
        private IContainer _container;
        private readonly ILifetimeScope _lifetimeScope;

        static ServiceLocator()
        {
            ServiceLocators = new ConcurrentDictionary<int, ServiceLocator>();
        }

        private ServiceLocator()
        {
            if (_container == null) Configure();
            _lifetimeScope = _container.BeginLifetimeScope();
        }

        private void Configure()
        {
            var builder = new ContainerBuilder();
            builder.RegisterType<FrameAdapter>().AsImplementedInterfaces().SingleInstance();
            builder.RegisterType<NavigationService>().AsImplementedInterfaces().SingleInstance();
            builder.RegisterType<LocationService>().AsImplementedInterfaces().SingleInstance();
            builder.RegisterType<PhotoService>().AsImplementedInterfaces().SingleInstance();
            builder.RegisterType<DialogService>().AsImplementedInterfaces().SingleInstance();

            builder.RegisterType<ShellViewModel>().InstancePerLifetimeScope();
            builder.RegisterType<PhotoViewModel>().InstancePerLifetimeScope();
            builder.RegisterType<PhotoGridViewModel>().InstancePerLifetimeScope();
            builder.RegisterType<SourcePageViewModel>().InstancePerLifetimeScope();
            _container = builder.Build();
        }

        public static ServiceLocator Current => GetForView();

        private static ServiceLocator GetForView()
        {
            var id = ApplicationView.GetForCurrentView().Id;
            return ServiceLocators.GetOrAdd(id, _ => new ServiceLocator());
        }

        public T GetService<T>()
        {
            return _lifetimeScope.Resolve<T>();
        }
    }
}
