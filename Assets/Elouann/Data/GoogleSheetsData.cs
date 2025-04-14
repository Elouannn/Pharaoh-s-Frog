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

        CreateCardConfigs(); // Appel de la fonction après le parsing
    }

    void CreateCardConfigs()
    {
        cards = new List<CardConfig>(); // Réinitialisation de la liste des cartes

        for (int i = 1; i < tableData.Count; i++) // On commence à 1 pour ignorer l'entête
        {
            List<string> row = tableData[i];
            if (row.Count < 14) continue; // Vérification pour éviter les erreurs d'index

            CardConfig card = ScriptableObject.CreateInstance<CardConfig>();

            // Remplissage des données
            int.TryParse(row[0], out card.Numéro_salle);
            int.TryParse(row[1], out card.Numéro_Carte);
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

        // Vérifier si la requête a réussi
        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Erreur lors du téléchargement de l'image : " + www.error);
        }
        else
        {
            // Télécharger la texture et l'assigner à l'image cible
            Texture2D texture = DownloadHandlerTexture.GetContent(www);
        }
    }
}

[Serializable]
public class CardConfig : ScriptableObject
{
    public int Numéro_salle;
    public int Numéro_Carte;
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



