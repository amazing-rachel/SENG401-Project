using UnityEngine;
using System.Collections;

public class Avatar : MonoBehaviour
{
    public int currentTileIndex = 0; // starting tile
    public float moveSpeed = 2f;

    public int playerIndex; // determines offset for stacking

    [HideInInspector] public Tile[] boardTiles;
    private float verticalOffset = 0.1f;

    public void Initialize(Tile[] tiles, int index)
    {
        boardTiles = tiles;
        playerIndex = index;

        MoveToCurrentTile();
    }

    public IEnumerator MoveSteps(int steps)
    {
        int direction = steps >= 0 ? 1 : -1;
        int absSteps = Mathf.Abs(steps);

        for (int i = 0; i < absSteps; i++)
        {
            int nextIndex = currentTileIndex + direction;

            if (nextIndex < 0 || nextIndex >= boardTiles.Length)
                yield break; // stop moving if out of bounds

            currentTileIndex = nextIndex;

            Vector3 targetPos = GetTilePosition(currentTileIndex);

            while (Vector3.Distance(transform.position, targetPos) > 0.01f)
            {
                transform.position = Vector3.MoveTowards(transform.position, targetPos, moveSpeed * Time.deltaTime);
                yield return null;
            }

            yield return new WaitForSeconds(0.05f);
        }

        MoveToCurrentTile();
    }

    public void JumpToTile(int tileNumber)
    {
        if (boardTiles == null || boardTiles.Length == 0) return;

        // Convert tileNumber (1-based) to 0-based index
        currentTileIndex = Mathf.Clamp(tileNumber - 1, 0, boardTiles.Length - 1);

        MoveToCurrentTile();
    }

    Vector3 GetTilePosition(int tileIndex)
    {
        if (boardTiles == null || boardTiles.Length == 0)
            return transform.position;

        Vector3 basePos = boardTiles[Mathf.Clamp(tileIndex, 0, boardTiles.Length - 1)].transform.position;


        float spacing = 0.25f;

        Vector3 offset = Vector3.zero;

        switch (playerIndex)
        {
            case 0:
                offset = new Vector3(-spacing, verticalOffset, 0f); // left side
                break;

            case 1:
                offset = new Vector3(spacing, verticalOffset, 0f); // right side
                break;
        }

        return basePos + offset;
    }

    void MoveToCurrentTile()
    {
        transform.position = GetTilePosition(currentTileIndex);
    }
}