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

    private Animation openChestAnimation;
    private bool isOpening;

    private void Start()
    {
        if (notEnoughCoinsMessage)
        {
            notEnoughCoinsMessage.gameObject.SetActive(false);
        }

        openChestAnimation = GetComponent<Animation>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player") || isOpen || isOpening)
        {
            return;
        }

        if (Globals.points < openAfterCoins)
        {
            ShowMissingCoinsMessage();
            return;
        }

        isOpening = true;

        if (notEnoughCoinsMessage)
        {
            notEnoughCoinsMessage.gameObject.SetActive(false);
        }

        if (openChestAnimation)
        {
            openChestAnimation.Play();
        }

        if (openChestSound)
        {
            AudioSource.PlayClipAtPoint(
                openChestSound,
                transform.position,
                volume);
        }

        StartCoroutine(OpenAfterDelay(keyDelay));
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") && notEnoughCoinsMessage)
        {
            notEnoughCoinsMessage.gameObject.SetActive(false);
        }
    }

    private void ShowMissingCoinsMessage()
    {
        if (!notEnoughCoinsMessage)
        {
            return;
        }

        int missingCoins = Mathf.Max(0, openAfterCoins - Globals.points);

        notEnoughCoinsMessage.text =
            $"You don't have enough coins to open the chest.\\n" +
            $"Collect {missingCoins} more coins.";

        notEnoughCoinsMessage.gameObject.SetActive(true);
    }

    private IEnumerator OpenAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        if (!Globals.hasKey && key)
        {
            key.PlayKeyAnimation();
        }

        isOpen = true;
        isOpening = false;
    }
}
