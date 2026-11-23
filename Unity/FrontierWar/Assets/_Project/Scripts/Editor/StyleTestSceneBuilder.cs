using FW.Game;
using FW.View;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Rendering;

namespace FW.EditorTools
{
    /// <summary>
    /// Builds the M0 style test: grey boxes at REAL scale (1 unit = 1 m) under the locked camera and pixel filter.
    /// Compare the Game view with Art/Concepts/pixel/11_style_lock.jpg.
    /// Menu: Frontier War ▸ Build Style Test Scene. Running it again rebuilds the scene from scratch.
    /// </summary>
    public static class StyleTestSceneBuilder
    {
        private const string ScenePath = "Assets/_Project/Scenes/Tests/StyleTest.unity";
        private const string MatFolder = "Assets/_Project/Art/Materials/_Placeholder";

        [MenuItem("Frontier War/Build Style Test Scene", priority = 20)]
        public static void Build()
        {
            if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo()) return;
            var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

            // Materials (muted, natural colours)
            var grass = Mat("M_Grass", new Color(0.42f, 0.46f, 0.30f));
            var road = Mat("M_Road", new Color(0.36f, 0.36f, 0.35f));
            var olive = Mat("M_Olive", new Color(0.33f, 0.36f, 0.22f));
            var team = Mat("M_TeamBlue", new Color(0.20f, 0.38f, 0.70f));
            var wall = Mat("M_HouseWall", new Color(0.82f, 0.78f, 0.68f));
            var roof = Mat("M_Roof", new Color(0.55f, 0.27f, 0.20f));
            var trunk = Mat("M_Trunk", new Color(0.30f, 0.22f, 0.15f));
            var leaves = Mat("M_Pine", new Color(0.17f, 0.27f, 0.15f));
            var concrete = Mat("M_Concrete", new Color(0.58f, 0.58f, 0.56f));
            var soldier = Mat("M_Soldier", new Color(0.28f, 0.31f, 0.20f));

            // Light: one sun, hard shadows, from the top-left of the screen
            RenderSettings.skybox = null;
            RenderSettings.ambientMode = AmbientMode.Flat;
            RenderSettings.ambientLight = new Color(0.42f, 0.40f, 0.36f);
            var sunGo = new GameObject("Sun");
            var sun = sunGo.AddComponent<Light>();
            sun.type = LightType.Directional;
            sun.shadows = LightShadows.Hard;
            sun.intensity = 1.2f;
            sun.color = new Color(1f, 0.96f, 0.88f);
            sunGo.transform.rotation = Quaternion.Euler(50f, 180f, 0f);

            var world = new GameObject("World").transform;

            // Ground 600 x 600 m and a 10 m wide road
            Box("Ground", world, new Vector3(0, -0.5f, 0), new Vector3(600, 1, 600), grass);
            Box("Road", world, new Vector3(0, 0.02f, 0), new Vector3(600, 0.04f, 10), road);

            // A column of 12 main battle tanks (hull 9.5 x 2.4 x 3.7 m)
            for (int i = 0; i < 12; i++)
            {
                var tank = new GameObject($"Tank_{i:00}").transform;
                tank.SetParent(world, false);
                tank.position = new Vector3(-70 + i * 12f, 0, (i % 2 == 0) ? -2 : 2);
                Box("Hull", tank, new Vector3(0, 0.9f, 0), new Vector3(9.5f, 1.4f, 3.7f), olive);
                Box("Turret", tank, new Vector3(-0.5f, 2.0f, 0), new Vector3(4.2f, 0.9f, 3.0f), olive);
                Box("Barrel", tank, new Vector3(3.6f, 2.05f, 0), new Vector3(4.8f, 0.3f, 0.3f), olive);
                Box("TeamStripe", tank, new Vector3(-0.5f, 2.47f, 0), new Vector3(1.2f, 0.06f, 3.02f), team);
            }

            // Jeep (4.5 m) and a squad of soldiers (1.8 m)
            Box("Jeep", world, new Vector3(-90, 0.9f, 0), new Vector3(4.5f, 1.8f, 2.0f), olive);
            for (int i = 0; i < 8; i++)
            {
                var s = GameObject.CreatePrimitive(PrimitiveType.Capsule);
                s.name = $"Soldier_{i}";
                Object.DestroyImmediate(s.GetComponent<Collider>());
                s.transform.SetParent(world, false);
                s.transform.position = new Vector3(-40 + i * 1.6f, 0.9f, -8 - (i % 2) * 1.5f);
                s.transform.localScale = new Vector3(0.5f, 0.9f, 0.5f); // capsule is 2 m tall at scale 1
                s.GetComponent<Renderer>().sharedMaterial = soldier;
            }

            // Six houses (12 x 7 x 10 m) with roofs
            Vector3[] housePos = { new(-60, 0, 30), new(-30, 0, 34), new(10, 0, 28), new(40, 0, -32), new(75, 0, -36), new(-80, 0, -34) };
            for (int i = 0; i < housePos.Length; i++)
            {
                var house = new GameObject($"House_{i}").transform;
                house.SetParent(world, false);
                house.position = housePos[i];
                Box("Walls", house, new Vector3(0, 3.5f, 0), new Vector3(12, 7, 10), wall);
                Box("Roof", house, new Vector3(0, 7.6f, 0), new Vector3(13, 1.2f, 11), roof);
            }

            // Factory / Command Core size reference (50 x 14 x 36 m)
            Box("Factory", world, new Vector3(90, 7, 70), new Vector3(50, 14, 36), concrete);

            // 60 pine trees, about 16 m tall
            var rng = new System.Random(42);
            for (int i = 0; i < 60; i++)
            {
                float x = (float)(rng.NextDouble() * 360 - 180);
                float z = (float)(rng.NextDouble() * 360 - 180);
                if (Mathf.Abs(z) < 14) z += 30 * Mathf.Sign(z == 0 ? 1 : z); // keep the road clear
                var tree = new GameObject($"Tree_{i:00}").transform;
                tree.SetParent(world, false);
                tree.position = new Vector3(x, 0, z);
                Prim(PrimitiveType.Cylinder, "Trunk", tree, new Vector3(0, 2, 0), new Vector3(0.6f, 2, 0.6f), trunk);
                Prim(PrimitiveType.Capsule, "Crown", tree, new Vector3(0, 10, 0), new Vector3(5, 6, 5), leaves);
            }

            // Camera rig: locked angle + pixel filter
            var camGo = new GameObject("WorldCamera");
            camGo.tag = "MainCamera";
            var cam = camGo.AddComponent<Camera>();
            cam.clearFlags = CameraClearFlags.SolidColor;
            cam.backgroundColor = new Color(0.12f, 0.13f, 0.11f);
            var rts = camGo.AddComponent<RtsCameraController>();
            camGo.AddComponent<PixelFilterController>();
            camGo.transform.position = new Vector3(0, 0, 0);

            // Game runner (tick loop + debug overlay)
            var runnerGo = new GameObject("GameRunner");
            var runner = runnerGo.AddComponent<GameRunner>();
            var so = new SerializedObject(runner);
            so.FindProperty("cameraController").objectReferenceValue = rts;
            so.ApplyModifiedPropertiesWithoutUndo();

            EditorSceneManager.SaveScene(scene, ScenePath);
            EditorBuildSettings.scenes = new[] { new EditorBuildSettingsScene(ScenePath, true) };
            Debug.Log($"[Frontier War] Style test scene built: {ScenePath}. Press Play and compare with Art/Concepts/pixel/11_style_lock.jpg");
        }

        private static GameObject Box(string name, Transform parent, Vector3 localPos, Vector3 size, Material mat) =>
            Prim(PrimitiveType.Cube, name, parent, localPos, size, mat);

        private static GameObject Prim(PrimitiveType type, string name, Transform parent, Vector3 localPos, Vector3 scale, Material mat)
        {
            var go = GameObject.CreatePrimitive(type);
            go.name = name;
            Object.DestroyImmediate(go.GetComponent<Collider>()); // no physics in this game
            go.transform.SetParent(parent, false);
            go.transform.localPosition = localPos;
            go.transform.localScale = scale;
            go.GetComponent<Renderer>().sharedMaterial = mat;
            return go;
        }

        private static Material Mat(string name, Color color)
        {
            string path = $"{MatFolder}/{name}.mat";
            var mat = AssetDatabase.LoadAssetAtPath<Material>(path);
            if (mat == null)
            {
                var shader = Shader.Find("Universal Render Pipeline/Lit");
                mat = new Material(shader) { name = name };
                AssetDatabase.CreateAsset(mat, path);
            }
            mat.SetColor("_BaseColor", color);
            mat.SetFloat("_Smoothness", 0.1f);
            EditorUtility.SetDirty(mat);
            return mat;
        }
    }
}
