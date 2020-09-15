using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;
using UnityEngine.Tilemaps;
using UnityEditor.Tilemaps;
using System.IO;

public class TilemapLayerWindow : EditorWindow
{
    static Color highlightColor = new Color(1f, 1f, .5f, .3f);
    Texture2D highlightTexture;
    TilemapLayer editingNameLayer;

    [MenuItem("Window/2D/Tile Layers")]
    public static void ShowWindow()
    {
        EditorWindow.GetWindow(typeof(TilemapLayerWindow), false, "Tile Layers");
    }

    void Awake()
    {
        highlightTexture = new Texture2D(1, 1);
        highlightTexture.SetPixel(0, 0, highlightColor);
        highlightTexture.Apply();

        GridPaintingState.scenePaintTargetChanged += _ => Repaint();
    }

    void OnGUI()
    {
        var layers = GetLayers();
        RenderLayerList(layers);
    }

    void RenderLayerList(List<TilemapLayer> layers)
    {
        GUILayout.Label("Layers");
        GUILayout.BeginVertical(new GUIStyle { padding = new RectOffset(8, 8, 8, 8) });

        GUILayout.BeginHorizontal();
        GUILayout.Label("Name", GUILayout.MaxWidth(200), GUILayout.MinWidth(100));
        GUILayout.Space(8);
        GUILayout.Label("Sort", GUILayout.Width(80));
        GUILayout.Space(8);
        GUILayout.Label("Blocking", GUILayout.MaxWidth(80), GUILayout.MinWidth(30));
        GUILayout.EndHorizontal();

        foreach (var layer in layers)
        {
            RenderLayer(layer);
        }

        if (GUILayout.Button("+ Add Layer", GUILayout.ExpandWidth(false)))
        {
            AddTilemap();
        }

        GUILayout.EndVertical();
    }

    void RenderLayer(TilemapLayer layer)
    {
        var rowStyle = new GUIStyle();

        if (layer.IsPaintTarget())
        {
            rowStyle.normal.background = highlightTexture;
        }

        GUILayout.BeginHorizontal(rowStyle);

        RenderActionButtons(layer);

        if (editingNameLayer == layer)
        {
            layer.Name = GUILayout.TextField(layer.Name, GUILayout.Width(100));
        }
        else
        {
            GUILayout.Label(layer.Name, GUILayout.Width(100));
        }

        GUILayout.Space(8);

        GUILayout.BeginHorizontal(
            GUILayout.ExpandWidth(true),
            GUILayout.Width(80));

        if (GUILayout.Button("-", GUILayout.ExpandWidth(false)))
        {
            layer.SortingOrder--;
        }

        var sortingOrder = GUILayout.TextField(
            $"{layer.SortingOrder}",
            GUILayout.ExpandWidth(true));

        if (int.TryParse(sortingOrder, out var parsedSortingOrder))
            layer.SortingOrder = parsedSortingOrder;

        if (GUILayout.Button("+", GUILayout.ExpandWidth(false)))
        {
            layer.SortingOrder++;
        }

        GUILayout.EndHorizontal();

        GUILayout.Space(8);

        var isColliding = GUILayout.Toggle(
            layer.IsColliding(), "",
            GUILayout.MinWidth(30),
            GUILayout.MaxWidth(80));

        layer.SetColliding(isColliding);

        GUILayout.EndHorizontal();

    }

    void RenderActionButtons(TilemapLayer layer)
    {
        if (GUILayout.Button(EditorGUIUtility.IconContent("Grid.PaintTool"), GUILayout.Width(50)))
        {
            layer.SetPaintTarget();
            ShowGridPaintPaletteWindow();
        }

        var visIcon = layer.IsVisible() ? "scenevis_visible_hover" : "scenevis_hidden_hover";
        if (GUILayout.Button(EditorGUIUtility.IconContent(visIcon), GUILayout.Width(30)))
        {
            layer.ToggleVisibility();
        }

        if (editingNameLayer == layer)
        {
            if (
                GUILayout.Button("done", GUILayout.Width(40)) ||
                (hasFocus && UnityEngine.Event.current.keyCode == KeyCode.Return))
            {
                editingNameLayer = null;
            }
        }
        else
        {
            if (GUILayout.Button("edit", GUILayout.Width(40)))
            {
                editingNameLayer = layer;
            }
        }
    }

    List<TilemapLayer> GetLayers()
    {
        return GridPaintingState.validTargets
            .Select(obj => new TilemapLayer(obj))
            .OrderByDescending(layer => layer.SortingOrder)
            .ToList();
    }

    void AddTilemap()
    {
        var grid = GameObject.FindObjectOfType<Grid>();

        if (grid == null)
        {
            Debug.LogWarning("No grid found in scene");
            return;
        }

        var name = $"Tile Layer {GetLayers().Count}";
        var tilemap = new GameObject(name);
        tilemap.AddComponent<Tilemap>();
        tilemap.AddComponent<TilemapRenderer>();
        tilemap.transform.parent = grid.transform;
    }

    void ShowGridPaintPaletteWindow()
    {
        // hack due to GridPaintPaletteWindow being internal class
        foreach (var assembly in System.AppDomain.CurrentDomain.GetAssemblies())
        {
            foreach (var type in assembly.GetTypes())
            {
                if (type.Name == "GridPaintPaletteWindow")
                {
                    EditorWindow.FocusWindowIfItsOpen(type);
                }
            }
        }

    }
}
