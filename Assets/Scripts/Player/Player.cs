using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class Player : NetworkBehaviour
{
    [SerializeField] private int maxHealth = 100;
    [SerializeField] private Behaviour[] componentsToDisable;
    private bool _colliderEnabled;

    private bool[] _componentsEnabled;

    private NetworkVariable<int> _currentHealth = new();
    private NetworkVariable<bool> _isDead = new();

    public void Setup()
    {
        _componentsEnabled = new bool[componentsToDisable.Length];
        for (var i = 0; i < componentsToDisable.Length; i++) _componentsEnabled[i] = componentsToDisable[i].enabled;

        var col = GetComponent<Collider>();
        _colliderEnabled = col.enabled;

        SetDefaults();
    }

    private void SetDefaults()
    {
        for (var i = 0; i < componentsToDisable.Length; i++) componentsToDisable[i].enabled = _componentsEnabled[i];

        var col = GetComponent<Collider>();
        _colliderEnabled = col.enabled;

        if (IsServer)
        {
            _currentHealth.Value = maxHealth;
            _isDead.Value = false;
        }
    }

    public void TakeDamage(int damage) // 受到了伤害，只在服务器端调用
    {
        if (_isDead.Value) return;

        _currentHealth.Value -= damage;
        if (_currentHealth.Value <= 0)
        {
            _currentHealth.Value = 0;
            _isDead.Value = true;

            if (!IsHost) DieOnServer();
            DieClientRpc();
        }
    }

    private IEnumerator Respawn() // 重生
    {
        yield return new WaitForSeconds(GameManager.Singleton.matchingSettings.respawnTime);
        SetDefaults();

        if (IsLocalPlayer) transform.position = new Vector3(0f, 10f, 0f);
    }

    private void DieOnServer()
    {
        Die();
    }

    [ClientRpc]
    private void DieClientRpc()
    {
        Die();
    }

    private void Die()
    {
        foreach (var t in componentsToDisable) t.enabled = false;

        var col = GetComponent<Collider>();
        col.enabled = false;

        StartCoroutine(Respawn());
    }

    public int GetHealth()
    {
        return _currentHealth.Value;
    }
}