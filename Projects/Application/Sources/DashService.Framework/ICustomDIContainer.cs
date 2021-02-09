using Autofac;

namespace DashService.Framework
{
    public interface ICustomDIContainer
    {
        IContainer AutofacContainer
        {
            get;
        }
    }
}
