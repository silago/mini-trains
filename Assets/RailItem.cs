using System;
using System.Collections.Generic;
using System.Linq;
using NaughtyBezierCurves;
using Unity.VisualScripting;
using UnityEngine;

public class RailItem : BuildItem {
  private const float MinDistance = 0.75f;

  [SerializeField]
  private NaughtyBezierCurves.BezierCurve3D spline;

  [SerializeField]
  private List<RailItem> neighboursStart = new List<RailItem>();

  [SerializeField]
  private List<RailItem> neighboursEnd = new List<RailItem>();

  [SerializeField]
  public int priority;

  public int Priority => priority;


  public RailItem GetStart() {
    return neighboursStart.Any() ? neighboursStart.OrderByDescending(x => x.priority).First() : null;
  }

  public RailItem GetEnd() {
    return neighboursEnd.Any() ? neighboursEnd.OrderByDescending(x => x.priority).First() : null;
  }

  public override void Initialize(int size) {
    base.Initialize(size);
    var resultsA = Physics.OverlapBox(GetLinearPoint(0), 0.5f * Vector3.one).ToList();
    var resultsB = Physics.OverlapBox(GetLinearPoint(1), 0.5f * Vector3.one).ToList();
    var results = resultsA.Concat(resultsB);

    var neighbours = results.Select(x => x.GetComponent<RailItem>())
        .Where(x => x != null && x != this && x.Initialized == true).ToList();

    foreach (var rail in neighbours) {
      Debug.Log($"{rail.name}");
    }

    SetupNeighbours(neighbours);
  }

  private void SetupNeighbours(List<RailItem> neighbours) {
    //assume we have one chiold item
    foreach (var n in neighbours) {
      var track = n;

      var currentFirst = this.Spline.KeyPoints[0].Position;
      var currentLast = this.Spline.KeyPoints[^1].Position;

      var trackFirst = track.Spline.KeyPoints[0].Position;
      var trackLast = track.Spline.KeyPoints[^1].Position;

      var lastFirst = Vector3.Distance(currentLast, trackFirst);
      var firstLast = Vector3.Distance(currentFirst, trackLast);

      var firstFirst = Vector3.Distance(currentFirst, trackFirst);
      var lastLast = Vector3.Distance(currentLast, trackLast);

      if (lastFirst < firstLast && lastFirst < firstFirst && lastFirst < lastLast) {
        if (!IsAvailableByDirection(track, 1, 0)) {
          continue;
          ;
        }

        track.Spline.KeyPoints[0].Position = currentLast;

        neighboursEnd.Add(track);
        track.neighboursStart.Add(this);
      } else if (firstLast < lastFirst && firstLast < firstFirst && firstLast < lastLast) {
        if (!IsAvailableByDirection(track, 0, 1)) {
          continue;
          ;
        }

        track.Spline.KeyPoints[^1].Position = currentFirst;

        neighboursStart.Add(track);
        track.neighboursEnd.Add(this);
      } else if (firstFirst < lastFirst && firstFirst < firstLast && firstFirst < lastLast) {
        if (!IsAvailableByDirection(track, 0, 0)) {
          continue;
        }

        track.Spline.KeyPoints[0].Position = currentFirst;

        neighboursStart.Add(track);
        track.neighboursStart.Add(this);
      } else {
        if (!IsAvailableByDirection(track, 1, 1)) {
          continue;
        }

        track.Spline.KeyPoints[^1].Position = currentLast;

        neighboursEnd.Add(track);
        track.neighboursEnd.Add(this);
      }

      UpdateRailSwitchers();
      track.UpdateRailSwitchers();
    }
  }

  public void UpdateRailSwitchers() {
    //check existing switcher

    //railSwitcherEnd.Initialize(neighboursEnd);
    //railSwitcherStart.Initialize(neighboursStart);
  }

  bool IsAvailableByDirection(RailItem other, float selfIndex, float otherIndex) {
    var d = Vector3.Distance(GetLinearPoint(selfIndex), other.GetLinearPoint(otherIndex));
    if (d > MinDistance) {
      Debug.Log($"Too long distance: {d}>{MinDistance} {selfIndex}->{otherIndex}");
      return false;
    }

    var selfDir = Spline.GetRotation(selfIndex, Vector3.up)
        * (selfIndex == 0 ? Vector3.forward : Vector3.back);
    var otherDir = other.Spline.GetRotation(otherIndex, Vector3.up)
        * (otherIndex == 0 ? Vector3.forward : Vector3.back);

    var dot = Vector3.Dot(selfDir, otherDir);
    dot *= -1f;
    Debug.Log($"Dot = {dot}");

    if (dot > 0.15f) {
      return true;
    } else {
      return false;
    }

    //var startPoint = track.GetLinearPoint(0);
    //var endPoint = track.GetLinearPoint(1);
    //bool fromStart = Vector3.Distance(item.transform.position, startPoint)
    //    < Vector3.Distance(item.transform.position, endPoint);

    //Vector3 dir;
    //if (fromStart) {
    //  dir = track.Spline.GetRotation(0, Vector3.up)
    //      * Vector3.forward; //initialTrack.Spline.KeyPoints[0].transform.forward;
    //} else {
    //  dir = track.Spline.GetRotation(1, Vector3.up)
    //      * -Vector3.forward; //initialTrack.Spline.KeyPoints[0].transform.forward;
    //}

    //var dot = Vector3.Dot(item.forward, dir);

    //if (dot > 0) {
    //  return true;
    //} else {
    //  return false;
    //}
  }

  private void OnDrawGizmos() {
    Gizmos.color = Color.black;
    
    Gizmos.DrawLine(GetLinearPoint(0) + Vector3.up
        , GetLinearPoint(0) + Spline.GetRotation(0f, Vector3.up) * Vector3.forward + Vector3.up);
    Gizmos.DrawLine(GetLinearPoint(1) + Vector3.up,
        GetLinearPoint(1) + Spline.GetRotation(1f, Vector3.up) * Vector3.back
        + Vector3.up);
  }

  private void OnTriggerEnter(Collider other) {
    Debug.Log("Trigger Enter" + other.gameObject.name);
  }

  public Vector3 GetLinearPoint(float t) {
    // The evaluated points is between these two points
    BezierPoint3D startPoint;
    BezierPoint3D endPoint;
    float timeRelativeToSegment;

    spline.GetLinearSegment(t, out startPoint, out endPoint, out timeRelativeToSegment);

    return BezierCurve3DLinear.GetPointOnLinearCurve(timeRelativeToSegment, startPoint, endPoint);
  }

  public NaughtyBezierCurves.BezierCurve3D Spline => spline;
}
