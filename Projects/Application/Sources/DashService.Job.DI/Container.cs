using Autofac;
using DashService.Framework;

namespace DashService.Context.DI
{
    public class CustomContainer : ICustomContainer
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
