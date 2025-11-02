using UnityEngine;
using UnityEngine.SceneManagement;

public class RestartScene : MonoBehaviour
{
    public void RestartCurrentScene()
    {
        // Hol dir den Namen der aktuellen Szene
        string currentSceneName = SceneManager.GetActiveScene().name;

        // Lade die aktuelle Szene neu
        SceneManager.LoadScene(currentSceneName);
    }

    public void QuitGame()
    {
        // Beende die Anwendung
        Application.Quit();
    }
}
