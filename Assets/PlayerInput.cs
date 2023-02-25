using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private float speed = 5f;
    [SerializeField] private float lookSensitivity = 3f;
    [SerializeField] private PlayerController controller;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked; // 游戏开始时，鼠标消失
    }

    // Update is called once per frame
    private void Update()
    {
        var xMov = Input.GetAxisRaw("Horizontal");
        var yMov = Input.GetAxisRaw("Vertical");

        var velocity = (transform.right * xMov + transform.forward * yMov).normalized * speed;
        controller.Move(velocity);

        var xMouse = Input.GetAxisRaw("Mouse X");
        var yMouse = Input.GetAxisRaw("Mouse Y");
        // print(xMouse.ToString() + " " + yMouse.ToString()); // 调试用

        var yRotation = new Vector3(0f, xMouse, 0f) * lookSensitivity;
        var xRotation = new Vector3(-yMouse, 0f, 0f) * lookSensitivity;
        controller.Rotate(yRotation, xRotation);
    }
}