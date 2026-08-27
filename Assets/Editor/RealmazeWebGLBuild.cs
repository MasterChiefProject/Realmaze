using System;
using System.IO;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;
using UnityEngine.Rendering;

public static class RealmazeWebGLBuild
{
    private const string OutputPath = "docs";
    private const string StagingPath = "Builds/RealmazeWebGLStaging";
    private const string MobilePipelinePath =
        "Assets/Settings/Mobile_RPAsset.asset";

    private static readonly string[] ProductionScenes =
    {
        "Assets/Scenes/MainMenuScene.unity",
        "Assets/Scenes/GameScene.unity"
    };

    [MenuItem("Realmaze/Build WebGL for GitHub Pages")]
    public static void BuildWebGLForGitHubPages()
    {
        WebBuildQualityScope qualityScope = null;

        try
        {
            Debug.Log("[Realmaze Build] 1/7 Validating production assets...");
            ValidateProductionAssets();

            Debug.Log("[Realmaze Build] 2/7 Preparing WebGL target...");
            EnsureWebGLTarget();

            Debug.Log("[Realmaze Build] 3/7 Applying production WebGL settings...");
            ApplyProductionWebGLSettings();

            Debug.Log("[Realmaze Build] 4/7 Applying temporary WebGL quality profile...");
            qualityScope = WebBuildQualityScope.Apply();

            Debug.Log("[Realmaze Build] 5/7 Preparing staging directory...");
            DeleteDirectoryIfPresent(StagingPath);
            Directory.CreateDirectory(StagingPath);

            Debug.Log("[Realmaze Build] 6/7 Building Unity player...");

            BuildPlayerOptions options = new BuildPlayerOptions
            {
                scenes = ProductionScenes,
                locationPathName = StagingPath,
                target = BuildTarget.WebGL,
                options = BuildOptions.None
            };

            BuildReport report = BuildPipeline.BuildPlayer(options);

            if (report == null)
            {
                throw new InvalidOperationException(
                    "Unity failed before producing a BuildReport. " +
                    "Inspect the Console entry immediately above this message.");
            }

            BuildSummary summary = report.summary;

            if (summary.result != BuildResult.Succeeded)
            {
                throw new InvalidOperationException(
                    $"Realmaze WebGL build failed with result {summary.result}. " +
                    $"Errors: {summary.totalErrors}, warnings: {summary.totalWarnings}.");
            }

            Debug.Log("[Realmaze Build] 7/7 Publishing successful build to docs/...");
            ReplacePublishedBuild(StagingPath, OutputPath);

            File.WriteAllText(
                Path.Combine(OutputPath, ".nojekyll"),
                string.Empty);

            AssetDatabase.Refresh();

            double megabytes = summary.totalSize / (1024d * 1024d);

            Debug.Log(
                $"[Realmaze Build] SUCCESS. Published to '{OutputPath}'. " +
                $"Unity build size: {megabytes:F1} MB.");
        }
        catch (Exception exception)
        {
            Debug.LogError(
                "[Realmaze Build] FAILED.\n" +
                exception);
            throw;
        }
        finally
        {
            qualityScope?.Dispose();
            DeleteDirectoryIfPresent(StagingPath);
        }
    }

    private static void EnsureWebGLTarget()
    {
        if (EditorUserBuildSettings.activeBuildTarget == BuildTarget.WebGL)
        {
            return;
        }

        bool switched = EditorUserBuildSettings.SwitchActiveBuildTarget(
            BuildTargetGroup.WebGL,
            BuildTarget.WebGL);

        if (!switched)
        {
            throw new InvalidOperationException(
                "Unity could not switch the active build target to WebGL. " +
                "Confirm that WebGL Build Support is installed for Unity 6000.0.47f1.");
        }
    }

    private static void ApplyProductionWebGLSettings()
    {
        NamedBuildTarget web = NamedBuildTarget.WebGL;

        PlayerSettings.WebGL.template = "PROJECT:Realmaze";
        PlayerSettings.WebGL.compressionFormat = WebGLCompressionFormat.Gzip;
        PlayerSettings.WebGL.decompressionFallback = true;
        PlayerSettings.WebGL.dataCaching = true;

        PlayerSettings.WebGL.debugSymbolMode = WebGLDebugSymbolMode.Off;
        PlayerSettings.WebGL.exceptionSupport = WebGLExceptionSupport.None;
        PlayerSettings.WebGL.wasm2023 = true;
        PlayerSettings.WebGL.threadsSupport = false;
        PlayerSettings.WebGL.showDiagnostics = false;
        PlayerSettings.WebGL.closeOnQuit = false;
        PlayerSettings.WebGL.nameFilesAsHashes = false;
        PlayerSettings.WebGL.powerPreference = WebGLPowerPreference.HighPerformance;

        PlayerSettings.stripEngineCode = true;
        PlayerSettings.stripUnusedMeshComponents = true;

        PlayerSettings.SetIl2CppCodeGeneration(
            web,
            Il2CppCodeGeneration.OptimizeSize);

        PlayerSettings.SetManagedStrippingLevel(
            web,
            ManagedStrippingLevel.Medium);

        EditorUserBuildSettings.webGLBuildSubtarget =
            WebGLTextureSubtarget.Generic;
    }

    private static void ValidateProductionAssets()
    {
        foreach (string scene in ProductionScenes)
        {
            if (!File.Exists(scene))
            {
                throw new FileNotFoundException(
                    $"Required production scene was not found: {scene}");
            }
        }

        if (!AssetDatabase.IsValidFolder("Assets/WebGLTemplates/Realmaze"))
        {
            throw new DirectoryNotFoundException(
                "The custom WebGL template was not found at " +
                "'Assets/WebGLTemplates/Realmaze'.");
        }

        RenderPipelineAsset mobilePipeline =
            AssetDatabase.LoadAssetAtPath<RenderPipelineAsset>(
                MobilePipelinePath);

        if (mobilePipeline == null)
        {
            throw new FileNotFoundException(
                "The existing Mobile URP asset could not be loaded: " +
                MobilePipelinePath);
        }
    }

    private static void ReplacePublishedBuild(
        string source,
        string destination)
    {
        if (!Directory.Exists(source))
        {
            throw new DirectoryNotFoundException(
                $"Successful build staging directory is missing: {source}");
        }

        DeleteDirectoryIfPresent(destination);
        Directory.CreateDirectory(destination);

        foreach (string directory in Directory.GetDirectories(
                     source,
                     "*",
                     SearchOption.AllDirectories))
        {
            string relative = Path.GetRelativePath(source, directory);
            Directory.CreateDirectory(Path.Combine(destination, relative));
        }

        foreach (string file in Directory.GetFiles(
                     source,
                     "*",
                     SearchOption.AllDirectories))
        {
            string relative = Path.GetRelativePath(source, file);
            string target = Path.Combine(destination, relative);
            string targetDirectory =
                Path.GetDirectoryName(target) ?? destination;

            Directory.CreateDirectory(targetDirectory);
            File.Copy(file, target, true);
        }
    }

    private static void DeleteDirectoryIfPresent(string path)
    {
        if (!Directory.Exists(path))
        {
            return;
        }

        try
        {
            Directory.Delete(path, true);
        }
        catch (Exception exception)
        {
            Debug.LogWarning(
                $"[Realmaze Build] Could not remove '{path}': " +
                exception.Message);
        }
    }

    /// <summary>
    /// Temporarily makes every quality level use the project's existing
    /// Mobile URP asset and a 2-level mip limit. This lets WebGL mip stripping
    /// omit high-resolution texture mips and avoids shipping PC URP variants.
    /// The Editor/project settings are restored after the build.
    /// </summary>
    private sealed class WebBuildQualityScope : IDisposable
    {
        private readonly int originalQualityLevel;
        private readonly RenderPipelineAsset originalDefaultPipeline;
        private readonly RenderPipelineAsset[] originalQualityPipelines;
        private readonly int[] originalMipLimits;
        private readonly bool originalMipStripping;
        private bool disposed;

        private WebBuildQualityScope(
            int originalQualityLevel,
            RenderPipelineAsset originalDefaultPipeline,
            RenderPipelineAsset[] originalQualityPipelines,
            int[] originalMipLimits,
            bool originalMipStripping)
        {
            this.originalQualityLevel = originalQualityLevel;
            this.originalDefaultPipeline = originalDefaultPipeline;
            this.originalQualityPipelines = originalQualityPipelines;
            this.originalMipLimits = originalMipLimits;
            this.originalMipStripping = originalMipStripping;
        }

        public static WebBuildQualityScope Apply()
        {
            RenderPipelineAsset mobilePipeline =
                AssetDatabase.LoadAssetAtPath<RenderPipelineAsset>(
                    MobilePipelinePath);

            if (mobilePipeline == null)
            {
                throw new InvalidOperationException(
                    "Mobile URP asset is unavailable.");
            }

            int originalLevel = QualitySettings.GetQualityLevel();
            int levelCount = QualitySettings.names.Length;

            RenderPipelineAsset[] pipelines =
                new RenderPipelineAsset[levelCount];

            int[] mipLimits = new int[levelCount];

            for (int i = 0; i < levelCount; i++)
            {
                QualitySettings.SetQualityLevel(i, false);
                pipelines[i] = QualitySettings.renderPipeline;
                mipLimits[i] = QualitySettings.globalTextureMipmapLimit;
            }

            RenderPipelineAsset defaultPipeline =
                GraphicsSettings.defaultRenderPipeline;

            bool mipStripping = PlayerSettings.mipStripping;

            WebBuildQualityScope scope =
                new WebBuildQualityScope(
                    originalLevel,
                    defaultPipeline,
                    pipelines,
                    mipLimits,
                    mipStripping);

            GraphicsSettings.defaultRenderPipeline = mobilePipeline;
            PlayerSettings.mipStripping = true;

            for (int i = 0; i < levelCount; i++)
            {
                QualitySettings.SetQualityLevel(i, false);
                QualitySettings.renderPipeline = mobilePipeline;
                QualitySettings.globalTextureMipmapLimit = 2;
            }

            if (levelCount > 0)
            {
                QualitySettings.SetQualityLevel(0, true);
            }

            Debug.Log(
                "[Realmaze Build] WebGL quality profile: Mobile URP + " +
                "2 mip levels stripped from eligible textures.");

            return scope;
        }

        public void Dispose()
        {
            if (disposed)
            {
                return;
            }

            disposed = true;

            PlayerSettings.mipStripping = originalMipStripping;
            GraphicsSettings.defaultRenderPipeline =
                originalDefaultPipeline;

            int levelCount = Math.Min(
                QualitySettings.names.Length,
                originalQualityPipelines.Length);

            for (int i = 0; i < levelCount; i++)
            {
                QualitySettings.SetQualityLevel(i, false);
                QualitySettings.renderPipeline =
                    originalQualityPipelines[i];
                QualitySettings.globalTextureMipmapLimit =
                    originalMipLimits[i];
            }

            if (QualitySettings.names.Length > 0)
            {
                int restoredLevel = Mathf.Clamp(
                    originalQualityLevel,
                    0,
                    QualitySettings.names.Length - 1);

                QualitySettings.SetQualityLevel(
                    restoredLevel,
                    true);
            }

            Debug.Log(
                "[Realmaze Build] Original Editor quality settings restored.");
        }
    }
}
