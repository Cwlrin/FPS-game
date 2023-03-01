using Unity.Netcode;
using UnityEngine;

public class PlayerSetUp : NetworkBehaviour
{
    [SerializeField] private Behaviour[] componentsToDisable;

    private Camera _sceneCamera;

    // Start is called before the first frame update
    private void Start()
    {
        if (!IsLocalPlayer)
        {
            DisableComponents();
        }
        else
        {
            _sceneCamera = Camera.main;
            if (_sceneCamera != null) _sceneCamera.gameObject.SetActive(false);
        }

        SetPlayerName();
    }

    // Update is called once per frame
    private void Update()
    {
    }

    private void OnDisable()
    {
        if (_sceneCamera != null) _sceneCamera.gameObject.SetActive(true);
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