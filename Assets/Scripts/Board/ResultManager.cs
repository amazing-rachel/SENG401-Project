using UnityEngine;
using System.Collections;

public class ResultManager : MonoBehaviour
{
    public GameResultSender resultSender;
    public TurnManager turnManager;
    public PostGameUIManager postGameUI;


    public void EndGame(bool playerWon)
    {
        StartCoroutine(HandleEnd(playerWon));
    }

    IEnumerator HandleEnd(bool playerWon)
    {

        // check if SessionManager exists
        string username = SessionManager.Instance != null 
            ? SessionManager.Instance.CurrentUsername 
            : "Unknown";

        if (playerWon)
        {
            Debug.Log("YOU WIN!");
            resultSender.SendResult(username, "win");
        }
        else
        {
            Debug.Log("YOU LOSE!");
            resultSender.SendResult(username, "loss");
        }

        yield return new WaitForSeconds(3f);

        postGameUI.ShowStats();
    }

    public void ResetBoard()
    {

        if (turnManager == null || turnManager.avatars == null) return;

        foreach (Avatar avatar in turnManager.avatars)
        {
            // only jump if the board tiles exist
            if (avatar != null && avatar.boardTiles != null && avatar.boardTiles.Length > 0)
                avatar.JumpToTile(1); // human-readable first tile
        }

        // Reset TurnManager so game can start again
        if (turnManager != null)
            turnManager.ResetTurnManager(); 

        Debug.Log("Board reset. New game starting.");
    }
}
