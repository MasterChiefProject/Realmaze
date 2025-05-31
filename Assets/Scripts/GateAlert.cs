using UnityEngine;
using UnityEngine.UI;
public class GateAlert : MonoBehaviour
{
    public Text noGateKey;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (!Globals.hasKey)
            {
                noGateKey.gameObject.SetActive(true);
            }

        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            noGateKey.gameObject.SetActive(false);
        }
    }
}
