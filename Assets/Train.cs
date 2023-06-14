using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class Train : BuildItem {
  [SerializeField]
  private Rigidbody body;

  //[SerializeField]
  //private RailItem initialTrack;

  [SerializeField]
  private float speedParameter;

  [SerializeField]
  private float currentSpeed = 1f;

  public Rigidbody Body => body;

  [SerializeField]
  public RailItem currentTrack;

  //[SerializeField]
  //private List<RailItem> nextTracks = new List<RailItem>();

  [SerializeField]
  private float t;

  [SerializeField]
  private float time;

  [SerializeField]
  private float currentLength;

  [SerializeField]
  private bool reverse;

  private RailItem nextTrack = null;

  public void ProcessUpdate() { }

  public override void Initialize(ItemSettings settings, int size) {
    base.Initialize(settings, size);
    currentLength = currentTrack.Spline.GetApproximateLength();
    currentSpeed = currentLength / speedParameter;
  }

  private void Update() {
    if (!Initialized) {
      return;
    }

    time += Time.deltaTime;

    var s = time * currentSpeed;
    t = s / currentLength;

    if (t < 1) {
      var point = currentTrack.GetLinearPoint(reverse ? 1 - t : t);
      this.transform.LookAt(point);
      this.transform.position = point; //this.Body.MovePosition(point);
      return;
    }

    //if (nextTracks.Count > 0 && nextTrack == null) {
    //  var available = nextTracks.Where(x => IsAvailableByDirection(x, this.transform));
    //  if (available.Any()) {
    //    nextTrack = available.OrderByDescending(x => x.priority)
    //        .First();
    //  } else {
    //    return;
    //  }

    //  nextTracks.Clear();
    nextTrack = reverse
        ? currentTrack.GetStart()
        : currentTrack.GetEnd();

    if (nextTrack == null) {
      return;
    }

    currentTrack = nextTrack;
    currentLength = currentTrack.Spline.GetApproximateLength();
    currentSpeed = currentLength / speedParameter;

    var firstPoint = currentTrack.GetLinearPoint(0);
    var lastPoint = currentTrack.GetLinearPoint(1);
    var firstDistance = Vector3.Distance(firstPoint, this.transform.position);
    var lastDistance = Vector3.Distance(lastPoint, this.transform.position);

    reverse = firstDistance > lastDistance;

    nextTrack = null;
    time = 0;
    return;
    //}
  }

  [SerializeField]
  private GameObject simulationPoint;

  [SerializeField]
  private GameObject nextPointA;

  [SerializeField]
  private GameObject nextPointB;

  bool IsAvailableByDirection(RailItem track, Transform item) {
    var startPoint = track.GetLinearPoint(0);
    var endPoint = track.GetLinearPoint(1);
    bool fromStart = Vector3.Distance(item.transform.position, startPoint)
        < Vector3.Distance(item.transform.position, endPoint);

    Vector3 dir;
    if (fromStart) {
      dir = track.Spline.GetRotation(0, Vector3.up)
          * Vector3.forward; //initialTrack.Spline.KeyPoints[0].transform.forward;
    } else {
      dir = track.Spline.GetRotation(1, Vector3.up)
          * -Vector3.forward; //initialTrack.Spline.KeyPoints[0].transform.forward;
    }

    var dot = Vector3.Dot(item.forward, dir);

    if (dot > 0) {
      return true;
    } else {
      return false;
    }
  }

  private void OnTriggerEnter(Collider other) {
    var track = other.gameObject.GetComponent<RailItem>();

    if (!Initialized) {
      currentTrack = track;
      return;
    }

    if (track != null && track.enabled == true) {
      ProcessSplineCollisionEnter(track);
    }
  }

  private void OnTriggerExit(Collider other) {
    if (!Initialized) {
      return;
    }

    var track = other.gameObject.GetComponent<RailItem>();
    if (track != null) {
      ProcessSplineCollisionExit(track);
    }
  }

  public void ProcessSplineCollisionEnter(RailItem track) {
    //var this = collider.gameObject.GetComponent<Train>();

    if (track == this.currentTrack) {
      return;
    }

    /*

    var currentFirst = this.currentTrack.Spline.KeyPoints[0].Position;
    var currentLast = this.currentTrack.Spline.KeyPoints[^1].Position;

    var trackFirst = track.Spline.KeyPoints[0].Position;
    var trackLast = track.Spline.KeyPoints[^1].Position;

    var lastFirst = Vector3.Distance(currentLast, trackFirst);
    var firstLast = Vector3.Distance(currentFirst, trackLast);

    var firstFirst = Vector3.Distance(currentFirst, trackFirst);
    var lastLast = Vector3.Distance(currentLast, trackLast);

    //todo: attach points. move to track

    if (lastFirst < firstLast && lastFirst < firstFirst && lastFirst < lastLast) {
      track.Spline.KeyPoints[0].Position = currentLast;
    } else if (firstLast < lastFirst && firstLast < firstFirst && firstLast < lastLast) {
      track.Spline.KeyPoints[^1].Position = currentFirst;
    } else if (firstFirst < lastFirst && firstFirst < firstLast && firstFirst < lastLast) {
      track.Spline.KeyPoints[0].Position = currentFirst;
    } else {
      track.Spline.KeyPoints[^1].Position = currentLast;
    }

    this.nextTracks.Add(track);
    */
  }

  public void ProcessSplineCollisionExit(RailItem railTrackElement) {
    //nextTracks.Remove(railTrackElement);
  }

  private void OnDrawGizmos() {
    if (simulationPoint != null && nextPointA != null && nextPointB != null) { } else {
      return;
    }

    Gizmos.DrawLine(simulationPoint.transform.position,
        simulationPoint.transform.position + simulationPoint.transform.forward * 2f);
    var middle = (nextPointA.transform.position + nextPointB.transform.position) / 2f;
    Gizmos.DrawSphere(middle, 0.5f);

    var forward = nextPointA.transform.position - nextPointB.transform.position;
    var backward = -forward; //nextPointB.transform.position - nextPointA.transform.position;

    Gizmos.DrawLine(middle, middle + forward);
    Gizmos.DrawLine(middle, middle + backward);

    Gizmos.color = Color.red;
    ;
    Handles.color = Color.red;
    ;
    Handles.Label(simulationPoint.transform.position + Vector3.up * 2,
        $"dot forward: {Vector3.Dot(simulationPoint.transform.forward, forward)}");
    Handles.Label(simulationPoint.transform.position + Vector3.up * 3,
        $"dot backward: {Vector3.Dot(simulationPoint.transform.forward, backward)}");

    if (currentTrack == null) {
      return;
    }

    Gizmos.color = Color.magenta;
    var a1 = currentTrack.Spline.KeyPoints[0].transform;
    var a2 = currentTrack.Spline.KeyPoints[^1].transform;
    Gizmos.DrawLine(a1.position, a1.position + a1.forward);
    Gizmos.DrawLine(a2.position, a2.position + a2.forward);
    Gizmos.color = Color.yellow;

    var x = currentTrack.Spline.GetRotation(0, Vector3.up)
        * -Vector3.forward; //initialTrack.Spline.KeyPoints[0].transform.forward;
    Gizmos.DrawLine(a1.position, a1.position + x);

    var y = currentTrack.Spline.GetRotation(1, Vector3.up)
        * Vector3.forward; //initialTrack.Spline.KeyPoints[^1].transform.forward;
    Gizmos.DrawLine(a2.position, a2.position + y);
  }
}
