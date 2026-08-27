import test from "node:test";
import assert from "node:assert/strict";
import fs from "node:fs";
import path from "node:path";

const root = process.cwd();

function read(relativePath) {
  return fs.readFileSync(path.join(root, relativePath), "utf8");
}

test("victory flow never retags the player as a Zombie", () => {
  const escapeZone = read("Assets/Scripts/EscapeZone.cs");
  assert.doesNotMatch(escapeZone, /tag\s*=\s*"Zombie"/);
  assert.match(escapeZone, /GetComponentInParent<PlayerDeathHandler>/);
  assert.match(escapeZone, /deathHandler\.enabled\s*=\s*false/);
  assert.match(escapeZone, /tag\s*=\s*"Untagged"/);
});

test("renamed victory UI preserves the serialized reference", () => {
  const escapeZone = read("Assets/Scripts/EscapeZone.cs");
  assert.match(escapeZone, /FormerlySerializedAs\("gameOverUI"\)/);
  assert.match(escapeZone, /GameObject victoryUI/);
});

test("gate controller removes unrelated imports and validates Rigidbody", () => {
  const gate = read("Assets/Scripts/GateController.cs");
  assert.doesNotMatch(gate, /Unity\.VisualScripting/);
  assert.doesNotMatch(gate, /UnityEngine\.Rendering/);
  assert.doesNotMatch(gate, /UnityEngine\.UI/);
  assert.match(gate, /RequireComponent\(typeof\(Rigidbody\)\)/);
  assert.match(gate, /if \(!rb\)/);
});

test("score UI updates only when the score changes", () => {
  const score = read("Assets/Scripts/Score.cs");
  assert.match(score, /displayedScore/);
  assert.match(score, /currentScore == displayedScore/);
  assert.match(score, /\$"Score: \{displayedScore\}"/);
});

test("coin collection is single-shot", () => {
  const coin = read("Assets/Scripts/Coin.cs");
  assert.match(coin, /private bool isCollected/);
  assert.match(coin, /isCollected \|\| !other\.CompareTag/);
  assert.match(coin, /isCollected = true/);
});

test("Globals no longer imports Unity Visual Scripting", () => {
  const globals = read("Assets/Scripts/Globals.cs");
  assert.doesNotMatch(globals, /Unity\.VisualScripting/);
});

test("VictoryScreen has no unused UI import", () => {
  const victory = read("Assets/Scripts/VictoryScreen.cs");
  assert.doesNotMatch(victory, /UnityEngine\.UI/);
});
