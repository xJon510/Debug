using UnityEngine;
using UnityEngine.UI;

public class BuildingButton : MonoBehaviour
{
    public BuildingData buildingData;

    private void Start()
    {
        // Hook the button
        GetComponent<Button>().onClick.AddListener(OnClick);
    }

    private void OnClick()
    {
        // Tell BuildManager which building we picked
        BuildManager.Instance.SelectBuilding(buildingData);
    }
}
