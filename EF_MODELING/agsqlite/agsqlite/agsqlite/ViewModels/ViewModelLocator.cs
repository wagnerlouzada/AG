/// Mohamed Ali NOUIRA
/// http://www.sweetmit.com
/// http://www.mohamedalinouira.com
/// https://github.com/medalinouira
/// Copyright © Mohamed Ali NOUIRA. All rights reserved.

using Autofac;
using agsqlite.Models;
using agsqlite.Services;
using agsqlite.IServices;
using agsqlite.IViewModels;
using agsqlite.Repositories;
using Xamarin.Forms.Popups;
using Xamarin.Forms.Internals;
using Xamarin.Forms.Navigation;

namespace agsqlite.ViewModels
{
    public class ViewModelLocator
    {
        private IContainer _container;

        [Preserve]
        public ViewModelLocator()
        {
            ContainerBuilder builder = new ContainerBuilder();

            builder.RegisterType<HomeViewModel>().As<IHomeViewModel>().SingleInstance();
            builder.RegisterType<DetailsViewModel>().As<IDetailsViewModel>().SingleInstance();

            builder.RegisterType<UserServices>().As<IUserServices>().SingleInstance();
            builder.RegisterType<PopupsService>().As<IPopupsService>().SingleInstance();
            builder.RegisterType<NavigationService>().As<INavigationService>().SingleInstance();
            builder.RegisterType<Repository<TodoItem>>().As<IRepository<TodoItem>>().SingleInstance();

            _container = builder.Build();
        }

        public IHomeViewModel Home
        {
            get
            {
                return _container.Resolve<IHomeViewModel>();
            }
        }

        public IDetailsViewModel Details
        {
            get
            {
                return _container.Resolve<IDetailsViewModel>();
            }
        }
    }
}
