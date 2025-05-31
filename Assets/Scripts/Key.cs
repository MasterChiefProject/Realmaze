using UnityEngine;

public class Key : MonoBehaviour
{
    public Chest chest;
    public AudioClip takeKeyAudio;
    public float volume = 1f;


    private Animation animation;

    private void Start()
    {
        animation = GetComponent<Animation>();
    }

    public void PlayKeyAnimation()
    {
        animation.Play();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && chest.isOpen)
        {
            AudioSource.PlayClipAtPoint(takeKeyAudio, transform.position, volume);
            // TODO: show some text explaining where to go now
            Destroy(gameObject);
        }
    }
}
