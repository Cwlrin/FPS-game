using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class NetworkManagerUI : MonoBehaviour
{
    [SerializeField] private Button hostBtn; // 主机按钮
    [SerializeField] private Button serverBtn; // 服务器按钮
    [SerializeField] private Button clientBtn; // 客户端按钮

    // Start is called before the first frame update
    private void Start()
    {
        hostBtn.onClick.AddListener(() => // 添加主机按钮点击事件
        {
            NetworkManager.Singleton.StartHost(); // 启动主机
            DestroyAllButtons(); // 销毁所有按钮
        });
        serverBtn.onClick.AddListener(() => // 添加服务器按钮点击事件
        {
            NetworkManager.Singleton.StartServer(); // 启动服务器
            DestroyAllButtons(); // 销毁所有按钮
        });
        clientBtn.onClick.AddListener(() => // 添加客户端按钮点击事件
        {
            NetworkManager.Singleton.StartClient(); // 启动客户端
            DestroyAllButtons(); // 销毁所有按钮
        });
    }

    // Update is called once per frame
    private void DestroyAllButtons() // 销毁所有按钮
    {
        Destroy(hostBtn.gameObject); // 销毁主机按钮
        Destroy(serverBtn.gameObject); // 销毁服务器按钮
        Destroy(clientBtn.gameObject); // 销毁客户端按钮
    }
}