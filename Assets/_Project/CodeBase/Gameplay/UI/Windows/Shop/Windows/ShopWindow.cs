using _Project.CodeBase.Gameplay.UI.Factory;
using _Project.CodeBase.Gameplay.UI.Windows.Shop.Buttons;
using _Project.CodeBase.Gameplay.UI.Windows.Shop.Item;
using _Project.CodeBase.Gameplay.UI.Windows.Shop.ViewModels;
using _Project.CodeBase.UI.Common;
using ObservableCollections;
using R3;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace _Project.CodeBase.Gameplay.UI.Windows.Shop.Windows
{
  public class ShopWindow : BaseWindow<ShopViewModel>
  {
    [SerializeField] private Transform _buyButtonsContainer;
    [SerializeField] private Button _closeButton;

    private IGameplayUiFactory _uiFactory;

    [Inject]
    public void Construct(IGameplayUiFactory gameplayUiFactory)
    {
      _uiFactory = gameplayUiFactory;
    }

    public override void Setup(BaseWindowViewModel viewModel)
    {
      base.Setup(viewModel);

      foreach (var shopItem in ViewModel.Items)
        CreateBuyButton(shopItem);

      ViewModel.Items
        .ObserveAdd()
        .Subscribe(addEvent => CreateBuyButton(addEvent.Value))
        .AddTo(this);

      BindCloseActions(_closeButton.OnClickAsObservable());
    }

    private async void CreateBuyButton(IShopItem shopItem)
    {
      BuyButton buyButton =
        await _uiFactory.CreateBuyButton(shopItem, _buyButtonsContainer);

      buyButton.OnClick
        .Subscribe(_ => ViewModel.BuyItem(shopItem))
        .AddTo(this);
    }
  }
}