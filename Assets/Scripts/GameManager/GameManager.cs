using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private static Dictionary<string, Player> _players = new();

    [SerializeField] public MatchingSettings matchingSettings;
    public static GameManager Singleton;

    private void Awake()
    {
        Singleton = this;
    }

    private void OnGUI()
    {
        GUILayout.BeginArea(new Rect(200f, 200f, 200f, 400f));
        GUILayout.BeginVertical();

        GUI.color = Color.red;
        foreach (var name in _players.Keys)
        {
            var player = GetPlayer(name);
            GUILayout.Label(name + " - " + player.GetHealth());
        }

        GUILayout.EndVertical();
        GUILayout.EndArea();
    }

    public void RegisterPlayer(string name, Player player)
    {
        player.transform.name = name;
        _players.Add(name, player);
    }

    public void UnRegisterPlayer(string name)
    {
        _players.Remove(name);
    }

    public Player GetPlayer(string name)
    {
        return _players[name];
    }
}