using System;
using _Project.CodeBase.Infrastructure.Services;
using _Project.CodeBase.Infrastructure.Services.SaveService;
using _Project.CodeBase.Menu.UI.DifficultySelection;

namespace _Project.CodeBase.Data.Progress.Meta
{
  [Serializable]
  public readonly struct SaveMetaData
  {
    public SaveSlot SaveSlot { get; init; }
    public GameDifficulty Difficulty { get; init; }
    public string DisplayName { get; init; }
    public TimeSpan InGameTime { get; init; }
    public DateTime LastModifiedUtc { get; init; }
    public string BuildVersion { get; init; }
  }
}