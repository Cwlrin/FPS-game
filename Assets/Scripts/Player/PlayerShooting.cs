using Unity.Netcode;
using UnityEngine;

public class PlayerShooting : NetworkBehaviour
{
    private const string PlayerTag = "Player";

    private WeaponManager _weaponManager;
    private PlayerWeapon _currentWeapon;

    [SerializeField] private LayerMask mask;

    private Camera _cam;

    // Start is called before the first frame update
    private void Start()
    {
        _cam = GetComponentInChildren<Camera>();
        _weaponManager = GetComponent<WeaponManager>();
    }

    // Update is called once per frame
    private void Update()
    {
        _currentWeapon = _weaponManager.GetCurrentWeapon();

        if (_currentWeapon.shootRate <= 0) //单发
        {
            if (Input.GetButtonDown("Fire1")) Shoot();
        }
        else // 连发
        {
            if (Input.GetButtonDown("Fire1"))
                InvokeRepeating("Shoot", 0f, 1f / _currentWeapon.shootRate);
            else if (Input.GetButtonUp("Fire1")) CancelInvoke("Shoot");
        }
    }

    private void Shoot()
    {
        //Debug.Log("Shoot !!");

        if (!Physics.Raycast(_cam.transform.position, _cam.transform.forward, out var hit, _currentWeapon.range,
                mask)) return;
        if (hit.collider.CompareTag(PlayerTag))
            ShootServerRpc(hit.collider.name, _currentWeapon.damage);
    }

    [ServerRpc]
    private void ShootServerRpc(string name, int damage)
    {
        var player = GameManager.Singleton.GetPlayer(name);
        player.TakeDamage(damage);
    }
}