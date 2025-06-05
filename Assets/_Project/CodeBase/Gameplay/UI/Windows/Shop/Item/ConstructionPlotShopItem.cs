using _Project.CodeBase.Gameplay.Constants;
using _Project.CodeBase.Gameplay.Services.BuildingPlots;
using UnityEngine;

namespace _Project.CodeBase.Gameplay.UI.Windows.Shop.Item
{
  public class ConstructionPlotShopItem : ShopItem<ConstructionPlotType>
  {
    public override ConstructionPlotType ItemType { get; }

    public ConstructionPlotShopItem(ConstructionPlotType constructionPlotType, string title, Sprite itemIcon, int price,
      Sprite priceIcon) : base(title, itemIcon, price, priceIcon)
    {
      ItemType = constructionPlotType;
    }
  }
}