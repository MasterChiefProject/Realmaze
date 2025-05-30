using System.Collections;
using UnityEngine;

public class Chest : MonoBehaviour
{
    public bool isOpen = false;
    public int openAfterCoins = 20;
    public float keyDelay = 1.5f;
    public Key key;

    Animation openChestAnimation;

    private void Start()
    {
        openChestAnimation = GetComponent<Animation>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (Globals.points >= openAfterCoins)
            {
                openChestAnimation.Play();
                StartCoroutine(KeyAfterDelay(keyDelay));

            } else
            {
                // TODO: display a text
            }
        }
    }

    private IEnumerator KeyAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        key.PlayKeyAnimation();
        isOpen = true;
    }

}
