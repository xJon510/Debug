using UnityEngine;

public abstract class Building : MonoBehaviour
{
    public string buildingName;

    // Called when player clicks this building
    public abstract void OpenInfoPanel();
}
