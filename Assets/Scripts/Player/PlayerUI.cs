using TMPro;
using UnityEngine;

public class PlayerUI : MonoBehaviour
{
    public static PlayerUI Singleton; // 单例

    [SerializeField] private TextMeshProUGUI bulletsText; // 血量文本
    [SerializeField] private GameObject bulletsObject; // 血量文本游戏对象

    private Player _player = null; // 玩家

    private WeaponManager _weaponManager; // 武器管理器

    [SerializeField] private Transform healthBarFill; // 血条填充
    [SerializeField] private GameObject healthBarObject; // 血条游戏对象

    private void Awake()
    {
        Singleton = this; // 设置单例
    }

    // Update is called once per frame
    private void Update()
    {
        if (_player == null) return; // 如果玩家为空，返回

        var currentWeapon = _weaponManager.GetCurrentWeapon(); // 获取当前武器
        if (currentWeapon.isReloading)
            bulletsText.text = "Reloading..."; // 显示重新装填
        else
            bulletsText.text = "Bullets: " + currentWeapon.bullets + "/" + currentWeapon.maxBullets; // 显示子弹数量

        healthBarFill.localScale = new Vector3(_player.GetHealth() / 100f, 1f, 1f); // 更新血条
    }

    public void SetPlayer(Player localPlayer) // 设置玩家
    {
        _player = localPlayer; // 设置玩家
        _weaponManager = _player.GetComponent<WeaponManager>(); // 获取武器管理器
        bulletsObject.SetActive(true); // 激活血量文本游戏对象
        healthBarObject.SetActive(true); // 激活血条游戏对象
    }
}