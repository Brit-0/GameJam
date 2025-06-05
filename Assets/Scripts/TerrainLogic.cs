using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;
using static UnityEditor.FilePathAttribute;

public class TerrainLogic : MonoBehaviour
{

    [SerializeField] private Camera terrainCam;
    [SerializeField] private Tilemap tm;

    [Header("TILES")]
    [SerializeField] private TileBase grassTile;

    [Header("TORNADO")]
    [SerializeField] private GameObject tornadoPF;
    [SerializeField] private List<Transform> spawnLocations;

    public static TileBase currentTile;
    private Vector3Int mouseTile, lastMouseTile;
    public static bool isDestroying;

    private void Update()
    {

        if (Input.GetMouseButton(0) && terrainCam.pixelRect.Contains(Input.mousePosition))
        {
            if (currentTile)
            {
                BuildMachine();
            }
            else if (isDestroying)
            {
                DemolishTile();
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

        isDestroying = false;
    }

    private IEnumerator CompressCounter()
    {
        yield return new WaitForSeconds(1);

        CameraController.main.CompressViewport();
    }

    [ContextMenu("TORNADO")]

    private void Tornado()
    {
        GameObject tornado = Instantiate(tornadoPF, spawnLocations[Random.Range(0, spawnLocations.Count)].position, Quaternion.identity);

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
        }

    }

}
