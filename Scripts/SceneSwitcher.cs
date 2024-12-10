using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneSwitcher : MonoBehaviour
{
    public void LoadInGameScene()
    {
        // Replace "InGameScene" with the name of your in-game scene
        SceneManager.LoadScene("two player scene");
    }
    public void LoadMainMenu()
    {
        SceneManager.LoadScene("main menu scene");
    }

}