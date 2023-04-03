using Unity.Netcode;
using UnityEngine;

public class WeaponManager : NetworkBehaviour
{
    [SerializeField] private PlayerWeapon primaryWeapon;
    [SerializeField] private PlayerWeapon secondaryWeapon;
    [SerializeField] private GameObject weaponHolder;
    private AudioSource _currentAudioSource;

    private WeaponGraphics _currentGraphics;
    private PlayerWeapon _currentWeapon;

    // Start is called before the first frame update
    private void Start()
    {
        EquipWeapon(primaryWeapon);
    }

    // Update is called once per frame
    private void Update()
    {
        if (!IsLocalPlayer) return;
        if (Input.GetKeyDown(KeyCode.Q))
            ToggleWeaponServerRpc();
    }

    private void EquipWeapon(PlayerWeapon weapon)
    {
        _currentWeapon = weapon;

        if (weaponHolder.transform.childCount > 0) Destroy(weaponHolder.transform.GetChild(0).gameObject);

        var weaponObject = Instantiate(_currentWeapon.graphics, weaponHolder.transform.position,
            weaponHolder.transform.rotation);
        weaponObject.transform.SetParent(weaponHolder.transform);

        _currentGraphics = weaponObject.GetComponent<WeaponGraphics>();
        _currentAudioSource = weaponObject.GetComponent<AudioSource>();

        if (IsLocalPlayer) _currentAudioSource.spatialBlend = 0f;
    }

    private void ToggleWeapon()
    {
        EquipWeapon(_currentWeapon == primaryWeapon ? secondaryWeapon : primaryWeapon);
    }

    [ClientRpc]
    private void ToggleWeaponClientRpc()
    {
        ToggleWeapon();
    }

    [ServerRpc]
    private void ToggleWeaponServerRpc()
    {
        if (!IsHost) ToggleWeapon();
        ToggleWeaponClientRpc();
    }


    public PlayerWeapon GetCurrentWeapon()
    {
        return _currentWeapon;
    }

    public WeaponGraphics GetCurrentGraphics()
    {
        return _currentGraphics;
    }

    public AudioSource GetCurrentAudioSource()
    {
        return _currentAudioSource;
    }
}