using System;
using Unity.Netcode;
using Unity.Netcode.Transports.UNET;
using UnityEngine;
using UnityEngine.UI;

public class NetworkManagerUI : MonoBehaviour
{
    [SerializeField] private Button hostBtn; // 主机按钮
    [SerializeField] private Button serverBtn; // 服务器按钮
    [SerializeField] private Button clientBtn; // 客户端按钮

    [SerializeField] private Button room1; // 客户端按钮
    [SerializeField] private Button room2; // 客户端按钮

    // Start is called before the first frame update
    private void Start()
    {
        var args = Environment.GetCommandLineArgs(); // 获取命令行参数

        for (var i = 0; i < args.Length; i++)
            if (args[i] == "-port")
            {
                var port = int.Parse(args[i + 1]); // 获取端口号
                var transport = GetComponent<UNetTransport>(); // 获取传输组件
                transport.ConnectPort = transport.ServerListenPort = port; // 设置端口号
            }

        for (var i = 0; i < args.Length; i++)
            if (args[i] == "-launch-as-server") // 如果是服务器
            {
                NetworkManager.Singleton.StartServer(); // 启动服务器
                DestroyAllButtons(); // 销毁所有按钮
            }

        room1.onClick.AddListener(() => // 添加主机按钮点击事件
        {
            var transport = GetComponent<UNetTransport>(); // 获取传输组件
            transport.ConnectPort = transport.ServerListenPort = 7777; // 设置端口号
            NetworkManager.Singleton.StartClient(); // 启动主机
            DestroyAllButtons(); // 销毁所有按钮
        });

        room2.onClick.AddListener(() => // 添加主机按钮点击事件
        {
            var transport = GetComponent<UNetTransport>(); // 获取传输组件
            transport.ConnectPort = transport.ServerListenPort = 7778; // 设置端口号
            NetworkManager.Singleton.StartClient(); // 启动主机
            DestroyAllButtons(); // 销毁所有按钮
        });

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
        Destroy(room1.gameObject);
        Destroy(room2.gameObject);
    }
}