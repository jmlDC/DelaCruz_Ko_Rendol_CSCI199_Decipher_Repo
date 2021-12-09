using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PubDesScript : MonoBehaviour
{
    // Start is called before the first frame update

    public GameObject player;

    public List<string> localCopy;
    public List<string> localCopyBlock;

    public string localString;
    public string blockString;
    private string stringToAdd;
    private string stringToAdd2;

    public void closeBoard()
    {
        player.GetComponent<UnityTPS>().pubDesUI.SetActive(false);
        player.GetComponent<UnityTPS>().setFocusToGame();
    }

    public void refreshBoard()
    {
        player.GetComponent<UnityTPS>().pubDesUI.transform.Find("Contents").GetComponent<TextMeshProUGUI>().text = localString;
        player.GetComponent<UnityTPS>().pubDesUI.transform.Find("blockSection").GetComponent<TextMeshProUGUI>().text = blockString;
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


        if (player.GetComponent<UnityTPS>().blocksCreated.Count>0){
            blockString = "";

            foreach (var block in player.GetComponent<UnityTPS>().blocksCreated){
                stringToAdd2 = "Block "+block+"\n";
                if (!localCopyBlock.Contains(stringToAdd2)){
                    localCopyBlock.Add(stringToAdd2);
                }
            }

            foreach (var entry in localCopyBlock){
                blockString += entry;
            }
        } else {
            blockString = "No blocks have been created by the intern yet.";
        }

    }

    public void pubDesConsolidatedMethod(){
        updateLog();
        refreshBoard();
    }


}
