using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class TimerBar : MonoBehaviour
{
    public float duration = 8f;

    [Header("UI References")]
    public Image progressBar;           // Image UI en mode "Filled"
    public TextMeshProUGUI timerText;   // Texte TMP pour afficher le temps

    private float currentTime;
    public bool isRunning = false;

    public CardInfoMaj ParentScript;

    private void Start()
    {
        StartCoroutine(WaitOneSec());
    }
    IEnumerator WaitOneSec()
    {
        yield return new WaitForSeconds(1f);
        StartTimer();
    }

    public void StartTimer()
    {
        currentTime = duration;
        isRunning = true;
    }

    private void Update()
    {
        if (!isRunning) return;

        currentTime -= Time.deltaTime;
        currentTime = Mathf.Max(0f, currentTime); // Clamp à zéro

        // Update UI
        if (progressBar != null)
            progressBar.fillAmount = currentTime / duration;

        if (timerText != null)
            timerText.text = currentTime.ToString("F2") + "s";

        if (currentTime <= 0f)
        {
            isRunning = false;
            OnTimerEnd();
        }
    }

    private void OnTimerEnd()
    {
        ParentScript.SelectedSide=3;
        ParentScript.LetGoDrag();
    }
}
