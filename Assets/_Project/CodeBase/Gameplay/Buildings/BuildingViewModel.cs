using System;
using System.Collections.Generic;
using System.Linq;
using _Project.CodeBase.Data.Progress.Building.ModuleData;
using _Project.CodeBase.Gameplay.Buildings.Actions.Common;
using _Project.CodeBase.Gameplay.Buildings.Modules;
using _Project.CodeBase.Gameplay.Models.Persistent.Interfaces;
using _Project.CodeBase.Gameplay.Services.Grid;
using _Project.CodeBase.Gameplay.States.PhaseFlow;
using _Project.CodeBase.Gameplay.UI.PopUps.BuildingStatus;
using _Project.CodeBase.Services.LogService;
using R3;
using Vector3 = _Project.CodeBase.Gameplay.Models.Vector3;


namespace _Project.CodeBase.Gameplay.Buildings
{
  public class BuildingViewModel : IBuildingViewInteractor, IGameplayStartedListener, IBuildingIndicatorReader,
    IBuildingActionReader
  {
    private readonly ContractToModuleRegistry _contractToModuleRegistry;
    private readonly ILogService _logService;
    private readonly Subject<Unit> _isInitialized = new();
    private readonly Subject<Unit> _selected = new();
    private readonly Subject<Unit> _unselected = new();
    private readonly Subject<Unit> _destroyed = new();

    private readonly List<IBuildingIndicatorSource> _indicators = new();
    private readonly List<IBuildingActionsProvider> _actions = new();

    private IBuildingDataReader _buildingDataReader;
    private Dictionary<Type, BuildingModule> _modules;
    private Observable<bool> _buildingOperational;

    public int Id => _buildingDataReader.Id;
    public Subject<Unit> IsInitialized => _isInitialized;
    public Observable<Unit> Selected => _selected;
    public Observable<Unit> Unselected => _unselected;
    public Observable<Unit> Destroyed => _destroyed;
    public Observable<bool> BuildingOperational => _buildingOperational;
    public Vector3 WorldPosition => GridUtils.GetWorldPivot(_buildingDataReader.OccupiedCells);
    public IEnumerable<IBuildingIndicatorSource> Indicators => _indicators;
    public IEnumerable<IBuildingActionsProvider> Actions => _actions;

    public BuildingViewModel(ContractToModuleRegistry contractToModuleRegistry)
    {
      _contractToModuleRegistry = contractToModuleRegistry;
    }

    public void Setup(List<BuildingModule> modules)
    {
      _modules = modules.ToDictionary(module => module.GetType(), module => module);

      foreach (BuildingModule module in modules)
      {
        if (module is IBuildingActionsProvider actionProvider)
          _actions.Add(actionProvider);
      }
    }

    public void Initialize(IBuildingDataReader buildingDataProxy)
    {
      _buildingDataReader = buildingDataProxy;

      InitializeModules();
      _isInitialized.OnNext(Unit.Default);
    }

    public void OnGameplayStarted()
    {
      foreach (BuildingModule module in _modules.Values)
        module.Run();
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
      foreach (BuildingModule module in _modules.Values)
        module.Dispose();

      _destroyed.OnNext(Unit.Default);
    }

    public bool TryGetPublicModuleContract<TContract>(out TContract result) where TContract : class
    {
      result = null;

      if (!typeof(TContract).IsInterface || !TryGetModuleUnsafe(out result))
        return false;

      return result is BuildingModule module && module.IsModuleWorking.CurrentValue;
    }

    public bool TryGetModuleUnsafe(Type contractType, out BuildingModule result)
    {
      result = null;
      return TryGetExactModuleOfType(contractType, ref result) || TryGetModuleAssignableTo(contractType, ref result);
    }

    public bool TryGetModuleUnsafe<TContract>(out TContract result) where TContract : class
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
      _buildingOperational = Observable
        .CombineLatest(_modules.Values.Select(c => c.CanBuildingWork))
        .Select(flags => flags.All(x => x))
        .DistinctUntilChanged()
        .Replay(1)
        .RefCount();

      foreach (BuildingModule module in _modules.Values)
      {
        if (module is IProgressModule progressModule)
          AttachProgressData(progressModule);

        module.Initialize(Id, _buildingOperational);

        _indicators.AddRange(module.Indicators);
      }
    }

    private void AttachProgressData(IProgressModule progressModule)
    {
      if (_buildingDataReader.ModulesProgress.TryGetValue(progressModule.GetType(), out IModuleData data))
        progressModule.AttachData(data);
    }
  }
}