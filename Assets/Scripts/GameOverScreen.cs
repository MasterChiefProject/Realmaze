using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverScreen : MonoBehaviour
{
    public void Setup()
    {
        gameObject.SetActive(true);
    }

    public void RestartButton()
    {
        SceneManager.LoadScene("GameScene");
    }
    public void MainMenuButton()
    {
        SceneManager.LoadScene("MainMenuScene");
    }
}
