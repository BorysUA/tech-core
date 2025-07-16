using System.Collections.Generic;
using _Project.CodeBase.Gameplay.Constants;
using _Project.CodeBase.Gameplay.UI.Factory;
using _Project.CodeBase.Gameplay.UI.Windows.Shop.Buttons;
using _Project.CodeBase.Gameplay.UI.Windows.Shop.ViewModels;
using _Project.CodeBase.Services.LogService;
using _Project.CodeBase.UI.Core;
using Cysharp.Threading.Tasks;
using ObservableCollections;
using R3;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace _Project.CodeBase.Gameplay.UI.Windows.Shop.Windows
{
  public class BuildingsShopWindow : BaseWindow<BuildingsShopViewModel>
  {
    [SerializeField] private Transform _buyButtonsContainer;
    [SerializeField] private Button _closeButton;

    private readonly List<BuyButton> _buyButtons = new();
    private DisposableBag _subscriptions;
    private IGameplayUiFactory _uiFactory;

    [Inject]
    public void Construct(IGameplayUiFactory gameplayUiFactory)
    {
      _uiFactory = gameplayUiFactory;
    }

    public override void Setup(BaseWindowViewModel viewModel)
    {
      base.Setup(viewModel);

      ViewModel.ItemsToShow
        .ObserveAdd()
        .Subscribe(addEvent => CreateBuyButton(addEvent.Value, addEvent.Index).Forget())
        .AddTo(this);

      ViewModel.ItemsToShow.ObserveClear()
        .Subscribe(_ => Clear())
        .AddTo(this);

      BindCloseActions(_closeButton.OnClickAsObservable());
    }

    private void Clear()
    {
      _subscriptions.Clear();

      _buyButtons.ForEach(button => button.Deactivate());
      _buyButtons.Clear();
    }

    private async UniTask CreateBuyButton(BuildingType buildingType, int index)
    {
      BuyButton buyButton =
        await _uiFactory.CreateBuyButton(buildingType, _buyButtonsContainer);

      buyButton.Initialize(index);

      buyButton.OnClick
        .Subscribe(_ => ViewModel.BuyItem(buildingType))
        .AddTo(ref _subscriptions);

      _buyButtons.Add(buyButton);
    }
  }
}