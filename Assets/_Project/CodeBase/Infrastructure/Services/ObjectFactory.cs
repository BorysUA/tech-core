using Object = UnityEngine.Object;

namespace _Project.CodeBase.Infrastructure.Services
{
  public class ObjectFactory
  {
    public T Instantiate<T>(T source) where T : Object => 
      Object.Instantiate(source);
  }
}