using Unity.Netcode;
using UnityEngine;

public class PlayerSetUp : NetworkBehaviour
{
    [SerializeField] private Behaviour[] componentsToDisable;

    private Camera _sceneCamera;

    // Start is called before the first frame update
    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        if (!IsLocalPlayer)
        {
            DisableComponents();
        }
        else
        {
            _sceneCamera = Camera.main;
            if (_sceneCamera != null) _sceneCamera.gameObject.SetActive(false);
        }

        var name = "Player" + GetComponent<NetworkObject>().NetworkObjectId.ToString();
        var player = GetComponent<Player>();
        player.Setup();

        GameManager.Singleton.RegisterPlayer(name, player);
    }


    public override void OnNetworkDespawn()
    {
        base.OnNetworkDespawn();
        if (_sceneCamera != null) _sceneCamera.gameObject.SetActive(true);

        GameManager.Singleton.UnRegisterPlayer(transform.name);
    }

    private void SetPlayerName()
    {
        transform.name = "Player" + GetComponent<NetworkObject>().NetworkObjectId;
    }

    private void DisableComponents()
    {
        foreach (var t in componentsToDisable)
            t.enabled = false;
    }
}