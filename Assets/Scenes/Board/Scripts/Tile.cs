using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    public enum TileType { Active, Ghost, Locked, Empty }
    private TileType type = TileType.Empty;
    [SerializeField] private TileDataScriptableObject tileTypes;
    public TileData tileData;
    private SpriteRenderer spriteRenderer;
    private Color ghostColor = new(1, 1, 1, 0.5f);

    // Start is called before the first frame update
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void SetTileData(TileData data)
    {
        spriteRenderer.sprite = data.sprite;
        tileData = data;
    }

    public void SetTileType(TileType type)
    {
        this.type = type;
        switch (type)
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
        return type;
    }
}
