using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement; 
public class EnemyRematchConfirmPanel : MonoBehaviour
{
    
    public void Show()
    {
        gameObject.SetActive(true);
    }

    
   

    public void Cancel()
    {
        gameObject.SetActive(false);
    }
}