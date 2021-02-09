using Autofac;
using DashService.Framework;

namespace DashService.Context
{
    public class CustomDIContainer : ICustomDIContainer
    {
        private static IContainer _container;

        public IContainer AutofacContainer
        {
            get => _container;
        }

        public static void SetAutofacContainer(IContainer container)
        {
            if (_container == null)
                _container = container;
        }
    }
}
