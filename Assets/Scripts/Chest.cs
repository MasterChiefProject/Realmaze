using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Chest : MonoBehaviour
{
    public AudioClip openChestSound;
    public Text notEnoughCoinsMessage;
    public bool isOpen = false;
    public int openAfterCoins = 20;
    public float keyDelay = 1.5f;
    public Key key;
    public float volume = 1f;

    Animation openChestAnimation;

    private void Start()
    {
        notEnoughCoinsMessage.gameObject.SetActive(false);
        openChestAnimation = GetComponent<Animation>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (Globals.points >= openAfterCoins && !isOpen)
            {
                openChestAnimation.Play();
                AudioSource.PlayClipAtPoint(openChestSound, transform.position, volume);
                if (!Globals.hasKey)
                {
                    StartCoroutine(KeyAfterDelay(keyDelay));
                }
            }
            else
            {
                if (!isOpen)
                {
                    notEnoughCoinsMessage.gameObject.SetActive(true);
                }
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            notEnoughCoinsMessage.gameObject.SetActive(false);
        }
    }

    private IEnumerator KeyAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        key.PlayKeyAnimation();
        isOpen = true;
    }

}
