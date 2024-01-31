using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GameOfLife : MonoBehaviour
{
    // Start is called before the first frame update

    [SerializeField] private Tilemap currentState;
    [SerializeField] private Tile aliveTile;
    [SerializeField] private List<Vector2Int> pattern;
    [SerializeField] private float updateInterval = 0.5f;
    [SerializeField] private int generation = 0;

    [SerializeField] public new Camera camera;

    private HashSet<Vector3Int> aliveCells;
    private HashSet<Vector3Int> cellsToCheck;

    private bool show = true;
    private bool isCoroutineRunning = false;

    [SerializeField] public int population = 0;
    [SerializeField] public float time = 0;

    private void Awake()
    {
        aliveCells = new HashSet<Vector3Int>();
        cellsToCheck = new HashSet<Vector3Int>();
    }

    void Start()
    {
        SetPattern();
    }

    // Update is called once per frame
    void Update()
    {
        if (!isCoroutineRunning)
        {
            if (Input.GetKeyDown("space"))
            {
                StartCoroutine(Simulate());
            }
            if (Input.GetMouseButton(0))
            {
                {
                    Vector3 worldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                    Vector2Int cell = new Vector2Int((int)worldPosition.x, (int)worldPosition.y);
                    pattern.Add(cell);
                    SetPattern();
                }
            }
        }

        if (Input.GetKeyDown("escape"))
        {
            StopAllCoroutines();
            currentState.ClearAllTiles();
            generation = population = 0;
            isCoroutineRunning = false;
            pattern.Clear();
            aliveCells.Clear();
            cellsToCheck.Clear();
        }

        if (Input.GetMouseButtonDown(1))
        {
            show = !show;
        }

        if (Input.GetKey(KeyCode.UpArrow))
        {
            camera.transform.Translate(0, 1, 0); 
        }
        if (Input.GetKey(KeyCode.DownArrow))
        {
            camera.transform.Translate(0, -1, 0);
        }
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            camera.transform.Translate(-1, 0, 0);
        }
        if (Input.GetKey(KeyCode.RightArrow))
        {
            camera.transform.Translate(1, 0, 0);
        }

        if (Input.GetAxis("Mouse ScrollWheel") != 0f) // forward
        {
            camera.orthographicSize += -Input.GetAxis("Mouse ScrollWheel") * 10;
        }
    }
    void OnGUI()
    {
        GUIStyle style = new GUIStyle();
        style.fontSize = 30;
        style.normal.textColor = Color.red;
        if (show)
        {
            GUI.Label(new Rect(Screen.height / 25, Screen.width / 25, 320, 80), "Generation: " + generation + " Population: " + population + "\nHold down left mouse to place cell. Press Space to start the simulation. Esc to reset.", style);
        }
    }

    private void SetPattern()
    {
        Clear();
        
        foreach (Vector2Int vector2Cell in pattern)
        {
            Vector3Int cell = (Vector3Int)vector2Cell;
            currentState.SetTile(cell, aliveTile);
            aliveCells.Add(cell);
        }
    }

    private IEnumerator Simulate()
    {
        var interval = new WaitForSeconds(updateInterval);
        isCoroutineRunning = true;

        while (enabled)
        {
            UpdateState();

            population = aliveCells.Count;
            generation++;
            time += updateInterval;

            yield return interval;
        }
    }

    private void Clear()
    {
        currentState.ClearAllTiles();
    }

    private void UpdateState()
    {
        cellsToCheck.Clear();

        foreach (Vector3Int cell in aliveCells)
        {
            for (int x = -1; x <= 1; x++)
            {
                for (int y = -1; y <= 1; y++)
                {
                    cellsToCheck.Add(cell + new Vector3Int(x, y, 0));
                }
            }
        }

        foreach (Vector3Int cell in cellsToCheck)
        {
            int neighbors = CountNeighbors(cell);
            bool alive = IsAlive(cell);

            if (!alive && neighbors == 3)
            {
                aliveCells.Add(cell);
            } 
            else if (alive && (neighbors < 2 || neighbors > 3))
            {
                aliveCells.Remove(cell);
            }
        }

        currentState.ClearAllTiles();
        foreach (Vector3Int cell in aliveCells)
        {
            currentState.SetTile(cell, aliveTile);
        }

    }

    private int CountNeighbors(Vector3Int cell)
    {
        int count = 0;
        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                Vector3Int neighbor = cell + new Vector3Int(x, y, 0);
                if (x == 0 && y == 0)
                {
                    continue;
                } 
                else if (IsAlive(neighbor))
                {
                    count++;
                }
            }
        }
        return count;
    }

    private bool IsAlive(Vector3Int cell)
    {
        return currentState.HasTile(cell);
    }
}
