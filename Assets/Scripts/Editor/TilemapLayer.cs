using UnityEngine;
using UnityEditor;
using UnityEngine.Tilemaps;
using UnityEditor.Tilemaps;

class TilemapLayer
{
    private GameObject Tilemap { get; }
    private TilemapRenderer Renderer { get; }

    public TilemapLayer(GameObject tilemap)
    {
        Tilemap = tilemap;
        Renderer = tilemap.GetComponent<TilemapRenderer>();

        if (Renderer == null)
        {
            Debug.LogError($"No tilemap renderer found on {tilemap.name}");
        }
    }

    public override bool Equals(System.Object obj)
    {
        if ((obj == null) || !this.GetType().Equals(obj.GetType()))
        {
            return false;
        }
        else
        {
            var layer = (TilemapLayer)obj;
            return layer.Tilemap == Tilemap;
        }
    }

    public static bool operator ==(TilemapLayer a, TilemapLayer b)
    {
        return a?.Tilemap == b?.Tilemap;
    }

    public static bool operator !=(TilemapLayer a, TilemapLayer b)
    {
        return a?.Tilemap != b?.Tilemap;
    }

    public override int GetHashCode() => Tilemap.GetHashCode();

    public int SortingOrder
    {
        get => Renderer.sortingOrder;
        set => Renderer.sortingOrder = value;
    }

    public string Name
    {
        get => Tilemap.name;
        set => Tilemap.name = value;
    }

    public bool IsColliding()
    {
        var collider = Tilemap.GetComponent<TilemapCollider2D>();
        return collider != null;
    }

    public void SetColliding(bool isColliding)
    {
        if (isColliding == IsColliding())
        {
            return;
        }

        if (isColliding)
        {
            var rigidbody = Tilemap.AddComponent<Rigidbody2D>();
            var tilemapCollider = Tilemap.AddComponent<TilemapCollider2D>();
            var compositeCollider = Tilemap.AddComponent<CompositeCollider2D>();

            tilemapCollider.usedByComposite = true;
            rigidbody.bodyType = RigidbodyType2D.Static;
        }
        else
        {
            GameObject.DestroyImmediate(Tilemap.GetComponent<CompositeCollider2D>());
            GameObject.DestroyImmediate(Tilemap.GetComponent<TilemapCollider2D>());
            GameObject.DestroyImmediate(Tilemap.GetComponent<Rigidbody2D>());
        }
    }

    public void ToggleVisibility()
    {
        SceneVisibilityManager.instance.ToggleVisibility(Tilemap, true);
    }

    public bool IsVisible()
    {
        return !SceneVisibilityManager.instance.IsHidden(Tilemap);
    }

    public bool IsPaintTarget()
    {
        return Tilemap == GridPaintingState.scenePaintTarget;
    }

    public void SetPaintTarget()
    {
        GridPaintingState.scenePaintTarget = Tilemap;
    }
}
