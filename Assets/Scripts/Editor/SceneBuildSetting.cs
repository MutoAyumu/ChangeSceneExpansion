using System.Linq;
using UnityEditor;

public class SceneBuildSetting : AssetPostprocessor
{
    private static void OnPostprocessAllAssets(
        string[] importedAssets,
        string[] deletedAssets,
        string[] movedAssets,
        string[] movedFromAssetPaths
    )
    {
        var sceneList = EditorBuildSettings.scenes.ToList();

        sceneList.AddRange(importedAssets
            .Where(c => c.EndsWith(".unity"))
            .Where(c => !sceneList.Any(s => s.path == c))
            .Select(c => new EditorBuildSettingsScene(c, true)));
        sceneList.RemoveAll(c => deletedAssets.Contains(c.path));

        EditorBuildSettings.scenes = sceneList.ToArray();
    }
}
