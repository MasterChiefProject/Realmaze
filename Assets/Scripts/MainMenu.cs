using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    private bool isLoadingGame;

    public void ExitButton()
    {
        Application.Quit();
        Debug.Log("Quit called!");
    }

    public void StartGame()
    {
        if (isLoadingGame)
        {
            return;
        }

        Globals.InitGlobals();

#if UNITY_WEBGL && !UNITY_EDITOR
        StartCoroutine(LoadGameSceneAsync());
#else
        SceneManager.LoadScene("GameScene");
#endif
    }

#if UNITY_WEBGL && !UNITY_EDITOR
    private IEnumerator LoadGameSceneAsync()
    {
        isLoadingGame = true;

        AsyncOperation operation =
            SceneManager.LoadSceneAsync(
                "GameScene",
                LoadSceneMode.Single);

        if (operation == null)
        {
            isLoadingGame = false;
            Debug.LogError(
                "[Realmaze] Could not start asynchronous GameScene loading.");
            yield break;
        }

        // Lower priority lets Unity distribute scene loading work over frames
        // instead of competing as aggressively with the current menu frame.
        operation.priority = -1;

        while (!operation.isDone)
        {
            yield return null;
        }
    }
#endif
}
