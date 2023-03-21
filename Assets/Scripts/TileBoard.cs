using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileBoard : MonoBehaviour
{
    public Tile tilePrefab;

    public TileState[] tileStates;

    private TileGrid grid;

    private List<Tile> tiles;

    private bool waiting;

    public GameManager gameManager;

    private void Awake()
    {
        grid = GetComponentInChildren<TileGrid>();
        tiles = new List<Tile>(16);
    }

    public void ClearBoard()
    {
        foreach (TileCell cell in this.grid.cells)
        {
            cell.tile = null;
        }

        foreach (Tile tile in tiles)
        {
            Destroy(tile.gameObject);
        }

        tiles.Clear();
    }

    public void CreateTile()
    {
        Tile tile = Instantiate(this.tilePrefab, this.grid.transform);
        tile.SetState(this.tileStates[0], 2);
        tile.Spawn(this.grid.GetRandomEmptyCell());
        this.tiles.Add(tile);
    }

    private void Update()
    {
        if (waiting)
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
        {
            MoveTiles(Vector2Int.up, 0, 1, 1, 1);
        }
        else if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow)) 
        {
            MoveTiles(Vector2Int.down, 0, 1, this.grid.height - 2, -1);
        }
        else if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
        {
            MoveTiles(Vector2Int.left, 1, 1, 0, 1);
        }
        else if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
        {
            MoveTiles(Vector2Int.right, this.grid.width - 2, -1, 0, 1);
        }
    }

    private void MoveTiles(Vector2Int direction, int startX, int incrementX, int startY, int incrementY)
    {
        bool changed = false;

        for (int x  = startX; x >= 0 && x < this.grid.width; x += incrementX)
        {
            for (int y = startY; y >= 0 && y < this.grid.height; y += incrementY)
            {
                TileCell cell = grid.GetCell(x, y);
                if (cell.occupied)
                {
                    changed |= MoveTile(cell.tile, direction); 
                }
            }
        }

        if (changed)
        {
            StartCoroutine(this.WaitingForChange());
        }
    }

    private bool MoveTile(Tile tile, Vector2Int direction)
    {
        TileCell newCell = null;
        TileCell adjacent = this.grid.GetAdjacentCell(tile.cell, direction);

        while (adjacent != null)
        {
            if (adjacent.occupied)
            {
                if (this.CanMerge(tile, adjacent.tile))
                {
                    this.MergeTiles(tile, adjacent.tile);
                    return true;
                }
                break;
            }

            newCell= adjacent;
            adjacent = this.grid.GetAdjacentCell(adjacent, direction);
        }

        if (newCell != null)
        {
            tile.MoveTo(newCell);
            return true;
        }
        return false;
    }

    private void MergeTiles(Tile a, Tile b)
    {
        tiles.Remove(a);
        a.Merge(b.cell);

        int index = Mathf.Clamp(this.IndexOf(b.state) + 1, 0, this.tileStates.Length - 1);
        b.SetState(tileStates[index], b.number * 2);
        this.gameManager.IncreaseScore(b.number);
    }

    private int IndexOf(TileState state)
    {
        for (int i = 0; i < this.tileStates.Length; i++)
        {
            if (state == this.tileStates[i])
            {
                return i;
            }
        }
        return -1;
    }

    private bool CanMerge(Tile a, Tile b)
    {
        return a.number == b.number && !b.locked;
    }

    private IEnumerator WaitingForChange()
    {
        waiting = true;
        yield return new WaitForSeconds(0.1f);
        waiting = false;

        foreach (Tile tile in this.tiles)
        {
            tile.locked = false;
        }

        if (this.tiles.Count != this.grid.size)
        {
            this.CreateTile();
        }

        if (this.CheckForGameOver())
        {
            this.gameManager.Gameover();
        }
    }

    private bool CheckForGameOver()
    {
        if (this.tiles.Count != this.grid.size)
        {
            return false;
        }

        foreach (Tile tile in tiles)
        {
            TileCell up = this.grid.GetAdjacentCell(tile.cell, Vector2Int.up);
            TileCell down = this.grid.GetAdjacentCell(tile.cell, Vector2Int.down);
            TileCell left = this.grid.GetAdjacentCell(tile.cell, Vector2Int.left);
            TileCell right = this.grid.GetAdjacentCell(tile.cell, Vector2Int.right);
            if (up != null && this.CanMerge(tile, up.tile))
            {
                return false;
            }
            if (down != null && this.CanMerge(tile, down.tile))
            {
                return false;
            }
            if (left != null && this.CanMerge(tile, left.tile))
            {
                return false;
            }
            if (right != null && this.CanMerge(tile, right.tile))
            {
                return false;
            }
        }

        return true;
    }
}
