using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TornadoLogic : MonoBehaviour
{
    private Rigidbody2D rb;
    private SpriteRenderer sr;
    private ParticleSystem ps;
    public Tilemap tm;

    [SerializeField] TileBase destroyedTile;

    private int currentMoveIndex;
    public List<Vector3> locationsToGo;
    private bool finished;
    float t = 0;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        ps = GetComponent<ParticleSystem>();
    }

    private void Start()
    {
        TornadoLife();
    }

    private void FixedUpdate()
    {
        if (locationsToGo.Count == 0) return;

        if (!finished)
        {
            Move();
        } else
        {
            Fade();
        } 
    }

    private void Move()
    {
        rb.MovePosition(Vector2.MoveTowards(rb.position, locationsToGo[currentMoveIndex], 2f * Time.fixedDeltaTime));

        if (Vector2.Distance(rb.position, locationsToGo[currentMoveIndex]) < .5f)
        {
            DestroyTiles();

            currentMoveIndex++;

            if (currentMoveIndex == locationsToGo.Count)
            {
                finished = true;
            }
        }
    }

    private void DestroyTiles()
    {
        Vector3Int ogCell = tm.WorldToCell(locationsToGo[currentMoveIndex]);
        List<Vector3Int> cellsToDestroy = new()
        {
            ogCell + Vector3Int.up,
            ogCell + Vector3Int.up + Vector3Int.left,
            ogCell + Vector3Int.up + Vector3Int.right,
            ogCell + Vector3Int.left,
            ogCell,
            ogCell + Vector3Int.right,
            ogCell + Vector3Int.down + Vector3Int.left,
            ogCell + Vector3Int.down,
            ogCell + Vector3Int.down + Vector3Int.right
        };

        foreach (Vector3Int cell in cellsToDestroy)
        {
            if (!tm.HasTile(cell)) return;

            Machine machine = (Machine)MachinesHandler.main.GetMachine(tm.GetTile(cell).name);

            machine ??= (Machine)MachinesHandler.main.GetHelper(tm.GetTile(cell).name);

            tm.SetTile(cell, destroyedTile);

            if (machine != null)
            {
                machine.qnt--;
                HUDHandler.main.UpdateQuantities(machine);
                ResourcesHandler.co2LvlIncreaser -= machine.co2Lvl;
            }
        }

        TerrainLogic.main.StartCoroutine(TerrainLogic.main.RestoreTiles(cellsToDestroy));
    }
    
    private IEnumerator TornadoLife()
    {
        yield return new WaitForSeconds(15f);

        finished = true;
    }

    private void Fade()
    {
        sr.color = new Color(1, 1, 1, Mathf.Lerp(1f, 0f, t));
        t += .8f * Time.deltaTime;

        if (t >= 1f)
        {
            ps.Stop();
            StartCoroutine(DestroyTornado());
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        print(collision.gameObject);
    }

    private IEnumerator DestroyTornado()
    {
        yield return new WaitForSeconds(1f);
        TerrainLogic.main.activeTornadoes.Remove(gameObject);
        if (TerrainLogic.main.activeTornadoes.Count == 0)
        {
            TerrainLogic.main.isTornadoing = false;
        }
        TerrainLogic.main.StartCoroutine(TerrainLogic.main.IncreaseTornadoCounter());
        Destroy(gameObject);
    }
}
