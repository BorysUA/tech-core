// using System;
// using System.Collections.Generic;
// using _Project.CodeBase.Data.StaticData.Building.Conditions;
// using _Project.CodeBase.Gameplay.Building.Conditions;
// using _Project.CodeBase.Gameplay.Building.Modules;
// using _Project.CodeBase.Services.LogService;
//
// namespace _Project.CodeBase.Data.StaticData.Building.Modules
// {
//   public abstract class BuildingModuleWithConditionsConfig : BuildingModuleConfig
//   {
//     public OperationalConditionConfig[] OperationalConditions;
//
//     public BuildingModule CreateModuleWithConditions(Func<Type, BuildingModule> buildingInstantiator,
//       Func<Type, OperationalCondition> conditionInstantiator, BuildingConfig buildingConfig, ILogService logService)
//     {
//       //BuildingModule module = CreateModule(buildingInstantiator, buildingConfig);
//       BuildingModule module = CreateModule(buildingInstantiator, buildingConfig);
//
//       if (module is not IConditionBoundModule conditionBoundModule || OperationalConditions is null)
//       {
//         logService.LogWarning(GetType(),
//           $"Module {module.GetType().Name} does not support condition binding or OperationalConditions is null.");
//         return module;
//       }
//
//       List<OperationalCondition> localConditions = new();
//       List<OperationalCondition> globalConditions = new();
//
//       foreach (OperationalConditionConfig condition in OperationalConditions)
//       {
//         if (!condition.IsValidFor(conditionBoundModule))
//         {
//           logService.LogError(GetType(),
//             $"Condition {condition.GetType().Name} is not valid for module {module.GetType().Name}",
//             new InvalidOperationException());
//           continue;
//         }
//
//         OperationalCondition operationalCondition =
//           condition.CreateOperationalCondition(conditionInstantiator, conditionBoundModule);
//
//         if (condition.ForcesBuildingShutdown)
//           globalConditions.Add(operationalCondition);
//         else
//           localConditions.Add(operationalCondition);
//       }
//
//       conditionBoundModule.RegisterLocalConditions(localConditions);
//       conditionBoundModule.RegisterGlobalConditions(globalConditions);
//       return module;
//     }
//   }
// }