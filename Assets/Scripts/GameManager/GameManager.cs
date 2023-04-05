using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private static readonly Dictionary<string, Player> _players = new(); // 所有玩家的列表
    public static GameManager Singleton; // 单例

    [SerializeField] public MatchingSettings matchingSettings; // 匹配设置

    private void Awake() // 初始化
    {
        Singleton = this; // 单例
    }

    private void OnGUI() // 在屏幕上显示玩家的血量
    {
        GUILayout.BeginArea(new Rect(200f, 200f, 200f, 400f)); // 设置显示区域
        GUILayout.BeginVertical(); // 设置垂直显示

        GUI.color = Color.red; // 设置字体颜色
        foreach (var name in _players.Keys) // 遍历所有玩家
        {
            var player = GetPlayer(name); // 获取玩家
            GUILayout.Label(name + " - " + player.GetHealth()); // 显示玩家的血量
        }

        GUILayout.EndVertical(); // 结束垂直显示
        GUILayout.EndArea(); // 结束显示区域
    }

    public void RegisterPlayer(string name, Player player) // 注册玩家
    {
        player.transform.name = name; // 设置玩家的名字
        _players.Add(name, player); // 添加玩家
    }

    public void UnRegisterPlayer(string name) // 注销玩家
    {
        _players.Remove(name); // 移除玩家
    }

    public Player GetPlayer(string name) // 获取玩家
    {
        return _players[name]; // 返回玩家
    }
}