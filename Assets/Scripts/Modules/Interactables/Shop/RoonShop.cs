using UnityEngine;
using UnityEngine.SceneManagement;

public class RoonShop : Interactable
{
    [Tooltip("Canvas used to display gameplay UI")]
    [SerializeField]
    private Canvas gameplayCanvas;

    public override void OnInteractionEnter()
    {
        if (!SceneManager.GetSceneByName(SceneNames.RoonShop).isLoaded)
        {
            Time.timeScale = 0.0f;
            gameplayCanvas.gameObject.SetActive(false);
            SceneManager.LoadScene(SceneNames.RoonShop, LoadSceneMode.Additive);
        }
    }

    public override void OnInteractionExit()
    {
        gameplayCanvas.gameObject.SetActive(true);
    }
}