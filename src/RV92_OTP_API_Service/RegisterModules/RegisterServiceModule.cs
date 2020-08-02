// Configure Dependency Injection

using Autofac;
using RV92.Otp.Api.Service.Implementation;
using RV92.Otp.Api.Service.Interface;

namespace RV92.Otp.Api.Service.RegisterModules
{
    public class RegisterServiceModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            #region register services

            builder.RegisterType<ClickATellService>().As<IClickATellService>();

            #endregion
        }
    }
}