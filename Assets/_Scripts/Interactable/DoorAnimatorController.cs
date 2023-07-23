using UnityEngine;

public class DoorAnimatorController : MonoBehaviour
{

    [SerializeField] private Animator _animator;

    public void Open(bool value)
    {
        _animator.SetBool("Open", value);
    }

}
