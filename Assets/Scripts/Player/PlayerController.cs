using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private Rigidbody rb;
    [SerializeField] private Camera cam;

    [SerializeField] private float cameraRotationTotal; // 累计转了多少角度
    [SerializeField] private float cameraRotationLimits = 85f;
    private float _recoilForce; // 后坐力

    private Vector3 _thrusterForce = Vector3.zero; // 向上的推力

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

    public void Thrust(Vector3 thrusterForce)
    {
        _thrusterForce = thrusterForce;
    }

    public void AddRecoilForce(float newRecoilForce)
    {
        _recoilForce = newRecoilForce;
    }

    private void PerformMovement()
    {
        if (_velocity != Vector3.zero) rb.MovePosition(rb.position + _velocity * Time.fixedDeltaTime);
        if (_thrusterForce != Vector3.zero) rb.AddForce(_thrusterForce); // 作用 Time.fixedDeltaTime秒：0.02s
    }

    private void PerformRotation()
    {
        if (_yRotation != Vector3.zero || _recoilForce > 0)
            rb.transform.Rotate(_yRotation + rb.transform.up * Random.Range(-2f * _recoilForce, 2f * _recoilForce));
        if (_xRotation != Vector3.zero || _recoilForce > 0)
        {
            cameraRotationTotal += _xRotation.x - _recoilForce;
            cameraRotationTotal = Mathf.Clamp(cameraRotationTotal, -cameraRotationLimits, cameraRotationLimits);
            cam.transform.localEulerAngles = new Vector3(cameraRotationTotal, 0f, 0f);
        }

        _recoilForce *= 0.5f;
    }
}