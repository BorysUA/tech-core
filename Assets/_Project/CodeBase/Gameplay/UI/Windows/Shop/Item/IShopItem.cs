using UnityEngine;

namespace _Project.CodeBase.Gameplay.UI.Windows.Shop.Item
{
  public interface IShopItem
  {
    string Title { get; }
    Sprite ItemIcon { get; }
    int Price { get; }
    Sprite PriceIcon { get; }
  }
}