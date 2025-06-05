using System.Collections.Generic;
using _Project.CodeBase.Data.Progress;
using _Project.CodeBase.Data.Progress.Building.ModuleData;
using _Project.CodeBase.Data.Progress.ResourceData;
using _Project.CodeBase.Gameplay.UI.Factory;
using _Project.CodeBase.UI.Common;
using R3;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Zenject;

namespace _Project.CodeBase.Gameplay.UI.Windows.Trade
{
  public class TradeWindow : BaseWindow<TradeViewModel>
  {
    [SerializeField] private OfferView _offerView;
    [SerializeField] private WaitingView _waitingView;

    private ITradeContentView _currentContentView;

    [SerializeField] private PointerDownListener _fullScreenCloseButton;
    [SerializeField] private Button _closeButton;

    public override void Setup(BaseWindowViewModel viewModel)
    {
      base.Setup(viewModel);

      _offerView.SellButtonOnClick
        .Subscribe(_ => OnSellItemsRequested())
        .AddTo(this);

      BindCloseActions(_fullScreenCloseButton.PointerDown, _closeButton.OnClickAsObservable());
    }

    protected override void Open()
    {
      base.Open();

      if (ViewModel.AreOfferAvailable)
        ShowOfferView();
      else
        ShowWaitingView();

      ViewModel.TradeOfferOpened += ShowOfferView;
      ViewModel.TradeOfferClosed += ShowWaitingView;
    }

    protected override void Close()
    {
      base.Close();
      _currentContentView?.Clear();

      ViewModel.TradeOfferOpened -= ShowOfferView;
      ViewModel.TradeOfferClosed -= ShowWaitingView;
    }

    private void ShowOfferView()
    {
      _currentContentView?.Clear();
      _offerView.Show(ViewModel.CurrentTradeOffer, ViewModel.IsOfferFulfillable);
      _currentContentView = _offerView;
    }

    private void ShowWaitingView()
    {
      _currentContentView?.Clear();
      _waitingView.Show(ViewModel.NextOfferOpenCountdown);
      _currentContentView = _waitingView;
    }

    private void OnSellItemsRequested() =>
      ViewModel.SellItems();
  }
}