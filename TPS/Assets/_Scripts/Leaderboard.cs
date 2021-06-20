using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Leaderboard : MonoBehaviour
{
    [SerializeField] TMP_Text playerName, killsText, deathsText;

    public void SetDetails(string name, int kills, int deaths)
    {
        playerName.text = name;
        killsText.text = kills.ToString();
        deathsText.text = deaths.ToString();
    }
}
