using System.IO;
using UnityEditor;
using UnityEngine;

namespace Unity.MeshSync.Editor  {

[CustomEditor(typeof(SceneCachePlayer))]
internal class SceneCachePlayerInspector : MeshSyncPlayerInspector {
    

//----------------------------------------------------------------------------------------------------------------------

    public override void OnEnable() {
        base.OnEnable();
        m_sceneCachePlayer = target as SceneCachePlayer;
        
    }


//----------------------------------------------------------------------------------------------------------------------
    
    
    
    public override void OnInspectorGUI() {
        SerializedObject so = serializedObject;

        EditorGUILayout.Space();
        DrawCacheSettings(so);
        DrawPlayerSettings(m_sceneCachePlayer, so);
        MeshSyncPlayerConfig config = m_sceneCachePlayer.GetConfig();
        if (config.Profiling) {
            EditorGUILayout.TextArea(m_sceneCachePlayer.dbgProfileReport, GUILayout.Height(120));
            EditorGUILayout.Space();
        }

        DrawMaterialList(m_sceneCachePlayer);
        DrawTextureList(m_sceneCachePlayer);
        DrawAnimationTweak(m_sceneCachePlayer);
        DrawExportAssets(m_sceneCachePlayer);
        DrawPluginVersion();

        so.ApplyModifiedProperties();
    }

//----------------------------------------------------------------------------------------------------------------------
    
    void DrawCacheSettings(SerializedObject so) {
        GUIStyle styleFold = EditorStyles.foldout;
        styleFold.fontStyle = FontStyle.Bold;

        m_sceneCachePlayer.foldCacheSettings = EditorGUILayout.Foldout(m_sceneCachePlayer.foldCacheSettings, "Player", true, styleFold);
        if (m_sceneCachePlayer.foldCacheSettings) {
            //Show Selector GUI. Check if we should reopen
            string fullPath           = m_sceneCachePlayer.GetFilePath();
            string prevNormalizedPath = AssetUtility.NormalizeAssetPath(fullPath);

            string newNormalizedPath = InspectorUtility.ShowFileSelectorGUI("Cache File Path", "MeshSync", 
                prevNormalizedPath, OnSceneCacheFileReload, AssetUtility.NormalizeAssetPath);

            if (newNormalizedPath != prevNormalizedPath) {
                ChangeSceneCacheFileInInspector(m_sceneCachePlayer, newNormalizedPath);
            }
            
            if (!string.IsNullOrEmpty(fullPath) && !fullPath.StartsWith(Application.streamingAssetsPath)) {
                GUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                const float BUTTON_WIDTH = 50.0f;
                if (GUILayout.Button("Copy", GUILayout.Width(BUTTON_WIDTH))) {
                    string dstPath = Misc.CopyFileToStreamingAssets(fullPath);
                    ChangeSceneCacheFileInInspector(m_sceneCachePlayer, dstPath);
                }
                GUILayout.Label("or");
                if (GUILayout.Button("Move", GUILayout.Width(BUTTON_WIDTH))) {
                    string dstPath = Misc.MoveFileToStreamingAssets(fullPath);
                    ChangeSceneCacheFileInInspector(m_sceneCachePlayer, dstPath);
                }
                GUILayout.Label("to StreamingAssets");
                GUILayout.EndHorizontal();
            }
            EditorGUILayout.Space();

            // time / frame
            System.Action resetTimeAnimation = () => {
                so.ApplyModifiedProperties();
                m_sceneCachePlayer.ResetTimeAnimation();
            };

            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(so.FindProperty("m_timeUnit"));
            if (EditorGUI.EndChangeCheck())
                resetTimeAnimation();

            if (m_sceneCachePlayer.timeUnit == SceneCachePlayer.TimeUnit.Seconds) {
                EditorGUILayout.PropertyField(so.FindProperty("m_time"));
                EditorGUILayout.PropertyField(so.FindProperty("m_interpolation"));
            } else if (m_sceneCachePlayer.timeUnit == SceneCachePlayer.TimeUnit.Frames) {
                EditorGUI.BeginChangeCheck();
                EditorGUILayout.PropertyField(so.FindProperty("m_baseFrame"));
                if (EditorGUI.EndChangeCheck())
                    resetTimeAnimation();

                EditorGUILayout.PropertyField(so.FindProperty("m_frame"));
            }

            // preload
            {
                SerializedProperty preloadLength = so.FindProperty("m_preloadLength");
                preloadLength.intValue = EditorGUILayout.IntSlider("Preload Length", preloadLength.intValue, 0, m_sceneCachePlayer.frameCount);
            }

            EditorGUILayout.Space();
        }
    }

//----------------------------------------------------------------------------------------------------------------------
    void OnSceneCacheFileReload() {
        string sceneCacheFilePath = m_sceneCachePlayer.GetFilePath();
        SceneCachePlayerEditorUtility.ChangeSceneCacheFile(m_sceneCachePlayer, sceneCacheFilePath);
        GUIUtility.ExitGUI();        
    }
    
//----------------------------------------------------------------------------------------------------------------------
    private static void ChangeSceneCacheFileInInspector(SceneCachePlayer cachePlayer, string sceneCacheFilePath) {
        SceneCachePlayerEditorUtility.ChangeSceneCacheFile(cachePlayer, sceneCacheFilePath);
        GUIUtility.ExitGUI();
    }


//----------------------------------------------------------------------------------------------------------------------

    private SceneCachePlayer m_sceneCachePlayer = null;



}

} //end namespace