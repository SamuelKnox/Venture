using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Blacksmith : Interactable
{
    public override void OnInteractionEnter()
    {
        if (!SceneManager.GetSceneByName(SceneNames.Inventory).isLoaded)
        {
            Time.timeScale = 0.0f;
            SceneManager.LoadScene(SceneNames.Inventory, LoadSceneMode.Additive);
        }
    }

    public override void OnInteractionExit() { }
}