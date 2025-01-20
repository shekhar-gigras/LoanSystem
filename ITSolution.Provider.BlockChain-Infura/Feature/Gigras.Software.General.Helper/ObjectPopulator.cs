using System.Reflection;

namespace Gigras.Software.General.Helper
{
    public static class ObjectPopulator
    {
        public static void PopulateObject<T>(T obj, Dictionary<string, string> data) where T : class
        {
            if (obj == null || data == null) return;
            Type type = typeof(T);
            foreach (var entry in data)
            {
                try
                {
                    var property = type.GetProperty(entry.Key, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
                    if (property == null || !property.CanWrite) continue;
                    if (property.Name.ToLower() == "id") continue;
                    var value = entry.Value;
                    if (string.IsNullOrWhiteSpace(value) || value == "0")
                    {
                        if (property.PropertyType.IsGenericType && property.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>))
                        {
                            property.SetValue(obj, null);
                        }
                        else if (property.PropertyType == typeof(string))
                        {
                            property.SetValue(obj, null);
                        }
                        continue;
                    }

                    object convertedValue;
                    if (property.PropertyType == typeof(Guid) || property.PropertyType == typeof(Guid?))
                    {
                        convertedValue = Guid.Parse(value);
                    }
                    else
                    {
                        var targetType = Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType;
                        convertedValue = Convert.ChangeType(value, targetType);
                    }
                    property.SetValue(obj, convertedValue); property.SetValue(obj, convertedValue);
                }
                catch (Exception ex)
                {
                }
            }
        }

        public static void CopyObject<T>(T source, T target) where T : class
        {
            if (source == null || target == null) return;

            Type type = typeof(T);

            foreach (var property in type.GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                try
                {
                    if (!property.CanRead || !property.CanWrite) continue; // Skip properties that can't be read or written
                    if (property.Name.Equals("Id", StringComparison.OrdinalIgnoreCase)) continue; // Skip the ID field

                    var value = property.GetValue(source);
                    property.SetValue(target, value);
                }
                catch (Exception ex)
                {
                    // Log the exception (optional)
                    Console.WriteLine($"Error copying property {property.Name}: {ex.Message}");
                }
            }
        }

        public static void CopyObject(object source, object target)
        {
            if (source == null || target == null) return;

            var sourceType = source.GetType();
            var targetType = target.GetType();

            foreach (var sourceProperty in sourceType.GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                try
                {
                    if (!sourceProperty.CanRead) continue; // Skip properties that can't be read

                    var targetProperty = targetType.GetProperty(sourceProperty.Name, BindingFlags.Public | BindingFlags.Instance);

                    if (targetProperty == null || !targetProperty.CanWrite) continue; // Skip if the target property doesn't exist or isn't writable

                    if (targetProperty.PropertyType != sourceProperty.PropertyType) continue; // Skip if the property types don't match

                    var value = sourceProperty.GetValue(source);
                    targetProperty.SetValue(target, value);
                }
                catch (Exception ex)
                {
                    // Log the exception (optional)
                    Console.WriteLine($"Error copying property {sourceProperty.Name}: {ex.Message}");
                }
            }
        }
    }
}