using UnityEngine;

public class GameManager : MonoBehaviour
{
    private static string _info;

    private void OnGUI()
    {
        GUILayout.BeginArea(new Rect(200f, 200f, 200f, 400f));
        GUILayout.BeginVertical();

        GUILayout.Label(_info);

        GUILayout.EndVertical();
        GUILayout.EndArea();
    }

    public static void UpdateInfo(string info)
    {
        _info = info;
    }
}