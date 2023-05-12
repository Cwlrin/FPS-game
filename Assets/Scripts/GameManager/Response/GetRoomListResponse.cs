using System;

[Serializable]
public class GetRoomListResponse
{
    public string error_message; // 错误信息
    public Room[] rooms; // 房间列表
}