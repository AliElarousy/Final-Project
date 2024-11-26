using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneSwitcher : MonoBehaviour
{
    public void LoadInGameScene()
    {
        // Replace "InGameScene" with the name of your in-game scene
        SceneManager.LoadScene("2 Player Scene");
    }
}
