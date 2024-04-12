using UnityEngine;

public class MainStatueController : Singleton<MainStatueController>
{

    [SerializeField] private Animator _animator;
    [SerializeField] private float _distance;

    private PlayerController m_player;
    private PlayerController PlayerController
    {
        get
        {
            if (m_player == null)
                m_player = PlayerController.Instance;

            return m_player;
        }
    }

    private int m_pose = -1;
    private bool m_change = true;

    protected override void Init()
    {
        ChangeAnimation();
    }

    void Update()
    {
        var distance = (transform.position - PlayerController.transform.position).magnitude;

        if (distance >= _distance)
        {
            if (m_change)
            {
                m_change = false;
                ChangeAnimation();
            }
        }
        else
        {
            m_change = true;
        }
    }

    private void ChangeAnimation()
    {
        m_pose = Random.Range(1, 12);
        _animator.Play($"Pose_{m_pose}");
    }

}
