using UnityEngine;
using System.Collections;

namespace Devdog.InventorySystem.Demo
{
    public class LoadLevelOnTriggerEnter : MonoBehaviour
    {
        public string levelToLoad;


        public void OnTriggerEnter(Collider col)
        {
            LoadLevel();
        }


        public void LoadLevel()
        {
            InventorySceneUtility.LoadScene(levelToLoad);
        }
    }
}