using System.Collections;
using System.Collections.Generic;
using Microsoft.Unity.VisualStudio.Editor;
using UnityEngine;
using UnityEngine.UI;

public class PlayerCanvas : MonoBehaviour
{
    public GameObject healthBar;
    public GameObject foodBar;
    public void UpdateHealth(int currentHealth)
    {
        for(int i = 0; i < 10; i++)
        {
            if(i < currentHealth)
            {
                healthBar.transform.GetChild(i).gameObject.transform.GetChild(0).gameObject.SetActive(true);
            }
            else
            {
                healthBar.transform.GetChild(i).gameObject.transform.GetChild(0).gameObject.SetActive(false);
            }
        }
    }
    public void UpdateFood(int currentFood)
    {
        for(int i = 0; i < 10; i++)
        {
            if(i < currentFood)
            {
                foodBar.transform.GetChild(9 - i).gameObject.transform.GetChild(0).gameObject.SetActive(true);
            }
            else
            {
                foodBar.transform.GetChild(9 - i).gameObject.transform.GetChild(0).gameObject.SetActive(false);
            }
        }
    }
}
