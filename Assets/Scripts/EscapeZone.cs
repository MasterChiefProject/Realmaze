using UnityEngine;
using UnityEngine.Serialization;

public class EscapeZone : MonoBehaviour
{
    [FormerlySerializedAs("gameOverUI")]
    [Tooltip("Canvas or panel that contains the Victory UI.")]
    [SerializeField] private GameObject victoryUI;

    private bool isWinner;

    private void Awake()
    {
        if (victoryUI)
        {
            victoryUI.SetActive(false);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (isWinner || !other.CompareTag("Player"))
        {
            return;
        }

        isWinner = true;

        PlayerDeathHandler deathHandler =
            other.GetComponentInParent<PlayerDeathHandler>();

        if (deathHandler)
        {
            deathHandler.enabled = false;
        }

        other.gameObject.tag = "Untagged";

        if (victoryUI)
        {
            victoryUI.SetActive(true);
        }

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
}
