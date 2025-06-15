using System;
using Object = UnityEngine.Object;

namespace _Project.CodeBase.Infrastructure
{
  public class ObjectFactory
  {
    public T Instantiate<T>(T source) where T : Object => 
      Object.Instantiate(source);
  }
}