using System.Threading.Tasks;
using _Project.CodeBase.Gameplay.Building.Actions;
using _Project.CodeBase.Gameplay.Building.Actions.Common;
using _Project.CodeBase.Gameplay.Constants;
using _Project.CodeBase.Gameplay.LiveEvents;
using _Project.CodeBase.Gameplay.UI.HUD;
using _Project.CodeBase.Gameplay.UI.HUD.BuildingAction;
using _Project.CodeBase.Gameplay.UI.HUD.GameEvent;
using _Project.CodeBase.Gameplay.UI.HUD.Notification;
using _Project.CodeBase.Gameplay.UI.Indicators;
using _Project.CodeBase.Gameplay.UI.PopUps.BuildingStatus;
using _Project.CodeBase.Gameplay.UI.Windows.Shop.Buttons;
using _Project.CodeBase.Gameplay.UI.Windows.Trade;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace _Project.CodeBase.Gameplay.UI.Factory
{
  public interface IGameplayUiFactory
  {
    UniTask<HudView> CreateHud();
    UniTask<FlyText> CreateFlyText();
    UniTask<BuildingActionButton> CreateBuildingActionButton(ActionType actionType, Transform container);

    UniTask<BuildingIndicatorView> CreateBuildingIndicator(BuildingIndicatorType indicatorType,
      Transform statusIconsContainer);

    UniTask<ResourceAmountItem> CreateResourceAmountItem(ResourceKind resourceKind, int amount,
      Transform itemsContainer);

    UniTask<PaymentItem> CreateTradePaymentItem(ResourceKind resourceKind, int amount,
      Transform paymentsContainer);

    UniTask<NotificationMessage> CreateNotification(ToastData data, Transform container);
    UniTask<GameEventIndicator> CreateGameEventIndicator(GameEventType gameEventType, Transform container);
    UniTask<BuyButton> CreateBuyButton(BuildingType buildingType, Transform container);
    UniTask<BuyButton> CreateBuyButton(ConstructionPlotType plotType, Transform container);
  }
}