using System.Collections;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TerrainLogic : MonoBehaviour
{

    [SerializeField] private Camera terrainCam;
    [SerializeField] private Tilemap tm;

    [Header("TILES")]
    [SerializeField] private TileBase grassTile;

    public static TileBase currentTile;
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

}
