using UnityEngine;

public class BaseItem : MonoBehaviour {

  public bool Initialized { get; protected set; } = false;

  public virtual void Initialize(int size) {
    Initialized = true;
  }
}
