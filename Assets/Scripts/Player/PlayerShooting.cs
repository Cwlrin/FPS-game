using Unity.Netcode;
using UnityEngine;

public class PlayerShooting : NetworkBehaviour
{
    private const string PlayerTag = "Player";

    [SerializeField] private PlayerWappon weapon;
    [SerializeField] private LayerMask mask;

    private Camera _cam;

    // Start is called before the first frame update
    private void Start()
    {
        _cam = GetComponentInChildren<Camera>();
    }

    // Update is called once per frame
    private void Update()
    {
        if (Input.GetButtonDown("Fire1")) Shoot();
    }

    private void Shoot()
    {
        if (Physics.Raycast(_cam.transform.position, _cam.transform.forward, out var hit, weapon.range, mask))
            if (hit.collider.CompareTag(PlayerTag))
                ShootServerRpc(hit.collider.name, weapon.damage);
    }

    [ServerRpc]
    private void ShootServerRpc(string name, int damage)
    {
        var player = GameManager.Singleton.GetPlayer(name);
        player.TakeDamage(damage);
    }
}