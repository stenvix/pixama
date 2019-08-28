﻿using Autofac;
using Pixama.ViewModels.ViewModels.Photo;
using System.Collections.Concurrent;
using Windows.UI.ViewManagement;
using Pixama.ViewModels.ViewModels.Shell;

namespace Pixama.App.Services
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
            //builder.RegisterType<FileSystemService>().AsImplementedInterfaces().SingleInstance();
            //builder.RegisterType<PhotoService>().AsImplementedInterfaces().SingleInstance();
            builder.RegisterType<ShellViewModel>().InstancePerLifetimeScope();
            builder.RegisterType<PhotoViewModel>().InstancePerLifetimeScope();
            //builder.RegisterType<PhotoGridViewModel>().InstancePerLifetimeScope();
            _container = builder.Build();
        }

        public static ServiceLocator Current => GetForView();

        private static ServiceLocator GetForView()
        {
            var id = ApplicationView.GetForCurrentView().Id;
            return ServiceLocators.GetOrAdd(id, new ServiceLocator());
        }

        public T GetService<T>()
        {
            return _lifetimeScope.Resolve<T>();
        }
    }
}
