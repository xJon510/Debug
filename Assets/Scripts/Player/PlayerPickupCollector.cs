using UnityEngine;

public class PlayerPickupCollector : MonoBehaviour
{
    public Transform pickupTarget;

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<PickupResource>(out var pickup))
        {
            pickup.BeginCollect(pickupTarget != null ? pickupTarget : transform);
        }
    }
}
