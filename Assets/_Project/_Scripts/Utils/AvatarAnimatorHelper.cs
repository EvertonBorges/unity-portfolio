using ReadyPlayerMe.Core;
using UnityEngine;

public class AvatarAnimatorHelper : MonoBehaviour
{

    [SerializeField] private AvatarData _avatarData;

    private void Awake()
    {
        AvatarAnimationHelper.SetupAnimator(_avatarData.AvatarMetadata, _avatarData.gameObject);
    }

}
