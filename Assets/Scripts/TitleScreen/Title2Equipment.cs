using UnityEngine;
using UnityEngine.SceneManagement;

public class Title2Equipment : MonoBehaviour
{
    // Called from a button via the Unity Inspector
    public void LoadEquipment()
    {
        SceneManager.LoadScene("Equipment");
    }
}
