using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class dayQuestList
{
    public string[] dayQuestListArray;

    public string[] afterDayDialogue;
 
    public string[] returnArray(){
        return dayQuestListArray;
    }

    public string[] returnAfterDayDialogueArray(){
        return afterDayDialogue;
    }


}
