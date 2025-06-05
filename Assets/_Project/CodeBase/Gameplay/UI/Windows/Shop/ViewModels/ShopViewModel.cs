using System.Collections.Generic;
using _Project.CodeBase.Gameplay.UI.Windows.Shop.Item;
using _Project.CodeBase.UI.Common;
using _Project.CodeBase.UI.Services;
using ObservableCollections;

namespace _Project.CodeBase.Gameplay.UI.Windows.Shop.ViewModels
{
  public abstract class ShopViewModel : BaseWindowViewModel
  {
    protected ObservableList<IShopItem> _items { get; } = new();

    public IObservableCollection<IShopItem> Items => _items;

    public abstract void BuyItem(IShopItem shopItem);
  }
}