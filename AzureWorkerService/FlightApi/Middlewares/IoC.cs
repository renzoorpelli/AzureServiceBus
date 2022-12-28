using DataAccess.Generic;

namespace FlightApi.Middlewares
{
    public static class IoC
    {
        public static IServiceCollection AddDependency(this IServiceCollection services)
        {
            services.AddScoped <IUnitOfWork, UnitOfWork> ();
            services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));

            return services;
        }
    }
}
