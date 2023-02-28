using Unity.Netcode;
using UnityEngine;

public class PlayerSetUp : NetworkBehaviour
{
    [SerializeField] private Behaviour[] componentsToDisable;
    [SerializeField] private Camera sceneCamera;

    // Start is called before the first frame update
    private void Start()
    {
        if (!IsLocalPlayer)
        {
            foreach (var t in componentsToDisable)
                t.enabled = false;
        }
        else
        {
            sceneCamera = Camera.main;
            if (sceneCamera != null) sceneCamera.gameObject.SetActive(false);
        }
    }

    // Update is called once per frame
    private void Update()
    {
    }

    private void OnDisable()
    {
        if (sceneCamera != null) sceneCamera.gameObject.SetActive(true);
    }
}