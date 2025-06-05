using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using _Project.CodeBase.Data.Progress.Building.ModuleData;
using _Project.CodeBase.Data.Progress.ResourceData;
using _Project.CodeBase.Data.StaticData.Building.Modules;
using _Project.CodeBase.Extensions;
using _Project.CodeBase.Gameplay.Building.Actions;
using _Project.CodeBase.Gameplay.Building.Actions.Common;
using _Project.CodeBase.Gameplay.Data.Modules;
using _Project.CodeBase.Gameplay.Services.Resource;
using _Project.CodeBase.Infrastructure.Services.Interfaces;
using _Project.CodeBase.Services.LogService;
using R3;
using UnityEngine;
using static UnityEngine.Random;

namespace _Project.CodeBase.Gameplay.Building.Modules.Spaceport
{
  public class SpaceportTradeModule : BuildingModuleWithProgressData<TradeData>, IBuildingActionsProvider
  {
    private TradeConfig _tradeConfig;
    private TradeDataProxy _tradeDataProxy;
    private CancellationTokenSource _lifetimeCts;

    private readonly ActionFactory _actionFactory;
    private readonly ICoroutineRunner _coroutineRunner;
    private readonly IResourceService _resourceService;

    private readonly IBuildingAction[] _actions = new IBuildingAction[1];
    private ResourceRange[] _tradeOfferResources;

    private Coroutine _closeOfferCoroutine;
    private Coroutine _generateOfferCoroutine;
    private readonly WaitForEndOfFrame _waitForEndOfFrame = new();

    public CancellationToken Lifetime  => _lifetimeCts.Token;
    public ReadOnlyReactiveProperty<float> OfferCloseCountdown => _tradeDataProxy.OfferCloseCountdown;
    public ReadOnlyReactiveProperty<float> NextOfferOpenCountdown => _tradeDataProxy.NextOfferOpenCountdown;
    public TradeOfferData CurrentTradeOffer => _tradeDataProxy.CurrentTradeOffer.CurrentValue;
    public IEnumerable<IBuildingAction> Actions => _actions;

    public event Action TradeOfferOpened;
    public event Action TradeOfferClosed;

    public SpaceportTradeModule(ActionFactory actionFactory, ILogService logService, ICoroutineRunner coroutineRunner,
      IResourceService resourceService) : base(logService)
    {
      _actionFactory = actionFactory;
      _coroutineRunner = coroutineRunner;
      _resourceService = resourceService;
    }

    public void Setup(TradeConfig tradeConfig)
    {
      _tradeConfig = tradeConfig;
      _tradeOfferResources = _tradeConfig.PurchaseResources.ToArray();
    }

    public override void Initialize() =>
      InitializeActions();

    public override void AttachData(IModuleData moduleData)
    {
      base.AttachData(moduleData);
      _tradeDataProxy = new TradeDataProxy(ModuleData);
    }

    public override void Activate()
    {
      _lifetimeCts = new CancellationTokenSource();
      base.Activate();

      if (_tradeDataProxy.OfferCloseCountdown.CurrentValue > 0.01f)
      {
        TradeOfferOpened?.Invoke();
        _closeOfferCoroutine = _coroutineRunner.ExecuteCoroutine(CloseCurrentOfferAfterCountdown());
      }
      else
      {
        TradeOfferClosed?.Invoke();
        _generateOfferCoroutine = _coroutineRunner.ExecuteCoroutine(GenerateTradeOfferAfterCountdown());
      }
    }

    public override void Deactivate()
    {
      _lifetimeCts?.Cancel();
      _lifetimeCts?.Dispose();
      _lifetimeCts = null;

      base.Deactivate();

      _coroutineRunner.TerminateCoroutine(_closeOfferCoroutine);
      _coroutineRunner.TerminateCoroutine(_generateOfferCoroutine);
    }

    public override IModuleData CreateInitialData(string buildingId) =>
      new TradeData(buildingId, 0);

    public void FulfillOffer()
    {
      if (_tradeDataProxy.CurrentTradeOffer.CurrentValue is not { } tradeOffer)
      {
        LogService.LogError(GetType(), "Failed to fulfill offer: no active trade offer available.");
        return;
      }

      if (_resourceService.TrySpend(tradeOffer.PurchaseResources))
      {
        _tradeDataProxy.IncrementFulfillOffersCount();
        _resourceService.AddResource(tradeOffer.Payment.Kind, tradeOffer.Payment.Amount);
        _coroutineRunner.TerminateCoroutine(_closeOfferCoroutine);
        CloseCurrentOffer();
      }
    }

    private IEnumerator CloseCurrentOfferAfterCountdown()
    {
      while (_tradeDataProxy.OfferCloseCountdown.CurrentValue > 0.01f)
      {
        _tradeDataProxy.OfferCloseCountdown.Value =
          Mathf.Max(_tradeDataProxy.OfferCloseCountdown.CurrentValue - Time.deltaTime, 0);

        yield return _waitForEndOfFrame;
      }

      CloseCurrentOffer();
    }

    private IEnumerator GenerateTradeOfferAfterCountdown()
    {
      while (_tradeDataProxy.NextOfferOpenCountdown.CurrentValue > 0.01f)
      {
        _tradeDataProxy.NextOfferOpenCountdown.Value =
          Mathf.Max(_tradeDataProxy.NextOfferOpenCountdown.CurrentValue - Time.deltaTime, 0);

        yield return _waitForEndOfFrame;
      }

      GenerateTradeOffer();
    }

    private void GenerateTradeOffer()
    {
      _tradeDataProxy.CurrentTradeOffer.Value = CreateTradeOffer();
      TradeOfferOpened?.Invoke();

      _tradeDataProxy.OfferCloseCountdown.Value = _tradeConfig.OfferCloseCountdown;
      _closeOfferCoroutine = _coroutineRunner.ExecuteCoroutine(CloseCurrentOfferAfterCountdown());
    }

    private void CloseCurrentOffer()
    {
      _tradeDataProxy.CurrentTradeOffer.Value = null;
      _tradeDataProxy.OfferCloseCountdown.Value = 0;
      TradeOfferClosed?.Invoke();

      _tradeDataProxy.NextOfferOpenCountdown.Value = _tradeConfig.NextOfferOpenCountdown;

      if (IsActive.CurrentValue)
        _generateOfferCoroutine = _coroutineRunner.ExecuteCoroutine(GenerateTradeOfferAfterCountdown());
    }

    private TradeOfferData CreateTradeOffer()
    {
      _tradeOfferResources.Shuffle();

      int randomResourceCount = Mathf.RoundToInt(Mathf.Lerp(_tradeConfig.MinResourcesCount,
        _tradeConfig.MaxResourcesCount,
        _tradeConfig.ResourceCountCoeffPerLevel * _tradeDataProxy.CompletedTradesCount));

      ResourceAmountData[] purchaseResources = new ResourceAmountData[randomResourceCount];

      float totalPrice = 0f;

      for (int i = 0; i < randomResourceCount; i++)
      {
        ResourceRange resource = _tradeOfferResources[i];
        int amount = Range(resource.Amount.Min, resource.Amount.Max);

        ResourceAmountData purchaseResource = new ResourceAmountData(resource.Resource.Kind, amount);
        purchaseResources[i] = purchaseResource;

        float priceForThisResource = purchaseResource.Amount * resource.Resource.Price * Range(0.9f, 1.1f);
        totalPrice += priceForThisResource;
      }

      int finalPrice = Mathf.RoundToInt(totalPrice);
      ResourceAmountData payment = new ResourceAmountData(_tradeConfig.PaymentResource, finalPrice);

      return new TradeOfferData(purchaseResources, payment);
    }

    private void InitializeActions()
    {
      TradeAction tradeAction = _actionFactory.CreateAction<TradeAction>();
      tradeAction.Setup(this);
      _actions[0] = tradeAction;
    }
  }
}