using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using System;
using TMPro;
using UnityEngine.UI;
using Unity.VisualScripting;
using System.Collections;
using UnityEngine.Networking;

public class CardInfoMaj : MonoBehaviour
{
    public TMP_Text Titre;
    public TMP_Text Description;
    public TMP_Text Choix;
    public TMP_Text Consequence;
    public SpriteRenderer ArrowLeft;
    public SpriteRenderer ArrowRight;
    public SpriteRenderer ArrowTop;
    public SpriteRenderer ArrowBottom;
    private List<SpriteRenderer> AllArrows = new List<SpriteRenderer>();
    private SpriteRenderer SelectedArrow;
    public GoogleSheetImporter CardData;
    public int CardNumber;
    public MajCard majCard;
    public TMP_Text GoNextText;
    public Texture2D BackgroundTexture;
    public Image BackgroundImage;

    private int NextCardIS = 0;

    private Color LowColor = new Color(1, 1, 1, 0.2f);
    public int SelectedSide = 4;

    public bool lockUI = false;
    public Animator animator;
    public AnimationClip customAppearClip;

    float elapsed = 0f;
    float x;
    
    float y;

    public List<Image> hearts = new List<Image>();

    private Dictionary<string, Action> eventActions;

    public GameObject TimerObject;

    private bool TimerStarted = false;

    public GameObject timerInstance;
    public TimerBar NewTimerObject;

    private void Start()
    {
        eventActions = new Dictionary<string, Action>
        {
            { "UnlockFootprints", UnlockFootprints },
            { "Timer", Timer }
        };
    }

    public void PlayEvent(string eventName)
    {
        if (eventActions.TryGetValue(eventName, out Action action))
        {
            action.Invoke();
        }
        else
        {
            Debug.LogWarning($"Événement '{eventName}' non reconnu !");
        }
    }

   

    public void FakeStart()
    {
        lockUI = true;
        if (majCard.DataBase.cards[CardNumber].Choix_1 != "")
        {
            AllArrows.Add(ArrowLeft);
        }
        else
        {
            ArrowLeft.gameObject.SetActive(false);
        }
        if (majCard.DataBase.cards[CardNumber].Choix_2 != "")
        {
            AllArrows.Add(ArrowRight);
        }
        else
        {
            ArrowRight.gameObject.SetActive(false);
        }
        if (majCard.DataBase.cards[CardNumber].Choix_3 != "")
        {
            AllArrows.Add(ArrowTop); ;
        }
        else
        {
            ArrowTop.gameObject.SetActive(false);
        }
        if (majCard.DataBase.cards[CardNumber].Choix_4 != "")
        {
            AllArrows.Add(ArrowBottom);
        }
        else
        {
            ArrowBottom.gameObject.SetActive(false);
        }
        foreach (var card in AllArrows) 
        {
            card.color = LowColor;
        }
        hearts.Add(CardData.Heart3);
        hearts.Add(CardData.Heart2);
        hearts.Add(CardData.Heart1);
    }
    public void FirstDownloadDatas(int i, GoogleSheetImporter DataBase, MajCard majcard)
    {
        StartCoroutine(DownloadDatas(i,DataBase,majcard));
    }
    IEnumerator DownloadDatas(int i,GoogleSheetImporter DataBase, MajCard majcard)
    {
        CardData = DataBase;
        majCard = majcard;
        CardNumber = i;
        string googleDriveFileId = CardData.cards[CardNumber].DriveField;
        string url = $"https://drive.google.com/uc?export=download&id={googleDriveFileId}";
        UnityWebRequest www = UnityWebRequestTexture.GetTexture(url);
        yield return www.SendWebRequest();
        print(url);
        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Erreur lors du téléchargement de l'image : " + www.error);
        }
        else
        {
            // Télécharger la texture et l'assigner à l'image cible
            BackgroundTexture = DownloadHandlerTexture.GetContent(www);

            Rect rect = new Rect(0, 0, BackgroundTexture.width, BackgroundTexture.height);
            Vector2 pivot = new Vector2(0.5f, 0.5f);

            Sprite sprite = Sprite.Create(BackgroundTexture, rect, pivot);
            BackgroundImage.sprite = sprite;
        }
        GetComponent<Animator>().SetTrigger("Cardin");
        FakeStart();
        StartCoroutine(EnterCard());
    }
    IEnumerator WaitASec()
    {
        yield return new WaitForSeconds(1f);
        
    }
    IEnumerator EnterCard()
    {
        yield return new WaitForSeconds(1);
        StartCoroutine(MajProgression(CardData.cards[CardNumber].Progress));
        if (CardData.cards[CardNumber].EventBind != "")
        {
            PlayEvent(CardData.cards[CardNumber].EventBind);
        }
        if (CardData.SeeFootSteps == true & CardData.cards[CardNumber].Footspteps !="None")
        {

            print(CardData.SeeFootSteps);
            Instantiate(CardData.RefFootsteps,transform.GetChild(0));
        }

        SpawnText(CardData.cards[CardNumber].Titre, CardData.Titre, () => SpawnText(CardData.cards[CardNumber].description, Description, () => DoLockUI(false)));
    }
    public void DoLockUI(bool boolean)
    {
        lockUI = boolean;
        LoseHp(CardData.cards[CardNumber].HpModifier);
    }
    public void SelectSide(int i)
    {
        
        if (i != SelectedSide)
        {
            if (i == 4)
            {
                foreach (var card in AllArrows)
                {
                    
                    CancelSetText();
                    //card.color = LowColor;
                    if(SelectedSide !=4)
                    {
                        AllArrows[SelectedSide].transform.gameObject.GetComponent<Animator>().SetTrigger("Desappear");
                    }
                    Choix.text = "";
                    SelectedSide = 4;
                }
            }
            else
            {
                CancelSetText();
                Choix.text = "";

                if (AllArrows.Count > i)
                {

                    SelectedSide = i;
                    foreach (var card in AllArrows)
                    {
                        if (card != AllArrows[i] && card.color.a > 0.3f)
                        {
                            card.transform.gameObject.GetComponent<Animator>().SetTrigger("Desappear");
                        }
                    }
                    AllArrows[i].GetComponent<Animator>().SetTrigger("Appear");
                    
                    string TempString = "Choix_" + (i + 1);
                    var fieldInfo = CardData.cards[CardNumber].GetType().GetField(TempString);
                    string TempText = fieldInfo.GetValue(CardData.cards[CardNumber])?.ToString();
                    SpawnText(TempText, Choix, null); 
                }
                else
                {
                    SelectSide(4);
                }
            }  
        }        
        
    }

    public bool once = false;
    public void LetGoDrag()
    {
        
        CancelSetText();
        Choix.text = "";
        string TempString = "Consequence_" + (SelectedSide + 1);
        var fieldInfo = CardData.cards[CardNumber].GetType().GetField(TempString);
        string temptext = fieldInfo.GetValue(CardData.cards[CardNumber])?.ToString();
        if (temptext != "")
        {
            once = true;
            lockUI = true;
            if(TimerStarted)
            {
                Timer();
            }
        }
        SpawnText(temptext, Consequence, () => EndLetGoDrag());
    }
    public void EndLetGoDrag()
    {
        if (Consequence.text == "")
        {
        }
        else
        {
            string TempStringBis = "Connexion_" + (SelectedSide + 1);
            var fieldInfoBis = CardData.cards[CardNumber].GetType().GetField(TempStringBis);
            string connexion = fieldInfoBis.GetValue(CardData.cards[CardNumber])?.ToString();
            
            int connexionInt = int.Parse(connexion);
            StartCoroutine(Wait2Sec(connexionInt, () => RdyForNextCard()));
        }
    }
    IEnumerator Wait2Sec(int i, Action onComplete)
    {
        NextCardIS = i;
        yield return new WaitForSeconds(1);
        onComplete?.Invoke();
    }

    private void RdyForNextCard()
    {
        GoNextText.text = "Click to Continue";
        TypingDone = true;
    }
    private bool tempOnce = false;
    private void Update()
    {
        if(TypingDone)
        {
            if(Input.GetMouseButtonDown(0) && tempOnce == false)
            {
                tempOnce = true;
                this.gameObject.GetComponent<Animator>().SetTrigger("Cardout");
                StartCoroutine(AttendreFinAnimation());
            }
        }
    }
    IEnumerator AttendreFinAnimation()
    {
        AnimatorStateInfo stateInfo = this.gameObject.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0);
        yield return new WaitForSeconds(1);
        majCard.CreateCard(NextCardIS);
        if(timerInstance != null)
        {
            Destroy(timerInstance);
        }
        Destroy(this.gameObject);
    }


    public TextMeshProUGUI textComponent;
    public float typingSpeed = 0.05f;
    private string fullText;
    private bool isTyping = false;
    private TMP_Text textBox;
    private bool TypingDone = false;

    public void SpawnText(string text, TMP_Text TextBox, Action onComplete = null)
    {
        textBox = TextBox;
        fullText = text;
        textBox.text = ""; // Vide le texte avant d'écrire
        StartCoroutine(TypeText(onComplete));
    }

    private IEnumerator TypeText(Action onComplete = null)
    {
        while (isTyping) yield return null;

        isTyping = true;
        foreach (char letter in fullText)
        {
            
            textBox.text += letter;
            yield return new WaitForSeconds(typingSpeed);
        }
        isTyping = false;

        onComplete?.Invoke();
    }
    public void CancelSetText()
    {
        
        StopAllCoroutines();
        isTyping = false;
    }
    public void FillText()
    {
        StopAllCoroutines();
        textComponent.text = fullText;
        isTyping = false;
    }


    IEnumerator MajProgression(int target)
    {
        x = CardData.progressBar.fillAmount;
        y = (float)target/100;
        elapsed = 0f;
        while (elapsed < 1)
        {
            elapsed += Time.deltaTime;
            CardData.progressBar.fillAmount = Mathf.Lerp(x, y, elapsed / 1);
            yield return null;

        }
    }

    public void LoseHp(int i)
    {
        bool tempFull = false;
        if (i < 0)
        {
            for (int x = 0; x < -i; x++)
            {
                foreach (Image heart in hearts)
                {
                    if (heart.sprite == CardData.SpriteHeartFull)
                    {
                        heart.sprite = CardData.SpriteHeartEmpty;
                        heart.GetComponent<Animator>().enabled = false;
                        tempFull = false;
                        break;
                    }
                    else
                    {
                        tempFull = true;
                    }
                }
                if (tempFull)
                {
                    Die();
                    break;
                }
            }
        }
        else
        {
            for (int x = 0; x < i; x++)
            {
                foreach (Image heart in hearts)
                {
                    if (heart.sprite == CardData.SpriteHeartEmpty)
                    {
                        heart.sprite = CardData.SpriteHeartFull;
                        heart.GetComponent<Animator>().enabled = true;
                        tempFull = false;
                        break;
                    }
                    else
                    {
                        tempFull = true;
                    }
                }
                if(tempFull)
                {
                    break;
                }
            }
        }
        
    }
    public void Die()
    {
        print("Game Over");
    }

    // All special events
    public void UnlockFootprints()
    {
        CardData.SeeFootSteps = true;
    }
    
    public void Timer()
    {
        if (TimerStarted == false)
        {
            timerInstance = Instantiate(TimerObject, CardData.Header.transform);
            RectTransform timerRect = timerInstance.GetComponent<RectTransform>();

            // Positionnement responsive
            timerRect.anchorMin = new Vector2(1f, 1f); // coin supérieur droit
            timerRect.anchorMax = new Vector2(1f, 1f);
            timerRect.pivot = new Vector2(1f, 1f);

            NewTimerObject = timerInstance.GetComponent<TimerBar>();
            NewTimerObject.ParentScript = this;
            NewTimerObject.duration = CardData.cards[CardNumber].TimerTime;
            TimerStarted = true;
        }
        else
        {
            if(NewTimerObject != null)
            {
                NewTimerObject.isRunning = false;
            }
        }
    }

}
