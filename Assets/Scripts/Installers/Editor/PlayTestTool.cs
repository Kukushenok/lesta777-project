using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace Game.Editor
{
    public class FastPlayTestWindow : EditorWindow
    {
        private const string RootLifetimeScopePrefabPath = "Assets/Settings/VContainer/RootLifetimeScope.prefab";

        private GameObject _prefab;
        private SerializedObject _serializedRootLifetimeScope;
        private Vector2 _scroll;

        [MenuItem("Tools/PlayTest")]
        private static void Open()
        {
            GetWindow<FastPlayTestWindow>("Play Test");
        }

        protected void OnEnable()
        {
            LoadRootLifetimeScopePrefab();
        }

        protected void OnGUI()
        {
            DrawSceneControls();
            DrawRootLifetimeScopeInspector();
        }

        private void DrawSceneControls()
        {
            EditorSceneManager.playModeStartScene = (SceneAsset)EditorGUILayout.ObjectField(
                new GUIContent("Start Scene"),
                EditorSceneManager.playModeStartScene,
                typeof(SceneAsset),
                false
            );

            // TODO: CHANGE IT TO THE ACTUAL ENTRY POINT
            var scenePath = "Assets/Scenes/SampleScene.unity";

            if (GUILayout.Button("Set start Scene: " + scenePath))
            {
                SetPlayModeStartScene(scenePath);
            }

            if (GUILayout.Button("Disable start scene"))
            {
                RemovePlayModeStartScene();
            }

            EditorGUILayout.Space(10);
        }

        private void LoadRootLifetimeScopePrefab()
        {
            _prefab = AssetDatabase.LoadAssetAtPath<GameObject>(RootLifetimeScopePrefabPath);

            if (_prefab == null)
            {
                Debug.LogWarning("RootLifetimeScope prefab not found at: " + RootLifetimeScopePrefabPath);
                return;
            }

            Component root = _prefab.GetComponent("RootLifetimeScope");
            if (root == null)
            {
                Debug.LogWarning("RootLifetimeScope component not found on prefab.");
                return;
            }

            _serializedRootLifetimeScope = new SerializedObject(root);
        }

        private void DrawRootLifetimeScopeInspector()
        {
            EditorGUILayout.LabelField("RootLifetimeScope Inspector", EditorStyles.boldLabel);

            if (_serializedRootLifetimeScope == null)
            {
                EditorGUILayout.HelpBox("RootLifetimeScope not found.", MessageType.Warning);
                return;
            }

            _serializedRootLifetimeScope.Update();

            _scroll = EditorGUILayout.BeginScrollView(_scroll);

            SerializedProperty prop = _serializedRootLifetimeScope.GetIterator();
            var enterChildren = true;

            while (prop.NextVisible(enterChildren))
            {
                // Skip script reference for clarity
                if (prop.propertyPath == "m_Script")
                {
                    continue;
                }

                EditorGUILayout.PropertyField(prop, true);
                enterChildren = false;
            }

            EditorGUILayout.EndScrollView();

            _serializedRootLifetimeScope.ApplyModifiedProperties();
        }

        private static void SetPlayModeStartScene(string scenePath)
        {
            SceneAsset sceneAsset = AssetDatabase.LoadAssetAtPath<SceneAsset>(scenePath);

            if (sceneAsset != null)
            {
                EditorSceneManager.playModeStartScene = sceneAsset;
            }
            else
            {
                Debug.Log("Could not find Scene " + scenePath);
            }
        }

        private static void RemovePlayModeStartScene()
        {
            EditorSceneManager.playModeStartScene = null;
        }
    }
}
