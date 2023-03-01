using Unity.Netcode;
using UnityEngine;

public class Player : NetworkBehaviour
{
    [SerializeField] private int maxHealth = 100;

    private NetworkVariable<int> _currentHealth = new NetworkVariable<int>();

    public void Setup()
    {
        SetDefaults();
    }

    private void SetDefaults()
    {
        if (IsServer) _currentHealth.Value = maxHealth;
    }

    public void TakeDamage(int damage) // 受到了伤害，只在服务器端调用
    {
        _currentHealth.Value -= damage;
        if (_currentHealth.Value <= 0) _currentHealth.Value = 0;
    }

    public int GetHealth()
    {
        return _currentHealth.Value;
    }
}