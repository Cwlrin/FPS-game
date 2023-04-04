using Unity.Netcode;
using UnityEngine;

public class PlayerShooting : NetworkBehaviour
{
    private const string PlayerTag = "Player";

    [SerializeField] private LayerMask mask;
    private int _autoShootCount; // 连发模式下，已经射击的次数
    private Camera _cam;
    private PlayerWeapon _currentWeapon;
    private PlayerController _playerController;

    private float _shootCoolDownTime; // 距离上次射击的时间，单位：秒
    private WeaponManager _weaponManager;

    // Start is called before the first frame update
    private void Start()
    {
        _cam = GetComponentInChildren<Camera>();
        _weaponManager = GetComponent<WeaponManager>();
        _playerController = GetComponent<PlayerController>();
    }

    // Update is called once per frame
    private void Update()
    {
        _shootCoolDownTime += Time.deltaTime;

        if (!IsLocalPlayer) return;

        _currentWeapon = _weaponManager.GetCurrentWeapon();

        if (_currentWeapon.shootRate <= 0) // 单发
        {
            if (Input.GetButtonDown("Fire1") && _shootCoolDownTime >= _currentWeapon.shootCoolDownTime)
            {
                _autoShootCount = 0;
                Shoot();
                _shootCoolDownTime = 0f; // 重置冷却时间
            }
        }
        else // 连发
        {
            if (Input.GetButtonDown("Fire1"))
                InvokeRepeating(nameof(Shoot), 0f, 1f / _currentWeapon.shootRate);
            else if (Input.GetButtonUp("Fire1") || Input.GetKeyDown(KeyCode.Q)) CancelInvoke(nameof(Shoot));
        }
    }

    private void OnShoot(float recoilForce) // 每次射击相关的逻辑，包括特效、声音等
    {
        _weaponManager.GetCurrentGraphics().muzzleFlash.Play();
        _weaponManager.GetCurrentAudioSource().Play();

        if (IsLocalPlayer) _playerController.AddRecoilForce(recoilForce);
    }

    [ClientRpc]
    private void OnShootClientRpc(float recoilForce)
    {
        OnShoot(recoilForce);
    }

    [ServerRpc]
    private void OnShootServerRpc(float recoilForce)
    {
        if (!IsHost) OnShoot(recoilForce);
        OnShootClientRpc(recoilForce);
    }

    private void OnHit(Vector3 pos, Vector3 normal, HitEffectMaterial material) // 击中点的特效
    {
        var hitEffectPrefab = material == HitEffectMaterial.Metal
            ? _weaponManager.GetCurrentGraphics().metalHitEffectPrefab
            : _weaponManager.GetCurrentGraphics().stoneHitEffectPrefab;

        var hitEffectObject = Instantiate(hitEffectPrefab, pos, Quaternion.LookRotation(normal));
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
        if (!IsHost) OnHit(pos, normal, material);
        OnHitClientRpc(pos, normal, material);
    }

    private void Shoot()
    {
        _autoShootCount++;
        var recoilForce = _currentWeapon.recoilForce;

        if (_autoShootCount <= 3) recoilForce *= 0.2f;

        OnShootServerRpc(recoilForce);

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


    private enum HitEffectMaterial
    {
        Metal,
        Stone
    }
}