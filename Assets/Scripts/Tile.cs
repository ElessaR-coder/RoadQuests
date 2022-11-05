using System.Threading;
using UnityEngine;

public class Tile : MonoBehaviour
{

    [SerializeField] private Transform _point;
    [SerializeField] public SpriteRenderer spriteRenderer;
    public bool isSelected;
    public bool isWall = false;
    public bool isEmpty
    {
        get
        {
            return spriteRenderer.sprite == null ? true : false;
        }
    }


}
