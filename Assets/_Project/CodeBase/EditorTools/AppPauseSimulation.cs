#if UNITY_EDITOR
using System.Reflection;

namespace _Project.CodeBase.EditorTools
{
  using UnityEditor;
  using UnityEngine;

  public class PauseSimulatorWindow : EditorWindow
  {
    [MenuItem("Tools/Simulate Application Pause")]
    public static void ShowWindow()
    {
      GetWindow<PauseSimulatorWindow>("Pause Simulator");
    }

    void OnGUI()
    {
      if (GUILayout.Button("Pause"))
        Simulate(true);
      if (GUILayout.Button("Resume"))
        Simulate(false);
    }

    private void Simulate(bool paused)
    {
      foreach (MonoBehaviour obj in FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None))
      {
        MethodInfo method = obj.GetType().GetMethod("OnApplicationPause",
          BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
        method?.Invoke(obj, new object[] { paused });
      }
    }
  }
}
#endif