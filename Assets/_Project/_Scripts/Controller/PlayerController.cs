using System.Collections;
using System.Collections.Generic;
using ReadyPlayerMe.Core;
using UnityEngine;

public class PlayerController : Singleton<PlayerController>
{

    // main https://models.readyplayer.me/63b62a0d6b5c5b3acae7d89e.glb
    // Ada NPC https://models.readyplayer.me/648cadcd0b3048a825f564de.glb
    // Lara NPC https://models.readyplayer.me/648c93f71e94941b96ce3053.glb
    [SerializeField] private CharacterController _characterController;
    [SerializeField] private Transform _cinemachineFpsTarget;
    [SerializeField] private bool _useModel = false;
    [SerializeField] private GameObject _model;
    [SerializeField] private RuntimeAnimatorController _runtineAnimatorController;
    [SerializeField] private Avatar _avatarAnimator;

    [Header("Movement Parameters")]
    [SerializeField] private float _speed = 2f;
    [Range(1f, 10f)][SerializeField] private float _speedRunFactor = 3f;
    [Range(1f, 100f)][SerializeField] private float _speedChangeRate = 1f;
    [Tooltip("How fast the character turns to face movement direction")]
    [Range(0f, 0.3f)][SerializeField] private float _rotationSmoothTime = 0.12f;
    [Range(0f, 5f)][SerializeField] private float _rotationSpeed = 1f;
    [Range(0f, 180f)][SerializeField] private float _topClamp = 90f;
    [Range(180f, -90f)][SerializeField] private float _bottomClamp = -90f;
    [SerializeField] private SO_Sound _SO_SoundFootstep;
    [SerializeField] private SO_Sound _SO_SoundLand;

    [Header("Interactable Parameters")]
    [SerializeField] private LayerMask _interactablesLayers;
    [SerializeField] private LayerMask _minigameLayers;
    [SerializeField] private GameObject _interactableWarn;
    private IInteractable m_interactable = null;
    private bool m_interacting = false;
    private float SphereRadius => _characterController.radius / 2f;
    private Vector3 SphereDirection => -MyTransform.up;
    private Vector3 Origin => MyTransform.position;
    private readonly float m_maxDistance = 5f;

    [Header("Gravity Parameters")]
    [SerializeField] private float _fallTimeout = 0.15f;
    [SerializeField] private float _gravity = -15f;
    [SerializeField] private float _maxVerticalVelocity = 53f;
    [Tooltip("What layers the character uses as ground")]
    [SerializeField] private LayerMask _groundLayers;

    [Header("ReadyPlayerMe Parameters")]
    [SerializeField] private string _glbUrl = "https://models.readyplayer.me/63b62a0d6b5c5b3acae7d89e.glb";

    private PlayerAnimatorController m_playerAnimatorController;
    private AvatarObjectLoader m_avatarLoader;
    private GameObject m_avatar;

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

    private bool m_avatarLoaded = false;

    private bool IsFpsCam => CameraController.Instance.IsFpsCam;
    private bool Pause => GameManager.Instance.Pause;
    private bool CanMove => m_avatarLoaded && !m_interacting && !Manager_Dialog.Instance.Show;

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

        _characterController ??= GetComponent<CharacterController>();

        _interactableWarn.SetActive(false);
    }

    protected override void StartInit()
    {
        base.StartInit();

        StartAvatar();
    }

    void Update()
    {
        if (!CanMove)
            return;

        if (Pause)
            return;

        ApplyGravity();

        CollisionCheck();

        Move();
    }

    void LateUpdate()
    {
        if (Pause)
            return;
            
        CameraRotation();
    }

    private void StartAvatar()
    {
        m_avatarLoaded = false;

        if (_useModel)
            StartAvatarByModel();
        else
            StartAvatarReadyPlayerMe();
    }

    private void StartAvatarByModel()
    {
        m_avatar = Instantiate(_model, m_transform);

        var metadata = m_avatar.GetComponent<AvatarData>().AvatarMetadata;

        AvatarLoaded(metadata);
    }

    private void StartAvatarReadyPlayerMe()
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

            m_playerAnimatorController.OnFreeFall(false);

            if (m_verticalVelocity < 0.0f)
                m_verticalVelocity = -2f;
        }
        else
        {
            if (m_fallTimeoutDelta >= 0.0f)
                m_fallTimeoutDelta -= Time.deltaTime;
            else
                m_playerAnimatorController.OnFreeFall(true);
        }

        m_verticalVelocity += _gravity * Time.deltaTime;

        if (m_verticalVelocity >= _maxVerticalVelocity)
            m_verticalVelocity = _maxVerticalVelocity;
    }

    private void CollisionCheck()
    {
        GroundedCheck();

        var hadInteractable = m_interactable != null;

        InteractableCheck();

        if (!hadInteractable && m_interactable != null && !_interactableWarn.activeSelf)
        {
            _interactableWarn.SetActive(true);
        }
        else if (hadInteractable && m_interactable == null && _interactableWarn.activeSelf)
        {
            _interactableWarn.SetActive(false);
        }
    }

    private void GroundedCheck()
    {
        m_ground = Physics.CheckSphere(Origin, SphereRadius, _groundLayers, QueryTriggerInteraction.Ignore);

        m_playerAnimatorController.OnGround(m_ground);
    }

    private void InteractableCheck()
    {
        m_interactable = null;

        var origin = Origin + Vector3.up * (m_maxDistance - 0.05f);

        if (!Physics.Raycast(origin, SphereDirection, out RaycastHit hitInfo,
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

        bool isWalking = m_move != Vector2.zero && !Manager_Dialog.Instance.Show;

        Vector3 targetDirection = IsFpsCam ? GetMoveFpsDirection() : GetMoveTpsDirection();

        m_speed = Mathf.Lerp(m_speed, isWalking ? maxSpeed : 0f, Time.deltaTime * _speedChangeRate);

        _characterController.Move(
            targetDirection.normalized * (m_speed * Time.deltaTime) +
            new Vector3(0.0f, m_verticalVelocity, 0.0f) * Time.deltaTime
        );

        Vector3 direction = new(m_move.x, 0.0f, m_move.y);

        m_playerAnimatorController.SetMotionSpeed(direction.magnitude);
        m_playerAnimatorController.SetSpeed(m_speed);
    }

    private Vector3 GetMoveFpsDirection()
    {
        Vector3 direction = new Vector3(m_move.x, 0.0f, m_move.y).normalized;

        if (m_move != Vector2.zero)
            direction = MyTransform.right * m_move.x + MyTransform.forward * m_move.y;

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

            if (!Manager_Dialog.Instance.Show)
            {
                float rotation =
                    Mathf.SmoothDampAngle(
                        MyTransform.eulerAngles.y,
                        m_targetRotation,
                        ref m_rotationVelocity,
                        _rotationSmoothTime
                    );

                MyTransform.rotation = Quaternion.Euler(0.0f, rotation, 0.0f);
            }
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

        AvatarLoaded(args.Metadata);

        m_avatarLoader.OnCompleted -= AvatarLoaderOnCompleted;
    }

    private void AvatarLoaded(AvatarMetadata metadata)
    {
        m_avatar.SetLayerRecursively("Player");
        m_avatar.transform.SetParent(MyTransform);
        m_avatar.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
        m_avatar.transform.name = "RPM_Model";

        var animator = m_avatar.GetComponent<Animator>();
        animator.runtimeAnimatorController = _runtineAnimatorController;
        animator.avatar = _avatarAnimator;

        m_playerAnimatorController = m_avatar.AddComponent<PlayerAnimatorController>();
        m_playerAnimatorController.Setup(this);

        AvatarAnimationHelper.SetupAnimator(metadata, m_avatar);

        PopulateRenderers(m_transform);

        m_avatarLoaded = true;
    }

    private void PopulateRenderers(Transform parent)
    {
        var renderers = parent.GetComponentsInChildren<SkinnedMeshRenderer>();

        m_renderers.Clear();

        foreach (var renderer in renderers)
            m_renderers.Add(renderer.transform.name, renderer.transform);
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
        CheckClickOnTicTacToePiece();

        CheckClickOnDotsLine();

        CheckClickOnMemoryPiece();
    }

    private void CheckClickOnTicTacToePiece()
    {
        var ray = CameraController.Instance.MainCamera.ScreenPointToRay(m_cursorPosition);

        if (!Physics.Raycast(ray, out RaycastHit hit, 10f, _minigameLayers.value))
            return;

        if (!hit.transform.TryGetComponent(out TicTacToe_Piece piece))
            return;

        piece.Select();
    }

    private void CheckClickOnDotsLine()
    {
        var ray = CameraController.Instance.MainCamera.ScreenPointToRay(m_cursorPosition);

        if (!Physics.Raycast(ray, out RaycastHit hit, 10f, _minigameLayers.value))
            return;

        if (!hit.transform.TryGetComponent(out Dots_Line line))
            return;

        line.Select();
    }

    private void CheckClickOnMemoryPiece()
    {
        var ray = CameraController.Instance.MainCamera.ScreenPointToRay(m_cursorPosition);

        if (!Physics.Raycast(ray, out RaycastHit hit, 100f, _minigameLayers.value))
            return;

        if (!hit.transform.TryGetComponent(out Memory_Piece piece))
            return;

        piece.Select();
    }

    private void OnPause()
    {
        if (!CanMove)
            return;

        if (Pause)
            Manager_Events.GameManager.Unpause.Notify();
        else
            Manager_Events.GameManager.Pause.Notify();
    }

    private void OnInteract()
    {
        if (Manager_Dialog.Instance.Show)
        {
            Manager_Events.Dialog.NextDialog.Notify();

            return;
        }

        if (m_interactable != null)
        {
            m_interacting = !m_interacting;

            if (m_interacting)
                m_interactable.Interact();
            else
                m_interactable.InverseInteract();

            _interactableWarn.SetActive(!m_interacting);
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

        m_playerAnimatorController.SetMotionSpeed(1f);

        m_playerAnimatorController.SetSpeed(_speed);

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

        m_playerAnimatorController.SetMotionSpeed(0f);
        
        m_playerAnimatorController.SetSpeed(0f);

        m_canMoveByInput = true;

        m_interacting = false;

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
        Manager_Events.Player.OnPause += OnPause;
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
        Manager_Events.Player.OnPause -= OnPause;
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

    public void OnFootstep()
    {
        Manager_Events.Sound.OnPlay.Notify(_SO_SoundFootstep);
    }

    public void OnLand()
    {
        Manager_Events.Sound.OnPlay.Notify(_SO_SoundLand);
    }

}
