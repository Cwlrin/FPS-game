using Unity.Netcode;
using UnityEngine;

public class PlayerSetUp : NetworkBehaviour
{
    [SerializeField] private Behaviour[] componentsToDisable; // 禁用的组件

    private Camera _sceneCamera; // 场景相机

    // Start is called before the first frame update
    public override void OnNetworkSpawn() // 网络生成
    {
        base.OnNetworkSpawn(); // 调用基类方法

        if (!IsLocalPlayer) // 如果不是本地玩家
        {
            SetLayerMaskForAllChildren(transform, LayerMask.NameToLayer("Remote Player")); // 设置所有子物体的层
            DisableComponents(); // 禁用组件
        }
        else
        {
            SetLayerMaskForAllChildren(transform,LayerMask.NameToLayer("Player"));
            _sceneCamera = Camera.main; // 获取场景相机
            if (_sceneCamera != null) _sceneCamera.gameObject.SetActive(false); // 禁用场景相机
        }

        var name = "Player" + GetComponent<NetworkObject>().NetworkObjectId; // 获取玩家名称
        var player = GetComponent<Player>(); // 获取玩家
        player.Setup(); // 设置玩家

        GameManager.Singleton.RegisterPlayer(name, player); // 注册玩家
    }


    public override void OnNetworkDespawn() // 网络销毁
    {
        base.OnNetworkDespawn(); // 调用基类方法
        if (_sceneCamera != null) _sceneCamera.gameObject.SetActive(true); // 启用场景相机

        GameManager.Singleton.UnRegisterPlayer(transform.name); // 注销玩家
    }

    private void SetPlayerName() // 设置玩家名称
    {
        transform.name = "Player" + GetComponent<NetworkObject>().NetworkObjectId; // 设置玩家名称
    }

    private void DisableComponents() // 禁用组件
    {
        foreach (var t in componentsToDisable) // 遍历所有组件
            t.enabled = false; // 禁用组件
    }

    private void SetLayerMaskForAllChildren(Transform transform, LayerMask layerMask) // 设置所有子物体的层
    {
        transform.gameObject.layer = layerMask; // 设置层
        for (var i = 0; i < transform.childCount; i++) SetLayerMaskForAllChildren(transform.GetChild(i), layerMask);    // 遍历所有子物体
    }
}