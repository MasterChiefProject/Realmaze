using UnityEngine;

public class PlayerDeathHandler : MonoBehaviour
{
    [Header("References")]
    [Tooltip("Movement/controller component that should be disabled on death.")]
    [SerializeField] private MonoBehaviour movementScript;

    [Tooltip("Canvas or panel that contains the Game-Over UI.")]
    [SerializeField] private GameObject gameOverUI;

    [Tooltip("Optional player Rigidbody to freeze on death.")]
    [SerializeField] private Rigidbody rb;

    [Header("Settings")]
    [Tooltip("Tag that identifies zombie trigger colliders.")]
    [SerializeField] private string zombieTag = "Zombie";

    private bool isDead;

    private void Awake()
    {
        if (!rb)
        {
            rb = GetComponent<Rigidbody>();
        }

        if (!movementScript)
        {
            Debug.LogWarning(
                $"[{nameof(PlayerDeathHandler)}] No movement script is assigned. " +
                "The Game Over UI will still work, but player movement cannot " +
                "be disabled automatically.",
                this);
        }

        if (gameOverUI)
        {
            gameOverUI.SetActive(false);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (isDead ||
            CompareTag(zombieTag) ||
            !other.CompareTag(zombieTag))
        {
            return;
        }

        Die();
    }

    private void Die()
    {
        isDead = true;

        if (movementScript)
        {
            movementScript.enabled = false;
        }

        if (rb)
        {
            rb.constraints = RigidbodyConstraints.FreezeAll;
        }

        gameObject.tag = "Untagged";

        if (gameOverUI)
        {
            gameOverUI.SetActive(true);
        }

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
}
