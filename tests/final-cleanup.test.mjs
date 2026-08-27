import test from "node:test";
import assert from "node:assert/strict";
import fs from "node:fs";
import path from "node:path";

const root = process.cwd();

function read(relativePath) {
  return fs.readFileSync(path.join(root, relativePath), "utf8");
}

test("CI sparse checkout includes scripts required by repository tests", () => {
  const workflow = read(".github/workflows/ci.yml");

  for (const script of [
    "Assets/Scripts/MainMenu.cs",
    "Assets/Scripts/WayPointScript.cs",
    "Assets/Scripts/WebGLPerformanceBootstrap.cs",
  ]) {
    assert.ok(workflow.includes(script));
  }
});

test("chest guards against duplicate opening coroutines", () => {
  const chest = read("Assets/Scripts/Chest.cs");
  assert.match(chest, /private bool isOpening/);
  assert.match(chest, /isOpen \|\| isOpening/);
  assert.match(chest, /isOpening = true/);
});

test("chest message uses an escaped newline", () => {
  const chest = read("Assets/Scripts/Chest.cs");
  assert.match(chest, /\\n/);
});

test("key collection is single-shot and hides the message before destroy", () => {
  const key = read("Assets/Scripts/Key.cs");
  assert.match(key, /private bool isCollected/);
  assert.match(key, /keyCollider\.enabled = false/);
  assert.match(key, /message\.gameObject\.SetActive\(false\)/);
  assert.match(key, /Destroy\(gameObject\)/);
});

test("death handler does not guess an arbitrary MonoBehaviour", () => {
  const death = read("Assets/Scripts/PlayerDeathHandler.cs");
  assert.doesNotMatch(death, /GetComponent<MonoBehaviour>/);
});

