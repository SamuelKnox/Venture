using UnityEngine;
using UnityEngine.SceneManagement;

public class UILevelUpController : MonoBehaviour
{
    [Tooltip("View used to display the runes")]
    [SerializeField]
    private LevelUpRunesView runesView;

    [Tooltip("View used to display the description of the currently selected rune")]
    [SerializeField]
    private LevelUpRuneDescriptionView runeDescriptionView;

    private bool dirty;

    void Start()
    {
        runesView.CreateTabs();
        runesView.UpdateDescription(null);
        runeDescriptionView.UpdateDescription(null);
        dirty = true;
    }

    void Update()
    {
        if (dirty)
        {
            dirty = false;
            runesView.MoveTab(0);
        }
    }

    /// <summary>
    /// Finishes leveling up runes and returns to game
    /// </summary>
    private void FinishLevelingUpRunes()
    {
        SceneManager.LoadScene(SceneNames.Venture);
    }
}