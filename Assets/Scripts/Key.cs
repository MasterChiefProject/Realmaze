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

    private Animation keyAnimation;
    private Collider keyCollider;
    private Renderer[] keyRenderers;
    private bool isCollected;

    private void Awake()
    {
        keyAnimation = GetComponent<Animation>();
        keyCollider = GetComponent<Collider>();
        keyRenderers = GetComponentsInChildren<Renderer>(true);
    }

    public void PlayKeyAnimation()
    {
        if (keyAnimation)
        {
            keyAnimation.Play();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (isCollected ||
            !other.CompareTag("Player") ||
            !chest ||
            !chest.isOpen)
        {
            return;
        }

        isCollected = true;
        Globals.hasKey = true;

        if (gate1)
        {
            gate1.UnlockGate();
        }

        if (gate2)
        {
            gate2.UnlockGate();
        }

        if (takeKeyAudio)
        {
            AudioSource.PlayClipAtPoint(
                takeKeyAudio,
                transform.position,
                volume);
        }

        if (message)
        {
            message.text = "Now you can open the gate!";
            message.gameObject.SetActive(true);
        }

        if (keyCollider)
        {
            keyCollider.enabled = false;
        }

        foreach (Renderer keyRenderer in keyRenderers)
        {
            if (keyRenderer)
            {
                keyRenderer.enabled = false;
            }
        }

        StartCoroutine(FinishCollection());
    }

    private IEnumerator FinishCollection()
    {
        yield return new WaitForSeconds(Mathf.Max(0f, hideMsgAfterDelay));

        if (message)
        {
            message.gameObject.SetActive(false);
        }

        Destroy(gameObject);
    }
}
