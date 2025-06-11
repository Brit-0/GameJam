using System.Collections;
using System.Collections.Generic;
using System.Xml.Schema;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TerrainLogic : MonoBehaviour
{
    public static TerrainLogic main;

    public Tilemap tm;
    [SerializeField] private Camera terrainCam;

    [Header("TILES")]
    [SerializeField] private TileBase grassTile;
    [SerializeField] private List<TileBase> treesTiles;
    [SerializeField] private TileBase explosionTile;
    [SerializeField] private TileBase smallReserveTile;
    [SerializeField] private TileBase[] grandReserveTiles, grandMetalIndustryTiles, grandPowerPlant;

    [Header("TORNADO")]
    [SerializeField] private GameObject tornadoPF;
    [SerializeField] private List<Transform> spawnLocations;
    public List<GameObject> activeTornadoes;
    public bool isTornadoing;
    public int tornadoCounter;
    public int numOfTornadoes = 1;

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

    #region BUYS

    private void BuildMachine()
    {
        Vector3Int clickedTilepos = tm.WorldToCell(terrainCam.ScreenToWorldPoint(Input.mousePosition));
        TileBase clickedTile = tm.GetTile(clickedTilepos);

        if (clickedTile != grassTile) return;

        tm.SetTile(clickedTilepos, currentTile);

        AudioManager.PlayAudio(AudioManager.main.construction);

        StartCoroutine(CompressCounter());

        Machine machine = (Machine)MachinesHandler.main.GetMachine(currentTile.name);
        machine ??= (Machine)MachinesHandler.main.GetHelper(currentTile.name);

        if (machine.isCombinable)
        {
            CheckCombine(clickedTilepos, machine);
        }

        machine.qnt++; //AUMENTA A CONTAGEM DESSA MÁQUINA

        HUDHandler.main.UpdateQuantities(machine);
        ResourcesHandler.co2Lvl += machine.co2Lvl; //ADICIONA A EMISSÃO DE CARBOONO

        currentTile = null;
    }

    private void DemolishTile()
    {
        Vector3Int clickedTilepos = tm.WorldToCell(terrainCam.ScreenToWorldPoint(Input.mousePosition));
        TileBase clickedTile = tm.GetTile(clickedTilepos);

        if (clickedTile == grassTile) return;

        AudioManager.PlayAudio(AudioManager.main.explosion, .5f);

        Machine machine = OnMachineDestroy(clickedTile.name);

        if (machine != null && machine.isBigMachine)
        {
            StartCoroutine(BigExplosion(GetMachineBounds(clickedTilepos)));
        }
        else
        { 
            StartCoroutine(Explosion(clickedTilepos));
        }

        StartCoroutine(CompressCounter());

        isDemolishing = false;
    }

    private IEnumerator Explosion(Vector3Int cell)
    {
        tm.SetTile(cell, explosionTile);

        yield return new WaitForSeconds(.5f);

        tm.SetTile(cell, grassTile);
    }

    private IEnumerator BigExplosion(BoundsInt bounds)
    {
        tm.SetTilesBlock(bounds, new TileBase[] { explosionTile, explosionTile, explosionTile, explosionTile });

        yield return new WaitForSeconds(.5f);

        tm.SetTilesBlock(bounds, new TileBase[] { grassTile, grassTile, grassTile, grassTile });
    }

    private void ChopTile()
    {
        if (!tm.ContainsTile(treesTiles[0]) && tm.ContainsTile(treesTiles[1]) && tm.ContainsTile(treesTiles[2]))
        {
            StartCoroutine(HUDHandler.main.FlashFeedback(CapitalistDialog.SelectDialog(CapitalistDialog.chopWithoutTrees)));
        }

        Vector3Int clickedTilepos = tm.WorldToCell(terrainCam.ScreenToWorldPoint(Input.mousePosition));
        TileBase clickedTile = tm.GetTile(clickedTilepos);

        if (!treesTiles.Contains(clickedTile)) return;

        AudioManager.PlayAudio(AudioManager.main.chop);
        tm.SetTile(clickedTilepos, grassTile);

        StartCoroutine(CompressCounter());

        isChopping = false;
    }

    #endregion

    #region TORNADO

    public IEnumerator IncreaseTornadoCounter()
    {
        yield return new WaitForSeconds(1f);

        if (!isTornadoing)
        {
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

            if (!AudioManager.tornadoSource.isPlaying)
            {
                AudioManager.main.PlayTornadoAudio(AudioManager.main.tornado, AudioManager.main.siren);
            }

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

    public IEnumerator RestoreTiles(List<Vector3Int> cellsToRestore, bool hasExtraCells = default, BoundsInt extraCells = default)
    {
        yield return new WaitForSeconds(5f);

        foreach (Vector3Int cell in cellsToRestore)
        {
            tm.SetTile(cell, grassTile);
        }

        if (hasExtraCells)
        {
            tm.SetTilesBlock(extraCells, new TileBase[] { grassTile, grassTile, grassTile, grassTile });
        }
    }

    #endregion

    #region BIG MACHINES

    private void CheckCombine(Vector3Int cell, Machine machine)
    {

        Vector3Int[] directionsLeftRight = new Vector3Int[2]
        {
           Vector3Int.right,
           Vector3Int.left,
        };

        Vector3Int[] directionsUpDown = new Vector3Int[2]
        {
           Vector3Int.up,
           Vector3Int.down,
        };

        for (int i = 0; i < 2; i++)
        {
            for (int k = 0; k < 2; k++)
            {
                if (tm.GetTile(cell + directionsLeftRight[i]) == machine.tile)
                {
                    if (tm.GetTile(cell + directionsUpDown[k]) == machine.tile)
                    {
                        if (tm.GetTile(cell + directionsLeftRight[i] + directionsUpDown[k]) == machine.tile)
                        {
                            int xMin = Mathf.Min(cell.x, (cell + directionsLeftRight[i]).x);
                            int yMin = Mathf.Min(cell.y, (cell + directionsUpDown[k]).y);

                            BoundsInt bounds = new BoundsInt(xMin, yMin, cell.z, 2, 2, 1);
                            TileBase[] tiles = new TileBase[4];
                            Machine newMachine = new();

                            if (machine.name == "SmallReserve")
                            {
                                tiles = grandReserveTiles;
                                newMachine = MachinesHandler.main.grandReserve;
                            }
                            else if (machine.name == "SmallMetalIndustry")
                            {
                                tiles = grandMetalIndustryTiles;
                                newMachine = MachinesHandler.main.grandMetalIndustry;

                                if (newMachine.qnt == 0)
                                {
                                    MachinesHandler.main.StartCoroutine(MachinesHandler.main.GrandMetalIndustryCoroutine());
                                }
                            }
                            else if (machine.name == "ThermalPowerPlant")
                            {
                                tiles = grandPowerPlant;
                                newMachine = MachinesHandler.main.grandPowerPlant;

                                if (newMachine.qnt == 0)
                                {
                                    MachinesHandler.main.StartCoroutine(MachinesHandler.main.GrandPowerPlantCoroutine());
                                }
                            }

                            tm.SetTilesBlock(bounds, tiles);

                            machine.qnt -= 4;
                            HUDHandler.main.UpdateQuantities(machine);

                            newMachine.qnt++;
                            HUDHandler.main.UpdateQuantities(newMachine);

                            if (newMachine.qnt == 1)
                            {
                                HUDHandler.main.UnlockPage();
                                HUDHandler.main.FirstPurchaseUpdate(newMachine);
                            }

                            ResourcesHandler.co2Lvl -= machine.co2Lvl * 4;
                            ResourcesHandler.co2Lvl += newMachine.co2Lvl;
                        }
                    }
                }
            }  
        }
    }

    public BoundsInt GetMachineBounds(Vector3Int cell)
    {
        TileBase tile = tm.GetTile(cell);

        Machine machine = (Machine)MachinesHandler.main.GetMachine(tile.name);
        machine ??= (Machine)MachinesHandler.main.GetHelper(tile.name);

        int xMin = 0, yMin = 0;

        if (tile.name == machine.name + "DL")
        {
            xMin = cell.x;
            yMin = cell.y;
        }
        else if (tile.name == machine.name + "DR")
        {
            xMin = (cell + Vector3Int.left).x;
            yMin = (cell + Vector3Int.left).y;
        }
        else if (tile.name == machine.name + "UL")
        {
            xMin = (cell + Vector3Int.down).x;
            yMin = (cell + Vector3Int.down).y;
        }
        else if (tile.name == machine.name + "UR")
        {
            xMin = (cell + Vector3Int.down + Vector3Int.left).x;
            yMin = (cell + Vector3Int.down + Vector3Int.left).y;
        }

        BoundsInt bounds = new BoundsInt(xMin, yMin, cell.z, 2, 2, 1);

        return bounds; 
    }

    #endregion

    public Machine OnMachineDestroy(string machineName)
    {
        Machine machine = (Machine)MachinesHandler.main.GetMachine(machineName);
        machine ??= (Machine)MachinesHandler.main.GetHelper(machineName);

        if (machine != null)
        {
            machine.qnt--;
            HUDHandler.main.UpdateQuantities(machine);
            ResourcesHandler.co2Lvl -= machine.co2Lvl;
        }

        return machine;
    }
    private IEnumerator CompressCounter()
    {
        yield return new WaitForSeconds(1.5f);

        Cursor.SetCursor(default, Vector2.zero, CursorMode.ForceSoftware);
        CameraController.main.CompressViewport();
    }
}
