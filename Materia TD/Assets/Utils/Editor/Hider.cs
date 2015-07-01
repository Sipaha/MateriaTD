using UnityEngine;
using UnityEditor;

public class Hider : EditorWindow
{

    [MenuItem("Window/Hider&Destroyer")]
    public static void Create()
    {
        GetWindow<Hider>();
    }

    void OnGUI()
    {
        GameObject[] objects = FindObjectsOfType<GameObject>();

        foreach (GameObject obj in objects)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label(obj.name);
            if (GUILayout.Button("Destroy"))
            {
                DestroyImmediate(obj);
            }
            GUILayout.EndHorizontal();
        }
    }
}
