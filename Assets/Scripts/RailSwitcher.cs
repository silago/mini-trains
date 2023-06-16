using System;
using System.Collections.Generic;
using UnityEngine;

public class RailSwitcher : MonoBehaviour {
  [SerializeField]
  private List<GameObject> activeArrows;

  [SerializeField]
  private List<GameObject> inactiveArrows;

  private int selectedIndex = 0;

  [SerializeField]
  private List<RailItem> railItems;

  private void Awake() {
    inactiveArrows.ForEach(x=>x.SetActive(false));
    activeArrows.ForEach(x=>x.SetActive(false));
  }

  public void Initialize() {
    if (railItems.Count <= 1) {
      gameObject.SetActive(false);
      return;
    }

    gameObject.SetActive(true);

    for (var index = 0; index < railItems.Count; index++) {
      if (activeArrows.Count < index + 1) {
        break;
      }

      var item = railItems[index];
      var activeArrow = activeArrows[index];
      var inactiveArrow = inactiveArrows[index];

      RotateArrow(activeArrow, item);
      RotateArrow(inactiveArrow, item);

      activeArrow.SetActive(index == selectedIndex);
      inactiveArrow.SetActive(index != selectedIndex);
    }

    for (var index = railItems.Count; index < activeArrows.Count; index++) {
      activeArrows[index].gameObject.SetActive(false);
    }

    for (var index = railItems.Count; index < inactiveArrows.Count; index++) {
      inactiveArrows[index].gameObject.SetActive(false);
    }
  }

  private void RotateArrow(GameObject arrow, RailItem item) {
    var pos = item.GetLinearPoint(0.5f);
    pos.y = arrow.transform.position.y;
    arrow.transform.LookAt(pos);
    arrow.transform.Rotate(0, 90, 0);
  }

  public void Switch() {
    selectedIndex++;
    selectedIndex %= railItems.Count;
    for (var index = 0; index < railItems.Count; index++) {
      if (activeArrows.Count < index + 1) {
        break;
      }

      var activeArrow = activeArrows[index];
      var inactiveArrow = inactiveArrows[index];

      activeArrow.SetActive(index == selectedIndex);
      inactiveArrow.SetActive(index != selectedIndex);
    }
    
    railItems.ForEach(x=>x.priority = 0);
    railItems[selectedIndex].priority++;
  }

  private void OnMouseDown() {
    this.Switch();
  }

  public RailItem Next() {
    return railItems.Count != 0 ? railItems[selectedIndex] : null;
  }
}
