using Autofac;
using RV92.Otp.Api.Service.RegisterModules;

namespace RV92.Otp.WebApi.Infrastructure.Installers
{
    /// <summary>
    /// Configure Dependency Injection
    /// </summary>
    public class RegisterContainer
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="builder"></param>
        public static void RegisterModules(ContainerBuilder builder)
        {
            #region register external services
            builder.RegisterModule<RegisterServiceModule>();
            #endregion
        }
    }
}