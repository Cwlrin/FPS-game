using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private Rigidbody rb; // 刚体
    [SerializeField] private Camera cam; // 摄像机

    [SerializeField] private float cameraRotationTotal; // 累计转了多少角度
    [SerializeField] private float cameraRotationLimits = 85f; // 视角转动的限制角度
    private float _recoilForce; // 后坐力

    private Vector3 _thrusterForce = Vector3.zero; // 向上的推力

    private Vector3 _velocity = Vector3.zero; // 速度，每秒移动的距离
    private Vector3 _xRotation = Vector3.zero; // 旋转视角
    private Vector3 _yRotation = Vector3.zero; // 旋转角色

    // Update is called once per frame
    private void FixedUpdate()
    {
        PerformMovement(); // 移动
        PerformRotation(); // 旋转
    }

    public void Move(Vector3 velocity) // 移动
    {
        _velocity = velocity; // 速度
    }

    public void Rotate(Vector3 yRotation, Vector3 xRotation) // 旋转
    {
        _yRotation = yRotation; // 旋转角色
        _xRotation = xRotation; // 旋转视角
    }

    public void Thrust(Vector3 thrusterForce) // 推力
    {
        _thrusterForce = thrusterForce; // 向上的推力
    }

    public void AddRecoilForce(float newRecoilForce) // 后坐力
    {
        _recoilForce = newRecoilForce; // 后坐力
    }

    private void PerformMovement() // 移动
    {
        if (_velocity != Vector3.zero)
            rb.MovePosition(rb.position + _velocity * Time.fixedDeltaTime); // 作用 Time.fixedDeltaTime秒：0.02s
        if (_thrusterForce != Vector3.zero) rb.AddForce(_thrusterForce); // 作用 Time.fixedDeltaTime秒：0.02s
    }

    private void PerformRotation()
    {
        if (_yRotation != Vector3.zero || _recoilForce > 0) // 旋转角色
            rb.transform.Rotate(_yRotation +
                                rb.transform.up *
                                Random.Range(-2f * _recoilForce, 2f * _recoilForce)); // 作用 Time.fixedDeltaTime秒：0.02s
        if (_xRotation != Vector3.zero || _recoilForce > 0) // 旋转视角
        {
            cameraRotationTotal += _xRotation.x - _recoilForce; // 作用 Time.fixedDeltaTime秒：0.02s
            cameraRotationTotal =
                Mathf.Clamp(cameraRotationTotal, -cameraRotationLimits, cameraRotationLimits); // 限制视角转动的角度
            cam.transform.localEulerAngles = new Vector3(cameraRotationTotal, 0f, 0f); // 作用 Time.fixedDeltaTime秒：0.02s
        }

        _recoilForce *= 0.5f; // 后坐力衰减
    }
}