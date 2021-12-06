using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class questAcceptUIScript : MonoBehaviour
{
    // Start is called before the first frame update

    public GameObject player;

    public void declineQuest()
    {
        player.GetComponent<UnityTPS>().questAcceptUI.SetActive(false);
        player.GetComponent<UnityTPS>().setFocusToGame();
        player.GetComponent<UnityTPS>().globalQuestTracker = false;
    }

    public void acceptQuest(){

        player.GetComponent<UnityTPS>().questAcceptUI.SetActive(false);
        player.GetComponent<UnityTPS>().currentQuest.startQuestUI();
        player.GetComponent<UnityTPS>().setFocusToGame();

    }




}
