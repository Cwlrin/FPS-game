using System;
using UnityEngine;

[Serializable]
public class PlayerWeapon
{
    public string name = "M16A1";
    public int damage = 10;
    public float range = 100f;

    public float shootRate = 10f; // 一秒可以打多少发子弹，如果 <= 0，则表示单发

    public GameObject graphics;
}