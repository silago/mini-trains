using System;
using NaughtyBezierCurves;
using UnityEngine;

public class RailTrackElement : MonoBehaviour {
  [SerializeField]
  private NaughtyBezierCurves.BezierCurve3D spline;

  [SerializeField]
  public int priority;

  public int Priority => priority;

  public float LinearLength {
    get {
      var result = 0f;
      for (var i = 0; i < spline.KeyPointsCount - 1; i++) {
        result += Vector3.Distance(spline.KeyPoints[i].Position, spline.KeyPoints[i + 1].Position);
      }
      return result;
    }
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

  private async void OnTriggerExit(Collider other) {
    var gameController = await GameController.Instance();
    gameController.ProcessSplineCollisionExit(this, other);
  }

  private async void OnTriggerEnter(Collider other) {
    var gameController = await GameController.Instance();
    gameController.ProcessSplineCollisionEnter(this, other);
  }

  private void OnDrawGizmosSelected() {
    if (spline == null) {
      return;
    }

    for (var i = 0f; i < 1; i += 0.05f) {
      //var point = GetPoint(i);
      var point = spline.GetPoint(i);
      Gizmos.color = Color.red;
      Gizmos.DrawSphere(point, 0.1f);
    }
  }
}
