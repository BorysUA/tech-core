using System;
using _Project.CodeBase.Gameplay.Constants;

namespace _Project.CodeBase.Data.StaticData.Building
{
  [Serializable]
  public struct PlacementFilter
  {
    public CellContentType MustHave;
    public CellContentType MustBeEmpty;
  }
}