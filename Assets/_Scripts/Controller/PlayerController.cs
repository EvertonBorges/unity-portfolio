using ReadyPlayerMe.AvatarLoader;
using ReadyPlayerMe.Core;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class PlayerController : Singleton<PlayerController>
{

    // main https://models.readyplayer.me/63b62a0d6b5c5b3acae7d89e.glb
    // Ada NPC https://models.readyplayer.me/648cadcd0b3048a825f564de.glb
    // Lara NPC https://models.readyplayer.me/648c93f71e94941b96ce3053.glb
    [SerializeField] private CharacterController _characterController;

    [Header("Movement Parameters")]
    [SerializeField] private Animator _animator;
    [SerializeField] private float _speed = 2f;
    [Range(1f, 10f)][SerializeField] private float _speedRunFactor = 3f;
    [Range(1f, 100f)][SerializeField] private float _speedChangeRate = 1f;
    [Tooltip("How fast the character turns to face movement direction")]
    [Range(0.0f, 0.3f)][SerializeField] private float _rotationSmoothTime = 0.12f;
    [SerializeField] private SO_Sound _SO_SoundFootstep;
    [SerializeField] private SO_Sound _SO_SoundLand;

    [Header("Interactable Parameters")]
    [SerializeField] private LayerMask _platformLayers;
    [SerializeField] private LayerMask _interactablesLayers;
    private Platform m_platform = null;
    private Interactable m_interactable = null;
    private float SphereRadius => _characterController.radius / 2f;
    private Vector3 m_sphereDirection => -MyTransform.up;
    private Vector3 m_origin => MyTransform.position;
    private float m_maxDistance = 0.1f;
    private bool m_interactingPlatform = false;
    private bool m_interactingInteractable = false;

    [Header("Gravity Parameters")]
    [SerializeField] private float _fallTimeout = 0.15f;
    [SerializeField] private float _gravity = -15f;
    [SerializeField] private float _maxVerticalVelocity = 53f;
    [Tooltip("What layers the character uses as ground")]
    [SerializeField] private LayerMask _groundLayers;

    [Header("ReadyPlayerMe Parameters")]
    [SerializeField] private string _glbUrl = "https://models.readyplayer.me/63b62a0d6b5c5b3acae7d89e.glb";

    private AvatarObjectLoader m_avatarLoader;

    private GameObject m_avatar;

    private int _animIDSpeed = 0;
    private int _animIDGrounded = 0;
    private int _animIDJump = 0;
    private int _animIDFreeFall = 0;
    private int _animIDMotionSpeed = 0;

    private float m_targetRotation = 0f;
    private float m_rotationVelocity;
    private Vector2 m_move = default;

    private float m_speed = 0f;
    private float m_verticalVelocity = 0f;
    private float m_fallTimeoutDelta = 0f;
    private bool m_running = false;
    private bool m_ground = true;

    private Camera MainCamera => CameraController.Instance.MainCamera;
    private Transform m_transform = null;
    private Transform MyTransform
    {
        get
        {
            m_transform ??= transform;

            return m_transform;
        }
    }

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

        AssignAnimationIDs();
    }

    private void Update()
    {
        if (m_interactingPlatform)
            return;

        ApplyGravity();

        CollisionCheck();

        Move();
    }

    private void AssignAnimationIDs()
    {
        _animIDSpeed = Animator.StringToHash("Speed");
        _animIDGrounded = Animator.StringToHash("Grounded");
        _animIDJump = Animator.StringToHash("Jump");
        _animIDFreeFall = Animator.StringToHash("FreeFall");
        _animIDMotionSpeed = Animator.StringToHash("MotionSpeed");
    }

    private void StartAvatar()
    {
        ApplicationData.Log();

        m_avatarLoader = new();

        m_avatarLoader.OnCompleted += AvatarLoaderOnCompleted;

        m_avatarLoader.LoadAvatar(_glbUrl);
    }

    private void ApplyGravity()
    {
        if (m_ground)
        {
            m_fallTimeoutDelta = _fallTimeout;

            _animator.SetBool(_animIDFreeFall, false);

            if (m_verticalVelocity < 0.0f)
                m_verticalVelocity = -2f;
        }
        else
        {
            if (m_fallTimeoutDelta >= 0.0f)
                m_fallTimeoutDelta -= Time.deltaTime;
            else
                _animator.SetBool(_animIDFreeFall, true);
        }

        m_verticalVelocity += _gravity * Time.deltaTime;

        if (m_verticalVelocity >= _maxVerticalVelocity)
            m_verticalVelocity = _maxVerticalVelocity;
    }

    private void CollisionCheck()
    {
        GroundedCheck();

        PlatformCheck();

        InteractableCheck();
    }

    private void GroundedCheck()
    {
        m_ground = Physics.CheckSphere(m_origin, SphereRadius, _groundLayers, QueryTriggerInteraction.Ignore);

        _animator.SetBool(_animIDGrounded, m_ground);
    }

    private void PlatformCheck()
    {
        m_platform = null;

        var origin = m_origin + Vector3.up * (m_maxDistance - 0.05f);

        if (!Physics.Raycast(origin, m_sphereDirection, out RaycastHit hitInfo,
            m_maxDistance, _platformLayers))
            return;

        if (!hitInfo.transform.TryGetComponent(out Platform platform))
            return;

        m_platform = platform;
    }

    private void InteractableCheck()
    {
        m_interactable = null;

        var origin = m_origin + Vector3.up * (m_maxDistance - 0.05f);

        if (!Physics.Raycast(origin, m_sphereDirection, out RaycastHit hitInfo,
            m_maxDistance, _interactablesLayers))
            return;

        if (!hitInfo.transform.TryGetComponent(out Interactable interactable))
            return;

        m_interactable = interactable;
    }

    private void Move()
    {
        var maxSpeed = m_running ? _speed * _speedRunFactor : _speed;

        bool isWalking = m_move != Vector2.zero;

        m_speed = Mathf.Lerp(m_speed, isWalking ? maxSpeed : 0f, Time.deltaTime * _speedChangeRate);

        Vector3 direction = new(m_move.x, 0.0f, m_move.y);

        Vector3 normalizedDirection = direction.normalized;

        if (isWalking)
        {
            m_targetRotation =
                Mathf.Atan2(normalizedDirection.x, normalizedDirection.z) * Mathf.Rad2Deg +
                MainCamera.transform.eulerAngles.y;

            float rotation =
                Mathf.SmoothDampAngle(
                    MyTransform.eulerAngles.y,
                    m_targetRotation,
                    ref m_rotationVelocity,
                    _rotationSmoothTime
                );

            MyTransform.rotation = Quaternion.Euler(0.0f, rotation, 0.0f);
        }

        Vector3 targetDirection = Quaternion.Euler(0.0f, m_targetRotation, 0.0f) * Vector3.forward;

        _characterController.Move(
            targetDirection.normalized * (m_speed * Time.deltaTime) +
            new Vector3(0.0f, m_verticalVelocity, 0.0f) * Time.deltaTime
        );

        _animator.SetFloat(_animIDMotionSpeed, direction.magnitude);

        _animator.SetFloat(_animIDSpeed, m_speed);
    }

    private void AvatarLoaderOnCompleted(object obj, CompletionEventArgs args)
    {
        m_avatar = args.Avatar;

        m_avatar.SetLayerRecursively("Player");
        m_avatar.transform.SetParent(MyTransform);
        m_avatar.transform.localPosition = Vector3.zero;
        m_avatar.transform.name = "RPM_Model";

        _animator.Rebind();
        _animator.Update(0f);

        m_avatarLoader.OnCompleted -= AvatarLoaderOnCompleted;
    }

    private void OnMove(Vector2 value)
    {
        m_move = value;
    }

    private void OnRun(bool value)
    {
        m_running = value;
    }

    private void OnInteract()
    {
        if (m_platform != null)
        {
            m_interactingPlatform = !m_interactingPlatform;

            if (m_interactingPlatform)
                m_platform.Interact();
            else
                m_platform.InverseInteract();
        }

        if (!m_interactingPlatform && m_interactable != null)
        {
            m_interactingInteractable = !m_interactingInteractable;

            if (m_interactingInteractable)
                m_interactable.Interact();
            else
                m_interactable.InverseInteract();
        }
    }

    void OnEnable()
    {
        Manager_Events.Player.OnMove += OnMove;

        Manager_Events.Player.OnRun += OnRun;

        Manager_Events.Player.OnInteract += OnInteract;
    }

    void OnDisable()
    {
        Manager_Events.Player.OnMove -= OnMove;

        Manager_Events.Player.OnRun -= OnRun;

        Manager_Events.Player.OnInteract -= OnInteract;
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();

        if (m_avatar != null)
            Destroy(m_avatar);
    }

    private void OnFootstep(AnimationEvent animationEvent)
    {
        if (animationEvent.animatorClipInfo.weight > 0.5f)
        {
            Manager_Events.Sound.OnPlay.Notify(_SO_SoundFootstep);
        }
    }

    private void OnLand(AnimationEvent animationEvent)
    {
        if (animationEvent.animatorClipInfo.weight > 0.5f)
        {
            Manager_Events.Sound.OnPlay.Notify(_SO_SoundLand);
        }
    }

}
