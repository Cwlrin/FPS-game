using Unity.Netcode;
using UnityEngine;

public class PlayerController : NetworkBehaviour
{
    private const float Eps = 0.001f; // 误差

    private static readonly int Direction = Animator.StringToHash("direction");

    // Start is called before the first frame update
    [SerializeField] private Rigidbody rb; // 刚体
    [SerializeField] private Camera cam; // 摄像机

    [SerializeField] private float cameraRotationTotal; // 累计转了多少角度
    [SerializeField] private float cameraRotationLimits = 85f; // 视角转动的限制角度

    private Animator _animator; // 动画

    private float _distToGround; // 距离地面的距离
    private Vector3 _lastFramePosition = Vector3.zero; // 上一帧的位置
    private float _recoilForce; // 后坐力

    private Vector3 _thrusterForce = Vector3.zero; // 向上的推力
    private Vector3 _velocity = Vector3.zero; // 速度，每秒移动的距离
    private Vector3 _xRotation = Vector3.zero; // 旋转视角
    private Vector3 _yRotation = Vector3.zero; // 旋转角色

    private void Start()
    {
        _lastFramePosition = transform.position; // 记录上一帧的位置
        _animator = GetComponentInChildren<Animator>(); // 动画
        _distToGround = GetComponent<Collider>().bounds.extents.y; // 距离地面的距离
    }

    private void Update()
    {
        if (!IsLocalPlayer) PerformAnimation(); // 动画
    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        if (IsLocalPlayer)
        {
            PerformMovement(); // 移动
            PerformRotation(); // 旋转
            PerformAnimation(); // 动画
        }
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

    private void PerformAnimation()
    {
        var deltaPosition = transform.position - _lastFramePosition; // 当前帧的位置
        _lastFramePosition = transform.position; // 记录上一帧的位置

        var forward = Vector3.Dot(deltaPosition, transform.forward); // 前进
        var right = Vector3.Dot(deltaPosition, transform.right); // 右移

        var direction = 0; // 静止
        switch (forward)
        {
            case > Eps:
                direction = 1;
                break;
            // 后退
            // 右后退
            case < -Eps when right > Eps:
                direction = 4;
                break;
            // 左后退
            case < -Eps when right < -Eps:
                direction = 6;
                break;
            // 后退
            case < -Eps:
                direction = 5; // 后退
                break;
            default:
            {
                direction = right switch
                {
                    > Eps => 3,
                    // 左
                    < -Eps => 7,
                    _ => direction
                };
                break;
            }
        }

        if (!Physics.Raycast(transform.position, -Vector3.up, _distToGround + 0.1f)) direction = 8;

        if (GetComponent<Player>().IsDead()) direction = -1;
        _animator.SetInteger(Direction, direction); // 动画
    }

    private void PerformMovement() // 移动
    {
        if (_velocity != Vector3.zero)
            rb.MovePosition(rb.position + _velocity * Time.fixedDeltaTime); // 作用 Time.fixedDeltaTime秒：0.02s
        if (_thrusterForce != Vector3.zero)
        {
            rb.AddForce(_thrusterForce);
            _thrusterForce = Vector3.zero; // 清空
        }
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