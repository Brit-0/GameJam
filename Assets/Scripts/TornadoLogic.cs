using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class TornadoLogic : MonoBehaviour
{
    private Rigidbody2D rb;
    private SpriteRenderer sr;
    private ParticleSystem ps;

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

        if (Vector2.Distance(rb.position, locationsToGo[currentMoveIndex]) < 1)
        {
            currentMoveIndex++;

            if (currentMoveIndex == locationsToGo.Count)
            {
                finished = true;
            }
        }
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

    private IEnumerator DestroyTornado()
    {
        yield return new WaitForSeconds(1f);
        Destroy(gameObject);
    }
}
