using System;
using _Project.CodeBase.Data.Progress;
using _Project.CodeBase.Data.StaticData.Resource;
using _Project.CodeBase.Gameplay.Models.Persistent;
using _Project.CodeBase.Gameplay.Services.Command;
using _Project.CodeBase.Gameplay.Services.Resource.Commands;
using _Project.CodeBase.Infrastructure.Services;
using _Project.CodeBase.Infrastructure.Services.Interfaces;
using _Project.CodeBase.Utility;
using static _Project.CodeBase.Utility.UniqueIdGenerator;

namespace _Project.CodeBase.Gameplay.Services.Resource.Handlers
{
  public class AddResourceDropHandler : ICommandHandler<AddResourceDropCommand, Unit>
  {
    private readonly IProgressWriter _progressWriter;
    private readonly IStaticDataProvider _staticDataProvider;

    public AddResourceDropHandler(IProgressWriter progressWriter, IStaticDataProvider staticDataProvider)
    {
      _progressWriter = progressWriter;
      _staticDataProvider = staticDataProvider;
    }

    public Unit Execute(in AddResourceDropCommand command)
    {
      ResourceDropConfig dropConfig = _staticDataProvider.GetResourceDropConfig(command.ResourceDropType);

      int id = GenerateUniqueIntId();
      int seed = MathUtils.Hash32(_progressWriter.GameStateModel.SessionInfoWriter.Seed, id);

      Random random = new Random(seed);
      int amount = random.Next(dropConfig.ResourceRange.Amount.Min, dropConfig.ResourceRange.Amount.Max + 1);

      ResourceDropData resourceDropData = new ResourceDropData(id, command.ResourceDropType,
        dropConfig.ResourceRange.Resource.Kind, command.Position, command.SpawnPoint, amount);

      ResourceDropModel resourceDropModel = new ResourceDropModel(resourceDropData);
      _progressWriter.GameStateModel.WriteOnlyResourceDrops.Add(resourceDropData.Id, resourceDropModel);

      return Unit.Default;
    }
  }
}