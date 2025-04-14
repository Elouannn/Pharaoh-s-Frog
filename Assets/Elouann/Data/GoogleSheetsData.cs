using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using TMPro;


public class GoogleSheetImporter : MonoBehaviour
{
    private string sheetUrl = "https://docs.google.com/spreadsheets/d/e/2PACX-1vRd7-nhE-of6k5lJpjj_V85bNFmXq_yJf0FJADRYhZN8y2YcDfb-4xffsyA5HXn8K7JTsyxR5NSCGw7/pub?gid=0&single=true&output=tsv";
    public List<List<string>> tableData = new List<List<string>>();

    public List<Texture2D> images = new List<Texture2D>(); // Liste pour stocker les images

    public string googleDriveFileId; // ID du fichier Google Drive



    public List<CardConfig> cards;

    public MajCard CardSpawner;

    public Image progressBar;

    public Canvas canvasParent;

    public TMP_Text Titre;
    public Sprite SpriteHeartFull;
    public Sprite SpriteHeartEmpty;
    public Image Heart1;
    public Image Heart2;
    public Image Heart3;

    public bool SeeFootSteps = false;
    public GameObject RefFootsteps;

    public GameObject Header;

    public void Start()
    {
        
        StartCoroutine(DownloadSheet());
    }
    IEnumerator DownloadSheet()
    {
        using (UnityWebRequest request = UnityWebRequest.Get(sheetUrl))
        {
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                ParseTSV(request.downloadHandler.text);
                
            }
            else
            {
                Debug.LogError("Error downloading sheet: " + request.error);
            }
        }
    }

    void ParseTSV(string tsvData)
    {
        tableData.Clear();
        string[] rows = tsvData.Split('\n');

        foreach (string row in rows)
        {
            string[] columns = row.Split('\t');
            tableData.Add(new List<string>(columns));
        }

        CreateCardConfigs(); // Appel de la fonction apr�s le parsing
    }

    void CreateCardConfigs()
    {
        cards = new List<CardConfig>(); // R�initialisation de la liste des cartes

        for (int i = 1; i < tableData.Count; i++) // On commence � 1 pour ignorer l'ent�te
        {
            List<string> row = tableData[i];
            if (row.Count < 14) continue; // V�rification pour �viter les erreurs d'index

            CardConfig card = ScriptableObject.CreateInstance<CardConfig>();

            // Remplissage des donn�es
            int.TryParse(row[0], out card.Num�ro_salle);
            int.TryParse(row[1], out card.Num�ro_Carte);
            card.Titre = row[2];
            card.description = row[3];
            card.Choix_1 = row[4];
            int.TryParse(row[5], out card.Connexion_1);
            card.Consequence_1 = row[6];
            card.Choix_2 = row[7];
            int.TryParse(row[8], out card.Connexion_2);
            card.Consequence_2 = row[9];
            card.Choix_3 = row[10];
            int.TryParse(row[11], out card.Connexion_3);
            card.Consequence_3 = row[12];
            card.Choix_4 = row[13];
            int.TryParse(row[14], out card.Connexion_4);
            card.Consequence_4 = row[15];



            int.TryParse(row[17], out card.Progress);
            int.TryParse(row[18], out card.HpModifier);
            card.Footspteps = row[19];
            card.EventBind = row[20];
            int.TryParse(row[21], out card.TimerTime);



            cards.Add(card);
        }
        
        
    }
    IEnumerator DownloadImages()
    {
        string url = $"https://drive.google.com/uc?export=download&id={googleDriveFileId}";
        UnityWebRequest www = UnityWebRequestTexture.GetTexture(url);
        yield return www.SendWebRequest();

        // V�rifier si la requ�te a r�ussi
        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Erreur lors du t�l�chargement de l'image : " + www.error);
        }
        else
        {
            // T�l�charger la texture et l'assigner � l'image cible
            Texture2D texture = DownloadHandlerTexture.GetContent(www);
        }
    }
}

[Serializable]
public class CardConfig : ScriptableObject
{
    public int Num�ro_salle;
    public int Num�ro_Carte;
    public string Titre;
    public string description;
    public string Choix_1;
    public int Connexion_1;
    public string Consequence_1;
    public string Choix_2;
    public int Connexion_2;
    public string Consequence_2;
    public string Choix_3;
    public int Connexion_3;
    public string Consequence_3;
    public string Choix_4;
    public int Connexion_4;
    public string Consequence_4;
    public Texture2D Image;
    public int Progress;
    public int HpModifier;
    public string Footspteps;
    public string EventBind;
    public int TimerTime;
}



