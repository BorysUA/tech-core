using System;
using _Project.CodeBase.Data.StaticData.Resource;
using Range = _Project.CodeBase.Data.StaticData.Common.Range;

namespace _Project.CodeBase.Data.StaticData.Building.Modules
{
  [Serializable]
  public class ResourceRange
  {
    public ResourceConfig Resource;
    public Range Amount;
  }
}