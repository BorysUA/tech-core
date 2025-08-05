using System;
using System.Collections.Generic;
using _Project.CodeBase.Data.Progress.Building.ModuleData;
using _Project.CodeBase.Data.Progress.ResourceData;
using _Project.CodeBase.Gameplay.UI.Factory;
using R3;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace _Project.CodeBase.Gameplay.UI.Windows.Trade
{
  public class OfferView : MonoBehaviour, ITradeContentView
  {
    [SerializeField] private Transform _purchaseContainer;
    [SerializeField] private Transform _paymentContainer;

    [SerializeField] private Button _sellButton;
    [SerializeField] private Image _sellButtonImage;

    private readonly List<ResourceAmountItem> _resourceAmountItems = new();
    private PaymentItem _paymentItem;
    private DisposableBag _disposable;

    private IGameplayUiFactory _uiFactory;

    public Observable<Unit> SellButtonOnClick => _sellButton.OnClickAsObservable();

    [Inject]
    public void Construct(IGameplayUiFactory uiFactory)
    {
      _uiFactory = uiFactory;
    }

    public void Show(TradeOfferData offer, ReadOnlyReactiveProperty<bool> isOfferFulfillable)
    {
      foreach (ResourceAmountData resource in offer.ResourcesToSell)
        CreateResourceItem(resource);

      CreatePaymentItem(offer.Reward);

      isOfferFulfillable
        .Subscribe(UpdateVisualState)
        .AddTo(ref _disposable);

      gameObject.SetActive(true);
    }

    public void Clear()
    {
      foreach (ResourceAmountItem resourceItem in _resourceAmountItems)
        resourceItem.Deactivate();
      _paymentItem?.Deactivate();

      _resourceAmountItems.Clear();
      _disposable.Clear();

      gameObject.SetActive(false);
    }

    private void UpdateVisualState(bool isFulfillable) =>
      _sellButtonImage.color = isFulfillable ? Color.green : Color.red;

    private async void CreatePaymentItem(ResourceAmountData offerPayment)
    {
      PaymentItem paymentItem =
        await _uiFactory.CreateTradePaymentItem(offerPayment.Kind, offerPayment.Amount, _paymentContainer);

      _paymentItem = paymentItem;
    }

    private async void CreateResourceItem(ResourceAmountData resource)
    {
      ResourceAmountItem resourceItem =
        await _uiFactory.CreateResourceAmountItem(resource.Kind, resource.Amount, _purchaseContainer);

      _resourceAmountItems.Add(resourceItem);
    }
  }
}