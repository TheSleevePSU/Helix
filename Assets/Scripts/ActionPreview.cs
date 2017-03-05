using UnityEngine;
using System.Collections;

public class ActionPreview : MonoBehaviour
{
    public enum PreviewSquare
    {
        none,
        move,
        spawn
    };
    public PreviewSquare[,] previewSquares;

    void Start()
    {
        previewSquares = new PreviewSquare[7, 4];
    }
}
