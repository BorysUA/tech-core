using _Project.CodeBase.Gameplay.InputHandlers;
using UnityEngine;

namespace _Project.CodeBase.Gameplay.Buildings
{
  public class BuildingPreview : PlacementPreview
  {
    [SerializeField] private MeshFilter _meshFilter;

    public void Setup(Mesh mesh)
    {
      _meshFilter.mesh = mesh;
    }
  }
}