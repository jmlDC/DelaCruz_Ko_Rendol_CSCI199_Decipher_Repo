using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;

public class introUIScript : MonoBehaviour
{
    // Start is called before the first frame update

    public GameObject player;

    public bool playAnimationState;

    void Start()
    {
        playAnimationState = false;
    }

    public void closeBoard()
    {
        playAnimationState = true;
        player.GetComponent<UnityTPS>().introUI.SetActive(false);
        player.GetComponent<UnityTPS>().updateDay();
        player.GetComponent<UnityTPS>().persistentUI.SetActive(true);
        player.GetComponent<UnityTPS>().smartContractUI.SetActive(true);
        player.GetComponent<UnityTPS>().setFocusToGame();
        try
        {
            player.GetComponent<UnityTPS>().currentQuest = player.GetComponent<QuestGiver>();
            player.GetComponent<UnityTPS>().currentQuest.startQuestUI();
        }
        catch (Exception e)
        {
            Debug.Log("No beginner quest set.");
        }


    }

}
