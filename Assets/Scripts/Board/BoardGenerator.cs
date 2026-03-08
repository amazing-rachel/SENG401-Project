using UnityEngine;

public class BoardGenerator : MonoBehaviour
{
    [Header("Board Settings")]
    public int boardSize = 10;
    public float tileSpacing = 1.1f;
    public GameObject tilePrefab;

    [HideInInspector]
    public Tile[] tiles;

    void Start()
    {
        GenerateBoard();
    }

    void GenerateBoard()
    {
        tiles = new Tile[boardSize * boardSize];
        int tileNumber = 1;

        for (int row = 0; row < boardSize; row++)
        {
            bool reverse = row % 2 == 1;

            for (int col = 0; col < boardSize; col++)
            {
                int x = reverse ? (boardSize - 1 - col) : col;

                Vector3 position = new Vector3(
                    x * tileSpacing,
                    0,
                    row * tileSpacing
                );

                GameObject tileObj = Instantiate(tilePrefab, position, Quaternion.identity, transform);
                tileObj.name = $"Tile_{tileNumber}";

                Tile tile = tileObj.GetComponent<Tile>();
                tile.Initialize(tileNumber);

                tiles[tileNumber - 1] = tile;
                tileNumber++;
            }
        }
    }
}
