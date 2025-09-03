using System;
using _Project.CodeBase.Infrastructure.Services.SaveService;

namespace _Project.CodeBase.Extensions
{
  public static class SaveSlotExtensions
  {
    private static readonly string[] FileKeys =
    {
      "none", "auto", "quick",
      "manual1", "manual2", "manual3", "manual4", "manual5",
      "manual6", "manual7", "manual8", "manual9", "manual10"
    };

    public static string ToStringKey(this SaveSlot saveSlot) =>
      (uint)saveSlot < (uint)FileKeys.Length
        ? FileKeys[(int)saveSlot]
        : throw new ArgumentOutOfRangeException(nameof(saveSlot), saveSlot, "Unknown SaveSlot");
  }
}