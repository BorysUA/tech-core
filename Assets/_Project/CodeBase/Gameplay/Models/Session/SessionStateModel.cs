using System.Collections.Generic;
using _Project.CodeBase.Gameplay.Constants;

namespace _Project.CodeBase.Gameplay.Models.Session
{
  public class SessionStateModel : ISessionProgress
  {
    private readonly Dictionary<ResourceKind, ResourceSessionModel> _resources = new();

    public ResourceSessionModel GetResourceModel(ResourceKind kind)
    {
      if (!_resources.TryGetValue(kind, out ResourceSessionModel model))
      {
        model = new ResourceSessionModel();
        _resources[kind] = model;
      }

      return model;
    }
  }
}