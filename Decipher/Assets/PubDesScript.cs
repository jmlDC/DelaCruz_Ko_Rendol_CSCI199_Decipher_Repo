using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PubDesScript : MonoBehaviour
{
    // Start is called before the first frame update

    public GameObject player;

    public List<string> localCopy;

    public string localString;
    private string stringToAdd;

    public void closeBoard()
    {
        player.GetComponent<UnityTPS>().pubDesUI.SetActive(false);
        player.GetComponent<UnityTPS>().setFocusToGame();
    }

    public void refreshBoard()
    {
        player.GetComponent<UnityTPS>().pubDesUI.transform.Find("Contents").GetComponent<TextMeshProUGUI>().text = localString;
    }

    public void updateLog()
    {
        if (player.GetComponent<UnityTPS>().completedQuests.Count > 0)
        {   
            localString = "";
            foreach (var quest in player.GetComponent<UnityTPS>().completedQuests)
            {   
                stringToAdd = "Day "+quest.Substring(0,2)+" | Unity-chan has completed the following quest: " + quest.Substring(2).ToString() + "\n";
                if (!localCopy.Contains(stringToAdd)){
                    localCopy.Add(stringToAdd);
                }
                
            }
            
            foreach (var entry in localCopy)
            {
                
                localString += entry;
            }
        } else {
            localString = "Unity-chan has not completed any quests yet.";
        }

    }

    public void pubDesConsolidatedMethod(){
        updateLog();
        refreshBoard();
    }


}
