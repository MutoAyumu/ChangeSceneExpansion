using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

//シーン切り替えEditor拡張
public class SceneChangeWindow : EditorWindow
{
    private string[] _sceneNameArray;                   //ファイル名保存用
    private string _folderPath = "Assets/Scenes/";      //シーンファイルの検索用パス
    private bool _isPathChanged = true;

    //シーン変更ウィンドウを開く
    [MenuItem("Editor/SceneChangeWindow")]
    static void WindowOpen()
    {
        var window = EditorWindow.GetWindow<SceneChangeWindow>();
    }

    //ウィンドウの見た目を描画
    private void OnGUI()
    {
        DisplayPathField();

        if (!_isPathChanged)
        {
            LoadSceneFile();
            DisplaySceneButtons();
        }
        else
        {
            ReloadSceneButton();
        }
    }

    //パスを入力するフィールドを表示
    private void DisplayPathField()
    {
        var folderPath = EditorGUILayout.TextField("FilePath", _folderPath);

        if (_folderPath != folderPath)
        {
            _isPathChanged = true;
            _folderPath = folderPath;
        }
    }
    // シーンファイルのファイル名を取得
    private void LoadSceneFile()
    {
        //フォルダ内のシーンファイルのパスを取得する
        _sceneNameArray = System.IO.Directory.GetFiles(_folderPath, "*", System.IO.SearchOption.TopDirectoryOnly).
            Where(s => !s.EndsWith(".meta", System.StringComparison.OrdinalIgnoreCase)).ToArray();

        //シーンファイルパスからファイル名のみを取得する
        for (int i = 0; i < _sceneNameArray.Length; i++)
        {
            _sceneNameArray[i] = System.IO.Path.GetFileName(_sceneNameArray[i]);
        }
    }

    //シーン変更ボタンを表示
    private void DisplaySceneButtons()
    {
        //シーンのファイルの数ボタンを描画
        for (int i = 0; i < _sceneNameArray.Length; i++)
        {
            //ボタンを描画
            if (GUILayout.Button(_sceneNameArray[i]))
            {
                OpenScene(_sceneNameArray[i]);
            }
        }
    }

    private void ReloadSceneButton()
    {
        if (!AssetDatabase.IsValidFolder(_folderPath))
        {
            EditorGUILayout.LabelField("A non-existent file path has been set.");
            return;
        }

        if (_folderPath[_folderPath.Length - 1] != '/')
        {
            EditorGUILayout.LabelField("The file path must end with /.");
            return;
        }

        if (GUILayout.Button("Reload"))
        {
            _isPathChanged = false;
        }
    }

    //シーンを開く
    private void OpenScene(string sceneName)
    {
        //シーンに変更があった場合に保存をするのかを尋ねる
        if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
        {
            EditorSceneManager.OpenScene(_folderPath + sceneName);
        }
    }
}