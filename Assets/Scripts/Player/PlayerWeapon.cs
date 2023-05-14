using System;
using UnityEngine;

[Serializable]
public class PlayerWeapon
{
    public string name = "M16A1"; // 武器名称
    public int damage = 10; // 武器伤害
    public float range = 100f; // 武器射程

    public float shootRate = 10f; // 一秒可以打多少发子弹，如果 <= 0，则表示单发
    public float shootCoolDownTime = 0.75f; // 单发模式的冷却时间
    public float recoilForce = 2f; // 武器后坐力

    public int maxBullets = 30; // 武器最大子弹数
    public int bullets = 30; // 武器当前子弹数
    public float reloadTime = 2f; // 武器装弹时间

    [HideInInspector] public bool isReloading = false; // 武器是否正在装弹

    public GameObject graphics; // 武器的模型
}