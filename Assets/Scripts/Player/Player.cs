using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class Player : NetworkBehaviour
{
    private static readonly int Direction = Animator.StringToHash("direction");
    [SerializeField] private int maxHealth = 100; // 最大血量
    [SerializeField] private Behaviour[] componentsToDisable; // 禁用的组件

    private readonly NetworkVariable<int> _currentHealth = new(); // 当前血量
    private readonly NetworkVariable<bool> _isDead = new(); // 是否死亡

    private bool _colliderEnabled; // 碰撞器是否启用

    private bool[] _componentsEnabled; // 组件是否启用

    public void Setup() // 设置
    {
        _componentsEnabled = new bool[componentsToDisable.Length]; // 初始化组件是否启用
        for (var i = 0; i < componentsToDisable.Length; i++)
            _componentsEnabled[i] = componentsToDisable[i].enabled; // 获取组件是否启用

        var col = GetComponent<Collider>(); // 获取碰撞器
        _colliderEnabled = col.enabled; // 获取碰撞器是否启用

        SetDefaults(); // 设置默认值
    }

    private void SetDefaults() // 设置默认值
    {
        for (var i = 0; i < componentsToDisable.Length; i++)
            componentsToDisable[i].enabled = _componentsEnabled[i]; // 设置组件是否启用

        var col = GetComponent<Collider>(); // 获取碰撞器
        _colliderEnabled = col.enabled; // 获取碰撞器是否启用

        if (IsServer) // 服务器端
        {
            _currentHealth.Value = maxHealth; // 设置当前血量
            _isDead.Value = false; // 设置是否死亡
        }
    }

    public bool IsDead()
    {
        return _isDead.Value;
    }

    public void TakeDamage(int damage) // 受到了伤害，只在服务器端调用
    {
        if (_isDead.Value) return; // 如果已经死亡，直接返回

        _currentHealth.Value -= damage; // 减少血量
        if (_currentHealth.Value <= 0) // 如果血量小于等于0
        {
            _currentHealth.Value = 0; // 设置血量为0
            _isDead.Value = true; // 设置是否死亡

            if (!IsHost) DieOnServer(); // 如果不是主机，调用服务器端的死亡方法
            DieClientRpc(); // 调用客户端的死亡方法
        }
    }

    private IEnumerator Respawn() // 重生
    {
        yield return new WaitForSeconds(GameManager.Singleton.matchingSettings.respawnTime); // 等待重生时间
        SetDefaults(); // 设置默认值
        GetComponentInChildren<Animator>().SetInteger(Direction, 0); // 设置死亡动画
        GetComponent<Rigidbody>().useGravity = true; // 启用重力

        var col = GetComponent<Collider>(); // 获取碰撞器
        col.enabled = true; // 设置碰撞器是否启用

        if (IsLocalPlayer)
            transform.position = new Vector3(Random.Range(-12f, 32f), 10f, Random.Range(1f, 24f)); // 如果是本地玩家，设置位置
    }

    private void DieOnServer() // 服务器端的死亡方法
    {
        Die(); // 调用死亡方法
    }

    [ClientRpc]
    private void DieClientRpc() // 客户端的死亡方法
    {
        Die(); // 调用死亡方法
    }

    private void Die() // 死亡方法
    {
        GetComponent<PlayerShooting>().StopShooting();


        GetComponentInChildren<Animator>().SetInteger(Direction, -1); // 设置死亡动画
        GetComponent<Rigidbody>().useGravity = false; // 禁用重力

        foreach (var t in componentsToDisable) t.enabled = false; // 禁用组件

        var col = GetComponent<Collider>(); // 获取碰撞器
        col.enabled = false; // 禁用碰撞器

        StartCoroutine(Respawn()); // 重生
    }

    public int GetHealth() // 获取血量
    {
        return _currentHealth.Value; // 返回血量
    }
}