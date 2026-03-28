using UnityEngine;
using TMPro;

public class Tile : MonoBehaviour
{
    public int tileIndex;
    public TextMeshPro numberText;

    public void Initialize(int index)
    {
        tileIndex = index;
        numberText.text = index.ToString();
    }
}
