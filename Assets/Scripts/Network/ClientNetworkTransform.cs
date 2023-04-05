using Unity.Netcode.Components;
using UnityEngine;

namespace ClientNetworkTransform
{
    [DisallowMultipleComponent]
    public class ClientNetworkTransform : NetworkTransform
    {
        protected override bool OnIsServerAuthoritative() // 服务器端控制
        {
            return false; // 客户端控制
        }
    }
}