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

    public ResultManager resultManager;

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

        bool isComputer = currentPlayerIndex == 1;

        if (isComputer){
            Debug.Log("Computer rolled " + steps);
        } else {
            Debug.Log("You rolled " + steps);
        }

        // EASY QUESTION BEFORE MOVING
        if (isComputer)
        {
            if (Random.value >= 0.8f) // Computer fails
            {
                Debug.Log("Computer failed easy question");
                StartCoroutine(NextPlayerDelayed());
                yield break;
            }
        }
        else
        {
            questionManager.AskQuestion("easy");
            yield return new WaitUntil(() => !questionManager.waitingForAnswer);

            if (!questionManager.lastAnswerCorrect)
            {
                Debug.Log("Wrong answer. Turn skipped.");
                StartCoroutine(NextPlayerDelayed());
                yield break;
            }
        }

        // MOVE PLAYER
        yield return StartCoroutine(currentAvatar.MoveSteps(steps));

        int finalTile = currentAvatar.currentTileIndex + 1;

        // LADDER CHECK
        if (ladders.ContainsKey(finalTile))
        {
            if (isComputer)
            {
                bool climb = Random.value < 0.7f;

                if (climb)
                {
                    int destinationTile = ladders[finalTile];

                    Debug.Log("Computer climbs ladder");

                    yield return new WaitForSeconds(0.3f);
                    currentAvatar.JumpToTile(destinationTile);
                } else {
                    Debug.Log("Computer fails to climb ladder");
                }
            }
            else
            {
                questionManager.AskQuestion("medium");
                yield return new WaitUntil(() => !questionManager.waitingForAnswer);

                if (questionManager.lastAnswerCorrect)
                {
                    int destinationTile = ladders[finalTile];

                    yield return new WaitForSeconds(0.3f);
                    currentAvatar.JumpToTile(destinationTile);
                }
            }
        }

        // SNAKE CHECK
        else if (snakes.ContainsKey(finalTile))
        {
            if (isComputer)
            {
                bool avoidSnake = Random.value < 0.6f;

                if (!avoidSnake)
                {
                    int destinationTile = snakes[finalTile];

                    Debug.Log("Computer slides down snake");

                    yield return new WaitForSeconds(0.3f);
                    currentAvatar.JumpToTile(destinationTile);
                } else {
                    Debug.Log("Computer successfully avoids sliding down snake");
                }

            }
            else
            {
                questionManager.AskQuestion("hard");
                yield return new WaitUntil(() => !questionManager.waitingForAnswer);

                if (!questionManager.lastAnswerCorrect)
                {
                    int destinationTile = snakes[finalTile];

                    yield return new WaitForSeconds(0.3f);
                    currentAvatar.JumpToTile(destinationTile);
                }
            }
        }

        // CHECK WIN
        if (currentAvatar.currentTileIndex >= 99)
        {
            bool playerWon = currentPlayerIndex == 0;

            resultManager.EndGame(playerWon);
            yield break;
        }

        StartCoroutine(NextPlayerDelayed());
    }

    void NextPlayer()
    {
        // Switch player index
        currentPlayerIndex = (currentPlayerIndex + 1) % avatars.Count;

        // Determine if it’s the computer’s turn
        bool computerTurn = currentPlayerIndex == 1;

        // Allow human to roll only on their turn
        diceRoller.playerCanRoll = !computerTurn;

        isMoving = false;

        if (computerTurn)
        {
            // Start computer's turn automatically
            StartCoroutine(ComputerTurn());
        } else {
            Debug.Log("Your turn!");
        }

    }

    IEnumerator NextPlayerDelayed()
    {
        yield return new WaitForSeconds(1f); // delay so previous debug logs are shown
        NextPlayer();
    }

    IEnumerator ComputerTurn()
    {
        yield return new WaitForSeconds(1f); 

        // Enable computer roll
        diceRoller.isComputerRoll = true;

        // Trigger dice animation
        diceRoller.Roll();

        // Wait until dice finishes rolling
        yield return new WaitUntil(() => !diceRoller.IsRolling);

        // TurnManager.OnDiceRolled will be triggered automatically
        // Reset the flag
        diceRoller.isComputerRoll = false;
    }

    public void ResetTurnManager()
    {
        currentPlayerIndex = 0;
        isMoving = false;
        diceRoller.playerCanRoll = true;
    }
}