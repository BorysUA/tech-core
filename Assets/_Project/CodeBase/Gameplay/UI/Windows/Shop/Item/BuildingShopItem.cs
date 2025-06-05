using _Project.CodeBase.Gameplay.Constants;
using UnityEngine;

namespace _Project.CodeBase.Gameplay.UI.Windows.Shop.Item
{
  public class BuildingShopItem : ShopItem<BuildingType>
  {
    public override BuildingType ItemType { get; }

    public BuildingShopItem(BuildingType buildingType, string title, Sprite itemIcon, int price, Sprite priceIcon) :
      base(title, itemIcon, price, priceIcon)
    {
      ItemType = buildingType;
    }
  }
}