namespace _Project.CodeBase.Gameplay.Services.Resource.Results
{
  public struct AddResourceResult : ICommandResult
  {
    public bool IsSuccessful { get; }
    public int Amount { get; }

    public AddResourceResult(bool isSuccessful, int amount = 0)
    {
      IsSuccessful = isSuccessful;
      Amount = amount;
    }
  }
}