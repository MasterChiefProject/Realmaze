using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Key : MonoBehaviour
{
    public Chest chest;
    public AudioClip takeKeyAudio;
    public float volume = 1f;
    public GateController gate1;
    public GateController gate2;

    public Text message;
    public float hideMsgAfterDelay = 2f;

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

            message.text = "Now you can open the gate!";
            message.gameObject.SetActive(true);

            Destroy(gameObject);
        }
    }

    private IEnumerator HideMessageAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        message.gameObject.SetActive(false);
    }
}
