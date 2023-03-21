using UnityEngine;

public class TileGrid : MonoBehaviour
{
    public TileRow[] rows { get; private set; }

    public TileCell[] cells { get; private set; }

    public int size => cells.Length;

    public int height => rows.Length;

    public int width => size / height;

    private void Awake()
    {
        rows = GetComponentsInChildren<TileRow>();
        cells = GetComponentsInChildren<TileCell>();
    }

    private void Start()
    {
        for (int i = 0; i < rows.Length; i++)
        {
            for (int j = 0; j < rows[i].cells.Length; j++)
            {
                rows[i].cells[j].coordinates = new Vector2Int(j, i);
            }
        }
    }

    public TileCell GetRandomEmptyCell()
    {
        int index = Random.Range(0, cells.Length);
        int startingIndex = index;

        while (this.cells[index].occupied)
        {
            index++;
            if (index == cells.Length)
            {
                index = 0;
            }

            if (index == startingIndex)
            {
                return null;
            }
        }

        return cells[index];
    }

    public TileCell GetCell (int x, int y)
    {
        return x < 0 || x == this.width || y < 0 || y == this.height ? null : rows[y].cells[x];
    }

    public TileCell GetAdjacentCell(TileCell cell, Vector2Int direction)
    {
        Vector2Int coordinates = cell.coordinates;
        coordinates.x += direction.x;
        coordinates.y -= direction.y;
        return this.GetCell(coordinates.x, coordinates.y);
    }
}
