using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TurnManager : MonoBehaviour
{


    // inside TurnManager
    private Dictionary<int, int> ladders = new Dictionary<int, int>()
    {
        // format: {startTile, endTile}
        {9, 27},
        {18, 37},
        {25, 54},
        {28, 51},
        {56, 64},
        {67, 88},
        {76, 97},
        {79, 99},
    };

    private Dictionary<int, int> snakes = new Dictionary<int, int>()
    {
        {20, 1},
        {16, 4},
        {26, 15},
        {30, 11},
        {40, 22},
        {48, 34},
        {50, 31},
        {42, 38},
        {55, 45},
        {69, 53},
        {62, 59},
        {80, 61},
        {85, 75},
        {90, 71},
        {92, 89},
        {96, 86},
        {98, 83},
        {72, 68},
    };

    public DiceRoller diceRoller;
    public List<Avatar> avatars;
    public BoardGenerator boardGenerator;

    private int currentPlayerIndex = 0;
    private bool isMoving = false;

    void Start()
    {
        // Make sure boardTiles exist before initializing avatars
        StartCoroutine(InitializeAvatars());
    }

    IEnumerator InitializeAvatars()
    {
        while (boardGenerator.tiles == null || boardGenerator.tiles.Length == 0)
            yield return null;

        Tile[] boardTiles = boardGenerator.tiles;

        for (int i = 0; i < avatars.Count; i++)
        {
            avatars[i].Initialize(boardTiles, i);
        }

        // Subscribe to dice roll event
        diceRoller.OnDiceRolled += OnDiceRolled;
    }

    void OnDiceRolled(int roll)
    {
        if (!isMoving)
            StartCoroutine(MoveCurrentPlayer(roll));
    }

    IEnumerator MoveCurrentPlayer(int steps)
    {
        isMoving = true;

        Avatar currentAvatar = avatars[currentPlayerIndex];

        yield return StartCoroutine(currentAvatar.MoveSteps(steps));

        int finalTile = currentAvatar.currentTileIndex + 1; // tile numbers are 1-based

        // check for ladder
        if (ladders.ContainsKey(finalTile))
        {
            int destinationTile = ladders[finalTile];

            Debug.Log($"{currentAvatar.name} climbed a ladder to {destinationTile}!");

            yield return new WaitForSeconds(0.3f); // small pause for effect
            currentAvatar.JumpToTile(destinationTile);
        }

        // check for snake
        else if (snakes.ContainsKey(finalTile))
        {
            int destinationTile = snakes[finalTile];

            Debug.Log($"{currentAvatar.name} slid down a snake to {destinationTile}!");

            yield return new WaitForSeconds(0.3f);
            currentAvatar.JumpToTile(destinationTile);
        }

        // Next player's turn
        currentPlayerIndex = (currentPlayerIndex + 1) % avatars.Count;

        isMoving = false;
    }
}