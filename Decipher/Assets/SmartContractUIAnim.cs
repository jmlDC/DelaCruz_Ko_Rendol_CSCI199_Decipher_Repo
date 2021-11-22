using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class SmartContractUIAnim : MonoBehaviour
{
    // Start is called before the first frame update

    public LeanTweenType easeType;
    public Vector3 screenWidth; 

    public void onPress()
    {
        LeanTween.moveX(gameObject.GetComponent<RectTransform>(), -250f, 0.3f).setEase(easeType);
    }

    // Update is called once per frame
    public void onRelease()
    {
        LeanTween.moveX(gameObject.GetComponent<RectTransform>(), 250f,0.3f).setEase(easeType);
    }
}
