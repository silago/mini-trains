using System.Collections.Generic;
using UnityEngine;

public class RailLever : MonoBehaviour {
  [SerializeField]
  private int index = 0;

  [SerializeField]
  List<RailItem> tracks = new List<RailItem>();

  public void Activate() {
    tracks.ForEach(x => x.priority = 0);
    index++;
    index %= tracks.Count;
    tracks[index].priority = 1;
  }

  private void OnMouseDown() {
    Activate();
  }
}
