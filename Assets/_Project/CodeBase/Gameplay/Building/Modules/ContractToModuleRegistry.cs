using System;
using System.Collections.Generic;

namespace _Project.CodeBase.Gameplay.Building.Modules
{
  public class ContractToModuleRegistry
  {
    private readonly Dictionary<Type, HashSet<Type>> _map = new();

    public void RegisterModuleContracts(Type concreteModuleType)
    {
      foreach (Type moduleInterfaceType in concreteModuleType.GetInterfaces())
        AddMapping(moduleInterfaceType, concreteModuleType);

      Type baseModuleType = concreteModuleType.BaseType;
      while (baseModuleType != null && baseModuleType != typeof(BuildingModule))
      {
        AddMapping(baseModuleType, concreteModuleType);
        baseModuleType = baseModuleType.BaseType;
      }
    }

    private void AddMapping(Type key, Type value)
    {
      if (!_map.TryGetValue(key, out HashSet<Type> moduleTypes))
      {
        moduleTypes = new HashSet<Type>();
        _map[key] = moduleTypes;
      }
      
      moduleTypes.Add(value);
    }

    public IReadOnlyCollection<Type> GetConcreteModuleTypesFor(Type contract)
      => _map.TryGetValue(contract, out HashSet<Type> moduleTypes) ? moduleTypes : Array.Empty<Type>();
    
    public bool TryGetConcreteModuleTypeFor(Type contract, out Type moduleType)
    {
      if (_map.TryGetValue(contract, out HashSet<Type> moduleTypes))
        return moduleTypes.TryGetValue(contract, out moduleType);

      moduleType = null;
      return false;
    }
  }
}