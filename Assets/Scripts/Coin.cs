using UnityEngine;

public class Coin : MonoBehaviour
{
    public AudioClip collectSound;
    public float volume = 1f;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            AudioSource.PlayClipAtPoint(collectSound, transform.position, volume);
            ++Globals.points;
            Destroy(gameObject);
        }
    }
}
