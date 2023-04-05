using Unity.Netcode;
using UnityEngine;

public class PlayerShooting : NetworkBehaviour
{
    private const string PlayerTag = "Player"; // 玩家标签

    [SerializeField] private LayerMask mask;
    private int _autoShootCount; // 连发模式下，已经射击的次数
    private Camera _cam; // 摄像机
    private PlayerWeapon _currentWeapon; // 当前武器
    private PlayerController _playerController; // 玩家控制器

    private float _shootCoolDownTime; // 距离上次射击的时间，单位：秒
    private WeaponManager _weaponManager; // 武器管理器

    // Start is called before the first frame update
    private void Start() // 初始化
    {
        _cam = GetComponentInChildren<Camera>(); // 获取摄像机
        _weaponManager = GetComponent<WeaponManager>(); // 获取武器管理器
        _playerController = GetComponent<PlayerController>(); // 获取玩家控制器
    }

    // Update is called once per frame
    private void Update()
    {
        _shootCoolDownTime += Time.deltaTime; // 更新距离上次射击的时间

        if (!IsLocalPlayer) return; // 如果不是本地玩家，直接返回

        _currentWeapon = _weaponManager.GetCurrentWeapon(); // 获取当前武器

        if (_currentWeapon.shootRate <= 0) // 单发
        {
            if (Input.GetButtonDown("Fire1") && _shootCoolDownTime >= _currentWeapon.shootCoolDownTime) // 按下鼠标左键
            {
                Shoot(); // 射击
                _shootCoolDownTime = 0f; // 重置冷却时间
            }
            else
            {
                _autoShootCount = 0; // 重置连发次数
                Shoot(); // 射击
                _shootCoolDownTime = 0f; // 重置冷却时间
            }
        }
        else // 连发
        {
            if (Input.GetButtonDown("Fire1")) // 按下鼠标左键
                InvokeRepeating(nameof(Shoot), 0f, 1f / _currentWeapon.shootRate); // 按照射击频率连发
            else if (Input.GetButtonUp("Fire1") || Input.GetKeyDown(KeyCode.Q))
                CancelInvoke(nameof(Shoot)); // 松开鼠标左键或按下Q键
        }
    }

    private void OnShoot(float recoilForce) // 每次射击相关的逻辑，包括特效、声音等
    {
        _weaponManager.GetCurrentGraphics().muzzleFlash.Play(); // 播放枪口火焰特效
        _weaponManager.GetCurrentAudioSource().Play(); // 播放枪声

        if (IsLocalPlayer) _playerController.AddRecoilForce(recoilForce); // 如果是本地玩家，添加后坐力
    }

    [ClientRpc]
    private void OnShootClientRpc(float recoilForce) // 每次射击相关的逻辑，包括特效、声音等
    {
        OnShoot(recoilForce); // 每次射击相关的逻辑，包括特效、声音等
    }

    [ServerRpc]
    private void OnShootServerRpc(float recoilForce) // 每次射击相关的逻辑，包括特效、声音等
    {
        if (!IsHost) OnShoot(recoilForce); // 如果不是主机，执行每次射击相关的逻辑，包括特效、声音等
        OnShootClientRpc(recoilForce); // 每次射击相关的逻辑，包括特效、声音等
    }

    private void OnHit(Vector3 pos, Vector3 normal, HitEffectMaterial material) // 击中点的特效
    {
        var hitEffectPrefab = material == HitEffectMaterial.Metal
            ? _weaponManager.GetCurrentGraphics().metalHitEffectPrefab
            : _weaponManager.GetCurrentGraphics().stoneHitEffectPrefab; // 获取击中点特效预制体

        var hitEffectObject = Instantiate(hitEffectPrefab, pos, Quaternion.LookRotation(normal)); // 实例化击中点特效
        var particleSystem = hitEffectObject.GetComponent<ParticleSystem>(); // 获取击中点特效的粒子系统
        particleSystem.Emit(1); // 发射粒子
        particleSystem.Play(); // 播放粒子
        Destroy(hitEffectObject, 1f); // 销毁击中点特效
    }

    [ClientRpc]
    private void OnHitClientRpc(Vector3 pos, Vector3 normal, HitEffectMaterial material) // 击中点的特效
    {
        OnHit(pos, normal, material); // 击中点的特效
    }

    [ServerRpc]
    private void OnHitServerRpc(Vector3 pos, Vector3 normal, HitEffectMaterial material) // 击中点的特效
    {
        if (!IsHost) OnHit(pos, normal, material); // 如果不是主机，执行击中点的特效
        OnHitClientRpc(pos, normal, material); // 击中点的特效
    }

    private void Shoot() // 射击
    {
        _autoShootCount++; // 连发模式下，已经射击的次数
        var recoilForce = _currentWeapon.recoilForce; // 后坐力

        if (_autoShootCount <= 3) recoilForce *= 0.2f; // 连发模式下，前3次射击后坐力减小
        else if (_autoShootCount <= 5) recoilForce *= 0.5f; // 连发模式下，第4、5次射击后坐力减小
        else if (_autoShootCount <= 7) recoilForce *= 0.8f; // 连发模式下，第6、7次射击后坐力减小

        OnShootServerRpc(recoilForce); // 每次射击相关的逻辑，包括特效、声音等

        if (Physics.Raycast(_cam.transform.position, _cam.transform.forward, out var hit, _currentWeapon.range,
                mask)) // 射线检测
        {
            if (hit.collider.CompareTag(PlayerTag)) // 玩家
            {
                ShootServerRpc(hit.collider.name, _currentWeapon.damage); // 服务器端射击
                OnHitServerRpc(hit.point, hit.normal, HitEffectMaterial.Metal); // 服务器端击中点特效
            }
            else if (hit.collider.CompareTag("Metal")) // 金属
            {
                ShootServerRpc(hit.collider.name, _currentWeapon.damage); // 服务器端射击
                OnHitServerRpc(hit.point, hit.normal, HitEffectMaterial.Metal); // 服务器端击中点特效
            }
            else
            {
                OnHitServerRpc(hit.point, hit.normal, HitEffectMaterial.Stone); // 石头
            }
        }
    }

    [ServerRpc]
    private void ShootServerRpc(string name, int damage) // 服务器端射击
    {
        var player = GameManager.Singleton.GetPlayer(name); // 获取玩家
        player.TakeDamage(damage); // 玩家扣血
    }


    private enum HitEffectMaterial // 物体材质
    {
        Metal,
        Stone
    }
}