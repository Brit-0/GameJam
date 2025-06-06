using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TerrainLogic : MonoBehaviour
{
    public static TerrainLogic main;

    [SerializeField] private Camera terrainCam;
    [SerializeField] private Tilemap tm;

    [Header("TILES")]
    [SerializeField] private TileBase grassTile;
    [SerializeField] private List<TileBase> treesTiles;

    [Header("TORNADO")]
    [SerializeField] private GameObject tornadoPF;
    [SerializeField] private List<Transform> spawnLocations;
    public List<GameObject> activeTornadoes;
    public bool isTornadoing;
    public int tornadoCounter;
    public int numOfTornadoes = 1;
    public bool isWaiting;

    public static TileBase currentTile;
    private Vector3Int mouseTile, lastMouseTile;
    public static bool isDemolishing, isChopping;

    private void Awake()
    {
        main = this;
    }

    private void Update()
    {
        if (Input.GetMouseButton(0) && terrainCam.pixelRect.Contains(Input.mousePosition))
        {
            if (currentTile)
            {
                BuildMachine();
            }
            else if (isDemolishing)
            {
                DemolishTile();
            }
            else if (isChopping)
            {
                ChopTile();
            }
        }
    }

    private void BuildMachine()
    {
        Vector3Int clickedTilepos = tm.WorldToCell(terrainCam.ScreenToWorldPoint(Input.mousePosition));
        TileBase clickedTile = tm.GetTile(clickedTilepos);

        if (clickedTile != grassTile) return;

        tm.SetTile(clickedTilepos, currentTile);

        StartCoroutine(CompressCounter());

        Machine machine = (Machine)MachinesHandler.main.GetMachine(currentTile.name);

        if (machine == null)
        {
            machine = (Machine)MachinesHandler.main.GetHelper(currentTile.name);
        }

        machine.qnt++; //AUMENTA A CONTAGEM DESSA MÁQUINA

        HUDHandler.main.UpdateQuantities(machine);
        ResourcesHandler.co2LvlIncreaser += machine.co2Lvl; //ADICIONA A EMISSÃO DE CARBOONO

        currentTile = null;
    }

    private void DemolishTile()
    {
        Vector3Int clickedTilepos = tm.WorldToCell(terrainCam.ScreenToWorldPoint(Input.mousePosition));
        TileBase clickedTile = tm.GetTile(clickedTilepos);

        if (clickedTile == grassTile) return;

        tm.SetTile(clickedTilepos, grassTile);

        Machine machine = (Machine)MachinesHandler.main.GetMachine(clickedTile.name);

        if (machine != null)
        {
            machine.qnt--;
            HUDHandler.main.UpdateQuantities(machine);
            ResourcesHandler.co2LvlIncreaser -= machine.co2Lvl;
        }

        StartCoroutine(CompressCounter());

        isDemolishing = false;
    }

    private void ChopTile()
    {
        Vector3Int clickedTilepos = tm.WorldToCell(terrainCam.ScreenToWorldPoint(Input.mousePosition));
        TileBase clickedTile = tm.GetTile(clickedTilepos);

        if (!treesTiles.Contains(clickedTile)) return;
        
        tm.SetTile(clickedTilepos, grassTile);

        StartCoroutine(CompressCounter());

        isChopping = true;
    }

    private IEnumerator CompressCounter()
    {
        yield return new WaitForSeconds(1);

        Cursor.SetCursor(default, Vector2.zero, CursorMode.ForceSoftware);
        CameraController.main.CompressViewport();
    }

    public IEnumerator IncreaseTornadoCounter()
    {
        yield return new WaitForSeconds(1f);

        if (!isTornadoing)
        {
            isWaiting = false;
            tornadoCounter++;
            numOfTornadoes = (int)Mathf.Pow(2, tornadoCounter);
        }

    }

    [ContextMenu("TORNADO")]
    public void Tornado()
    {
        for (int i = 0; i < numOfTornadoes; i++)
        { 
            isTornadoing = true;

            if (tornadoCounter == 0)
            {
                Machine machine = (Machine)MachinesHandler.main.GetHelper("SmallReserve");
                StartCoroutine(HUDHandler.main.FlashFeedback("Reservas diminuem o nível de CO2!", 5f));
                machine.buyable.SetActive(true);
            }

            GameObject tornado = Instantiate(tornadoPF, spawnLocations[Random.Range(0, spawnLocations.Count)].position, Quaternion.identity);
            activeTornadoes.Add(tornado);

            List<Vector3> locationsToGo = new();

            while (locationsToGo.Count < 3)
            {
                int randomXIndex = Random.Range(tm.cellBounds.xMax , tm.cellBounds.xMin + 1);
                int randomYIndex = Random.Range(tm.cellBounds.yMax, tm.cellBounds.yMin + 1);

                for (int x = tm.cellBounds.xMin; x < tm.cellBounds.xMax; x++)
                {
                    for (int y = tm.cellBounds.yMin; y < tm.cellBounds.yMax; y++)
                    {
                        Vector3Int localLocation = new Vector3Int(
                            x: x,
                            y: y,
                            z: 0
                            );

                        Vector3 location = tm.CellToWorld(localLocation);

                        if (x == randomXIndex && y == randomYIndex)
                        {
                            locationsToGo.Add(location);
                        }
                    }
                }
                
                tornado.GetComponent<TornadoLogic>().locationsToGo = locationsToGo;
                tornado.GetComponent<TornadoLogic>().tm = tm;
            }
        }
    }

    public IEnumerator RestoreTiles(List<Vector3Int> cellsToRestore)
    {
        yield return new WaitForSeconds(5f);

        foreach (Vector3Int cell in cellsToRestore)
        {
            tm.SetTile(cell, grassTile);
        }
    }

}
