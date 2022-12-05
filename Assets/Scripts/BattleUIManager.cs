using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class BattleUIManager : MonoBehaviour
{
    //Responsible for the animations of all battle UI

    [SerializeField]
    private GameObject actionsMenu;


    public void ShowActionsMenu(){
        actionsMenu.SetActive(true);
        //Align on the side of the current unit?
    }

    public void HideActionsMenu(){
        actionsMenu.SetActive(false);
    }



} 
