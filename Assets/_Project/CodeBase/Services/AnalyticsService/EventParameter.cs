using System;

namespace _Project.CodeBase.Services.AnalyticsService
{
  public readonly struct EventParameter
  {
    public string Key { get; }
    public EventParameterType ParameterType { get; }
    public long Long { get; }
    public double Double { get; }
    public string String { get; }

    private EventParameter(string key, EventParameterType parameterType, long longValue = 0, double doubleValue = 0,
      string stringValue = null)
    {
      Key = key;
      ParameterType = parameterType;
      Long = longValue;
      Double = doubleValue;
      String = stringValue;
    }

    public static EventParameter Create(string key, long longValue) =>
      new(key, EventParameterType.Long, longValue: longValue);

    public static EventParameter Create(string key, string stringValue) =>
      new(key, EventParameterType.String, stringValue: stringValue);

    public static EventParameter Create(string key, int intValue) =>
      new(key, EventParameterType.Long, longValue: intValue);

    public static EventParameter Create(string key, bool boolValue) =>
      new(key, EventParameterType.Long, longValue: (boolValue ? 1 : 0));

    public static EventParameter Create(string key, float floatValue) =>
      new(key, EventParameterType.Double, doubleValue: floatValue);

    public static EventParameter Create(string key, double doubleValue) =>
      new(key, EventParameterType.Double, doubleValue: doubleValue);

    public static EventParameter Create(string key, Enum enumValue) =>
      new(key, EventParameterType.Double, stringValue: enumValue.ToString());
  }
}