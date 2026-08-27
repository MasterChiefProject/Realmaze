import test from "node:test";
import assert from "node:assert/strict";
import fs from "node:fs";
import path from "node:path";

const root = process.cwd();

function read(relativePath) {
  return fs.readFileSync(path.join(root, relativePath), "utf8");
}

test("final release metadata is pinned", () => {
  const build = read("Assets/Editor/RealmazeWebGLBuild.cs");

  assert.match(build, /PlayerSettings\.companyName\s*=\s*"MasterChiefProject"/);
  assert.match(build, /PlayerSettings\.productName\s*=\s*"Realmaze"/);
  assert.match(build, /PlayerSettings\.bundleVersion\s*=\s*"1\.0\.0"/);
});

test("README controls match the configured first-person controller", () => {
  const readme = read("README.md");

  assert.match(readme, /Left Ctrl/);
  assert.match(readme, /Right Mouse Button/);
  assert.doesNotMatch(readme, /\| Interact \|/);
});

test("README runs all repository tests", () => {
  const readme = read("README.md");

  assert.match(readme, /node --test tests\/\*\.test\.mjs/);
});
