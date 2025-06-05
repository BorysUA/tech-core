using System;
using System.Collections.Generic;
using _Project.CodeBase.Infrastructure.Services.Interfaces;

namespace _Project.CodeBase.Infrastructure.Services
{
  public class DataTransferService : IDataTransferService
  {
    private readonly Dictionary<Type, object> _data = new();

    public void SetData<T>(T value)
    {
      _data[typeof(T)] = value;
    }

    public bool TryGetData<T>(out T data)
    {
      data = default;

      if (_data.TryGetValue(typeof(T), out var value))
      {
        data = (T)value;
        return true;
      }

      return false;
    }

    public void Clear()
    {
      _data.Clear();
    }
  }
}