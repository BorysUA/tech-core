using System;
using System.Linq;
using System.Threading.Tasks;
using _Project.CodeBase.Data.StaticData.Building;
using _Project.CodeBase.Data.StaticData.Building.InteractionButtons;
using _Project.CodeBase.Data.StaticData.Building.StatusItems;
using _Project.CodeBase.Data.StaticData.Resource;
using _Project.CodeBase.Gameplay.Buildings.Actions.Common;
using _Project.CodeBase.Gameplay.Constants;
using _Project.CodeBase.Gameplay.LiveEvents;
using _Project.CodeBase.Gameplay.Services.Pool;
using _Project.CodeBase.Gameplay.UI.HUD;
using _Project.CodeBase.Gameplay.UI.HUD.BuildingAction;
using _Project.CodeBase.Gameplay.UI.HUD.GameEvent;
using _Project.CodeBase.Gameplay.UI.HUD.Notification;
using _Project.CodeBase.Gameplay.UI.Indicators;
using _Project.CodeBase.Gameplay.UI.PopUps.BuildingStatus;
using _Project.CodeBase.Gameplay.UI.Root;
using _Project.CodeBase.Gameplay.UI.Windows.Shop.Buttons;
using _Project.CodeBase.Gameplay.UI.Windows.Trade;
using _Project.CodeBase.Infrastructure.Constants;
using _Project.CodeBase.Infrastructure.Services.Interfaces;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;
using IAssetProvider = _Project.CodeBase.Infrastructure.Services.Interfaces.IAssetProvider;

namespace _Project.CodeBase.Gameplay.UI.Factory
{
  public class GameplayUiFactory : IGameplayUiFactory
  {
    private readonly IAssetProvider _assetProvider;
    private readonly IInstantiator _instantiator;
    private readonly IStaticDataProvider _staticDataProvider;
    private readonly GameEventsAddressMap _gameEventsAddressMap;

    private readonly Transform _uiRoot;
    private readonly PopUpsCanvas _popUpsCanvas;

    private readonly ObjectPool<FlyText> _flyTextPool = new();
    private readonly ObjectPool<ResourceAmountItem> _resourceItemPool = new();
    private readonly ObjectPool<BuyButton, Transform> _buyButtonPool = new();
    private readonly ActionButtonPool _actionButtonPool = new();

    private PaymentItem _cachedPaymentItem;

    public GameplayUiFactory(IAssetProvider assetProvider, IInstantiator instantiator,
      IStaticDataProvider staticDataProvider, Transform uiRoot,
      PopUpsCanvas popUpsContainer, GameEventsAddressMap gameEventsAddressMap)
    {
      _assetProvider = assetProvider;
      _instantiator = instantiator;
      _staticDataProvider = staticDataProvider;
      _uiRoot = uiRoot;
      _popUpsCanvas = popUpsContainer;
      _gameEventsAddressMap = gameEventsAddressMap;
    }

    public async UniTask<HudView> CreateHud()
    {
      GameObject hudPrefab = await _assetProvider.LoadAssetAsync<GameObject>(AssetAddress.HUD);
      return _instantiator.InstantiatePrefabForComponent<HudView>(hudPrefab, _uiRoot);
    }

    public async UniTask<FlyText> CreateFlyText()
    {
      if (_flyTextPool.TryGet(out FlyText cachedFlyText))
        return cachedFlyText;

      GameObject flyTextPrefab = await _assetProvider.LoadAssetAsync<GameObject>(AssetAddress.FlyText);
      FlyText flyText = _instantiator.InstantiatePrefabForComponent<FlyText>(flyTextPrefab, _popUpsCanvas.Root);

      _flyTextPool.Add(flyText);

      return flyText;
    }

    public async UniTask<BuildingActionButton> CreateBuildingActionButton(ActionType actionType, Transform container)
    {
      BuildingActionButtonConfig config = _staticDataProvider.GetBuildingActionButtonConfig(actionType);

      if (_actionButtonPool.TryGet(actionType, out BuildingActionButton cachedActionButton))
        return cachedActionButton;

      GameObject buttonPrefab = await _assetProvider.LoadAssetAsync<GameObject>(AssetAddress.BuildingActionButton);

      BuildingActionButton actionButton =
        _instantiator.InstantiatePrefabForComponent<BuildingActionButton>(buttonPrefab, container);

      actionButton.Setup(config.Title, config.Icon);
      _actionButtonPool.Add(actionType, actionButton);

      return actionButton;
    }

    public async Task<BuildingIndicatorView> CreateBuildingIndicator(BuildingIndicatorType indicatorType,
      Transform itemsContainer)
    {
      BuildingIndicatorConfig itemConfig = _staticDataProvider.GetBuildingIndicatorConfig(indicatorType);

      GameObject itemPrefab =
        await _assetProvider.LoadAssetAsync<GameObject>(AssetAddress.BuildingIndicatorItem);

      BuildingIndicatorView item =
        _instantiator.InstantiatePrefabForComponent<BuildingIndicatorView>(itemPrefab, itemsContainer);

      item.Setup(itemConfig.Icon);

      return item;
    }

    public async UniTask<ResourceAmountItem> CreateResourceAmountItem(ResourceKind resourceKind, int amount,
      Transform itemsContainer)
    {
      ResourceConfig resourceConfig = _staticDataProvider.GetResourceConfig(resourceKind);

      if (_resourceItemPool.TryGet(out ResourceAmountItem cachedResourceItem))
      {
        cachedResourceItem.Setup(resourceConfig.Icon, amount);
        return cachedResourceItem;
      }

      GameObject itemPrefab = await _assetProvider.LoadAssetAsync<GameObject>(AssetAddress.TradeResourceItem);

      ResourceAmountItem item =
        _instantiator.InstantiatePrefabForComponent<ResourceAmountItem>(itemPrefab, itemsContainer);

      item.Setup(resourceConfig.Icon, amount);
      _resourceItemPool.Add(item);

      return item;
    }

    public async UniTask<PaymentItem> CreateTradePaymentItem(ResourceKind resourceKind, int amount,
      Transform paymentsContainer)
    {
      ResourceConfig resourceConfig = _staticDataProvider.GetResourceConfig(resourceKind);

      if (_cachedPaymentItem is not null)
      {
        _cachedPaymentItem.Setup(resourceConfig.Icon, amount);
        _cachedPaymentItem.Activate();
        return _cachedPaymentItem;
      }

      GameObject itemPrefab = await _assetProvider.LoadAssetAsync<GameObject>(AssetAddress.PaymentItem);

      PaymentItem item = _instantiator.InstantiatePrefabForComponent<PaymentItem>(itemPrefab, paymentsContainer);

      item.Setup(resourceConfig.Icon, amount);
      _cachedPaymentItem = item;

      return item;
    }

    public async UniTask<NotificationMessage> CreateNotification(ToastData data, Transform container)
    {
      GameObject notificationPrefab = await _assetProvider.LoadAssetAsync<GameObject>(AssetAddress.NotificationMessage);
      NotificationMessage message =
        _instantiator.InstantiatePrefabForComponent<NotificationMessage>(notificationPrefab, container);

      message.Setup(data);
      return message;
    }

    public async UniTask<GameEventIndicator> CreateGameEventIndicator(GameEventType gameEventType, Transform container)
    {
      string address = _gameEventsAddressMap.GetAddress(gameEventType);
      GameObject gameEventPrefab = await _assetProvider.LoadAssetAsync<GameObject>(address);
      GameEventIndicator indicator =
        _instantiator.InstantiatePrefabForComponent<GameEventIndicator>(gameEventPrefab, container);

      return indicator;
    }

    public async UniTask<BuyButton> CreateBuyButton(BuildingType buildingType, Transform container)
    {
      BuildingConfig config = _staticDataProvider.GetBuildingConfig(buildingType);
      ResourceConfig resourceConfig = _staticDataProvider.GetResourceConfig(config.Price.Kind);

      return await CreateBuyButtonInternal(
        buyButton => buyButton.Setup(config.Title, config.Icon, config.Price.Amount, resourceConfig.Icon),
        container);
    }

    public async UniTask<BuyButton> CreateBuyButton(ConstructionPlotType plotType, Transform container)
    {
      ConstructionPlotConfig config = _staticDataProvider.GetConstructionPlotConfig(plotType);
      ResourceConfig resourceConfig = _staticDataProvider.GetResourceConfig(config.Price.Kind);

      return await CreateBuyButtonInternal(
        buyButton => buyButton.Setup(config.Title, config.Icon, config.Price.Amount, resourceConfig.Icon),
        container);
    }

    private async UniTask<BuyButton> CreateBuyButtonInternal(Action<BuyButton> setupButton, Transform container)
    {
      if (_buyButtonPool.TryGet(container, out BuyButton cachedBuyButton))
      {
        setupButton.Invoke(cachedBuyButton);
        return cachedBuyButton;
      }

      GameObject buttonPrefab =
        await _assetProvider.LoadAssetAsync<GameObject>(AssetAddress.BuyButton);

      BuyButton buyButton = _instantiator.InstantiatePrefabForComponent<BuyButton>(buttonPrefab, container);
      setupButton.Invoke(buyButton);
      _buyButtonPool.Add(buyButton);
      return buyButton;
    }
  }
}