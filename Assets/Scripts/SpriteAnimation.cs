using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Sprite Animation")]
public class SpriteAnimation : ScriptableObject
{
    public List<Sprite> sprites = new List<Sprite>();
    public float fps = 12f;
}
