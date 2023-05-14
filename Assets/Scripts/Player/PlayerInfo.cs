using TMPro;
using UnityEngine;

public class PlayerInfo : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI playerName; // 玩家名
    [SerializeField] private Transform playerHealth; // 血条填充
    [SerializeField] private Transform infoUI; // 玩家信息UI

    private Player _player; // 玩家

    // Start is called before the first frame update
    private void Start()
    {
        _player = GetComponent<Player>(); // 获取玩家
    }

    // Update is called once per frame
    private void Update()
    {
        playerName.text = transform.name; // 设置玩家名
        playerHealth.localScale = new Vector3(_player.GetHealth() / 100f, 1f, 1f); // 设置血条

        var camera = Camera.main;
        infoUI.LookAt(infoUI.transform.position + camera.transform.rotation * Vector3.back,
            camera.transform.rotation * Vector3.up);
        infoUI.Rotate(new Vector3(0f, 180f, 0f)); // 设置玩家信息UI的朝向
    }
}