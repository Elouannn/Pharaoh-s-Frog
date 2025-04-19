using UnityEngine;

public static class SaveManager
{
    // Sauvegarde de valeurs
    public static void SaveGame(int ActualCards, float SavedProgression, bool SavedFootSteps, int SavedHP)
    {
        PlayerPrefs.SetInt("SavedCards", ActualCards);
        PlayerPrefs.SetFloat("SavedProgression", SavedProgression);
        PlayerPrefs.GetInt("SavedFootsteps", SavedFootSteps ? 1 : 0);
        PlayerPrefs.GetInt("SavedHP", SavedHP);

        PlayerPrefs.Save(); // Enregistre immédiatement
    }

    // Chargement des valeurs
    public static void LoadGame(out int ActualCards, out float SavedProgression, out bool SavedFootsteps, out int SavedHP)
    {
        ActualCards = PlayerPrefs.GetInt("ActualCards", 0); 
        SavedProgression = PlayerPrefs.GetFloat("SavedProgression", 0f);
        SavedFootsteps = PlayerPrefs.GetInt("SavedFootsteps", 0) == 1;
        SavedHP = PlayerPrefs.GetInt("SavedHP", 2);
    }

    // Réinitialise les données
    public static void ResetSave()
    {
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();
    }
}