using UnityEngine;

namespace _Project.CodeBase.Gameplay.UI.Windows.Shop.Item
{
  public abstract class ShopItem<TItemType> : IShopItem
  {
    public abstract TItemType ItemType { get; }
    public string Title { get; private set; }
    public Sprite ItemIcon { get; private set; }
    public int Price { get; private set; }
    public Sprite PriceIcon { get; private set; }

    protected ShopItem(string title, Sprite itemIcon, int price, Sprite priceIcon)
    {
      Title = title;
      Price = price;
      ItemIcon = itemIcon;
      PriceIcon = priceIcon;
    }
  }
}