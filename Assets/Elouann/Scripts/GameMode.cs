using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;
using TMPro;

public class GameMode : MonoBehaviour
{
    public Button ResumeButton;
    public Button StartButton;
    public Button CreditsButton;
    public Button QuitButton;

    public GoogleSheetImporter DataImporter;

    public void Start()
    {
        int savedCard;
        float savedProgression;
        bool savedSeeFootsteps;
        int savedHP;
        SaveManager.LoadGame(out savedCard, out savedProgression, out savedSeeFootsteps, out savedHP);
        if(savedCard == 0)
        {
            ResumeButton.enabled = false;
            StartButton.GetComponent<TMP_Text>().text = "Start";
        }
        else
        {
            StartButton.GetComponent<TMP_Text>().text = "New Game";
            ResumeButton.enabled = true;
        }
    }

    public void ResumeGame()
    {

    }

    public void StratGame()
    {
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();
    }

    public void Credits()
    {

    }

    public void QuitGame()
    {

    }
}
