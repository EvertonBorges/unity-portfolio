using System.Collections;
using System.Collections.Generic;
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
    [SerializeField] private Transform _cinemachineFpsTarget;

    [Header("Movement Parameters")]
    [SerializeField] private Animator _animator;
    [SerializeField] private float _speed = 2f;
    [Range(1f, 10f)][SerializeField] private float _speedRunFactor = 3f;
    [Range(1f, 100f)][SerializeField] private float _speedChangeRate = 1f;
    [Tooltip("How fast the character turns to face movement direction")]
    [Range(0f, 0.3f)][SerializeField] private float _rotationSmoothTime = 0.12f;
    [Range(0f, 5f)][SerializeField] private float _rotationSpeed = 1f;
    [Range(0f, 180f)][SerializeField] private float _topClamp = 90f;
    [Range(180f, 0f)][SerializeField] private float _bottomClamp = -90f;
    [SerializeField] private SO_Sound _SO_SoundFootstep;
    [SerializeField] private SO_Sound _SO_SoundLand;

    [Header("Interactable Parameters")]
    [SerializeField] private LayerMask _platformLayers;
    [SerializeField] private LayerMask _interactablesLayers;
    [SerializeField] private LayerMask _minigameLayers;
    private Platform m_platform = null;
    private IInteractable m_interactable = null;
    private float SphereRadius => _characterController.radius / 2f;
    private Vector3 m_sphereDirection => -MyTransform.up;
    private Vector3 m_origin => MyTransform.position;
    private float m_maxDistance = 5f;
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

    private int _animIDSpeed = 0, _animIDGrounded = 0, _animIDJump = 0, _animIDFreeFall = 0, _animIDMotionSpeed = 0;

    private float m_targetRotation = 0f, m_rotationVelocity, m_cinemachineTargetPitch;
    private Vector2 m_move = default, m_look = default;
    private Vector2 m_cursorPosition = default;

    private float m_speed = 0f, m_verticalVelocity = 0f, m_fallTimeoutDelta = 0f;
    private bool m_running = false, m_ground = true, m_canMoveByInput = true, m_canRotateCamera = false;

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

    private bool IsFpsCam => CameraController.Instance.IsFpsCam;

    private readonly Dictionary<string, Transform> m_renderers = new();
    private Transform FindRenderer(string name)
    {
        if (m_renderers.ContainsKey(name))
            return m_renderers[name];

        return null;
    }

    private const float _threshold = 0.01f;

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

    void Update()
    {
        if (m_interactingPlatform)
            return;

        ApplyGravity();

        CollisionCheck();

        Move();
    }

    void LateUpdate()
    {
        CameraRotation();
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
            m_maxDistance, _platformLayers.value))
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
            m_maxDistance, _interactablesLayers.value))
            return;

        if (!hitInfo.transform.TryGetComponent(out IInteractable interactable))
            return;

        m_interactable = interactable;
    }

    private void Move()
    {
        if (!m_canMoveByInput)
            return;

        var maxSpeed = m_running ? _speed * _speedRunFactor : _speed;

        bool isWalking = m_move != Vector2.zero;

        m_speed = Mathf.Lerp(m_speed, isWalking ? maxSpeed : 0f, Time.deltaTime * _speedChangeRate);

        Vector3 targetDirection = IsFpsCam ? GetMoveFpsDirection() : GetMoveTpsDirection();

        _characterController.Move(
            targetDirection.normalized * (m_speed * Time.deltaTime) +
            new Vector3(0.0f, m_verticalVelocity, 0.0f) * Time.deltaTime
        );

        Vector3 direction = new(m_move.x, 0.0f, m_move.y);

        _animator.SetFloat(_animIDMotionSpeed, direction.magnitude);

        _animator.SetFloat(_animIDSpeed, m_speed);
    }

    private Vector3 GetMoveFpsDirection()
    {
        Vector3 direction = new Vector3(m_move.x, 0.0f, m_move.y).normalized;

        if (m_move != Vector2.zero)
            direction = transform.right * m_move.x + transform.forward * m_move.y;

        return direction;
    }

    private Vector3 GetMoveTpsDirection()
    {
        Vector3 direction = new Vector3(m_move.x, 0.0f, m_move.y).normalized;

        if (m_move != Vector2.zero)
        {
            m_targetRotation =
                Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg +
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

        return Quaternion.Euler(0.0f, m_targetRotation, 0.0f) * Vector3.forward;
    }

    private void CameraRotation()
    {
        if (!m_canMoveByInput)
            return;

        if (!m_canRotateCamera)
            return;

        if (!IsFpsCam)
            return;

        if (m_look.sqrMagnitude < _threshold)
            return;

        float deltaTimeMultiplier = Time.deltaTime;

        m_cinemachineTargetPitch += m_look.y * _rotationSpeed * deltaTimeMultiplier;
        m_rotationVelocity = m_look.x * _rotationSpeed * deltaTimeMultiplier;

        m_cinemachineTargetPitch = ClampAngle(m_cinemachineTargetPitch, _bottomClamp, _topClamp);

        _cinemachineFpsTarget.transform.localRotation = Quaternion.Euler(m_cinemachineTargetPitch, 0.0f, 0.0f);

        transform.Rotate(Vector3.up * m_rotationVelocity);
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

        var renderers = m_transform.GetComponentsInChildren<SkinnedMeshRenderer>();
        
        m_renderers.Clear();
        foreach (var renderer in renderers)
            m_renderers.Add(renderer.transform.name, renderer.transform);

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

    private void OnLook(Vector2 value)
    {
        m_look = value;
    }

    private void OnCursorPosition(Vector2 value)
    {
        m_cursorPosition = value;
    }

    private void OnClick()
    {
        var ray = CameraController.Instance.MainCamera.ScreenPointToRay(m_cursorPosition);

        if (!Physics.Raycast(ray, out RaycastHit hit, 10f,  _minigameLayers.value))
            return;

        if (!hit.transform.TryGetComponent(out TicTacToe_Piece piece))
            return;

        piece.Select();
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

    private void OnCanRotateFpsCamera(bool value)
    {
        m_canRotateCamera = value;

        if (value)
            _cinemachineFpsTarget.transform.localRotation = Quaternion.Euler(0.0f, 0.0f, 0.0f);
    }

    private void OnLookAt(Transform target)
    {
        m_canMoveByInput = false;

        var position = target.position;

        position.y = m_transform.position.y;

        m_transform.LookAt(position, m_transform.up);
    }

    private void OnStartWalk(GameObject go, Transform target)
    {
        m_canMoveByInput = false;

        if (!IsFpsCam)
            EnableRenderer(true);

        MonoBehaviourHelper.StartCoroutine(WaitWalk(go, target));
    }

    private IEnumerator WaitWalk(GameObject go, Transform target)
    {
        OnLookAt(target);

        _animator.SetFloat(_animIDMotionSpeed, 1f);

        _animator.SetFloat(_animIDSpeed, _speed);

        var lastDistance = float.PositiveInfinity;

        var distance = float.PositiveInfinity - 1f;

        while (distance >= 0.1f && distance <= lastDistance)
        {
            _characterController.Move(
                m_transform.forward * (_speed * Time.deltaTime) +
                new Vector3(0.0f, m_verticalVelocity, 0.0f) * Time.deltaTime
            );

            lastDistance = distance;

            distance = Vector3.Distance(m_transform.position, target.position);

            yield return null;
        }

        _animator.SetFloat(_animIDMotionSpeed, 0f);

        _animator.SetFloat(_animIDSpeed, 0f);

        m_canMoveByInput = true;

        if (IsFpsCam)
            EnableRenderer(false);

        Manager_Events.Player.OnFinishWalk.Notify(go);
    }

    private void EnableRenderer(bool enabled)
    {
        var layer = LayerMask.NameToLayer(enabled ? "Player" : "FpsIgnore");

        FindRenderer("Renderer_EyeLeft").gameObject.layer = layer;
        FindRenderer("Renderer_EyeRight").gameObject.layer = layer;
        FindRenderer("Renderer_Head").gameObject.layer = layer;
        FindRenderer("Renderer_Teeth").gameObject.layer = layer;
        FindRenderer("Renderer_Hair").gameObject.layer = layer;
        FindRenderer("Renderer_Beard").gameObject.layer = layer;
        FindRenderer("Renderer_Glasses").gameObject.layer = layer;
    }

    void OnEnable()
    {
        Manager_Events.Player.OnMove += OnMove;

        Manager_Events.Player.OnLook += OnLook;

        Manager_Events.Player.OnRun += OnRun;

        Manager_Events.Player.OnInteract += OnInteract;

        Manager_Events.Player.OnCanRotateFpsCamera += OnCanRotateFpsCamera;

        Manager_Events.Player.OnStartWalk += OnStartWalk;

        Manager_Events.Player.OnLookAt += OnLookAt;

        Manager_Events.Player.OnCursorPosition += OnCursorPosition;

        Manager_Events.Player.OnClick += OnClick;
    }

    void OnDisable()
    {
        Manager_Events.Player.OnMove -= OnMove;

        Manager_Events.Player.OnLook -= OnLook;

        Manager_Events.Player.OnRun -= OnRun;

        Manager_Events.Player.OnInteract -= OnInteract;

        Manager_Events.Player.OnCanRotateFpsCamera -= OnCanRotateFpsCamera;

        Manager_Events.Player.OnStartWalk -= OnStartWalk;

        Manager_Events.Player.OnLookAt -= OnLookAt;

        Manager_Events.Player.OnCursorPosition -= OnCursorPosition;

        Manager_Events.Player.OnClick -= OnClick;
    }

    private static float ClampAngle(float lfAngle, float lfMin, float lfMax)
    {
        if (lfAngle < -360f) lfAngle += 360f;
        if (lfAngle > 360f) lfAngle -= 360f;
        return Mathf.Clamp(lfAngle, lfMin, lfMax);
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
            Manager_Events.Sound.OnPlay.Notify(_SO_SoundFootstep);
    }

    private void OnLand(AnimationEvent animationEvent)
    {
        if (animationEvent.animatorClipInfo.weight > 0.5f)
            Manager_Events.Sound.OnPlay.Notify(_SO_SoundLand);
    }

}
