using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using _Project.CodeBase.Data.Progress;
using _Project.CodeBase.Data.Progress.Building.ModuleData;
using _Project.CodeBase.Gameplay.Building.Actions.Common;
using _Project.CodeBase.Gameplay.Building.Conditions;
using _Project.CodeBase.Gameplay.Building.Modules;
using _Project.CodeBase.Gameplay.Building.Modules.Health;
using _Project.CodeBase.Gameplay.Building.Modules.Spaceport;
using _Project.CodeBase.Gameplay.Constants;
using _Project.CodeBase.Gameplay.Data;
using _Project.CodeBase.Gameplay.Services.Grid;
using _Project.CodeBase.Gameplay.UI.PopUps.BuildingStatus;
using _Project.CodeBase.Services.LogService;
using ObservableCollections;
using R3;
using UnityEngine;

namespace _Project.CodeBase.Gameplay.Building
{
  public class BuildingViewModel
  {
    private BuildingDataProxy _buildingDataProxy;

    private readonly IGridService _gridService;
    private readonly ContractToModuleRegistry _contractToModuleRegistry;
    private readonly CompositeDisposable _disposable = new();
    private readonly ILogService _logService;
    private readonly Subject<Unit> _selected = new();
    private readonly Subject<Unit> _unselected = new();
    private readonly Subject<Unit> _destroyed = new();

    private Dictionary<Type, BuildingModule> _modules;
    private readonly List<IConditionBoundModule> _conditionBoundModules = new();
    private readonly List<IBuildingIndicatorSource> _indicators = new();
    private readonly List<IBuildingActionsProvider> _actions = new();
    private Observable<bool> _modulesOperationalObservable;

    public string Id => _buildingDataProxy.Id;
    public Observable<Unit> Selected => _selected;
    public Observable<Unit> Unselected => _unselected;
    public Observable<Unit> Destroyed => _destroyed;
    public Vector3 WorldPosition => _gridService.GetWorldPivot(_buildingDataProxy.OccupiedCells);
    public IEnumerable<IBuildingIndicatorSource> Indicators => _indicators;
    public IEnumerable<IBuildingActionsProvider> Actions => _actions;

    public BuildingViewModel(IGridService gridService, ContractToModuleRegistry contractToModuleRegistry)
    {
      _gridService = gridService;
      _contractToModuleRegistry = contractToModuleRegistry;
    }

    public void Setup(List<BuildingModule> modules)
    {
      _modules = modules.ToDictionary(module => module.GetType(), module => module);

      foreach (BuildingModule module in modules)
      {
        if (module is IConditionBoundModule conditionBound)
          _conditionBoundModules.Add(conditionBound);

        if (module is IBuildingActionsProvider actionProvider)
          _actions.Add(actionProvider);
      }
    }

    public void Initialize(BuildingDataProxy buildingDataProxy)
    {
      _buildingDataProxy = buildingDataProxy;

      InitializeModules();

      if (_conditionBoundModules.Count > 0)
        TrackConditionBoundModules();
      else
        ActivateBuildingModules();
    }

    public void Select()
    {
      foreach (BuildingModule module in _modules.Values)
        module.OnSelected();

      _selected.OnNext(Unit.Default);
    }

    public void Unselect()
    {
      foreach (BuildingModule module in _modules.Values)
        module.OnUnselected();

      _unselected.OnNext(Unit.Default);
    }

    public void Destroy()
    {
      _disposable?.Dispose();

      foreach (BuildingModule module in _modules.Values)
        module.OnDestroyed();

      _destroyed.OnNext(Unit.Default);
    }

    public bool TryGetModule<TContract>(out TContract result) where TContract : class
    {
      result = null;
      Type contractType = typeof(TContract);

      return TryGetExactModuleOfType(contractType, ref result) || TryGetModuleAssignableTo(contractType, ref result);
    }

    private bool TryGetModuleAssignableTo<TContract>(Type contractType, ref TContract result) where TContract : class
    {
      IReadOnlyCollection<Type> candidates = _contractToModuleRegistry.GetConcreteModuleTypesFor(contractType);

      if (candidates.Count >= _modules.Count)
      {
        foreach (Type moduleType in _modules.Keys)
          if (candidates.Contains(moduleType) && TryGetExactModuleOfType(moduleType, ref result))
            return true;
      }
      else
        foreach (Type moduleType in candidates)
          if (TryGetExactModuleOfType(moduleType, ref result))
            return true;

      return false;
    }

    private bool TryGetExactModuleOfType<TContract>(Type moduleType, ref TContract result) where TContract : class
    {
      if (_modules.TryGetValue(moduleType, out BuildingModule exactModule) && exactModule is TContract contractModule)
      {
        result = contractModule;
        return true;
      }

      return false;
    }

    private void InitializeModules()
    {
      foreach (BuildingModule module in _modules.Values)
      {
        module.Setup(Id);
        
        if (module is IProgressModule progressModule)
          AttachProgressData(progressModule);
        
        module.Initialize();

        if (module is IBuildingIndicatorsProvider statusesProvider)
          _indicators.AddRange(statusesProvider.Indicators);
      }
    }

    private void ActivateBuildingModules()
    {
      foreach (BuildingModule module in _modules.Values)
        module.Activate();
    }

    private void DeactivateBuildingModules()
    {
      foreach (BuildingModule module in _modules.Values)
        module.Deactivate();
    }

    private void TrackConditionBoundModules()
    {
      _modulesOperationalObservable = Observable
        .CombineLatest(_conditionBoundModules.Select(c => c.IsOperational))
        .Select(results => results.All(x => x));

      _modulesOperationalObservable.Subscribe(allOperational =>
        {
          if (allOperational)
            ActivateBuildingModules();
          else
            DeactivateBuildingModules();
        })
        .AddTo(_disposable);
    }

    private void AttachProgressData(IProgressModule progressModule)
    {
      if (_buildingDataProxy.ModulesProgress.TryGetValue(progressModule.GetType(), out IModuleData data))
        progressModule.AttachData(data);
      else
      {
        data = progressModule.CreateInitialData(_buildingDataProxy.Id);
        _buildingDataProxy.ModulesProgress[progressModule.GetType()] = data;
        progressModule.AttachData(data);
      }
    }
  }
}