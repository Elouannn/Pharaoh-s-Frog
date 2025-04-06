using UnityEngine;
using TMPro;
using System.Collections;

public class MajCard : MonoBehaviour
{
    public GoogleSheetImporter DataBase;
    public GameObject Cardref;
    public GameObject NewCard;

    public void CreateCard(int i)
    {
        NewCard = Instantiate(Cardref);
        NewCard.GetComponent<Animator>().SetTrigger("Cardin");
        NewCard.GetComponent<CardInfoMaj>().DownloadDatas(i,DataBase,this);
    }
}
