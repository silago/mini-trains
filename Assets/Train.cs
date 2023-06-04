using System;
using System.Threading.Tasks;
using UnityEngine;

public class Train : MonoBehaviour {
  [SerializeField]
  private Rigidbody body;
  
  public Rigidbody Body => body;
}
