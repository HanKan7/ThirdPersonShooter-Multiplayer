using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement;
using Photon.Realtime;
using ExitGames.Client.Photon;

public class MatchManager : MonoBehaviourPunCallbacks, IOnEventCallback
{
    public static MatchManager instance;

    private void Awake()
    {
        instance = this;
    }

    public enum EventCodes : byte
    {
        NewPlayer,
        ListPlayers,
        UpdateStat
    }

    public List<PlayerInfo> allPlayers = new List<PlayerInfo>();
    private int index;


    private List<Leaderboard> lBoardPlayer = new List<Leaderboard>();

    public enum GameState
    {
        Waiting,
        Playing,
        Ending
    }

    public int killsToWin = 3;
    public Transform mapCamPoint;
    public GameState state = GameState.Waiting;
    public float waitAfterEnding = 3f;

    // Start is called before the first frame update
    void Start()
    {
        if (!PhotonNetwork.IsConnected)
        {
            SceneManager.LoadScene(0);
        }
        else
        {
            NewPlayerSend(PhotonNetwork.NickName);
            state = GameState.Playing;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.Tab)   && state != GameState.Ending)
        {
            ShowLeaderBoard();
        }
        if (Input.GetKeyUp(KeyCode.Tab) && state != GameState.Ending)
        {
            UIController.instance.leaderBoardGO.SetActive(false);
        }
    }

    public void OnEvent(EventData photonEvent)  //Reads events happening
    {
        if(photonEvent.Code < 200)
        {
            EventCodes theEvent = (EventCodes)photonEvent.Code;
            object[] data = (object[])photonEvent.CustomData;
            //Debug.Log("Received Event " + theEvent + " from " + PhotonNetwork.NickName);

            switch (theEvent)
            {
                case EventCodes.NewPlayer:
                    NewPlayerReceive(data);
                    break;

                case EventCodes.ListPlayers:
                    ListPlayersReceive(data);
                    break;

                case EventCodes.UpdateStat:
                    UpdateStatsReceive(data);
                    break;
            }
        }
    }

    //Called in clones if editor triggers events
    public override void OnEnable()
    {
        PhotonNetwork.AddCallbackTarget(this);   //This script gets called when an event happens
    }

    public override void OnDisable()
    {
        PhotonNetwork.RemoveCallbackTarget(this);
    }


    public void NewPlayerSend(string username)
    {
        object[] package = new object[4];
        package[0] = username;
        package[1] = PhotonNetwork.LocalPlayer.ActorNumber;
        package[2] = 0;
        package[3] = 0;

        //Sending this info here only to master
        PhotonNetwork.RaiseEvent((byte)EventCodes.NewPlayer, package, new RaiseEventOptions { Receivers = ReceiverGroup.MasterClient }, new SendOptions { Reliability = true });
    }

    public void NewPlayerReceive(object[] dataReceived)
    {
        //Debug.Log("New Player Received from " + PhotonNetwork.NickName);
        PlayerInfo player = new PlayerInfo(dataReceived);
        allPlayers.Add(player);
        ListPlayersSend();
    }

    public void ListPlayersSend()
    {
        object[] packageOfPlayerInfo = new object[allPlayers.Count + 1];   //Array of playersInfo
        packageOfPlayerInfo[0] = state;
        for (int i = 0; i < allPlayers.Count; i++)  //object array of object arrays
        {
            object[] playerInfo = new object[4]; //Array of player info being stored
            playerInfo[0] = allPlayers[i].name;
            playerInfo[1] = allPlayers[i].actor;
            playerInfo[2] = allPlayers[i].kills;
            playerInfo[3] = allPlayers[i].deaths;

            packageOfPlayerInfo[i + 1] = playerInfo;
        }

        PhotonNetwork.RaiseEvent((byte)EventCodes.ListPlayers, packageOfPlayerInfo, new RaiseEventOptions { Receivers = ReceiverGroup.All }, new SendOptions { Reliability = true });
    }

    public void ListPlayersReceive(object[] dataReceived)
    {
        allPlayers.Clear();

        state = (GameState)dataReceived[0];
        //Debug.Log("all players cleared");

        for (int i = 1; i < dataReceived.Length; i++)
        {
            object[] pieceOfPlayer = (object[])dataReceived[i];
            PlayerInfo player = new PlayerInfo((string)pieceOfPlayer[0], (int)pieceOfPlayer[1], (int)pieceOfPlayer[2], (int)pieceOfPlayer[3]);
            allPlayers.Add(player);
            if (PhotonNetwork.LocalPlayer.ActorNumber == player.actor)   //referencing ourselves
            {
                index = i - 1;
            }
        }

        StateCheck();
    }

    public void UpdateStatsSend(int actorSending, int statToUpdate, int amountToChange)     //statupdate 0 for kills 1 for death
    {
        object[] package = new object[] { actorSending, statToUpdate, amountToChange };
        PhotonNetwork.RaiseEvent((byte)EventCodes.UpdateStat, package, new RaiseEventOptions { Receivers = ReceiverGroup.All }, new SendOptions { Reliability = true });
    }

    public void UpdateStatsReceive(object[] dataReceived)
    {
        int actor = (int)dataReceived[0];
        int statType = (int)dataReceived[1];
        int amount = (int)dataReceived[2];

        for (int i = 0; i < allPlayers.Count; i++)
        {
            if (allPlayers[i].actor == actor)
            {
                switch (statType)
                {
                    case 0: //kills
                        allPlayers[i].kills += amount;
                        //Debug.Log("Player " + allPlayers[i].name + " kills : " + allPlayers[i].kills);
                        break;

                    case 1:
                        allPlayers[i].deaths += amount;
                        //Debug.Log("Player " + allPlayers[i].name + " deaths : " + allPlayers[i].deaths);
                        break;
                }

                if (i == index)
                {
                    UpdateStatsDisplay();
                }

                //if (UIContoller.instance.leaderboard.activeInHierarchy)
                //{
                //    ShowLeaderBoard();
                //}

                break;
            }
        }

        ScoreCheck();
    }

    public void UpdateStatsDisplay()
    {
        if(allPlayers.Count > index)
        {
            UIController.instance.killsText.text =  allPlayers[index].kills.ToString();
            UIController.instance.deathsText.text = allPlayers[index].deaths.ToString();
        }
        else
        {
            UIController.instance.killsText.text = "00";
            UIController.instance.deathsText.text = "00";
        }
    }

    void ShowLeaderBoard()
    {
        UIController.instance.leaderBoardGO.SetActive(true);

        foreach(Leaderboard lp in lBoardPlayer)
        {
            Destroy(lp.gameObject);
        }
        lBoardPlayer.Clear();

        UIController.instance.leaderBoardPlayerDisplay.gameObject.SetActive(false);
        List<PlayerInfo> sorted = SortPlayers(allPlayers);

        foreach (var player in sorted)
        {
            Leaderboard newPlayerDisplay = Instantiate(UIController.instance.leaderBoardPlayerDisplay, UIController.instance.leaderBoardPlayerDisplay.transform.parent);
            newPlayerDisplay.SetDetails(player.name, player.kills, player.deaths);
            newPlayerDisplay.gameObject.SetActive(true);
            lBoardPlayer.Add(newPlayerDisplay);
        }
    }

    List<PlayerInfo> SortPlayers(List<PlayerInfo> players)
    {
        List<PlayerInfo> sorted = new List<PlayerInfo>();
        while(sorted.Count < players.Count)
        {
            int highest = -1;
            PlayerInfo selectedPlayer = players[0];
            foreach(PlayerInfo player in players)
            {
                if (!sorted.Contains(player))
                {
                    if (player.kills > highest)
                    {
                        selectedPlayer = player;
                        highest = player.kills;
                    }
                }
                
            }

            sorted.Add(selectedPlayer);
        }
        return sorted;
    }

    public override void OnLeftRoom()
    {
        base.OnLeftRoom();
        SceneManager.LoadScene(0);
    }

    void ScoreCheck()
    {
        bool winnerFound = false;
        foreach(PlayerInfo player in allPlayers)
        {
            if(player.kills >= killsToWin && killsToWin > 0)
            {
                winnerFound = true;
                break;
            }
        }

        if (winnerFound)
        {
            if(PhotonNetwork.IsMasterClient && state != GameState.Ending)
            {
                state = GameState.Ending;
                ListPlayersSend();
            }
        }
    }

    void StateCheck()
    {
        if(state == GameState.Ending)
        {
            EndGame();
        }
    }

    void EndGame()
    {
        state = GameState.Ending;
        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.DestroyAll();
        }

        UIController.instance.endScreen.SetActive(true);
        ShowLeaderBoard();
        //Cursor.lockState = CursorLockMode.None;
        //Cursor.visible = true;
        Camera.main.transform.position = mapCamPoint.position;
        Camera.main.transform.rotation = mapCamPoint.rotation;
        StartCoroutine(EndCo());
    }

    private IEnumerator EndCo()
    {
        yield return new WaitForSeconds(waitAfterEnding);
        //if (!perpetual)
        //{
            PhotonNetwork.AutomaticallySyncScene = false;
            PhotonNetwork.LeaveRoom();
        //}
        //else
        //{
        //    if (PhotonNetwork.IsMasterClient)
        //    {
        //        if (!Launcher.instance.changeMapsBetweenRounds)
        //        {
        //            NextMatchSend();
        //        }
        //        else
        //        {
        //            int newLevel = Random.Range(0, Launcher.instance.allMaps.Length);
        //            if (Launcher.instance.allMaps[newLevel] == SceneManager.GetActiveScene().name)
        //            {
        //                NextMatchSend();
        //            }
        //            else
        //            {
        //                PhotonNetwork.LoadLevel(Launcher.instance.allMaps[newLevel]);
        //            }
        //        }

        //    }
        //}

    }
}

[System.Serializable]
public class PlayerInfo
{
    public string name;
    public int actor, kills, deaths;

    public PlayerInfo(string _name, int _actor, int _kills, int _deaths)
    {
        name = _name;
        actor = _actor;
        kills = _kills;
        deaths = _deaths;
    }

    public PlayerInfo(object[] data)
    {
        name = (string)data[0];
        actor = (int)data[1];
        kills = (int)data[2];
        deaths = (int)data[3];
    }


}
