using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NewSceneCreator : EditorWindow
{
    protected string _newSceneName;
    protected string _presetName;
    protected string _sceneFileRoot;
    protected readonly GUIContent _nameContent = new GUIContent("NewScene");
    protected readonly GUIContent _presetContent = new GUIContent("TemplateScene");
    protected readonly GUIContent _fileContent = new GUIContent("FilePath");

    [MenuItem("Editor/CreateTemplateScene")]
    static void Init()
    {
        NewSceneCreator window = GetWindow<NewSceneCreator>();
        window.Show();
        window._newSceneName = "NewScene";  //ここが作られるシーンの名前（初期）
        window._presetName = "PresetScene";
        window._sceneFileRoot = "Assets/Scenes/";
    }

    void OnGUI()
    {
        _newSceneName = EditorGUILayout.TextField(_nameContent, _newSceneName);
        _presetName = EditorGUILayout.TextField(_presetContent, _presetName);
        _sceneFileRoot = EditorGUILayout.TextField(_fileContent, _sceneFileRoot);

        if (GUILayout.Button("Create"))
            CheckAndCreateScene(_presetName);
    }

    protected void CheckAndCreateScene(string presetName)
    {
        if (EditorApplication.isPlaying)
        {
            Debug.LogWarning("Cannot create scenes while in play mode.  Exit play mode first.");
            return;
        }

        if (string.IsNullOrEmpty(_newSceneName))
        {
            Debug.LogWarning("Please enter a scene name before creating a scene.");
            return;
        }

        Scene currentActiveScene = SceneManager.GetActiveScene();

        if (currentActiveScene.isDirty)
        {
            string title = currentActiveScene.name + " Has Been Modified";
            string message = "Do you want to save the changes you made to " + currentActiveScene.path + "?\nChanges will be lost if you don't save them.";
            int option = EditorUtility.DisplayDialogComplex(title, message, "Save", "Don't Save", "Cancel");

            if (option == 0)
            {
                EditorSceneManager.SaveScene(currentActiveScene);
            }
            else if (option == 2)
            {
                return;
            }
        }

        CreateScene(presetName);
    }

    protected void CreateScene(string presetName)
    {
        string[] result = AssetDatabase.FindAssets(presetName);

        if (result.Length > 0)
        {
            string newScenePath = _sceneFileRoot + _newSceneName + ".unity"; //作成するシーンのパス
            AssetDatabase.CopyAsset(AssetDatabase.GUIDToAssetPath(result[0]), newScenePath);
            AssetDatabase.Refresh();
            EditorSceneManager.OpenScene(newScenePath, OpenSceneMode.Single);
        }
        else
        {
            //Debug.LogError("The template scene <b>_TemplateScene</b> couldn't be found ");
            EditorUtility.DisplayDialog("Error",
                "The scene _TemplateScene was not found in Gamekit2D/Scenes folder. This scene is required by the New Scene Creator.",
                "OK");
        }
    }
}