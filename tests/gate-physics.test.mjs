import test from "node:test";
import assert from "node:assert/strict";
import fs from "node:fs";
import path from "node:path";

const root = process.cwd();
const gate = fs.readFileSync(
  path.join(root, "Assets/Scripts/GateController.cs"),
  "utf8"
);

test("gate controller does not require a Rigidbody on its own GameObject", () => {
  assert.doesNotMatch(gate, /RequireComponent/);
});

test("gate controller preserves child-hierarchy Rigidbody lookup", () => {
  assert.match(gate, /GetComponentInChildren<Rigidbody>\(true\)/);
  assert.match(gate, /GetComponentInChildren<HingeJoint>\(true\)/);
});

test("gate prepares colliders before becoming dynamic", () => {
  const prepare = gate.indexOf("PrepareDynamicColliders();");
  const dynamic = gate.indexOf("rb.isKinematic = !isUnlocked;");

  assert.ok(prepare >= 0);
  assert.ok(dynamic > prepare);
});

test("only concave MeshColliders attached to the gate Rigidbody are disabled", () => {
  assert.match(gate, /meshCollider\.attachedRigidbody != rb/);
  assert.match(gate, /meshCollider\.convex/);
  assert.match(gate, /meshCollider\.enabled = false/);
});

test("gate requires an existing primitive collider before disabling mesh collision", () => {
  assert.match(gate, /hasPrimitiveCollider/);
  assert.match(gate, /!\(collider is MeshCollider\)/);
});
