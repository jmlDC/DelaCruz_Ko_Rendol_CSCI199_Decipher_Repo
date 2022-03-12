using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [SerializeField]
    private Animator animator;
    [SerializeField]
    private Animator uiAnimator;
    [SerializeField]
    private GameObject player;


    public void PlayGame(){
        
        // gameObject.SetActive(false);

        player.GetComponent<UnityTPS>().introUI.SetActive(true);
        player.GetComponent<UnityTPS>().setFocusToUIIntroMode();

        // SceneManager.LoadScene("ConclusionScene");
        
        
    }

    void Update(){
        if (player.GetComponent<UnityTPS>().introUI.GetComponent<introUIScript>().playAnimationState){
            animator.Play("In-game Camera");
            uiAnimator.Play("In-game Camera");
            Destroy(gameObject);
        }
    }

    public void QuitGame(){
        Application.Quit();
    }





}




    
