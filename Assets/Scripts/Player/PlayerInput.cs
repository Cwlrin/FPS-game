using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private float speed = 5f; // 移动速度
    [SerializeField] private float lookSensitivity = 5f; // 视角转动的灵敏度
    [SerializeField] private PlayerController controller; // 玩家控制器
    [SerializeField] private float thrusterForce = 20f; // 推力

    private ConfigurableJoint _joint; // 关节

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked; // 游戏开始时，鼠标消失
        _joint = GetComponent<ConfigurableJoint>(); // 获取关节
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

        var force = Vector3.zero; // 推力
        if (Input.GetButton("Jump")) // 按下空格键
        {
            force = Vector3.up * thrusterForce; // 向上的推力
            _joint.yDrive = new JointDrive // 关节的驱动
            {
                positionSpring = 0f, // 弹簧
                positionDamper = 0f, // 阻尼
                maximumForce = 0f // 最大力
            };
        }
        else
        {
            _joint.yDrive = new JointDrive // 关节的驱动
            {
                positionSpring = 20f,
                positionDamper = 0f,
                maximumForce = 40f
            };
        }

        controller.Thrust(force); // 推力
    }
}