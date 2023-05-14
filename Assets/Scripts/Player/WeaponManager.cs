using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class WeaponManager : NetworkBehaviour
{
    [SerializeField] private PlayerWeapon primaryWeapon; // 主武器
    [SerializeField] private PlayerWeapon secondaryWeapon; // 副武器
    [SerializeField] private GameObject weaponHolder; // 武器挂载点
    private AudioSource _currentAudioSource; // 当前音频源

    private WeaponGraphics _currentGraphics; // 当前武器图形
    private PlayerWeapon _currentWeapon; // 当前武器

    // Start is called before the first frame update
    private void Start()
    {
        EquipWeapon(primaryWeapon); // 装备主武器
    }

    // Update is called once per frame
    private void Update()
    {
        if (!IsLocalPlayer) return; // 如果不是本地玩家
        if (Input.GetKeyDown(KeyCode.Q)) // 按下Q键
            ToggleWeaponServerRpc(); // 切换武器
    }

    private void EquipWeapon(PlayerWeapon weapon) // 装备武器
    {
        _currentWeapon = weapon; // 设置当前武器

        if (weaponHolder.transform.childCount > 0) Destroy(weaponHolder.transform.GetChild(0).gameObject); // 销毁当前武器

        var weaponObject = Instantiate(_currentWeapon.graphics, weaponHolder.transform.position,
            weaponHolder.transform.rotation); // 实例化武器
        weaponObject.transform.SetParent(weaponHolder.transform); // 设置武器的父物体

        _currentGraphics = weaponObject.GetComponent<WeaponGraphics>(); // 获取武器图形
        _currentAudioSource = weaponObject.GetComponent<AudioSource>(); // 获取音频源

        if (IsLocalPlayer) _currentAudioSource.spatialBlend = 0f; // 如果是本地玩家，设置音频源的空间混合因子为0
    }

    private void ToggleWeapon() // 切换武器
    {
        EquipWeapon(_currentWeapon == primaryWeapon ? secondaryWeapon : primaryWeapon); // 切换武器
    }

    [ClientRpc]
    private void ToggleWeaponClientRpc() // 切换武器
    {
        ToggleWeapon(); // 切换武器
    }

    [ServerRpc]
    private void ToggleWeaponServerRpc() // 切换武器
    {
        if (!IsHost) ToggleWeapon(); // 如果不是主机，切换武器
        ToggleWeaponClientRpc(); // 切换武器
    }


    public PlayerWeapon GetCurrentWeapon() // 获取当前武器
    {
        return _currentWeapon; // 返回当前武器
    }

    public WeaponGraphics GetCurrentGraphics() // 获取当前武器图形
    {
        return _currentGraphics; // 返回当前武器图形
    }

    public AudioSource GetCurrentAudioSource() // 获取当前音频源
    {
        return _currentAudioSource; // 返回当前音频源
    }

    public void Reload(PlayerWeapon playerWeapon) // 重新装弹
    {
        if (playerWeapon.isReloading) return; // 如果正在装弹，直接返回
        playerWeapon.isReloading = true; // 设置正在装弹
        print("Reloading...");

        StartCoroutine(ReloadCoroutine(playerWeapon));
    }

    private IEnumerator ReloadCoroutine(PlayerWeapon playerWeapon) // 重新装弹协程
    {
        yield return new WaitForSeconds(playerWeapon.reloadTime); // 等待装弹时间

        playerWeapon.bullets=playerWeapon.maxBullets; // 设置子弹数量为最大子弹数量
        playerWeapon.isReloading = false; // 设置正在装弹

    }
}