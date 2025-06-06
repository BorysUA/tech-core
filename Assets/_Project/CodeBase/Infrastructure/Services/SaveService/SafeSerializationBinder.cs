using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace _Project.CodeBase.Infrastructure.Services.SaveService
{
  public class SafeSerializationBinder : ISerializationBinder
  {
    private readonly Dictionary<string, Type> _allowedTypes;
    private readonly HashSet<string> _assemblyTrusted;

    public SafeSerializationBinder(IEnumerable<Type> allowedTypes, HashSet<string> assemblyTrusted)
    {
      _assemblyTrusted = assemblyTrusted;
      _allowedTypes = allowedTypes.ToDictionary(type => type.FullName, type => type);
    }

    public Type BindToType(string assemblyName, string typeName)
    {
      if (_allowedTypes.TryGetValue(typeName, out Type type))
        return type;

      if (typeName.Contains('['))
      {
        var qualifiedName = Assembly.CreateQualifiedName(assemblyName, typeName);
        type = Type.GetType(qualifiedName, throwOnError: false);

        if (type?.FullName == null || !type.IsGenericType)
          throw new JsonSerializationException($"Failed to resolve type '{typeName}' from assembly '{assemblyName}'.");

        if (_allowedTypes.ContainsKey(type.GetGenericTypeDefinition().FullName) && !type.ContainsGenericParameters)
        {
          foreach (Type genericType in type.GetGenericArguments())
          {
            string genericAssembly = genericType.Assembly.GetName().Name;
            if (!_assemblyTrusted.Contains(genericAssembly))
              throw new JsonSerializationException($"Generic argument '{genericType.FullName}' is not allowed.");
          }
        }

        return type;
      }

      throw new JsonSerializationException($"Type '{typeName}' is not allowed or could not be resolved.");
    }

    public void BindToName(Type serializedType, out string assemblyName, out string typeName)
    {
      assemblyName = serializedType.Assembly.GetName().Name;
      typeName = serializedType.FullName;
    }
  }
}