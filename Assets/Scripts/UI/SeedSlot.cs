using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SeedSlot : MonoBehaviour
{
    public TMP_Text SeedText;
    public Button SelectButton;
    private void Awake()
    {
        SelectButton.onClick.AddListener(() =>
        {
            MapManager.Instance.SetSeed(int.TryParse(SeedText.text, out int seed) ? seed : 11111);
        });
    }
}
