using UnityEngine;
using System.Collections;

public class SlideManager : MonoBehaviour
{
    public BoardGenerator boardGenerator;
    public GameObject slidePrefab;
    public float yOffset = 0f;

    [System.Serializable]
    public struct SlideTilePair
    {
        public int headTile;
        public int tailTile;
    }

    public SlideTilePair[] slides = new SlideTilePair[8];

    void Start()
    {
        if (boardGenerator.tiles == null || boardGenerator.tiles.Length == 0)
            StartCoroutine(WaitAndSpawn());
        else
            SpawnSlides();
    }

    IEnumerator WaitAndSpawn()
    {
        yield return null;
        if (boardGenerator.tiles != null && boardGenerator.tiles.Length > 0)
            SpawnSlides();
        else
            Debug.LogError("Board tiles still not ready!");
    }

    void SpawnSlides()
    {
        foreach (var pair in slides)
        {
            if (pair.headTile < 1 || pair.headTile > boardGenerator.tiles.Length ||
                pair.tailTile < 1 || pair.tailTile > boardGenerator.tiles.Length)
            {
                Debug.LogWarning("Slide tile index out of range!");
                continue;
            }

            Tile head = boardGenerator.tiles[pair.headTile - 1];
            Tile tail = boardGenerator.tiles[pair.tailTile - 1];

            Vector3 headPos = head.transform.position + Vector3.up * yOffset;
            Vector3 tailPos = tail.transform.position + Vector3.up * yOffset;

            // Instantiate at midpoint
            Vector3 midpoint = (headPos + tailPos) / 2;
            GameObject slideObj = Instantiate(slidePrefab, midpoint, Quaternion.identity);

            // Rotate to point toward tail
            slideObj.transform.LookAt(tailPos);

            // Hardcode scale
            slideObj.transform.localScale = new Vector3(0.01f, -0.05f, 0.5f);
        }
    }
}