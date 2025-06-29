using System;
using System.Collections.Generic;
using _Project.CodeBase.Services.LogService;
using _Project.CodeBase.UI.Core;

namespace _Project.CodeBase.UI.Services
{
  public class WindowsRepository
  {
    private readonly ILogService _logService;
    private readonly Dictionary<Type, BaseWindowViewModel> _windows = new();

    public WindowsRepository(ILogService logService)
    {
      _logService = logService;
    }

    public bool TryGetValue<TWindow>(out BaseWindowViewModel viewModel) where TWindow : BaseWindowViewModel
    {
      return _windows.TryGetValue(typeof(TWindow), out viewModel);
    }

    public void Register<TWindow>(BaseWindowViewModel viewModel) where TWindow : BaseWindowViewModel
    {
      if (_windows.ContainsKey(typeof(TWindow)))
      {
        _logService.LogWarning(GetType(), $"{typeof(TWindow)} already registered in WindowsRepository");
        return;
      }

      _windows.Add(typeof(TWindow), viewModel);
    }
  }
}