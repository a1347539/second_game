using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapBuilder : Singleton<MapBuilder>
{
    private const int charStartAt = 48;
    public const int MAP_WIDTH = 18;
    public const int MAP_HEIGHT = 10;
    public Vector3 origin { get; private set; }
    private Camera cam;
    public MapData mapData { get; private set; }
    private string[] charMap;
    private Tiles[] tilePrefabs;
    public Dictionary<Tiles, Point> tiles { get; private set; }
    [SerializeField] private Transform tileContainer;
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
        tilePrefabs = Resources.LoadAll<Tiles>("Prefabs/Tilesets/Tileset"+((int)mapData.tileset-charStartAt));
        cam = Camera.main;
        origin = cam.ScreenToWorldPoint(new Vector3(0, Screen.height));
        origin = new Vector3(Mathf.RoundToInt(origin.x), Mathf.RoundToInt(origin.y), origin.z);
    }

    // Start is called before the first frame update
    void Start() {
        tiles = new Dictionary<Tiles, Point>();
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

        Point p = new Point(x, y);
        newTile.setup(p, new Vector3(origin.x + tileWidth * (x + 0.5f), origin.y - tileHeight * (y + 0.5f), 0), tileContainer);
        tiles.Add(newTile, p);
    }
}
