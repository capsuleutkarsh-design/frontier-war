using UnityEditor;
using UnityEditor.Build;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace FW.EditorTools
{
    /// <summary>
    /// One click applies the project settings from Docs/04_Production/Unity_Project_Setup.md §3 and §5.
    /// Menu: Frontier War ▸ Apply Project Settings. Safe to run again at any time.
    /// </summary>
    public static class ProjectSetup
    {
        [MenuItem("Frontier War/Apply Project Settings", priority = 0)]
        public static void Apply()
        {
            // Identity
            PlayerSettings.companyName = "FrontierWarDev";
            PlayerSettings.productName = "Frontier War";

            // Landscape only (both directions)
            PlayerSettings.defaultInterfaceOrientation = UIOrientation.AutoRotation;
            PlayerSettings.allowedAutorotateToPortrait = false;
            PlayerSettings.allowedAutorotateToPortraitUpsideDown = false;
            PlayerSettings.allowedAutorotateToLandscapeLeft = true;
            PlayerSettings.allowedAutorotateToLandscapeRight = true;

            // Android: IL2CPP, ARM64, Vulkan + GLES3
            PlayerSettings.SetScriptingBackend(NamedBuildTarget.Android, ScriptingImplementation.IL2CPP);
            PlayerSettings.Android.targetArchitectures = AndroidArchitecture.ARM64;
            PlayerSettings.SetUseDefaultGraphicsAPIs(BuildTarget.Android, false);
            PlayerSettings.SetGraphicsAPIs(BuildTarget.Android,
                new[] { GraphicsDeviceType.Vulkan, GraphicsDeviceType.OpenGLES3 });

            // We do not use Unity physics for gameplay (00_Architecture §8)
            Physics.simulationMode = SimulationMode.Script;

            // Git-friendly assets
            EditorSettings.serializationMode = SerializationMode.ForceText;
            VersionControlSettings.mode = "Visible Meta Files";

            // URP: locked look = no MSAA, no HDR, hard shadows, long shadow distance for the ortho camera
            int urpCount = 0;
            foreach (string guid in AssetDatabase.FindAssets("t:UniversalRenderPipelineAsset"))
            {
                var asset = AssetDatabase.LoadAssetAtPath<UniversalRenderPipelineAsset>(AssetDatabase.GUIDToAssetPath(guid));
                if (asset == null) continue;
                asset.msaaSampleCount = 1;
                asset.supportsHDR = false;
                asset.shadowDistance = 400f;
                EditorUtility.SetDirty(asset);
                urpCount++;
            }

            AssetDatabase.SaveAssets();
            Debug.Log($"[Frontier War] Project settings applied (URP assets updated: {urpCount}). " +
                      "Remember: switch platform to Android in File ▸ Build Profiles.");
        }
    }
}
