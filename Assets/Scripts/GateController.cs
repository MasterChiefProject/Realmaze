using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

// Put this on the same GameObject that has the HingeJoint + Rigidbody
public class GateController : MonoBehaviour
{
    [SerializeField] HingeJoint hinge;     // drag in Inspector (optional, used only if you want to tweak motor etc.)
    [SerializeField] Rigidbody rb;         // drag the gate's own rigidbody

    void Awake()
    {
        if (!rb) rb = GetComponent<Rigidbody>();
        if (!hinge) hinge = GetComponent<HingeJoint>();

        ApplyState();
    }

    /* ---------- public API you call from other scripts ---------- */
    public void UnlockGate() { ApplyState(); }
    public void LockGate() { ApplyState(); }

    /* ---------- helper ---------- */
    void ApplyState()
    {
        // Locked  ➜ make rigidbody kinematic (or freeze constraints)
        // Unlocked ➜ let physics simulate normally
        rb.isKinematic = !Globals.hasKey;

        // Optional: enable/disable motor so it doesn’t try to move while locked
        if (hinge != null) hinge.useMotor = Globals.hasKey;
    }
}
