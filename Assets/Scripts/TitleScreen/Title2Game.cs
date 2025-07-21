using UnityEngine;
using UnityEngine.SceneManagement;

public class Title2Game : MonoBehaviour
{
    // Called from a button via the Unity Inspector
    public void LoadMainGame()
    {
        SceneManager.LoadScene("MainGame");
    }
}
