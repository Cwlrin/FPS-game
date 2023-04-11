using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private float speed = 5f; // 移动速度
    [SerializeField] private float lookSensitivity = 5f; // 视角转动的灵敏度
    [SerializeField] private PlayerController controller; // 玩家控制器
    [SerializeField] private float thrusterForce = 20f; // 推力

    private float _distToGround; // 距离地面的距离

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked; // 游戏开始时，鼠标消失
        _distToGround = GetComponent<Collider>().bounds.extents.y; // 距离地面的距离
    }

    // Update is called once per frame
    private void Update()
    {
        var xMov = Input.GetAxisRaw("Horizontal"); // 获取水平方向的输入
        var yMov = Input.GetAxisRaw("Vertical"); // 获取垂直方向的输入

        var velocity = (transform.right * xMov + transform.forward * yMov).normalized * speed; // 速度
        controller.Move(velocity); // 移动

        var xMouse = Input.GetAxisRaw("Mouse X"); // 获取鼠标的水平方向的输入
        var yMouse = Input.GetAxisRaw("Mouse Y"); // 获取鼠标的垂直方向的输入
        // print(xMouse.ToString() + " " + yMouse.ToString()); // 调试用

        var yRotation = new Vector3(0f, xMouse, 0f) * lookSensitivity; // 旋转角色
        var xRotation = new Vector3(-yMouse, 0f, 0f) * lookSensitivity; // 旋转视角
        controller.Rotate(yRotation, xRotation); // 旋转

        if (Input.GetButton("Jump")) // 按下空格键
        {
            if (Physics.Raycast(transform.position, -Vector3.up, _distToGround + 0.1f)) // 距离地面的距离 + 0.1f
            {
                var force = Vector3.up * thrusterForce; // 向上的推力
                controller.Thrust(force); // 推力
            }
        }
    }
}