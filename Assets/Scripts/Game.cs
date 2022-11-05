using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[System.Serializable]
public class BoardSetting
{
    public int xSize, ySize;
    public Tile tileGO;
    public List<Sprite> tileSprite;
    public List<Sprite> wallSprite;
}

public class Game : MonoBehaviour
{
    public BoardSetting boardSetting;

    private void Start()
    {
        BoardController.instance.SetValue(Board.instance.SetValue(boardSetting.xSize, boardSetting.ySize, boardSetting.tileGO, boardSetting.tileSprite, boardSetting.wallSprite),
            boardSetting.xSize, boardSetting.ySize, boardSetting.tileSprite);
    }


}