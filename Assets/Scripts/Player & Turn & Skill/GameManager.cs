using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }



    public List<Player> listOfPlayers = new List<Player>(); //Spielerliste

    string activPlayer;

    public int amountRealPlayer;
    public int amountNPC;
    public GameObject playerGameobject;
    public GameObject playerPrefab;


    private void Awake()
    {
        // Sicher stellen, dass nur eine Instanz existiert
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            // Eine Instanz existiert bereits, diese sollte zerstört werden
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        // Spieler erstellen
        //Player player1 = new Player(Player.PlayerOne, true);
        //listOfPlayers.Add(player1);

        //Player player2 = new Player(Player.PlayerTwo, false);
        //listOfPlayers.Add(player2);

        SetPlayerList();

    }

    private void SetPlayerList()
    {
        int totalAmount = amountRealPlayer + amountNPC;

        TurnManager turnManagerScript = GetComponent<TurnManager>();
        if (turnManagerScript == null) { Debug.Log("turnManagerScript == null"); }

        for (int i = 1; i < totalAmount + 1; i++)
        {
            if (i <= amountRealPlayer)
            {
                if (i == 1)
                {
                    GameObject newPlayerGameobject = Instantiate(playerPrefab, transform.position, Quaternion.identity, playerGameobject.transform);

                    NewPlayer player = newPlayerGameobject.GetComponent<NewPlayer>();

                    player.name = $"Main Player {i}";
                    player.isPlayer = true;
                    player.isMainPlayer = true;

                    turnManagerScript.Players.Add(player);
                }
                else
                {
                    GameObject newPlayerGameobject = Instantiate(playerPrefab, transform.position, Quaternion.identity, playerGameobject.transform);

                    NewPlayer player = newPlayerGameobject.GetComponent<NewPlayer>();

                    player.name = $"Player {i}";
                    player.isPlayer = true;
                    player.isMainPlayer = false;

                    turnManagerScript.Players.Add(player);
                }
            }
            else
            {
                GameObject newPlayerGameobject = Instantiate(playerPrefab, transform.position, Quaternion.identity, playerGameobject.transform);

                NewPlayer player = newPlayerGameobject.GetComponent<NewPlayer>();

                player.name = $"NPC {i - amountRealPlayer}";
                player.isPlayer = false;
                player.isMainPlayer = false;
                player.isNPC = true;

                turnManagerScript.Players.Add(player);
            }
        }

        turnManagerScript.StartTurn();
    }

    public string GetActivPlayer()
    {
        foreach (Player player in listOfPlayers)
        {
            if (player.isActivePlayer)
            {
                activPlayer = player.playerName;
            }
        }

        return activPlayer;
    }

    public List<Player> GetPlayerList()
    {
        return listOfPlayers;
    }
}
