using UnityEngine;

public class GateController : MonoBehaviour
{
    [SerializeField] private HingeJoint hinge;
    [SerializeField] private Rigidbody rb;

    private bool dynamicCollidersPrepared;

    private void Awake()
    {
        // The Realmaze gate prefab keeps its hinge physics components in
        // the imported child hierarchy, so do not require a Rigidbody on
        // this exact GameObject.
        if (!rb)
        {
            rb = GetComponentInChildren<Rigidbody>(true);
        }

        if (!hinge)
        {
            hinge = GetComponentInChildren<HingeJoint>(true);
        }

        if (!rb)
        {
            Debug.LogError(
                $"[{nameof(GateController)}] '{name}' has no gate Rigidbody.",
                this);

            enabled = false;
            return;
        }

        // Start locked and kinematic before applying the current global state.
        // This is also safe when entering the scene directly in the Editor.
        if (!Globals.hasKey)
        {
            rb.isKinematic = true;
        }

        ApplyState();
    }

    public void UnlockGate()
    {
        ApplyState();
    }

    public void LockGate()
    {
        ApplyState();
    }

    private void ApplyState()
    {
        if (!rb)
        {
            return;
        }

        bool isUnlocked = Globals.hasKey;

        if (isUnlocked)
        {
            PrepareDynamicColliders();
        }

        rb.isKinematic = !isUnlocked;

        if (hinge)
        {
            hinge.useMotor = isUnlocked;
        }
    }

    private void PrepareDynamicColliders()
    {
        if (dynamicCollidersPrepared || !rb)
        {
            return;
        }

        dynamicCollidersPrepared = true;

        // The gate scene already supplies a primitive BoxCollider for each
        // hinge Rigidbody. Imported Box1/Box2 meshes also carry concave
        // MeshColliders, which PhysX cannot use on a dynamic Rigidbody.
        //
        // Search the gate assembly, but only modify colliders that physics
        // associates with this exact Rigidbody.
        Transform gateRoot = transform.root;

        Collider[] colliders =
            gateRoot.GetComponentsInChildren<Collider>(true);

        bool hasPrimitiveCollider = false;

        foreach (Collider collider in colliders)
        {
            if (!collider ||
                collider.attachedRigidbody != rb ||
                !collider.enabled)
            {
                continue;
            }

            if (!(collider is MeshCollider))
            {
                hasPrimitiveCollider = true;
                break;
            }
        }

        if (!hasPrimitiveCollider)
        {
            Debug.LogWarning(
                $"[{nameof(GateController)}] '{name}' has no enabled " +
                "primitive collider for its dynamic Rigidbody. Concave " +
                "MeshColliders were left untouched.",
                this);

            return;
        }

        foreach (Collider collider in colliders)
        {
            if (!(collider is MeshCollider meshCollider) ||
                meshCollider.attachedRigidbody != rb ||
                !meshCollider.enabled ||
                meshCollider.convex)
            {
                continue;
            }

            meshCollider.enabled = false;

            Debug.Log(
                $"[{nameof(GateController)}] Disabled redundant concave " +
                $"MeshCollider '{meshCollider.name}' before unlocking gate.",
                meshCollider);
        }
    }
}
