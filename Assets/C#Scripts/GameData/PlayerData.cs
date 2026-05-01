using System;
using UnityEngine;

[CreateAssetMenu(menuName = "Data/PlayerData")]
public class PlayerData : ScriptableObject
{
    public int playerHealth = 10;
    public int playerFood = 10;
}
