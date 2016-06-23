using System;
using System.Linq;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Spawner))]
public class SpawnerEditor : Editor
{
    private const string SpawnChanceLabel = "Spawn Chance";
    private const string SpawnChanceTooltip = "This is the chance that any GameObject will be spawned.  0 means no GameObject will every spawn, and 1 means a GameObject will always spawn.";
    private const string DefaultLabel = "Add GameObject";
    private const string GameObjectTooltip = "GameObject which can be spawned at a given rate.";
    private const float MinSpawnChance = 0.0f;
    private const float MaxSpawnChance = 1.0f;

    public override void OnInspectorGUI()
    {
        var spawner = (Spawner)target;
        var spawnChanceGuiContent = new GUIContent(SpawnChanceLabel, SpawnChanceTooltip);
        float spawnChance = EditorGUILayout.Slider(spawnChanceGuiContent, spawner.GetSpawnChance(), MinSpawnChance, MaxSpawnChance);
        spawner.SetSpawnChance(spawnChance);
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