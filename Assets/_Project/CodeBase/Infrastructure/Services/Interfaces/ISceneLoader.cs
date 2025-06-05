using System;

namespace _Project.CodeBase.Infrastructure.Services.Interfaces
{
  public interface ISceneLoader
  {
    public void LoadScene(string sceneName, Action completed = null);
  }
}