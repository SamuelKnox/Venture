using System;
using System.Linq;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Spawner))]
public class SpawnerEditor : Editor
{
    private const string DefaultLabel = "Add GameObject";
    private const string GameObjectTooltip = "GameObject which can be spawned at a given rate.";

    public override void OnInspectorGUI()
    {
        var spawner = (Spawner)target;
        var spawnables = spawner.GetSpawnables();
        var weights = spawner.GetWeights();
        for (int i = 0; i < spawnables.Count && i < weights.Count; i++)
        {
            EditorGUILayout.BeginHorizontal();
            var existingGuiContent = new GUIContent(spawnables[i].name, GameObjectTooltip);
            weights[i] = EditorGUILayout.FloatField(existingGuiContent, weights[i]);
            var updatedSpawnable = (GameObject)EditorGUILayout.ObjectField(spawnables[i], typeof(GameObject), false);
            if (!updatedSpawnable)
            {
                spawnables.RemoveAt(i);
                weights.RemoveAt(i);
            }
            else
            {
                spawnables[i] = updatedSpawnable;
            }
            EditorGUILayout.EndHorizontal();
        }
        var nonExistingGuiContent = new GUIContent(DefaultLabel, GameObjectTooltip);
        var newSpawnable = (GameObject)EditorGUILayout.ObjectField(nonExistingGuiContent, null, typeof(GameObject), false);
        if (newSpawnable)
        {
            spawner.AddSpawnable(newSpawnable);
        }
        spawner.SetMaxSpawnableSize();
    }
}