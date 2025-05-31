using UnityEngine;

public class Key : MonoBehaviour
{
    public Chest chest;
    public AudioClip takeKeyAudio;
    public float volume = 1f;
    public GateController gate1;
    public GateController gate2;

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
            Globals.hasKey = true;
            gate1.UnlockGate();
            gate2.UnlockGate();
            AudioSource.PlayClipAtPoint(takeKeyAudio, transform.position, volume);
            // TODO: show some text explaining where to go now
            Destroy(gameObject);
        }
    }
}
