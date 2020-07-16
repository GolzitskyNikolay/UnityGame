using UnityEngine;
using UnityEngine.SceneManagement;

public class Game : MonoBehaviour
{
   public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    public void Exitgame()
    {
        Application.Quit();
    }
}
