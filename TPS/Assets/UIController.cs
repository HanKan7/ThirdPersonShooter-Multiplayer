using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIController : MonoBehaviour
{

    public static UIController instance;
    public GameObject DeathScreen;
    public TMP_Text deathText;

    [Header("Kills/Deaths UI")]
    public TMP_Text killsText, deathsText;

    [Header("Ammo UI")]
    public TMP_Text ammoCountText;

    public GameObject leaderBoardGO;
    public Leaderboard leaderBoardPlayerDisplay;

    private void Awake()
    {
        instance = this;
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
