using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

public class GameController : MonoBehaviour {
  //[SerializeField]
  //private Train train;

  private List<Train> trains;

  //[SerializeField]
  //private RailTrackElement initialTrack;

  //[SerializeField]
  //private float speedParameter;

  //[SerializeField]
  //private float currentSpeed = 1f;

  //[SerializeField]
  //private RailTrackElement currentTrack = null;

  //[SerializeField]
  //private List<RailTrackElement> nextTracks = new List<RailTrackElement>();

  //[SerializeField]
  //private float t;

  //[SerializeField]
  //private float time;

  //[SerializeField]
  //private float currentLength;

  //[SerializeField]
  //private bool reverse;

  //private RailTrackElement nextTrack = null;

  private static GameController instance { get; set; }

  public static async Task<GameController> Instance() {
    while (instance == null) {
      await Task.Yield();
    }

    return instance;
  }

  private void Awake() {
    instance ??= this;
    //currentTrack = initialTrack;
    //currentLength = currentTrack.Spline.GetApproximateLength();

    //currentSpeed = currentLength / speedParameter;
  }



  private void Update() {
  }

}
