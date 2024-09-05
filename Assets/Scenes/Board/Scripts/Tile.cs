using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    public enum TileType { Active, Ghost, Locked, Empty }
    public TileType tileType = TileType.Empty;
    [SerializeField] private TileDataScriptableObject tileTypes;
    private TileData tileData;
    private SpriteRenderer spriteRenderer;
    private Color ghostColor = new(1, 1, 1, 0.25f);

    public void Init()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        tileData = tileTypes.Empty;
    }

    public void SetTileData(TileData data)
    {
        spriteRenderer.sprite = data.sprite;
        tileData = data;
    }

    public TileData GetTileData()
    {
        return tileData;
    }

    public void SetTileType(TileType tileType)
    {
        this.tileType = tileType;
        switch (tileType)
        {
            case TileType.Active:
            case TileType.Empty:
            case TileType.Locked:
                spriteRenderer.color = Color.white;
                break;
            case TileType.Ghost:
                spriteRenderer.color = ghostColor;
                break;
        }
    }

    public TileType GetTileType()
    {
        return tileType;
    }
}
