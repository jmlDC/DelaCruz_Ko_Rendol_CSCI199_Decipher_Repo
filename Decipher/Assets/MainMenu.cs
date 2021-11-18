using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    [SerializeField]
    private Animator animator;
    [SerializeField]
    private GameObject player;

    public void PlayGame(){
        
        // gameObject.SetActive(false);
        animator.Play("In-game Camera");
        player.GetComponent<UnityTPS>().allowMove = true;
        player.GetComponent<UnityTPS>().updateDay();
        player.GetComponent<UnityTPS>().persistentUI.SetActive(true);
        Cursor.visible=false;
    }

    public void QuitGame(){
        Application.Quit();
    }



}




    
