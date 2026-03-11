using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TurnManager : MonoBehaviour
{
    private Dictionary<int, int> ladders = new Dictionary<int, int>()
    {
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
    public QuestionManager questionManager;

    private int currentPlayerIndex = 0;
    private bool isMoving = false;

    void Start()
    {
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

        diceRoller.OnDiceRolled += OnDiceRolled;
    }

    void OnDiceRolled(int roll)
    {
        if (!isMoving)
            StartCoroutine(HandleTurn(roll));
    }

    IEnumerator HandleTurn(int steps)
    {
        isMoving = true;

        Avatar currentAvatar = avatars[currentPlayerIndex];

        Debug.Log("Player " + currentPlayerIndex + " rolled " + steps);

        // EASY QUESTION BEFORE MOVING
        questionManager.AskQuestion("easy");

        yield return new WaitUntil(() => !questionManager.waitingForAnswer);

        if (!questionManager.lastAnswerCorrect)
        {
            Debug.Log("Wrong answer. Turn skipped.");
            NextPlayer();
            yield break;
        }

        // MOVE PLAYER
        yield return StartCoroutine(currentAvatar.MoveSteps(steps));

        int finalTile = currentAvatar.currentTileIndex + 1;

        // LADDER CHECK
        if (ladders.ContainsKey(finalTile))
        {
            Debug.Log("Ladder found!");

            questionManager.AskQuestion("medium");

            yield return new WaitUntil(() => !questionManager.waitingForAnswer);

            if (questionManager.lastAnswerCorrect)
            {
                int destinationTile = ladders[finalTile];

                Debug.Log("Correct! Climbing ladder.");

                yield return new WaitForSeconds(0.3f);

                currentAvatar.JumpToTile(destinationTile);
            }
            else
            {
                Debug.Log("Wrong. Stay on tile.");
            }
        }

        // SNAKE CHECK
        else if (snakes.ContainsKey(finalTile))
        {
            Debug.Log("Snake found!");

            questionManager.AskQuestion("hard");

            yield return new WaitUntil(() => !questionManager.waitingForAnswer);

            if (!questionManager.lastAnswerCorrect)
            {
                int destinationTile = snakes[finalTile];

                Debug.Log("Wrong! Sliding down snake.");

                yield return new WaitForSeconds(0.3f);

                currentAvatar.JumpToTile(destinationTile);
            }
            else
            {
                Debug.Log("Correct! Snake avoided.");
            }
        }

        NextPlayer();
    }

    void NextPlayer()
    {
        currentPlayerIndex = (currentPlayerIndex + 1) % avatars.Count;
        isMoving = false;
    }
}