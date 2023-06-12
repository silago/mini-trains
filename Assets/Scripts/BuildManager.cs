using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;

public class BuildManager : MonoBehaviour {
  [SerializeField]
  private ContentManager contentManager;

  [SerializeField]
  private Transform cursor;

  [SerializeField]
  private ItemsSettings items;

  [SerializeField]
  private Transform container;

  [SerializeField]
  private ItemBuildPreview itemBuildPreview;

  private ItemSettings selectedItemSettings;
  private BuildItem selectedItemPrefab;

  [SerializeField]
  private GameObject world;

  [SerializeField]
  private LayerMask floorLevel;

  [SerializeField]
  private LayerMask buildItemLevel;

  [SerializeField]
  private int tmpLayer = 0;

  private BuildItem[] grid = new BuildItem[100 * 100];

  [SerializeField]
  private float cellSize = 0.5f;

  private int prevLayer;

  private int WorldToGrid(Vector3 input) {
    var x = Mathf.FloorToInt(input.x / cellSize) + 50;
    var y = Mathf.FloorToInt(input.z / cellSize) + 50;
    var index = y * 100 + x;
    return index;
  }

  [SerializeField]
  Vector3 offset;

  private Vector3 ClampToGrid(Vector3 input) {
    var half = cellSize * 0.5f;
  
    //input += offset;
    input.x+= input.x > 0 ? half : -half;
    input.z+= input.z > 0 ? half : -half;
    
    
    var divx = input.x % cellSize;
    //divx *= divx > cellSize/2 ? 1 : -1;
    
    var divz = input.z % cellSize;
    //divz *= divz > cellSize/2 ? 1 : -1;
    
    var x = input.x - divx; 
    var z = input.z + /*divz > half ? (cellSize-divz) : */-divz; 
    return new Vector3(x, input.y, z);
  }

  private void Awake() {
    foreach (var item in items.items) {
      var newItem = Instantiate(itemBuildPreview, container);
      newItem.sprite.sprite = item.PreviewIcon;
      newItem.text.text = item.name;
      newItem.button.onClick.AddListener(() => OnBuildButtonClick(item));
    }

    Time.timeScale = 2f;
  }

  private Collider[] cols = null;

  private void OnDrawGizmos() {
    if (cols == null) {
      return;
    }
  }

  public bool HasIntersections(BuildItem item) {
    if (selectedItemPrefab == null) {
      return false;
    }

    var bounds = selectedItemPrefab.Collider.bounds;
    var result = Physics.OverlapBox(item.transform.position, bounds.extents * 0.99f, item.transform.rotation,
        buildItemLevel);

    Debug.Log(result.Length);
    return result.Length > 0;
  }

  private Vector3 GetMeshOffset(BuildItem item) {
    var bounds = item.Collider.bounds;
    var localCenter = item.transform.position - bounds.center;
    var result = new Vector3(0, bounds.extents.y - localCenter.y, 0);
    //Debug.Log(bounds.extents.y + ": " + item.Collider.bounds.center.y + ":" + item.transform.position.y);
    return result;
  }

  private void Update() {
    if (EventSystem.current.IsPointerOverGameObject()) {
      return;
    }

    if (Input.GetMouseButtonUp(1) && selectedItemPrefab != null) {
      Destroy(selectedItemPrefab.gameObject);
      selectedItemPrefab = null;
    }

    if (selectedItemPrefab) {
      var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
      RaycastHit hit;
      if (Physics.Raycast(ray, out hit, 100, floorLevel)) {
        Debug.Log(hit.transform.gameObject.name);
        var clampedPos = ClampToGrid(hit.point);
        selectedItemPrefab.transform.position = clampedPos ;//+ GetMeshOffset(selectedItemPrefab);
        cursor.transform.position = ClampToGrid(hit.point); 

        var hasIntersections = HasIntersections(selectedItemPrefab);
        if (hasIntersections) {
          selectedItemPrefab.SetWrongPlaceColor();
        } else {
          selectedItemPrefab.SetDefaultColor();
        }

        if (Input.GetMouseButtonUp(0) && !hasIntersections) {
          Build();
        }
      }

      if (Input.GetKeyUp(KeyCode.Q)) {
        var rotation = selectedItemPrefab.transform.rotation.eulerAngles;
        selectedItemPrefab.transform.rotation = Quaternion.Euler(rotation.x, rotation.y + 90, rotation.z);
      } else if (Input.GetKeyUp(KeyCode.E)) {
        var rotation = selectedItemPrefab.transform.rotation.eulerAngles;
        selectedItemPrefab.transform.rotation = Quaternion.Euler(rotation.x, rotation.y - 90, rotation.z);
      }

      if (Input.mouseScrollDelta.y > 0.1f) {
        selectedItemPrefab.transform.rotation *= Quaternion.Euler(0, 90f, 0);
      } else if (Input.mouseScrollDelta.y < -0.1f) {
        selectedItemPrefab.transform.rotation *= Quaternion.Euler(0, -90f, 0);
      }
    }
  }

  private void Build() {
    //cursor.gameObject.SetActive(false);
    selectedItemPrefab.Collider.gameObject.layer = prevLayer;
    var name = selectedItemPrefab.name;
    var buildItem = selectedItemPrefab;

    selectedItemPrefab = Instantiate(selectedItemPrefab, world.transform);
    selectedItemPrefab.name = name;

    prevLayer = selectedItemPrefab.Collider.gameObject.layer;
    selectedItemPrefab.Collider.gameObject.layer = tmpLayer;

    var settings = selectedItemSettings;

    buildItem.Initialize(settings);
    contentManager.AddItem(buildItem);
  }

  private void OnDrawGizmosSelected() {
    var cell = this.cellSize;
    var count = 2;

    for (var x = -cell * count; x < cell * count; x += cell) {
      for (var y = -cell * count; y < cell * count; y += cell) {
        var pos = ClampToGrid(new Vector3(x, -0.75f, y));
        Gizmos.DrawCube(pos, (cell - 0.5f) * Vector3.one);
      }
    }
  }

  private async void OnBuildButtonClick(ItemSettings settings) {
    selectedItemSettings = settings;
    await Task.Yield();
    if (selectedItemPrefab != null) {
      Destroy(selectedItemPrefab.gameObject);
    }
    selectedItemPrefab = Instantiate(settings.Prefab, world.transform);

    prevLayer = selectedItemPrefab.Collider.gameObject.layer;
    selectedItemPrefab.Collider.gameObject.layer = tmpLayer;
    cursor.gameObject.SetActive(true);
  }
}
