using _Project.CodeBase.Gameplay.Constants;
using _Project.CodeBase.Gameplay.ConstructionPlot;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace _Project.CodeBase.Gameplay.Services.BuildingPlots
{
  public interface IConstructionPlotFactory
  {
    UniTask<ConstructionPlotPreview> CreateConstructionPlotPreview();
    void Initialize();
    UniTask<ConstructionPlotViewModel> CreateConstructionPlot(ConstructionPlotType type, Vector3 worldPivot);
  }
}