using ReadyPlayerMe.AvatarLoader;
using ReadyPlayerMe.Core;
using UnityEngine;

public class RPM_AvatarLoader : MonoBehaviour
{

    // main https://models.readyplayer.me/63b62a0d6b5c5b3acae7d89e.glb
    // Ada NPC https://models.readyplayer.me/648cadcd0b3048a825f564de.glb
    // Lara NPC https://models.readyplayer.me/648c93f71e94941b96ce3053.glb
    [SerializeField] private Animator _animator;
    [SerializeField] private string _glbUrl = "https://models.readyplayer.me/63b62a0d6b5c5b3acae7d89e.glb";

    private AvatarObjectLoader m_avatarLoader;

    private GameObject m_avatar;

    private void Awake()
    {
        _animator ??= GetComponent<Animator>();
    }

    private void Start()
    {
        ApplicationData.Log();

        m_avatarLoader = new();

        m_avatarLoader.OnCompleted += AvatarLoaderOnCompleted;

        m_avatarLoader.LoadAvatar(_glbUrl);
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

    private void OnDestroy()
    {
        if (m_avatar != null)
            Destroy(m_avatar);
    }

}
