using System;
using UnityEngine;

[Serializable]
public class Interaction
{
    public Event event_;
    public bool interrupt = true;
    public Line[] lines;
}

[Serializable]
public class Line
{
    public string text;
    public float duration;

    public string Render(GameObject target)
    {
        var transformed = text;

        if (target != null)
        {
            transformed = text.Replace("{name}", target.name);
        }

        return transformed;
    }
}
