using ReadyPlayerMe.AvatarLoader;
using ReadyPlayerMe.Core;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class PlayerController : Singleton<PlayerController>
{

    // main https://models.readyplayer.me/63b62a0d6b5c5b3acae7d89e.glb
    // Ada NPC https://models.readyplayer.me/648cadcd0b3048a825f564de.glb
    // Lara NPC https://models.readyplayer.me/648c93f71e94941b96ce3053.glb
    [SerializeField] private Animator _animator;
    [SerializeField] private CharacterController _characterController;
    [SerializeField] private float _speed = 2f;
    [SerializeField] private string _glbUrl = "https://models.readyplayer.me/63b62a0d6b5c5b3acae7d89e.glb";

    [Tooltip("How fast the character turns to face movement direction")]
    [Range(0.0f, 0.3f)][SerializeField] private float _rotationSmoothTime = 0.12f;

    private AvatarObjectLoader m_avatarLoader;

    private GameObject m_avatar;

    private float m_targetRotation = 0f;
    private float m_rotationVelocity;
    private Vector2 m_move = default;

    private Camera MainCamera => CameraController.Instance.MainCamera;

    protected override void Init()
    {
        base.Init();

        _animator ??= GetComponent<Animator>();

        _characterController ??= GetComponent<CharacterController>();
    }

    protected override void StartInit()
    {
        base.StartInit();

        StartAvatar();
    }

    private void Update()
    {
        Move();
    }

    private void StartAvatar()
    {
        ApplicationData.Log();

        m_avatarLoader = new();

        m_avatarLoader.OnCompleted += AvatarLoaderOnCompleted;

        m_avatarLoader.LoadAvatar(_glbUrl);
    }

    private void Move()
    {
        if (m_move == Vector2.zero)
            return;

        Vector3 direction = new Vector3(m_move.x, 0.0f, m_move.y).normalized;

        m_targetRotation =
            Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + 
            MainCamera.transform.eulerAngles.y;

        float rotation =
            Mathf.SmoothDampAngle(
                transform.eulerAngles.y,
                m_targetRotation,
                ref m_rotationVelocity,
                _rotationSmoothTime
            );

        transform.rotation = Quaternion.Euler(0.0f, rotation, 0.0f);

        Vector3 targetDirection = Quaternion.Euler(0.0f, m_targetRotation, 0.0f) * Vector3.forward;

        _characterController.Move(targetDirection.normalized * (_speed * Time.deltaTime));
    }

    private void AvatarLoaderOnCompleted(object obj, CompletionEventArgs args)
    {
        m_avatar = args.Avatar;

        m_avatar.transform.SetParent(transform);
        m_avatar.transform.name = "RPM_Model";

        _animator.Rebind();
        _animator.Update(0f);

        m_avatarLoader.OnCompleted -= AvatarLoaderOnCompleted;
    }

    private void OnMove(Vector2 value)
    {
        m_move = value;
    }

    void OnEnable()
    {
        Manager_Events.Player.OnMove += OnMove;
    }

    void OnDisable()
    {
        Manager_Events.Player.OnMove -= OnMove;
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();

        if (m_avatar != null)
            Destroy(m_avatar);
    }

}
