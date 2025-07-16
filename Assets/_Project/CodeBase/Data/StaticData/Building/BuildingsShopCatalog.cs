using System;
using System.Collections.Generic;
using System.Linq;
using _Project.CodeBase.Gameplay.Constants;
using UnityEngine;

namespace _Project.CodeBase.Data.StaticData.Building
{
  [CreateAssetMenu(fileName = "BuildingsShopCatalog", menuName = "ScriptableObjects/BuildingsShopCatalog", order = 0)]
  public class BuildingsShopCatalog : ScriptableObject
  {
    public List<CategoryGroup> Categories;

    [Serializable]
    public class CategoryGroup
    {
      public BuildingCategory Category;
      public List<BuildingType> Buildings;
    }

    public int GetOrder(BuildingCategory category, BuildingType type)
    {
      CategoryGroup categoryGroup = Categories.FirstOrDefault(group => group.Category == category);
      return categoryGroup!.Buildings.IndexOf(type);
    }
  }
}