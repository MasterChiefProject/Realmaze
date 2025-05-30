using UnityEngine;

public class Key : MonoBehaviour
{
    public Chest chest;

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
            // TODO: can take the key - destroy object, play sound, save this in Globals variable hasKey
        }
    }
}
