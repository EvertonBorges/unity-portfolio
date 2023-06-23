using UnityEngine;

public class Coin : MonoBehaviour
{

    void OnTriggerEnter(Collider other)
    {
        if (!other.TryGetComponent(out PlayerController _))
            return;

        Manager_Events.GameManager.AddCoin.Notify();

        Destroy(gameObject);
    }

}
