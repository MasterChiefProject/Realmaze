using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void ExitButton()
    {
        Application.Quit();
        Debug.Log("Quit called!");
    }

    public void StartGame()
    {
        SceneManager.LoadScene("GameScene");
    }
}
