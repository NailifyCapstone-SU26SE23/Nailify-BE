using Microsoft.EntityFrameworkCore;
using Nailify.Capstone.Application.Common;
using Nailify.Capstone.Application.Interfaces.RepositoryInterfaces;
using Nailify.Capstone.Infrastructure.DBContext;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Nailify.Capstone.Infrastructure.Repository
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        protected readonly NailifyDbContext _context;
        protected readonly DbSet<T> _dbSet;
        #region CTOR
        public GenericRepository(NailifyDbContext context)
        {
            _context = context;
            _dbSet = _context.Set<T>();
        }
        #endregion CTOR
        /// <summary>
        /// Creates a new entity in the database asynchronously.
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task<T> CreateAsync(T entity)
        {
            await _dbSet.AddAsync(entity);
            return entity;
        }

        /// <summary>
        /// Deletes an entity from the database.
        /// </summary>
        /// <param name="entity"></param>
        public void Delete(T entity)
        {
            var statusProperty = typeof(T).GetProperty("Status",
                               System.Reflection.BindingFlags.Public |
                               System.Reflection.BindingFlags.Instance |
                               System.Reflection.BindingFlags.IgnoreCase);

            if (statusProperty != null && statusProperty.CanWrite)
            {
                var propertyType = statusProperty.PropertyType;

                if (propertyType == typeof(string))
                {
                    statusProperty.SetValue(entity, "Inactive");
                    _dbSet.Update(entity);
                }
                else if (propertyType == typeof(bool))
                {
                    statusProperty.SetValue(entity, false);
                    _dbSet.Update(entity);
                }
            }
            else
            {
                _dbSet.Remove(entity);
            }
        }

        /// <summary>
        /// Checks if any entity exists in the database that matches the given predicate asynchronously. Predicate is a condition to filter entities.
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public async Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate) => await _dbSet.AnyAsync(predicate);

        /// <summary>
        /// Find All (Ko include => ko await/async)
        /// </summary>
        /// <param name="trackChanges"></param>
        /// <returns></returns>
        public IQueryable<T> FindAll(bool trackChanges = false) => !trackChanges ? _context.Set<T>().AsNoTracking()
        : _context.Set<T>();

        /// <summary>
        /// Gets all entities from the database asynchronously.
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<T>> FindAllAsync(params Expression<Func<T, object>>[] includes)
        {
            IQueryable<T> query = _dbSet;

            // Kiểm tra xem includes có giá trị và có phần tử nào không
            if (includes != null && includes.Any())
            {
                foreach (var include in includes)
                {
                    query = query.Include(include);
                }
            }

            return await query.ToListAsync();
        }

        /// <summary>
        /// Find By Conditon (Ko can await/async)
        /// </summary>
        /// <param name="expression"></param>
        /// <param name="trackChanges"></param>
        /// <returns></returns>
        public IQueryable<T> FindByCondition(Expression<Func<T, bool>> expression, bool trackChanges = false) => !trackChanges ? _context.Set<T>().Where(expression).AsNoTracking()
     : _context.Set<T>().Where(expression);

        /// <summary>
        /// Gets an entity by its unique identifier asynchronously. 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<T?> GetByIdAsync(Guid id)
        {
            var entity = await _dbSet.FindAsync(id);
            return IsActiveEntity(entity) ? entity : null;
        }

        public async Task<T?> GetByIdAsync(int id)
        {
            var entity = await _dbSet.FindAsync(id);
            return IsActiveEntity(entity) ? entity : null;
        }

        public async Task<PagedList<T>> GetPagedAsync(int pageNumber, int pageSize,
       Expression<Func<T, bool>>? predicate = null,
       params Expression<Func<T, object>>[] includes)
        {
            IQueryable<T> query = _context.Set<T>();

            query = ApplyActiveStatusFilter(query);

            if (predicate != null) query = query.Where(predicate);
            foreach (var include in includes ?? [])
                query = query.Include(include);

            if (typeof(T).GetProperty("OrderIndex")?.PropertyType == typeof(int))
                query = query.OrderBy(x => EF.Property<int>(x, "OrderIndex"));

            var count = await query.CountAsync();
            var items = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PagedList<T>(items, count, pageNumber, pageSize);
        }

        /// <summary>
        /// Updates an existing entity in the database.
        /// </summary>
        /// <param name="entity"></param>
        public void Update(T entity) => _dbSet.Update(entity);

        private static IQueryable<T> ApplyActiveStatusFilter(IQueryable<T> query)
        {
            var statusProperty = typeof(T).GetProperty("Status",
                System.Reflection.BindingFlags.Public |
                System.Reflection.BindingFlags.Instance |
                System.Reflection.BindingFlags.IgnoreCase);

            if (statusProperty?.PropertyType == typeof(string))
            {
                return query.Where(entity => EF.Property<string>(entity, statusProperty.Name) == "Active" || EF.Property<string>(entity, statusProperty.Name) == "Open");
            }

            if (statusProperty?.PropertyType == typeof(bool))
            {
                return query.Where(entity => EF.Property<bool>(entity, statusProperty.Name));
            }

            return query;
        }

        private static bool IsActiveEntity(T? entity)
        {
            if (entity == null)
            {
                return false;
            }

            var statusProperty = typeof(T).GetProperty("Status",
                System.Reflection.BindingFlags.Public |
                System.Reflection.BindingFlags.Instance |
                System.Reflection.BindingFlags.IgnoreCase);

            if (statusProperty?.PropertyType == typeof(string))
            {
                var val = statusProperty.GetValue(entity) as string;
                return string.Equals(val, "Active", StringComparison.OrdinalIgnoreCase) ||
                       string.Equals(val, "Open", StringComparison.OrdinalIgnoreCase);
            }

            if (statusProperty?.PropertyType == typeof(bool))
            {
                return statusProperty.GetValue(entity) is true;
            }

            return true;
        }
    }
}
