using Unity.Netcode;
using UnityEngine;

public class PlayerShooting : NetworkBehaviour
{
    private const string PlayerTag = "Player";

    [SerializeField] private LayerMask mask;

    private Camera _cam;
    private PlayerWeapon _currentWeapon;

    private WeaponManager _weaponManager;


    enum HitEffectMaterial
    {
        Metal,
        Stone,
    }

    // Start is called before the first frame update
    private void Start()
    {
        _cam = GetComponentInChildren<Camera>();
        _weaponManager = GetComponent<WeaponManager>();
    }

    // Update is called once per frame
    private void Update()
    {
        if (!IsLocalPlayer) return;

        _currentWeapon = _weaponManager.GetCurrentWeapon();

        if (_currentWeapon.shootRate <= 0) //单发
        {
            if (Input.GetButtonDown("Fire1")) Shoot();
        }
        else // 连发
        {
            if (Input.GetButtonDown("Fire1"))
                InvokeRepeating("Shoot", 0f, 1f / _currentWeapon.shootRate);
            else if (Input.GetButtonUp("Fire1") || Input.GetKeyDown(KeyCode.Q)) CancelInvoke("Shoot");
        }
    }

    private void OnShoot() // 每次射击相关的逻辑，包括特效、声音等
    {
        _weaponManager.GetCurrentGraphics().muzzleFlash.Play();
    }

    [ClientRpc]
    private void OnShootClientRpc()
    {
        OnShoot();
    }

    [ServerRpc]
    private void OnShootServerRpc()
    {
        if (!IsHost) OnShoot();
        OnShootClientRpc();
    }

    private void OnHit(Vector3 pos, Vector3 normal, HitEffectMaterial material) // 击中点的特效
    {
        GameObject hitEffectPrefab;
        if (material == HitEffectMaterial.Metal)
        {
            hitEffectPrefab = _weaponManager.GetCurrentGraphics().metalHitEffectPrefab;
        }
        else
        {
            hitEffectPrefab = _weaponManager.GetCurrentGraphics().stoneHitEffectPrefab;
        }

        var hitEffectObject = Instantiate(hitEffectPrefab,pos,Quaternion.LookRotation(normal));
        var particleSystem = hitEffectObject.GetComponent<ParticleSystem>();
        particleSystem.Emit(1);
        particleSystem.Play();
        Destroy(hitEffectObject, 1f);
    }

    [ClientRpc]
    private void OnHitClientRpc(Vector3 pos, Vector3 normal, HitEffectMaterial material)
    {
        OnHit(pos, normal, material);
    }

    [ServerRpc]
    private void OnHitServerRpc(Vector3 pos, Vector3 normal, HitEffectMaterial material)
    {
        if (!IsHost)
        {
            OnHit(pos, normal, material);
        }
        OnHitClientRpc(pos, normal, material);
    }

    private void Shoot()
    {
        OnShootServerRpc();

        if (Physics.Raycast(_cam.transform.position, _cam.transform.forward, out var hit, _currentWeapon.range,
                mask))
        {
            if (hit.collider.CompareTag(PlayerTag))
            {
                ShootServerRpc(hit.collider.name, _currentWeapon.damage);
                OnHitServerRpc(hit.point, hit.normal, HitEffectMaterial.Metal);
            }
            else
            {
                OnHitServerRpc(hit.point, hit.normal, HitEffectMaterial.Stone);
            }
        }
    }

    [ServerRpc]
    private void ShootServerRpc(string name, int damage)
    {
        var player = GameManager.Singleton.GetPlayer(name);
        player.TakeDamage(damage);
    }
}