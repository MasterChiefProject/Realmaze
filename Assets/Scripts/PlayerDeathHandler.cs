using UnityEngine;
using UnityEngine.SceneManagement;

/// Attach to the player root object.           
/// Requires that every zombie collider carries the tag stored in `zombieTag` (default "Zombie").
public class PlayerDeathHandler : MonoBehaviour
{
    [Header("References")]
    [Tooltip("Movement / controller script that should be disabled on death " +
             "(e.g. StarterAssets.FirstPersonController).")]
    [SerializeField] MonoBehaviour movementScript;

    [Tooltip("Canvas or panel that contains the Game-Over UI.")]
    [SerializeField] GameObject gameOverUI;

    [Tooltip("Optional: the player’s rigidbody (will be frozen on death).")]
    [SerializeField] Rigidbody rb;

    [Header("Settings")]
    [Tooltip("Tag that identifies zombie trigger colliders.")]
    [SerializeField] string zombieTag = "Zombie";

    bool isDead = false;

    void Awake()
    {
        // Convenience—fill refs automatically if you forgot in the Inspector
        if (!movementScript) movementScript = GetComponent<MonoBehaviour>();
        if (!rb) rb = GetComponent<Rigidbody>();
        if (gameOverUI) gameOverUI.SetActive(false);     // hide at start
    }

    /* ------------------------------------------------------------ */
    void OnTriggerEnter(Collider other)
    {
        if (isDead || gameObject.tag == zombieTag) return;                    // already dead, ignore further hits
        if (!other.CompareTag(zombieTag)) return;

        Die();
    }

    /* ------------------------------------------------------------ */
    void Die()
    {
        isDead = true;

        // 1️⃣  Freeze / stop player control
        if (movementScript) movementScript.enabled = false;
        if (rb) rb.constraints = RigidbodyConstraints.FreezeAll;

        // 2️⃣  Untag so other scripts don’t treat us as a living player
        gameObject.tag = "Untagged";

        // 3️⃣  Show the Game-Over canvas
        if (gameOverUI) gameOverUI.SetActive(true);

        // Optional quality-of-life tweaks
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        // Time.timeScale  = 0f;   // uncomment if you want to pause the whole game
    }
}
