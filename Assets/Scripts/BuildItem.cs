using System;
using System.Collections.Generic;
using UnityEngine;

public class BuildItem : BaseItem {
  [SerializeField]
  private Renderer itemRenderer;

  [SerializeField]
  private Collider itemCollider;

  private List<Collision> collisions = new List<Collision>();
  public bool HasIntersections => collisions.Count != 0;

  public Collider Collider => itemCollider;
  public bool DoUpdate = false;

  protected virtual void Awake() {
    if (itemRenderer == null) {
      itemRenderer = GetComponent<Renderer>();
    }

    if (itemCollider == null) {
      itemCollider = GetComponent<Collider>();
    }
  }

  private void OnValidate() {
    if (DoUpdate) {
      if (itemRenderer == null) {
        itemRenderer = GetComponent<Renderer>();
      }

      if (itemCollider == null) {
        itemCollider = GetComponent<Collider>();
      }
    }
  }

  public void SetWrongPlaceColor() {
    //itemRenderer.material.color = Color.red;
  }

  public void SetDefaultColor() {
    //itemRenderer.material.color = Color.white;
  }

  private void OnCollisionEnter(Collision collision) {
    collisions.Add(collision);
    SetWrongPlaceColor();
  }

  private void OnCollisionExit(Collision other) {
    collisions.Remove(other);

    if (collisions.Count == 0) {
      SetDefaultColor();
    }
  }
}
