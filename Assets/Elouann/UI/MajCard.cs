using UnityEngine;
using TMPro;
using System.Collections;

public class MajCard : MonoBehaviour
{
    public GoogleSheetImporter DataBase;
    public GameObject Cardref;
    public GameObject NewCard;
    public Canvas canvasParent;

    public void CreateCard(int i)
    {

        NewCard = Instantiate(Cardref) ;
        
        NewCard.GetComponent<CardInfoMaj>().FirstDownloadDatas(i,DataBase,this);
    }
}
