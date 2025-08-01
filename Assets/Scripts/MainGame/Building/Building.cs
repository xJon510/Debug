using UnityEngine;

public abstract class Building : MonoBehaviour
{
    public string buildingName;

    // Called when BuildManager confirms placement
    public virtual void OnPlaced()
    {
        // Default does nothing
        // Child classes can override to add effects
    }

    // Called when player clicks this building
    public abstract void OpenInfoPanel();
}
