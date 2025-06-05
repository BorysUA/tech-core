using System;
using System.Collections.Generic;
using _Project.CodeBase.Services.LogService;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace _Project.CodeBase.Infrastructure.Services
{
  public class TypeKeyDictionaryConverter<TValue> : JsonConverter<Dictionary<Type, TValue>>
  {
    private readonly ILogService _logService;

    public TypeKeyDictionaryConverter(ILogService logService)
    {
      _logService = logService;
    }

    public override void WriteJson(JsonWriter writer, Dictionary<Type, TValue> value, JsonSerializer serializer)
    {
      writer.WriteStartObject();

      foreach (KeyValuePair<Type, TValue> keyValuePair in value)
      {
        writer.WritePropertyName(keyValuePair.Key.AssemblyQualifiedName);
        serializer.Serialize(writer, keyValuePair.Value, typeof(TValue));
      }

      writer.WriteEndObject();
    }

    public override Dictionary<Type, TValue> ReadJson(JsonReader reader, Type objectType,
      Dictionary<Type, TValue> existingValue, bool hasExistingValue, JsonSerializer serializer)
    {
      Dictionary<Type, TValue> dictionary = hasExistingValue ? existingValue : new Dictionary<Type, TValue>();
      JObject jsonObject = JObject.Load(reader);

      foreach (JProperty property in jsonObject.Properties())
      {
        Type type = Type.GetType(property.Name);

        if (type == null)
        {
          _logService.LogError(GetType(), $"Cannot resolve type [{property.Name}] during deserialization",
            new ArgumentNullException());
          continue;
        }

        TValue value = property.Value.ToObject<TValue>(serializer);
        dictionary.Add(type, value);
      }

      return dictionary;
    }
  }
}