using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class ComputerMinigameScript : MonoBehaviour
{   
    public GameObject player;

    public bool puzzleSolved;
    // Start is called before the first frame update

    public bool solutionIsAchieved;

    public TextMeshProUGUI thisTextBox;
    public TextMeshProUGUI taskDesc;

    void Start(){
        puzzleSolved = false;
        solutionIsAchieved = false;
    }

    void Update(){
        try{
            if (player.GetComponent<UnityTPS>().currentQuest.returnCurrentObjective().requiredInteractionObject == player.GetComponent<UnityTPS>().designatedKeyboard){

            
            thisTextBox.text = "Welcome to blockchain overview simulation.";
            taskDesc.text = player.GetComponent<UnityTPS>().currentQuest.returnCurrentObjective().virtualObjectiveDescription;

            player.GetComponent<UnityTPS>().computerUI.transform.Find("MonitorScreen/Window/AcceptButton").gameObject.SetActive(true);
            player.GetComponent<UnityTPS>().computerUI.transform.Find("MonitorScreen/Window/taskInstruction").gameObject.SetActive(true);
            player.GetComponent<UnityTPS>().computerUI.transform.Find("MonitorScreen/Window/taskDescription").gameObject.SetActive(true);

        } else {
            thisTextBox.text = "This is not part of your current objective!!!";
            puzzleSolved = false;
            solutionIsAchieved = false;
            player.GetComponent<UnityTPS>().computerUI.transform.Find("MonitorScreen/Window/taskInstruction").gameObject.SetActive(false);
            player.GetComponent<UnityTPS>().computerUI.transform.Find("MonitorScreen/Window/taskDescription").gameObject.SetActive(false);
            player.GetComponent<UnityTPS>().computerUI.transform.Find("MonitorScreen/Window/AcceptButton").gameObject.SetActive(false);
        }
        } catch (Exception e){

        }
        
    }



    public void miniGame(int a){

        switch (a){
            case 1:
                break;
            case 2:
                break;
            case 3:
                break;
        }
            
    }

    public void checkAnswer(){
        
        // Some code to confirm that the answer is correct and check the answer, allow the quest line to move forward.
        if (solutionIsAchieved){
            puzzleSolved = true;
            player.GetComponent<UnityTPS>().computerUI.SetActive(false);
            // player.GetComponent<UnityTPS>().controller.transform.position = new Vector3(320,2,285);

            player.GetComponent<UnityTPS>().controller.transform.position = player.GetComponent<UnityTPS>().currentQuest.returnCurrentObjective().setTeleportPosition;
        }
        player.GetComponent<UnityTPS>().setFocusToGame();

    }

    public void closeWindow(){
        player.GetComponent<UnityTPS>().computerUI.SetActive(false);
        player.GetComponent<UnityTPS>().setFocusToGame();
    }
}
