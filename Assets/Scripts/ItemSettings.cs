using UnityEngine;

[CreateAssetMenu(menuName = "Create ItemSettings", fileName = "ItemSettings", order = 0)]
public class ItemSettings : ScriptableObject 
{
    public ItemType ItemType;
    public BuildItem Prefab;
    public Sprite PreviewIcon;
    public Vector2 Size = new Vector2(1, 1);
    public int orientation = 0;
    public string Name;
}