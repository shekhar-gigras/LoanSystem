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
    }
}