using Autofac;

namespace DashService.Framework
{
    public interface ICustomContainer
    {
        IContainer AutofacContainer
        {
            get;
        }
    }
}
