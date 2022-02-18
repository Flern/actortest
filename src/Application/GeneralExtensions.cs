using System.Reflection;

namespace AbstractExample;

public static class GeneralExtensions
{
    // Enable nullable checks section
#nullable enable

    public static object? DeepCopy(this object obj, Dictionary<string, object>? overrides = null, object? copyTarget = null)
    {
        // Get the type of the input object
        Type type = obj.GetType();

        // Set up target object for the copy operation
        var objectReference = copyTarget ?? typeof(object).GetMethod("MemberwiseClone", BindingFlags.NonPublic | BindingFlags.Instance)?
                                                            .Invoke(obj, null);

        // Check for a valid copy target
        if (objectReference is null) return default;

        // Get property set for writing object to guard against differing types
        PropertyInfo[] targetProps = objectReference.GetType().GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                                        .Where(p => p.GetIndexParameters().Length == 0)
                                        .ToArray();

        // Clone properties - we are leaving fields alone here
        PropertyInfo[] properties = type.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                                        .Where(p => p.GetIndexParameters().Length == 0)
                                        .ToArray();

        // Loop and copy
        for (int i = 0; i < properties.Length; i++)
        {
            PropertyInfo property = properties[i];

            // Check for properties that don't match source and target objects
            if (targetProps.All(prop => prop.Name != property.Name || prop.GetType() != property.GetType()))
            {
                continue;
            }

            // Get source value
            var propertyValue = overrides?.ContainsKey(property.Name) != true
                    ? property.GetValue(obj)
                    : overrides[property.Name];

            // Set copy provider based on public/private setter
            MemberInfo? copyProvider = property.CanWrite && (property.GetSetMethod(true)?.IsPublic ?? false)
                ? property
                : property.DeclaringType?.GetField($"<{property.Name}>k__BackingField", BindingFlags.Instance | BindingFlags.NonPublic);

            // Check for nulls
            if (propertyValue is null || copyProvider is null) continue;

            // Get value to copy
            object? valueToCopy = propertyValue.GetType().IsValueType ||
                                 propertyValue.GetType().Equals(typeof(string)) ||
                                 propertyValue.GetType().IsEnum
                ? propertyValue
                : propertyValue.DeepCopy(overrides);

            // Make sure we have something to copy
            if (valueToCopy is null) continue;

            // Copy the value based on the copyProvider type
            switch (copyProvider.MemberType)
            {
                case MemberTypes.Property:
                    (copyProvider as PropertyInfo)?.SetValue(objectReference, valueToCopy);
                    break;
                case MemberTypes.Field:
                    (copyProvider as FieldInfo)?.SetValue(objectReference, valueToCopy);
                    break;
            }
        }

        // Return object if needed
        return objectReference;
    }

    // End nullable checks section
#nullable disable

}
