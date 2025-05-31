using UnityEngine;
using UnityEngine.SceneManagement;

public class EscapeZone : MonoBehaviour
{

    [Tooltip("Canvas or panel that contains the Game-Over UI.")]
    [SerializeField] GameObject gameOverUI;

    bool isWinner = false;

    void Awake()
    {
        if (gameOverUI) gameOverUI.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (isWinner) return;
        if (!other.CompareTag("Player")) return;

        isWinner = true;

        other.gameObject.tag = "Zombie";

        if (gameOverUI) gameOverUI.SetActive(true);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
}
