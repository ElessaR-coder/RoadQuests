using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using Unity.Burst.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class BoardController : MonoBehaviour
{
    public static BoardController instance;

    private int xSize, ySize;
    private List<Sprite> tileSprite = new List<Sprite>();
    private Tile[,] tileArray;
    private Vector2[] dirRay = new Vector2[] { Vector2.up, Vector2.down, Vector2.left, Vector2.right };
    private Tile firstSelectTile;
    private readonly Point[] winItemsCoordinates = new Point[3] { new Point(0, 6), new Point(2, 6), new Point(4, 6) };


    public void SetValue(Tile[,] tileArray, int xSize, int ySize, List<Sprite> tileSprite)
    {
        this.tileArray = tileArray;
        this.xSize = xSize;
        this.ySize = ySize;
        this.tileSprite = tileSprite;
    }

    private void Awake()
    {
        instance = this;
    }

    private void Update()
    {

        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit2D hit = Physics2D.GetRayIntersection(Camera.main.ScreenPointToRay(Input.mousePosition));
            if (hit != false)
                CheckSelectTile(hit.collider.gameObject.GetComponent<Tile>());

        }
    }

    private void SwapTileAndEmpty(Tile tile)
    {
        if (firstSelectTile.spriteRenderer.sprite == tile.spriteRenderer.sprite)
        {
            return;
        }
        Sprite cashSprite = firstSelectTile.spriteRenderer.sprite;
        firstSelectTile.spriteRenderer.sprite = tile.spriteRenderer.sprite;
        tile.spriteRenderer.sprite = cashSprite;
    }

    private List<Tile> SearchEmptyTile()
    {
        List<Tile> cashEmptyTile = new List<Tile>();
        for (int i = 0; i < dirRay.Length; i++)
        {
            RaycastHit2D hit = Physics2D.Raycast(firstSelectTile.transform.position, dirRay[i]);
            if (hit.collider != null && !hit.collider.gameObject.GetComponent<Tile>().isWall && hit.collider.gameObject.GetComponent<Tile>().spriteRenderer.sprite == null)
            {
                cashEmptyTile.Add(hit.collider.gameObject.GetComponent<Tile>());
            }
        }
        return cashEmptyTile;
    }

    private void SelectTile(Tile tile)
    {
        if (!tile.isWall)
        {
            tile.isSelected = true;
            tile.spriteRenderer.color = new UnityEngine.Color(0.5f, 0.5f, 0.5f);
            firstSelectTile = tile;
        }

    }

    private void DeselectTile(Tile tile)
    {
        tile.isSelected = false;
        tile.spriteRenderer.color = new UnityEngine.Color(1, 1, 1);
        firstSelectTile = null;
    }

    private void CheckSelectTile(Tile tile)
    {
        try
        {
            if (tile.isEmpty)
            {
                if (firstSelectTile == null)
                    return;
                else
                {
                    if (SearchEmptyTile().Contains(tile))
                    {
                        SwapTileAndEmpty(tile);
                        DeselectTile(firstSelectTile);
                        if (isWin())
                        {
                            SceneManager.LoadScene(1);
                        }
                    }
                }
            }
            else if (tile.isSelected)
            {
                DeselectTile(tile);
            }
            else
            {
                if (!tile.isSelected && firstSelectTile == null)
                {
                    SelectTile(tile);
                    var emptyTile = SearchEmptyTile();
                    if (emptyTile.Count == 1)
                    {
                        SwapTileAndEmpty(emptyTile[0]);
                        DeselectTile(tile);
                        if (isWin())
                        {
                            SceneManager.LoadScene(1);
                        }
                    }
                }
                else
                {
                    DeselectTile(firstSelectTile);
                    firstSelectTile = tile;
                    SelectTile(tile);
                }
            }
        }
        catch { }

    }

    private bool isWin()
    {
        List<Tile> cashFindTiles = new List<Tile>();
        for (int i = 0; i < winItemsCoordinates.Length; i++)
        {
            Point winItem = winItemsCoordinates[i];

            RaycastHit2D hit = Physics2D.Raycast(tileArray[winItem.X, 6].transform.position, Vector2.down);
            hit = Physics2D.Raycast(hit.collider.gameObject.transform.position, Vector2.down);

            while (hit.collider != null
            && hit.collider.gameObject.GetComponent<Tile>().spriteRenderer.sprite == tileArray[winItemsCoordinates[i].X, winItemsCoordinates[i].Y].spriteRenderer.sprite)
            {
                cashFindTiles.Add(hit.collider.gameObject.GetComponent<Tile>());
                hit = Physics2D.Raycast(hit.collider.gameObject.transform.position, Vector2.down);
            }
        }
        if (cashFindTiles.Count == 15)
            return true;
        else return false;
    }

}