using System;
using System.Collections.Generic;
using UnityEngine;

public class BuildItem : BaseItem {
  [SerializeField]
  private Renderer renderer;

  [SerializeField]
  private Collider collider;
  
  private List<Collision> collisions = new List<Collision>();
  public bool HasIntersections => collisions.Count != 0;

  public Collider Collider => collider;
  public bool DoUpdate = false;

  private void Awake() {
    if (renderer == null) {
      renderer = GetComponent<Renderer>();
    }

    if (collider == null) {
      collider = GetComponent<Collider>();
    }
  }

  private void OnValidate() {
    if (DoUpdate) {
      if (renderer == null) {
        renderer = GetComponent<Renderer>();
      }

      if (collider == null) {
        collider = GetComponent<Collider>();
      }
    }
  }

  public void SetWrongPlaceColor() {
    renderer.material.color = Color.red;
  }

  public void SetDefaultColor() {
    renderer.material.color = Color.white;
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
