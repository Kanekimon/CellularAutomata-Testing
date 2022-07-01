using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Tilemaps;

public class CellularAutomateEngine : MonoBehaviour
{
    private int[] xMask = { 0, 1, 1, 1, 0, -1, -1, -1 }; //xMask and yMask are used to simplify the neighbourhood iteration 
    private int[] yMask = { 1, 1, 0, -1, -1, -1, 0, 1 }; // 

    //Height of the map
    public int Height; 
    //Width of the Map
    public int Width;

    //Execute Tick function with update
    public bool AutoUpdate;

    //boolean map of alive/dead cells ( true = alive, false = dead)
    public bool[,] Map;
    //Timer for setting the delay between updates
    public float Timer;
    //The specified delay
    public float Delay;

    //TileMap for visualization
    public Tilemap TileMap;
    //Tile for dead cells
    public Tile Dead;
    //Tile for alive cells
    public Tile Alive;

    //For random intial spawn of cells
    [Range(0f, 1f)]
    public float SpawnProbability;

    // Start is called before the first frame update
    void Start()
    {
        GenerateGrid();
    }

    // Update is called once per frame
    void Update()
    {
        if (Timer > Delay)
        {
            Timer = 0;
            if (AutoUpdate)
                Tick();
        }
        Timer += Time.deltaTime;

    }


    /// <summary>
    /// Initializes a new TileMap with given Width and Height
    /// Creates bool map for 1 = alive, 0 = dead randomly
    /// </summary>
    public void GenerateGrid()
    {
        TileMap.size = new Vector3Int(Width, Height, 1);

        Map = new bool[Height, Width];

        for (int x = 0; x < Width; x++)
        {
            for (int y = 0; y < Height; y++)
            {
                Map[x, y] = SpawnProbability > UnityEngine.Random.Range(0f, 1f);
            }
        }
        DrawTileMap();
    }

    /// <summary>
    /// Apply rules to cells
    /// </summary>
    public void Tick()
    {
        bool[,] tempMap = new bool[Width, Height];
        Parallel.For(0, Width, x =>
        {
            Parallel.For(0, Height, y =>
            {
                tempMap[x, y] = HandleRules(x, y);
            });

        });


        Map = tempMap;
        DrawTileMap();
    }

    /// <summary>
    /// Draw TileMap from bool array
    /// Updates the tile only if the state changed
    /// </summary>
    void DrawTileMap()
    { 
        for (int x = 0; x < Width; x++)
        {
            for (int y = 0; y < Height; y++)
            {
                Tile t = Map[x, y] ? Alive : Dead;
                if(TileMap.GetTile(new Vector3Int(x,y,1)) != t) 
                    TileMap.SetTile(new Vector3Int(x, y, 1), t);
            }
        }
    }

    /// <summary>
    /// Handles all the rules
    /// </summary>
    /// <param name="x">X-Position of cell</param>
    /// <param name="y">Y-Position of cell</param>
    /// <returns>True if all rules are sufficied</returns>
    bool HandleRules(int x, int y)
    {
        bool alive = true;
        if (Map[x, y])
            alive = !Underpopulated(x, y, 2) && GoodNeighbourCount(x, y, 2, 3) && !Overpopulated(x, y, 3);
        else
        {
            alive = GetNeighbourCount(x, y) == 3;
        }

        return alive;
    }

    /// <summary>
    /// Checks if the neighbourhood is underpopulated
    /// Defined as there are less alive cells in the neighbourhood than specified
    /// by the threshold
    /// </summary>
    /// <param name="x">X-Position of current cell</param>
    /// <param name="y">Y-Position of current cell</param>
    /// <param name="threshold">At which value does it count as underpopulated</param>
    /// <returns>True if underpopulated</returns>
    bool Underpopulated(int x, int y, int threshold)
    {
        return GetNeighbourCount(x, y) < threshold;
    }

    /// <summary>
    /// Checks if the neighbourhood is overpopulated
    /// Defined as there are more alive cells in the neighbourhood than specified
    /// by the threshold
    /// </summary>
    /// <param name="x">X-Position of current cell</param>
    /// <param name="y">Y-Position of current cell</param>
    /// <param name="threshold">At which value does it count as overpopulated</param>
    /// <returns>True if overpopulated</returns>
    bool Overpopulated(int x, int y, int threshold)
    {
        return GetNeighbourCount(x, y) > threshold;
    }

    /// <summary>
    /// Checks if the amount of alive neighbours is within min,max bounds
    /// </summary>
    /// <param name="x">X-Position of current cell</param>
    /// <param name="y">Y-Position of current cell</param>
    /// <param name="min">Minimum alive cells</param>
    /// <param name="max">Maximum alive cells</param>
    /// <returns>True if count of alive neighbours are in bounds</returns>
    bool GoodNeighbourCount(int x, int y, int min, int max)
    {
        int nCount = GetNeighbourCount(x, y);

        return nCount >= min && nCount <= max;
    }

    /// <summary>
    /// Returns the count of alive neighbours
    /// </summary>
    /// <param name="x">X-Position of current cell</param>
    /// <param name="y">Y-Position of current cell</param>
    /// <returns>Amount of alive neighbours</returns>
    int GetNeighbourCount(int x, int y)
    {
        int neighbourCount = 0;

        for (int i = 0; i < xMask.Length; i++)
        {
            int newX = x + xMask[i];
            int newY = y + yMask[i];

            if (newX < 0 || newY < 0 || newX >= Width || newY >= Height)
                continue;

            if (Map[newX, newY])
                neighbourCount++;
        }
        return neighbourCount;
    }

}
