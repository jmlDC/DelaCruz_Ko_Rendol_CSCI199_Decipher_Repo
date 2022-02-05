using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ComputerMinigameScript : MonoBehaviour
{   
    public GameObject player;

    public bool puzzleSolved;
    // Start is called before the first frame update

    public bool solutionIsAchieved;

    public TextMeshProUGUI thisTextBox;

    void Start(){
        puzzleSolved = false;
        solutionIsAchieved = false;
    }

    void Update(){


        if (player.GetComponent<UnityTPS>().currentQuest.returnCurrentObjective().requiredInteractionObject == player.GetComponent<UnityTPS>().designatedKeyboard){
            thisTextBox.text = "Hello there!!";
            player.GetComponent<UnityTPS>().computerUI.transform.Find("MonitorScreen/Window/AcceptButton").gameObject.SetActive(true);

        } else {
            thisTextBox.text = "This is not part of your current objective!!!";
            puzzleSolved = false;
            solutionIsAchieved = false;
            player.GetComponent<UnityTPS>().computerUI.transform.Find("MonitorScreen/Window/AcceptButton").gameObject.SetActive(false);
        }
    }


    public void checkAnswer(){
        
        // Some code to confirm that the answer is correct and check the answer, allow the quest line to move forward.
        if (solutionIsAchieved){
            puzzleSolved = true;
            player.GetComponent<UnityTPS>().computerUI.SetActive(false);
        }
        player.GetComponent<UnityTPS>().setFocusToGame();

    }

    public void closeWindow(){
        player.GetComponent<UnityTPS>().computerUI.SetActive(false);
        player.GetComponent<UnityTPS>().setFocusToGame();
    }
}
