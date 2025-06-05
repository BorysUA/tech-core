namespace _Project.CodeBase.Infrastructure.Services.Interfaces
{
  public interface IDataTransferService
  {
    public void SetData<T>(T value);
    public bool TryGetData<T>(out T data);
    public void Clear();
  }
}