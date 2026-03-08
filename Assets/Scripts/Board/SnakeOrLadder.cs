using UnityEngine;

public class SnakeOrLadder : MonoBehaviour
{
    public int startTile;
    public int endTile;

    public bool IsSnake => endTile < startTile;
}
