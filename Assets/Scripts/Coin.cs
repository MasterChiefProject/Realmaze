using UnityEngine;

public class Coin : MonoBehaviour
{
    public AudioClip collectSound;
    public float volume = 1f;

    private bool isCollected;

    private void OnTriggerEnter(Collider other)
    {
        if (isCollected || !other.CompareTag("Player"))
        {
            return;
        }

        isCollected = true;

        if (collectSound)
        {
            AudioSource.PlayClipAtPoint(
                collectSound,
                transform.position,
                volume);
        }

        ++Globals.points;
        Destroy(gameObject);
    }
}
