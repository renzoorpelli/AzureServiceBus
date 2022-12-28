using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Generic
{
    public interface IGenericRepository<T> where T : class
    {
        /// <summary>
        /// metodo encargado de crear un recurso en la base de datos
        /// </summary>
        /// <param name="entity">Entidad generica, la cual se especializa donde ser[a su tabla de destino con el metodo set()</param>
        /// <returns></returns>
        Task CreateAsync(T entity);
        Task<IEnumerable<T>> GetAsync();

        /// <summary>
        /// metodo encargado de obtener datos hacia la BD. utilizando entidades genericas
        /// </summary>
        /// <param name="whereClause">query del tipo WHERE, espera un delegado que haga referencia a un metodo que cumpla la funcion de filtrar datros de consulta</param>
        /// <param name="orderByClause">query del tipo ORDEBY, espera delegado que haga referencia a un metodo que cumpla con la condicion de ordenamiento</param>
        /// <param name="includeProperties">nombre de la propiedad que se quiere integrar en la consulta del metodo Include()</param>
        /// <returns></returns>
        Task<IEnumerable<T>> GetAsync(Expression<Func<T, bool>> whereClause = null,
            Func<IQueryable<T>, IOrderedQueryable<T>> orderByClause = null,
            string includeProperties = "");
    }
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        private readonly IUnitOfWork _unitOfWork;

        public GenericRepository(IUnitOfWork unitOfWork)
        {
            this._unitOfWork = unitOfWork;
        }


        public async Task<IEnumerable<T>> GetAsync()
        {
            return await this._unitOfWork.Context.Set<T>().ToListAsync();
        }

        public async Task<IEnumerable<T>> GetAsync(Expression<Func<T, bool>> whereClause = null,
            Func<IQueryable<T>, IOrderedQueryable<T>> orderByClause = null,
            string includeProperties = "")
        {

            IQueryable<T> query = this._unitOfWork.Context.Set<T>();

            if (whereClause is not null)
            {
                query = query.Where(whereClause);
            }

            if (includeProperties is not null)
            {
                foreach (var property in includeProperties.Split(',', StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(property);
                }
            }

            if (orderByClause is not null)
            {
                return await orderByClause(query).ToListAsync();
            }
            else
            {
                return await query.ToListAsync();
            }
        }

        public async Task CreateAsync(T entity)
        {
            try
            {
                await _unitOfWork.Context.Set<T>().AddAsync(entity);
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
