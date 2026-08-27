#if UNITY_WEBGL && !UNITY_EDITOR
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

internal static class WebGLPerformanceBootstrap
{
    private const float WebFarClipDistance = 100f;
    private const float WebTerrainDetailDistance = 22f;
    private const float WebTerrainTreeDistance = 85f;
    private const float WebTerrainBillboardDistance = 25f;
    private const float WebTerrainBasemapDistance = 45f;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Initialize()
    {
        ApplyGlobalQuality();

        SceneManager.sceneLoaded -= OnSceneLoaded;
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private static void ApplyGlobalQuality()
    {
        if (QualitySettings.names.Length > 0)
        {
            QualitySettings.SetQualityLevel(0, true);
        }

        QualitySettings.globalTextureMipmapLimit = 2;
        QualitySettings.anisotropicFiltering = AnisotropicFiltering.Disable;
        QualitySettings.antiAliasing = 0;
        QualitySettings.shadows = UnityEngine.ShadowQuality.Disable;
        QualitySettings.shadowDistance = 0f;
        QualitySettings.realtimeReflectionProbes = false;
        QualitySettings.pixelLightCount = 1;
        QualitySettings.lodBias = Mathf.Min(QualitySettings.lodBias, 0.6f);
        QualitySettings.particleRaycastBudget =
            Mathf.Min(QualitySettings.particleRaycastBudget, 16);
        QualitySettings.softParticles = false;
        QualitySettings.vSyncCount = 0;

        // Keep texture/mesh upload work from monopolizing one frame.
        QualitySettings.asyncUploadTimeSlice = 2;
        QualitySettings.asyncUploadBufferSize = 16;
        QualitySettings.asyncUploadPersistentBuffer = true;

        Application.targetFrameRate = 60;
    }

    private static void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        ApplyGlobalQuality();

        Camera[] cameras =
            Object.FindObjectsByType<Camera>(FindObjectsSortMode.None);

        foreach (Camera camera in cameras)
        {
            camera.allowHDR = false;
            camera.allowMSAA = false;
            camera.farClipPlane =
                Mathf.Min(camera.farClipPlane, WebFarClipDistance);
        }

        Volume[] volumes =
            Object.FindObjectsByType<Volume>(FindObjectsSortMode.None);

        foreach (Volume volume in volumes)
        {
            volume.enabled = false;
        }

        Terrain[] terrains = Terrain.activeTerrains;

        foreach (Terrain terrain in terrains)
        {
            terrain.heightmapPixelError =
                Mathf.Max(terrain.heightmapPixelError, 24f);

            terrain.detailObjectDistance =
                Mathf.Min(
                    terrain.detailObjectDistance,
                    WebTerrainDetailDistance);

            terrain.detailObjectDensity =
                Mathf.Min(terrain.detailObjectDensity, 0.20f);

            terrain.treeDistance =
                Mathf.Min(
                    terrain.treeDistance,
                    WebTerrainTreeDistance);

            terrain.treeBillboardDistance =
                Mathf.Min(
                    terrain.treeBillboardDistance,
                    WebTerrainBillboardDistance);

            terrain.basemapDistance =
                Mathf.Min(
                    terrain.basemapDistance,
                    WebTerrainBasemapDistance);
        }

        Debug.Log(
            $"[Realmaze WebGL] Scene '{scene.name}' ready. " +
            $"Far clip: {WebFarClipDistance:0}m, " +
            $"{terrains.Length} terrain(s), " +
            $"{cameras.Length} camera(s), " +
            $"{volumes.Length} post-processing volume(s) disabled.");
    }
}
#endif
