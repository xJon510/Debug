using UnityEngine;
using UnityEngine.SceneManagement;

public class OpenMainMenu : MonoBehaviour
{
    // Called from a button via the Unity Inspector
    public void OpenTitleScreen()
    {
        SceneManager.LoadScene("TitleScreen");
    }
}
