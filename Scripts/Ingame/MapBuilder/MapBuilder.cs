using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapBuilder : MonoBehaviour
{
    private const int charStartAt = 97;
    public const int MAP_WIDTH = 18;
    public const int MAP_HEIGHT = 10;
    private Vector3 origin;
    private Camera cam;
    private MapData mapData;
    private string[] charMap;
    private Tiles[] tilePrefabs;
    private Dictionary<Tiles, Point> Tiles;
    public float tileWidth
    {
        get { return tilePrefabs[0].GetComponent<SpriteRenderer>().sprite.bounds.size.x; }
    }
    public float tileHeight
    {
        get { return tilePrefabs[0].GetComponent<SpriteRenderer>().sprite.bounds.size.y; }
    }

    private void Awake() {
        mapData = ResourcesLoader.Load<MapData>("Maps", "Map1001");
        tilePrefabs = Resources.LoadAll<Tiles>("Tilesets/Tileset"+((int)mapData.tileset-charStartAt));
    }

    // Start is called before the first frame update
    void Start() {
        cam = Camera.main;
        origin = cam.ScreenToWorldPoint(new Vector3(0, Screen.height));
        Tiles = new Dictionary<Tiles, Point>();
        charMap = mapData.map;

        createLevel();
    }

    private void createLevel() {
        for (int y = 0; y < MAP_HEIGHT; y++) {
            char[] row = charMap[y].ToCharArray();
            for (int x = 0; x < MAP_WIDTH; x++)
            {
                placeTiles((int)row[x]-charStartAt, x, y);
            }
        }
    }

    private void placeTiles(int tileIndex, int x, int y) {
        Tiles newTile = Instantiate(tilePrefabs[tileIndex]).GetComponent<Tiles>();
        // newTile.setup(new Point(x, y), new Vector3(origin.x + tileWidth * x, origin.y - tileHeight * y, 0), map);

    }

}
