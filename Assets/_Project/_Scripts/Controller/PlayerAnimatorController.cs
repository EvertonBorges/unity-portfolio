using UnityEngine;

public class PlayerAnimatorController : MonoBehaviour
{

    private PlayerController m_playerController;
    private Animator m_animator;

    private int _animIDSpeed = 0, _animIDGrounded = 0, _animIDJump = 0, _animIDFreeFall = 0, _animIDMotionSpeed = 0;

    public void Setup(PlayerController playerController)
    {
        m_playerController = playerController;

        m_animator = GetComponent<Animator>();

        AssignAnimationIDs();
    }

    private void AssignAnimationIDs()
    {
        _animIDSpeed = Animator.StringToHash("Speed");
        _animIDGrounded = Animator.StringToHash("Grounded");
        _animIDJump = Animator.StringToHash("Jump");
        _animIDFreeFall = Animator.StringToHash("FreeFall");
        _animIDMotionSpeed = Animator.StringToHash("MotionSpeed");
    }

    public void OnFreeFall(bool value)
    {
        m_animator.SetBool(_animIDFreeFall, value);
    }

    public void OnGround(bool value)
    {
        m_animator.SetBool(_animIDGrounded, value);
    }

    public void SetMotionSpeed(float value)
    {
        m_animator.SetFloat(_animIDMotionSpeed, value);
    }

    public void SetSpeed(float value)
    {
        m_animator.SetFloat(_animIDSpeed, value);
    }

    private void OnFootstep(AnimationEvent animationEvent)
    {
        if (animationEvent.animatorClipInfo.weight > 0.5f)
            m_playerController.OnFootstep();
    }

    private void OnLand(AnimationEvent animationEvent)
    {
        if (animationEvent.animatorClipInfo.weight > 0.5f)
            m_playerController.OnLand();
    }
    

}
