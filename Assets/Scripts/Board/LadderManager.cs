using UnityEngine;
using System.Collections;
using PropMaker;  // Needed to access the Ladder class

public class LadderManager : MonoBehaviour
{
    public BoardGenerator boardGenerator;
    public GameObject ladderPrefab;

    [System.Serializable]
    public struct LadderTilePair
    {
        public int startTile;
        public int endTile;
    }

    public LadderTilePair[] ladders = new LadderTilePair[8];

    // Replace the old Start() with this
    void Start()
    {
        // Wait until board tiles are ready
        if (boardGenerator.tiles == null || boardGenerator.tiles.Length == 0)
        {
            Debug.Log("Board not ready yet, delaying ladder spawn...");
            StartCoroutine(WaitAndSpawn());
        }
        else
        {
            SpawnLadders();
        }
    }

    IEnumerator WaitAndSpawn()
    {
        // Wait one frame
        yield return null;

        if (boardGenerator.tiles != null && boardGenerator.tiles.Length > 0)
        {
            SpawnLadders();
        }
        else
        {
            Debug.LogError("Board tiles still not ready!");
        }
    }

    void SpawnLadders()
    {
        foreach (var pair in ladders)
        {
            if (pair.startTile < 1 || pair.startTile > boardGenerator.tiles.Length ||
                pair.endTile < 1 || pair.endTile > boardGenerator.tiles.Length)
            {
                Debug.LogWarning("Tile index out of range!");
                continue;
            }

            Tile start = boardGenerator.tiles[pair.startTile - 1];
            Tile end = boardGenerator.tiles[pair.endTile - 1];

            Vector3 startPos = start.transform.position + Vector3.up * 0.2f;
            Vector3 endPos = end.transform.position + Vector3.up * 0.2f;

            GameObject ladderObj = Instantiate(ladderPrefab, Vector3.zero, Quaternion.identity);
            Ladder ladder = ladderObj.GetComponent<Ladder>();
            ladder.start = startPos;
            ladder.end = endPos;
            ladder.UpdateProp();
        }
    }
}
