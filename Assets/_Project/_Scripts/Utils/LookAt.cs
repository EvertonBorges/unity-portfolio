using UnityEngine;

public class LookAt : MonoBehaviour
{
    
    [SerializeField] private Transform _target;
    [SerializeField] private Vector3 _rotationOffset = default;

    void Update()
    {
        if (_target == null)
            return;

        transform.LookAt(_target, Vector3.up);

        transform.rotation *= Quaternion.Euler(_rotationOffset);
    }

}
