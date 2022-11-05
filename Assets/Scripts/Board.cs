using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using Unity.VisualScripting;
using UnityEngine;

public class Board : MonoBehaviour
{
    public static Board instance;

    private int xSize, ySize;
    private Tile tileGO;

    private List<Sprite> tileSprite = new List<Sprite>();
    private List<Sprite> wallSprite = new List<Sprite>();

    private readonly Point[] _wallCoordinates = new Point[6] { new Point(1,0), new Point(1, 2), new Point(1, 4), new Point(3, 0), new Point(3, 2),
        new Point(3, 4)};
    private readonly Point[] _emptyCoordinates = new Point[4] { new Point(1,1), new Point(1, 3), new Point(3, 1), new Point(3, 3)};


    private readonly Point[] _winItemsCoordinates = new Point[3] { new Point(0, 6), new Point(2, 6), new Point(4, 6) };

    private void Awake()
    {
        instance = this;
    }

    public Tile[,] SetValue(int xSize, int ySize, Tile tileGO, List<Sprite> tileSprite, List<Sprite> wallSprite)
    {
        this.xSize = xSize;
        this.ySize = ySize;
        this.tileGO = tileGO;
        this.tileSprite = tileSprite;
        this.wallSprite = wallSprite;

        return CreateBoard();
    }

    private Tile[,] CreateBoard()
    {
        Tile[,] tileArray = new Tile[xSize, ySize + 2];
        float xPos = transform.position.x;
        float yPos = transform.position.y;
        Vector2 tileSize = tileGO.spriteRenderer.bounds.size * 2;
        int wallIndex = 0;
        int emptyIndex = 0;
        int spriteNumber = 0;
        int[] spriteCounter = new int[3] { 0, 0, 0 };
        List<Sprite> winSprites = new List<Sprite>();

        for (int i = 0; i < 3; i++)
        {
            spriteNumber = Random.Range(0, tileSprite.Count - 1);
            winSprites.Add(tileSprite[spriteNumber]);
            tileSprite.RemoveAt(spriteNumber);
        }



        for (int x = 0; x < xSize+1; x++)
        {
            for (int y = 0; y < ySize; y++)
            {
                if (x == 0 || x == 4 || y == 0 || y == 4)
                {
                    var direction = CreateWall(x,y);

                    foreach(var point in direction)
                    {
                        Tile wall = Instantiate(tileGO);
                        wall.transform.position = new Vector3(xPos + (tileSize.x * x) + (tileSize.x * point.X), yPos + (tileSize.y * y) + (tileSize.x * point.Y), 0f);
                        wall.spriteRenderer.sprite = null;
                        wall.transform.SetParent(transform, false);
                        wall.isWall = true;
                    }

                }
                if (wallIndex < _wallCoordinates.Length && _wallCoordinates[wallIndex].X == x && _wallCoordinates[wallIndex].Y == y)
                {
                    Tile wall = Instantiate(tileGO, transform.position, Quaternion.identity);
                    wall.transform.SetParent(transform, false);
                    wall.transform.position = new Vector3(xPos + (tileSize.x * (x)), yPos + (tileSize.y * y), 0f);
                    wall.isWall = true;
                    wall.spriteRenderer.sprite = wallSprite[0];
                    wallIndex++;
                }
                else if (x % 2 == 0)
                {
                    Tile tile = Instantiate(tileGO, transform.position, Quaternion.identity);
                    tile.transform.SetParent(transform, false);
                    tile.transform.position = new Vector3(xPos + (tileSize.x * (x)), yPos + (tileSize.y * y), 0f);

                    while (true)
                    {
                        spriteNumber = Random.Range(0, 3);
                        if (spriteCounter[spriteNumber] < 5 || IsFilling(spriteCounter))
                        {
                            break;
                        }
                    }

                    tile.spriteRenderer.sprite = winSprites[spriteNumber];
                    spriteCounter[spriteNumber]++;
                    tileArray[x, y] = tile;
                }
                else if (emptyIndex < _emptyCoordinates.Length && _emptyCoordinates[emptyIndex].X == x && _emptyCoordinates[emptyIndex].Y == y)
                {
                    Tile empty = Instantiate(tileGO, transform.position, Quaternion.identity);
                    empty.transform.SetParent(transform, false);
                    empty.transform.position = new Vector3(xPos + (tileSize.x * (x)), yPos + (tileSize.y * y), 0f);
                    empty.spriteRenderer.sprite = null;
                    emptyIndex++;
                }
            }
        }

        List<Sprite> tileSpriteCopy = winSprites;
        for (var i = 0; i < _winItemsCoordinates.Length; i++)
        {

            Tile winItem = Instantiate(tileGO);
            winItem.transform.SetParent(transform, false);
            winItem.transform.position = new Vector3(xPos + (tileSize.x * _winItemsCoordinates[i].X), yPos + (tileSize.y * _winItemsCoordinates[i].Y), 0f);
            winItem.isWall = true;
            spriteNumber = Random.Range(0, tileSpriteCopy.Count);
            winItem.spriteRenderer.sprite = tileSpriteCopy[spriteNumber];
            tileSpriteCopy.RemoveAt(spriteNumber);
            tileArray[_winItemsCoordinates[i].X, _winItemsCoordinates[i].Y] = winItem;

        }

        return tileArray;

    }

    private bool IsFilling(int[] array)
    {
        var n = 0;
        foreach (int sprite in array)
            if (sprite == 5)
                n++;
        if (n == 3)
            return true;
        else return false;
    }

    private List <Point> CreateWall(int x, int y)
    {
        List <Point> WallCoordinates = new List <Point>();
        if (x == 0)
        {
            if (y== 0)
            {
                WallCoordinates.Add(new Point(0, -1));
                WallCoordinates.Add(new Point(-1, 0));
            }
            else
                WallCoordinates.Add(new Point(-1, 0));
        }

        if (y == 0)
        {
            if (x == 4)
            {
                WallCoordinates.Add(new Point(0, -1));
            }
            else if (x>0)
                WallCoordinates.Add(new Point(0, -1));
        }

        if (x == 4)
        {
            WallCoordinates.Add(new Point(1, 0));
        }

        if (y == 4)
        {
            WallCoordinates.Add(new Point(0, 1));
        }

        return WallCoordinates;
    }

}