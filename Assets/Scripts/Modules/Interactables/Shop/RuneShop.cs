using UnityEngine;
using UnityEngine.SceneManagement;

public class RuneShop : Interactable
{
    [Tooltip("Canvas used to display gameplay UI")]
    [SerializeField]
    private Canvas gameplayCanvas;

    public override void OnInteractionEnter()
    {
        if (!SceneManager.GetSceneByName(SceneNames.RuneShop).isLoaded)
        {
            Time.timeScale = 0.0f;
            gameplayCanvas.gameObject.SetActive(false);
            SceneManager.LoadScene(SceneNames.RuneShop, LoadSceneMode.Additive);
        }
    }

    public override void OnInteractionExit()
    {
        gameplayCanvas.gameObject.SetActive(true);
    }
}