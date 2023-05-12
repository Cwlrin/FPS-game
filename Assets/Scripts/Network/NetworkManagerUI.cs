using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using Unity.Netcode.Transports.UNET;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class NetworkManagerUI : MonoBehaviour
{
    // [SerializeField] private Button hostBtn; // 主机按钮
    // [SerializeField] private Button serverBtn; // 服务器按钮
    // [SerializeField] private Button clientBtn; // 客户端按钮

    [SerializeField] private Button refreshButton; // 刷新按钮
    [SerializeField] private Button buildButton; // 构建按钮

    [SerializeField] private GameObject roomButtonPrefab; // 房间按钮预制体
    [SerializeField] private Canvas menuUI; // 菜单UI

    private readonly List<Button> _rooms = new(); // 房间列表

    private int _buildRoomPort = -1;

    // Start is called before the first frame update
    private void Start()
    {
        SetConfig(); // 设置配置
        InitButtons(); // 初始化按钮
        RefreshRoomList(); // 刷新房间列表
        // hostBtn.onClick.AddListener(() => // 添加主机按钮点击事件
        // {
        //     NetworkManager.Singleton.StartHost(); // 启动主机
        //     DestroyAllButtons(); // 销毁所有按钮
        // });
        // serverBtn.onClick.AddListener(() => // 添加服务器按钮点击事件
        // {
        //     NetworkManager.Singleton.StartServer(); // 启动服务器
        //     DestroyAllButtons(); // 销毁所有按钮
        // });
        // clientBtn.onClick.AddListener(() => // 添加客户端按钮点击事件
        // {
        //     NetworkManager.Singleton.StartClient(); // 启动客户端
        //     DestroyAllButtons(); // 销毁所有按钮
        // });
    }

    private void OnApplicationQuit()
    {
        if (_buildRoomPort != -1)
        {
            RemoveRoom();
        }
    }

    private void RemoveRoom()
    {
        StartCoroutine(RemoveRoomRequest("http://8.131.53.27:8080/game/remove_room/?port=" + _buildRoomPort.ToString())); // 启动协程
    }

    private IEnumerator RemoveRoomRequest(string uri)
    {
        var uwr = UnityWebRequest.Get(uri); // 创建请求
        yield return uwr.SendWebRequest(); // 发送请求

        if (uwr.result != UnityWebRequest.Result.ConnectionError)
        {
            var resp = JsonUtility.FromJson<RemoveRoomResponse>(uwr.downloadHandler.text); // 解析响应

            if (resp.error_massage == "success")
            {

            }
        }
    }

    private void RefreshRoomList()
    {
        StartCoroutine(RefreshRoomListRequest("http://8.131.53.27:8080/game/get_room_list/")); // 启动协程
    }

    private IEnumerator RefreshRoomListRequest(string uri)
    {
        var uwr = UnityWebRequest.Get(uri); // 创建请求
        yield return uwr.SendWebRequest(); // 发送请求

        if (uwr.result != UnityWebRequest.Result.ConnectionError)
        {
            var resp = JsonUtility.FromJson<GetRoomListResponse>(uwr.downloadHandler.text); // 解析响应
            foreach (var room in _rooms)
            {
                room.onClick.RemoveAllListeners(); // 移除所有监听器
                Destroy(room.gameObject); // 销毁按钮
            }

            _rooms.Clear(); // 清空房间列表

            var k = 0; // 计数器
            foreach (var room in resp.rooms) // 遍历房间列表
            {
                var buttonObj = Instantiate(roomButtonPrefab, menuUI.transform); // 实例化按钮
                buttonObj.transform.localPosition = new Vector3(40, 130 - k * 190, 0); // 设置位置
                var button = buttonObj.GetComponent<Button>(); // 获取按钮组件
                button.GetComponentInChildren<TextMeshProUGUI>().text = room.name; // 设置按钮文本
                button.onClick.AddListener(() => // 添加按钮点击事件
                {
                    var transport = GetComponent<UNetTransport>(); // 获取传输组件
                    transport.ConnectPort = transport.ServerListenPort = room.port; // 设置端口号
                    NetworkManager.Singleton.StartClient(); // 启动主机
                    DestroyAllButtons(); // 销毁所有按钮
                });
                _rooms.Add(button); // 添加到房间列表
                k++; // 计数器自增
            }

            print(resp.error_message); // 打印错误信息
            foreach (var room in resp.rooms) print(room.name + " - " + room.port); // 打印房间列表
        }
    }

    private void BuildRoomList()
    {
        StartCoroutine(BuildRoomListRequest("http://8.131.53.27:8080/game/build_room/")); // 启动协程
    }

    private IEnumerator BuildRoomListRequest(string uri)
    {
        var uwr = UnityWebRequest.Get(uri); // 创建请求
        yield return uwr.SendWebRequest(); // 发送请求

        if (uwr.result != UnityWebRequest.Result.ConnectionError)
        {
            var resp = JsonUtility.FromJson<BulidRoomResponse>(uwr.downloadHandler.text); // 解析响应
            if (resp.error_message == "success")
            {
                var transport = GetComponent<UNetTransport>(); // 获取传输组件
                transport.ConnectPort = transport.ServerListenPort = resp.port; // 设置端口号
                _buildRoomPort = resp.port; // 设置端口号
                NetworkManager.Singleton.StartClient(); // 启动主机
                DestroyAllButtons(); // 销毁所有按钮
            }
        }
    }

    private void InitButtons()
    {
        refreshButton.onClick.AddListener(RefreshRoomList); // 添加刷新按钮点击事件
        buildButton.onClick.AddListener(BuildRoomList); // 添加构建按钮点击事件
    }


    private void SetConfig()
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
    }

    // Update is called once per frame
    private void DestroyAllButtons() // 销毁所有按钮
    {
        refreshButton.onClick.RemoveAllListeners(); // 移除所有监听器
        buildButton.onClick.RemoveAllListeners(); // 移除所有监听器
        // Destroy(hostBtn.gameObject); // 销毁主机按钮
        // Destroy(serverBtn.gameObject); // 销毁服务器按钮
        // Destroy(clientBtn.gameObject); // 销毁客户端按钮
        Destroy(refreshButton.gameObject); // 销毁刷新按钮
        Destroy(buildButton.gameObject); // 销毁构建按钮

        foreach (var room in _rooms)
        {
            room.onClick.RemoveAllListeners(); // 移除所有监听器
            Destroy(room.gameObject); // 销毁按钮
        }
    }
}