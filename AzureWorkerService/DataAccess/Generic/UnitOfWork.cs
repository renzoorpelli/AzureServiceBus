using Entidades.DataContext;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Generic
{
    public interface IUnitOfWork
    {
        /// <summary>
        /// propiedad  que nos otorgara el acceso a la base de datos
        /// </summary>
        AzureServiceBusContext Context { get; }

        /// <summary>
        /// metodo encargado de realizar el commit (guardar cambios) a la base de datos
        /// </summary>
        void Commit();
    }
    public class UnitOfWork : IUnitOfWork
    {
        public AzureServiceBusContext Context { get; }

        public UnitOfWork(AzureServiceBusContext context)
        {
            Context = context;
        }

        public void Commit()
        {
            Context.SaveChanges();
        }

        /// <summary>
        /// metodo encargado de liberar memoria a los recursos asignados para el contexto
        /// </summary>
        public void Dispose()
        {
            Context.Dispose();
        }
    }
}
