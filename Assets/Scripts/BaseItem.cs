using UnityEngine;

public class BaseItem : MonoBehaviour {
  public ItemType ItemType { get; private set; }

  public bool Initialized { get; protected set; } = false;

  [SerializeField]
  public Transform slot;

  public virtual void Initialize(ItemSettings settings, int size) {
    ItemType = settings.ItemType;
    Initialized = true;
  }
}
