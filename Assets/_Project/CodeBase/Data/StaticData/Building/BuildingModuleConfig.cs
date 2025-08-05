using System;
using System.Collections.Generic;
using _Project.CodeBase.Data.StaticData.Building.Conditions;
using _Project.CodeBase.Gameplay.Building.Conditions;
using _Project.CodeBase.Gameplay.Building.Modules;
using _Project.CodeBase.Services.LogService;
using UnityEngine;

namespace _Project.CodeBase.Data.StaticData.Building
{
  public abstract class BuildingModuleConfig : ScriptableObject
  {
    public OperationalConditionConfig[] OperationalConditions;
    public bool IgnoreGlobalConditions;

    public abstract Type ModuleType { get; }

    public BuildingModule CreateModule(Func<Type, BuildingModule> moduleInstantiator,
      Func<Type, OperationalCondition> conditionInstantiator, BuildingConfig buildingConfig, ILogService logService)
    {
      BuildingModule module = InstantiateModule(moduleInstantiator);
      SetupModule(module, buildingConfig);

      if (OperationalConditions is null)
      {
        logService.LogWarning(GetType(),
          $"Module {module.GetType().Name} does not support condition binding or OperationalConditions is null.");
        return module;
      }

      List<OperationalCondition> localConditions = new();
      List<OperationalCondition> globalConditions = new();

      foreach (OperationalConditionConfig condition in OperationalConditions)
      {
        if (!condition.IsValidFor(module))
        {
          logService.LogError(GetType(),
            $"Condition {condition.GetType().Name} is not valid for module {module.GetType().Name}",
            new InvalidOperationException());
          continue;
        }

        OperationalCondition operationalCondition =
          condition.CreateOperationalCondition(conditionInstantiator, module);

        if (condition.IsGlobalCondition)
          globalConditions.Add(operationalCondition);
        else
          localConditions.Add(operationalCondition);
      }

      module.RegisterConditions(localConditions, globalConditions);
      return module;
    }

    protected abstract BuildingModule InstantiateModule(Func<Type, BuildingModule> instantiator);

    protected virtual void SetupModule(BuildingModule module, BuildingConfig buildingConfig)
    {
      module.Setup(IgnoreGlobalConditions);
    }
  }
}