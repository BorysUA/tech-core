using System.Threading.Tasks;
using _Project.CodeBase.Data.Progress.Meta;
using _Project.CodeBase.Menu.UI.SaveSelection;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace _Project.CodeBase.Menu.UI.Factories
{
  public interface IMenuUiFactory
  {
    public SaveSlotItem CreateSaveSlot(SaveMetaData data, Transform saveSlotsContainer);
  }
}