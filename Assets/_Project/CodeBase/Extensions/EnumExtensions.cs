using _Project.CodeBase.Gameplay.Constants;

namespace _Project.CodeBase.Extensions
{
  public static class EnumExtensions
  {
    public static bool ContainsAll(this CellContentType mask, CellContentType required)
      => (mask & required) == required;
  }
}