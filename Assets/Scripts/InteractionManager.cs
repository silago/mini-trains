using System;
using UnityEngine;

public class InteractionManager : MonoBehaviour
{
/*
    [SerializeField] private ContentManager _contentManager;
    [SerializeField] private ActivityManager _activityManager;

    private void Update()
    {
        DetectClick();
    }

    public void DetectClick()
    {
        if (Camera.main == null)
        {
            return;
        }

        if (!Input.GetMouseButtonDown(0)) return;

        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out var hit))
        {
            Debug.Log($"{hit.collider.name} Detected",
                hit.collider.gameObject);

            var go = hit.collider.gameObject;
            var buildItem = go.GetComponent<Production>();
            if (buildItem != null && buildItem.ProductionAction != null)
            {
                _activityManager.AddActivity(new Activity()
                {
                    target = go.transform,
                    Status = ActivityStatus.Paused,
                    Type = ActivityType.Create,
                    callbackAfter = () => ProductionActivityCallback(buildItem.ProductionAction, buildItem)
                });
            }
        }
    }

    public void ProductionActivityCallback(ProductionAction action, Production production)
    {
        var item = Instantiate(action.result, production.slot);
        item.transform.position = production.slot.transform.position;
        var storage = _contentManager.GetEmptyStorage();

        production.ProductionItem = item;
        if (storage == null)
        {
            return;
        }

        _activityManager.AddActivity(new Activity()
        {
            Type = ActivityType.Move,
            item = item.gameObject,
            target = storage.transform,
            callbackBefore = () =>
            {
                production.ProductionItem = null;
                item.gameObject.SetActive(false);
            },

            callbackAfter = () =>
            {
                storage.ProductionItem = item;
                item.gameObject.SetActive(true);
                item.transform.position = storage.slot.transform.position;
            },
        });
    }
    */
}