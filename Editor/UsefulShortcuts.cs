using System.Reflection;
using UnityEditor;
using UnityEditor.ShortcutManagement;
using UnityEngine;
 
[InitializeOnLoad]
static class UsefulShortcuts
{
    // Alt + C
    [Shortcut("Clear Console", KeyCode.D, ShortcutModifiers.Alt)]
    public static void ClearConsole()
    {
        var assembly = Assembly.GetAssembly(typeof(SceneView));
        var type = assembly.GetType("UnityEditor.LogEntries");
        var method = type.GetMethod("Clear");
        Debug.Log("This is working");
        method.Invoke(new object(), null);
    }
}