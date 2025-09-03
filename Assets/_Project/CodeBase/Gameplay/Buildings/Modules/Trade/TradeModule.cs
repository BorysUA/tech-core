using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using _Project.CodeBase.Data.Progress.Building.ModuleData;
using _Project.CodeBase.Data.StaticData.Building.Modules;
using _Project.CodeBase.Gameplay.Buildings.Actions;
using _Project.CodeBase.Gameplay.Buildings.Actions.Common;
using _Project.CodeBase.Gameplay.Services.Command;
using _Project.CodeBase.Infrastructure.Services.Interfaces;
using _Project.CodeBase.Services.LogService;
using R3;
using UnityEngine;

namespace _Project.CodeBase.Gameplay.Buildings.Modules.Trade
{
  public class TradeModule : BuildingModuleWithProgressData<IReadOnlyTradeData>, IBuildingActionsProvider
  {
    private const float SnapshotPeriod = 10f;
    private const float CountdownEpsilon = 0.01f;

    private readonly ActionFactory _actionFactory;
    private readonly ICommandBroker _commandBroker;
    private readonly ICoroutineProvider _coroutineProvider;
    private readonly IBuildingAction[] _actions = new IBuildingAction[1];
    private readonly WaitForEndOfFrame _waitForEndOfFrame = new();

    private readonly ReactiveProperty<float> _offerCloseCountdown = new();
    private readonly ReactiveProperty<float> _nextOfferOpenCountdown = new();

    private TradeConfig _tradeConfig;
    private CancellationTokenSource _lifetimeCts;

    private Coroutine _closeOfferCoroutine;
    private Coroutine _generateOfferCoroutine;
    private Coroutine _snapshotCoroutine;

    public CancellationToken Lifetime => _lifetimeCts?.Token ?? CancellationToken.None;
    public ReadOnlyReactiveProperty<float> OfferCloseCountdown => _offerCloseCountdown;
    public ReadOnlyReactiveProperty<float> NextOfferOpenCountdown => _nextOfferOpenCountdown;
    public TradeOfferData CurrentTradeOffer => ModuleData.CurrentOffer;
    public IReadOnlyList<IBuildingAction> Actions => _actions;

    public event Action TradeOfferOpened;
    public event Action TradeOfferClosed;

    public TradeModule(ActionFactory actionFactory, ILogService logService, ICoroutineProvider coroutineProvider,
      ICommandBroker commandBroker) : base(logService)
    {
      _actionFactory = actionFactory;
      _coroutineProvider = coroutineProvider;
      _commandBroker = commandBroker;
    }

    public void Setup(TradeConfig tradeConfig)
    {
      _tradeConfig = tradeConfig;
    }

    protected override void OnInitialize()
    {
      CreateActions();
      UpdateCountdowns();
    }

    public void FulfillOffer()
    {
      if (CurrentTradeOffer is not { } tradeOffer)
      {
        LogService.LogError(GetType(), "Failed to fulfill offer: no active trade offer available.");
        return;
      }

      if (_commandBroker.ExecuteCommand<FulfillTradeOfferCommand, bool>(
            new FulfillTradeOfferCommand(BuildingId)))
      {
        _coroutineProvider.TerminateCoroutine(_closeOfferCoroutine);
        CloseCurrentOffer();
      }
    }

    protected override void Activate()
    {
      _lifetimeCts = new CancellationTokenSource();

      if (_offerCloseCountdown.CurrentValue > CountdownEpsilon)
      {
        TradeOfferOpened?.Invoke();
        _closeOfferCoroutine = _coroutineProvider.ExecuteCoroutine(CloseCurrentOfferAfterCountdown());
      }
      else
      {
        TradeOfferClosed?.Invoke();
        _generateOfferCoroutine = _coroutineProvider.ExecuteCoroutine(GenerateTradeOfferAfterCountdown());
      }

      _snapshotCoroutine = _coroutineProvider.ExecuteCoroutine(SnapshotLoop());
    }

    protected override void Deactivate()
    {
      _lifetimeCts?.Cancel();
      _lifetimeCts?.Dispose();
      _lifetimeCts = null;

      TerminateCoroutines();
    }

    public override void Dispose()
    {
      base.Dispose();
      _lifetimeCts?.Dispose();
      TerminateCoroutines();
    }

    private IEnumerator CloseCurrentOfferAfterCountdown()
    {
      while (_offerCloseCountdown.CurrentValue > CountdownEpsilon)
      {
        _offerCloseCountdown.Value =
          Mathf.Max(_offerCloseCountdown.CurrentValue - Time.deltaTime, 0);

        yield return _waitForEndOfFrame;
      }

      CloseCurrentOffer();
    }

    private IEnumerator GenerateTradeOfferAfterCountdown()
    {
      while (_nextOfferOpenCountdown.CurrentValue > CountdownEpsilon)
      {
        _nextOfferOpenCountdown.Value =
          Mathf.Max(_nextOfferOpenCountdown.CurrentValue - Time.deltaTime, 0);

        yield return _waitForEndOfFrame;
      }

      GenerateTradeOffer();
    }

    private void GenerateTradeOffer()
    {
      _commandBroker.ExecuteCommand(new OpenTradeOfferCommand(BuildingId));

      TradeOfferOpened?.Invoke();
      UpdateCountdowns();

      _closeOfferCoroutine = _coroutineProvider.ExecuteCoroutine(CloseCurrentOfferAfterCountdown());
    }

    private void CloseCurrentOffer()
    {
      _commandBroker.ExecuteCommand(new CloseTradeOfferCommand(BuildingId));

      TradeOfferClosed?.Invoke();
      UpdateCountdowns();

      if (IsModuleWorking.CurrentValue)
        _generateOfferCoroutine = _coroutineProvider.ExecuteCoroutine(GenerateTradeOfferAfterCountdown());
    }

    private void CreateActions()
    {
      TradeAction tradeAction = _actionFactory.CreateAction<TradeAction>();
      tradeAction.Setup(this);
      _actions[0] = tradeAction;
    }

    private void UpdateCountdowns()
    {
      _offerCloseCountdown.Value = ModuleData.OfferCloseCountdown;
      _nextOfferOpenCountdown.Value = ModuleData.NextOfferOpenCountdown;
    }

    private IEnumerator SnapshotLoop()
    {
      WaitForSeconds waitForSeconds = new WaitForSeconds(SnapshotPeriod);

      while (true)
      {
        SnapshotOnce();
        yield return waitForSeconds;
      }
    }

    private void SnapshotOnce()
    {
      _commandBroker.ExecuteCommand(new UpdateTradeCountdownCommand(
        BuildingId,
        _offerCloseCountdown.CurrentValue,
        _nextOfferOpenCountdown.CurrentValue));
    }

    private void TerminateCoroutines()
    {
      _coroutineProvider.TerminateCoroutine(_closeOfferCoroutine);
      _coroutineProvider.TerminateCoroutine(_generateOfferCoroutine);
      _coroutineProvider.TerminateCoroutine(_snapshotCoroutine);
    }
  }
}