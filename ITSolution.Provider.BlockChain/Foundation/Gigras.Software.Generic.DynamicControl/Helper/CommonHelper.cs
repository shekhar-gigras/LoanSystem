using Gigras.Software.Database;
using Microsoft.EntityFrameworkCore;
using System.Linq.Dynamic.Core;
using System.Reflection;

namespace Gigras.Software.Generic.DynamicControl.Helper
{
    public static class CommonHelper
    {
        public static async Task<List<T>> GetDataDynamically<T>(CytContext context, string contextDbSet) where T : class
        {
            try
            {
                // Step 1: Retrieve the DbSet property
                var dbSetProperty = context.GetType().GetProperty(contextDbSet);
                if (dbSetProperty == null)
                {
                    throw new ArgumentException($"DbSet '{contextDbSet}' not found in the context.");
                }

                // Step 2: Get the DbSet instance
                var dbSet = dbSetProperty.GetValue(context);
                if (dbSet == null)
                {
                    throw new InvalidOperationException($"DbSet '{contextDbSet}' is null.");
                }

                // Step 3: Ensure the DbSet is IQueryable
                var queryableDbSet = dbSet as IQueryable<T>;
                if (queryableDbSet == null)
                {
                    throw new InvalidOperationException($"DbSet '{contextDbSet}' is not IQueryable.");
                }

                // Step 4: Execute the query and return results
                return await queryableDbSet.ToListAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetDataDynamically: {ex.Message}");
                return new List<T>();
            }
        }

        public static object? GetDataDynamically(Type entityType, DbContext context, string dbsetname, string condition)
        {
            try
            {
                // Get the DbSet for the given entity type
                var dbSetProperty = context.GetType()
                    .GetProperties()
                    .FirstOrDefault(p => p.PropertyType.IsGenericType &&
                                         p.PropertyType.GetGenericTypeDefinition() == typeof(DbSet<>) &&
                                         string.Equals(p.Name, dbsetname, StringComparison.OrdinalIgnoreCase));

                if (dbSetProperty == null)
                    return null;

                var dbSet = dbSetProperty.GetValue(context) as IQueryable;
                if (dbSet == null)
                    return null;

                // Apply the dynamic condition
                var filteredQuery = dbSet;
                if (!string.IsNullOrEmpty(condition))
                {
                    filteredQuery = dbSet.Where(condition);
                }

                // Convert filteredQuery to IEnumerable<T>
                var castMethod = typeof(Queryable)
                    .GetMethod("Cast", BindingFlags.Static | BindingFlags.Public)?
                    .MakeGenericMethod(entityType);

                if (castMethod == null)
                    throw new Exception("Unable to find Queryable.Cast<T> method.");

                var enumerableQuery = castMethod.Invoke(null, new object[] { filteredQuery });

                // Call ToList dynamically
                var toListMethod = typeof(Enumerable)
                    .GetMethod("ToList", BindingFlags.Static | BindingFlags.Public)?
                    .MakeGenericMethod(entityType);

                if (toListMethod == null)
                    throw new Exception("Unable to find Enumerable.ToList<T> method.");

                return toListMethod.Invoke(null, new object[] { enumerableQuery });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetDataDynamically: {ex.Message}");
                return null;
            }
        }

        public static Type? GetDbSetType(string dbsetname)
        {
            return typeof(CytContext)
                .GetProperties()
                .Where(p => p.PropertyType.IsGenericType &&
                            p.PropertyType.GetGenericTypeDefinition() == typeof(DbSet<>) &&
                            string.Equals(p.Name, dbsetname, StringComparison.OrdinalIgnoreCase))
                .Select(p => p.PropertyType.GenericTypeArguments.FirstOrDefault())
                .FirstOrDefault();
        }
    }
}