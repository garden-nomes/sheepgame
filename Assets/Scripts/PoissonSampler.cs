using System.Collections.Generic;
using UnityEngine;

public class PoissonSampler
{
    // samples to try before rejecting
    private const int k = 10;

    private readonly Rect rect;
    private readonly float radius;
    private readonly float radiusSq;
    private readonly float cellSize;
    private Vector2[,] grid;
    private List<Vector2> activeSamples;

    public PoissonSampler(Rect rect, float radius)
    {
        if (radius <= 0)
        {
            throw new System.ArgumentException("Radius must be non-zero positive number");
        }

        this.rect = rect;
        this.radius = radius;
        radiusSq = radius * radius;
        cellSize = radius / Mathf.Sqrt(2f);
        activeSamples = new List<Vector2>();
        var gridWidth = Mathf.CeilToInt(rect.width / cellSize);
        var gridHeight = Mathf.CeilToInt(rect.height / cellSize);
        grid = new Vector2[gridWidth, gridHeight];
    }

    public IEnumerable<Vector2> Samples()
    {
        var startPoint = rect.min + new Vector2(Random.value, Random.value) * rect.size;
        yield return AddSample(startPoint);

        while (activeSamples.Count > 0)
        {
            var index = (int)(Random.value * activeSamples.Count);
            var sample = activeSamples[index];

            var found = false;
            var seed = Random.value;
            for (int i = 0; i < k; i++)
            {
                var angle = Mathf.PI * 2f * (seed + i / (float)k);
                var r = radius + Mathf.Epsilon;
                var testPoint = sample + new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * r;

                if (IsValid(testPoint))
                {
                    found = true;
                    yield return AddSample(testPoint);
                    break;
                }
            }

            if (!found)
            {
                activeSamples.RemoveAt(index);
            }
        }
    }

    private bool IsValid(Vector2 sample)
    {
        if (!rect.Contains(sample))
        {
            return false;
        }

        var index = GridIndex(sample);

        var xMin = Mathf.Max(index.x - 2, 0);
        var yMin = Mathf.Max(index.y - 2, 0);
        var xMax = Mathf.Min(index.x + 2, grid.GetLength(0) - 1);
        var yMax = Mathf.Min(index.y + 2, grid.GetLength(1) - 1);

        for (int x = xMin; x <= xMax; x++)
        {
            for (int y = yMin; y <= yMax; y++)
            {
                var gridSample = grid[x, y];

                if (
                    gridSample != default(Vector2) &&
                    (gridSample - sample).sqrMagnitude < radiusSq)
                {
                    return false;
                }
            }
        }

        return true;
    }

    private Vector2 AddSample(Vector2 sample)
    {
        activeSamples.Add(sample);
        var index = GridIndex(sample);
        grid[index.x, index.y] = sample;
        return sample;
    }

    private Vector2Int GridIndex(Vector2 sample)
    {
        var x = Mathf.FloorToInt((sample.x - rect.xMin) / cellSize);
        var y = Mathf.FloorToInt((sample.y - rect.yMin) / cellSize);
        return new Vector2Int(x, y);
    }
}
