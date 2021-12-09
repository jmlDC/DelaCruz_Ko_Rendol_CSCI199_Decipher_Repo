using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
[System.Serializable]
public class dayQuestList
{
    public string[] dayQuestListArray;

    public string[] afterDayDialogue;
    [System.Serializable]
    public struct DialogueImage{
        public string line;
        public Texture image;
    }

    public DialogueImage[] afterDayDialogueImage;
 
    public string[] returnArray(){
        return dayQuestListArray;
    }

    public string[] returnAfterDayDialogueArray(){
        return afterDayDialogue;
    }

    public DialogueImage[] returnAfterDayDialogueImageArray(){
        return afterDayDialogueImage;
    }


}
