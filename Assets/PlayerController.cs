using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private Rigidbody rb;
    [SerializeField] private Camera cam;

    private Vector3 _velocity = Vector3.zero; // 速度，每秒移动的距离
    private Vector3 _xRotation = Vector3.zero; // 旋转视角
    private Vector3 _yRotation = Vector3.zero; // 旋转角色

    private void Start()
    {
    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        PerformMovement();
        PerformRotation();
    }

    public void Move(Vector3 velocity)
    {
        _velocity = velocity;
    }


    public void Rotate(Vector3 yRotation, Vector3 xRotation)
    {
        _yRotation = yRotation;
        _xRotation = xRotation;
    }

    private void PerformMovement()
    {
        if (_velocity != Vector3.zero) rb.MovePosition(rb.position + _velocity * Time.fixedDeltaTime);
    }

    private void PerformRotation()
    {
        if (_yRotation != Vector3.zero) rb.transform.Rotate(_yRotation);
        if (_xRotation != Vector3.zero) cam.transform.Rotate(_xRotation);
    }
}