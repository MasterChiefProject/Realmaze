import test from "node:test";
import assert from "node:assert/strict";
import fs from "node:fs";
import path from "node:path";

const root = process.cwd();

function read(relativePath) {
  return fs.readFileSync(path.join(root, relativePath), "utf8");
}

test("project stays pinned to Unity 6000.0.47f1", () => {
  const version = read("ProjectSettings/ProjectVersion.txt");
  assert.match(version, /m_EditorVersion:\s*6000\.0\.47f1/);
});

test("WebGL build stages outside Unity Library", () => {
  const build = read("Assets/Editor/RealmazeWebGLBuild.cs");
  assert.match(build, /Builds\/RealmazeWebGLStaging/);
  assert.doesNotMatch(build, /Library\/RealmazeWebGLStaging/);
});

test("WebGL build contains only production scenes", () => {
  const build = read("Assets/Editor/RealmazeWebGLBuild.cs");

  const matches =
    [...build.matchAll(/Assets\/Scenes\/[^"]+\.unity/g)]
      .map(match => match[0]);

  assert.deepEqual(
    [...new Set(matches)],
    [
      "Assets/Scenes/MainMenuScene.unity",
      "Assets/Scenes/GameScene.unity",
    ]
  );
});

test("WebGL build uses Mobile URP and mip stripping", () => {
  const build = read("Assets/Editor/RealmazeWebGLBuild.cs");

  assert.match(build, /Assets\/Settings\/Mobile_RPAsset\.asset/);
  assert.match(build, /PlayerSettings\.mipStripping\s*=\s*true/);
  assert.match(
    build,
    /QualitySettings\.globalTextureMipmapLimit\s*=\s*2/
  );
  assert.match(
    build,
    /QualitySettings\.renderPipeline\s*=\s*mobilePipeline/
  );
});

test("temporary quality settings are restored", () => {
  const build = read("Assets/Editor/RealmazeWebGLBuild.cs");

  assert.match(build, /class WebBuildQualityScope/);
  assert.match(build, /IDisposable/);
  assert.match(build, /qualityScope\?\.Dispose\(\)/);
  assert.match(build, /Original Editor quality settings restored/);
});

test("menu loads GameScene asynchronously in WebGL", () => {
  const menu = read("Assets/Scripts/MainMenu.cs");

  assert.match(menu, /#if UNITY_WEBGL && !UNITY_EDITOR/);
  assert.match(menu, /SceneManager\.LoadSceneAsync/);
  assert.match(menu, /operation\.priority\s*=\s*-1/);
  assert.match(menu, /Globals\.InitGlobals\(\)/);
});

test("runtime upload budget is constrained", () => {
  const bootstrap =
    read("Assets/Scripts/WebGLPerformanceBootstrap.cs");

  assert.match(bootstrap, /asyncUploadTimeSlice\s*=\s*2/);
  assert.match(bootstrap, /asyncUploadBufferSize\s*=\s*16/);
  assert.match(bootstrap, /asyncUploadPersistentBuffer\s*=\s*true/);
});

test("runtime distance and terrain culling remain enabled", () => {
  const bootstrap =
    read("Assets/Scripts/WebGLPerformanceBootstrap.cs");

  assert.match(bootstrap, /WebFarClipDistance\s*=\s*100f/);
  assert.match(bootstrap, /camera\.farClipPlane/);
  assert.match(bootstrap, /WebTerrainDetailDistance\s*=\s*22f/);
});

test("zombie avoidance does not allocate an overlap array every frame", () => {
  const waypoint = read("Assets/Scripts/WayPointScript.cs");

  assert.match(waypoint, /Physics\.OverlapSphereNonAlloc/);
  assert.doesNotMatch(waypoint, /Physics\.OverlapSphere\(/);
  assert.match(waypoint, /avoidanceBuffer/);
});

test("zombie WebGL avoidance work is staggered", () => {
  const waypoint = read("Assets/Scripts/WayPointScript.cs");

  assert.match(
    waypoint,
    /WebAvoidanceSampleInterval\s*=\s*0\.12f/
  );
  assert.match(waypoint, /nextAvoidanceSampleTime/);
  assert.match(waypoint, /Random\.value/);
});

test("zombie movement caches the active terrain", () => {
  const waypoint = read("Assets/Scripts/WayPointScript.cs");

  assert.match(waypoint, /activeTerrain\s*=\s*Terrain\.activeTerrain/);
  assert.match(waypoint, /activeTerrain\.SampleHeight/);
});

test("README documents async loading and Mobile URP browser profile", () => {
  const readme = read("README.md");

  assert.match(readme, /asynchronous scene loading/i);
  assert.match(readme, /Mobile_RPAsset/);
  assert.match(readme, /OverlapSphereNonAlloc/);
  assert.match(readme, /additive\/streamed chunks or Addressables/);
});
