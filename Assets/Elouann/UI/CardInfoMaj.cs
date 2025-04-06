using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using System;
using TMPro;
using UnityEngine.UI;
using Unity.VisualScripting;
using System.Collections;

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
    private int CardNumber;
    public MajCard majCard;
    public TMP_Text GoNextText;
    public Texture2D BackgroundTexture;

    private int NextCardIS = 0;

    private Color LowColor = new Color(1, 1, 1, 0.2f);
    public int SelectedSide = 4;

    public bool lockUI = false;
    public Animator animator;
    public AnimationClip customAppearClip;

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
    }
   
    public void DownloadDatas(int i,GoogleSheetImporter DataBase, MajCard majcard)
    {
        CardData = DataBase;
        majCard = majcard;
        CardNumber = i;
        BackgroundTexture = CardData.images[CardNumber];
        FakeStart();
        StartCoroutine(EnterCard());
        
        
    }
    IEnumerator EnterCard()
    {
        yield return new WaitForSeconds(1);
        SpawnText(CardData.cards[CardNumber].Titre, Titre, () => SpawnText(CardData.cards[CardNumber].description, Description, () => DoLockUI(false)));
    }
    public void DoLockUI(bool boolean)
    {
        lockUI = boolean;
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
                    card.color = LowColor;
                    Choix.text = "";
                    SelectedSide = 4;
                }
            }
            else
            {
                CancelSetText();
                Choix.text = "";

                SelectedSide = i;
                if (AllArrows.Count > SelectedSide)
                {

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

}
