using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class BulidRoomResponse
{
    public string error_message; // 错误信息
    public string name; // 房间名
    public int port; // 端口
}
