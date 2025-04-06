using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;


public class GoogleSheetImporter : MonoBehaviour
{
    private string sheetUrl = "https://docs.google.com/spreadsheets/d/e/2PACX-1vRd7-nhE-of6k5lJpjj_V85bNFmXq_yJf0FJADRYhZN8y2YcDfb-4xffsyA5HXn8K7JTsyxR5NSCGw7/pub?gid=0&single=true&output=tsv";
    public List<List<string>> tableData = new List<List<string>>();
    private string baseURL = "https://raw.githubusercontent.com/Elouannn/Pharaoh-s-Frog/main/ImagesCartes/"; // URL de ton dossier Git
    public List<Texture2D> images = new List<Texture2D>(); // Liste pour stocker les images

    public List<CardConfig> cards;

    public MajCard CardSpawner;

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
            

            cards.Add(card);
        }
        StartCoroutine(DownloadImages());
        
    }
    IEnumerator DownloadImages()
    {
        int i = 0;

        // Télécharge tant qu'il existe des images avec les noms '1.png', '2.png', '3.png', etc.
        while (true)
        {
            string imageUrl = baseURL + i + ".jpg"; // URL de l'image (changer l'extension si nécessaire)
            UnityWebRequest request = UnityWebRequestTexture.GetTexture(imageUrl);

            // Envoyer la requête
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                // Si l'image est téléchargée avec succès, l'ajouter à la liste
                Texture2D texture = DownloadHandlerTexture.GetContent(request);
                images.Add(texture);
                Debug.Log($"Image {i} téléchargée avec succès.");
            }
            else
            {
                // Si l'image n'existe pas (erreur 404 ou autre), on arrête la boucle
                Debug.Log($"Aucune image trouvée à {imageUrl}. Fin du téléchargement.");
                break;
            }

            i++; // Passer à l'image suivante
        }

        CardSpawner.CreateCard(0);
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
}