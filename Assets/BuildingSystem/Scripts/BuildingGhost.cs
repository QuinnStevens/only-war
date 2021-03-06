using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingGhost : MonoBehaviour
{
    private Transform visual;
    private PlacedObjectTypeSO placedObjectTypeSO;
    public bool active;

    private GameState.State state;

    // Start is called before the first frame update
    void Start()
    {
        this.active = false;

        GameState.Instance.OnStateChanged += Instance_OnSelectedChanged;

        GridBuildingSystem.Instance.OnSelectedChanged += Instance_OnSelectedChanged;
    }

    private void Instance_OnSelectedChanged(object sender, System.EventArgs e) {
        RefreshVisual();
    }

    private void LateUpdate() {
        Vector3 targetPosition = Utils.GetMouseWorldPositionAtCameraY();
        targetPosition = GridBuildingSystem.Instance.SnapWorldPositionToGrid(targetPosition);
        targetPosition += new Vector3(0, 1f, 0);
        transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * 15f);

        transform.rotation = Quaternion.Lerp(transform.rotation, GridBuildingSystem.Instance.GetPlacedObjectRotation(), Time.deltaTime * 15f);
    }

    private void RefreshVisual() {
        if (visual != null) {
            Destroy(visual.gameObject);
            visual = null;
        }

        if (GameState.Instance.currentState != GameState.State.Building) {
            return;
        }

        if (GridBuildingSystem.Instance.placementMode == GridBuildingSystem.PlacementMode.Building) {
            PlacedObjectTypeSO placedObjectTypeSO = GridBuildingSystem.Instance.GetPlacedObjectTypeSO();

            if (placedObjectTypeSO != null) {
                visual = Instantiate(placedObjectTypeSO.visual, Vector3.zero, Quaternion.identity);
                visual.parent = transform;
                visual.localPosition = Vector3.zero;
                visual.localEulerAngles = Vector3.zero;
                SetLayerRecursive(visual.gameObject, 11);
            }
        } else if (GridBuildingSystem.Instance.placementMode == GridBuildingSystem.PlacementMode.Wall) {
            WallTypeSO placedWallTypeSO = GridBuildingSystem.Instance.GetWallTypeSO();

            if (placedWallTypeSO != null) {
                visual = Instantiate(placedWallTypeSO.visual, Vector3.zero, Quaternion.identity);
                visual.parent = transform;
                visual.localPosition = Vector3.zero;
                visual.localEulerAngles = Vector3.zero;
                SetLayerRecursive(visual.gameObject, 11);
            }
        }
    }

    private void SetLayerRecursive(GameObject targetGameObject, int layer) {
        targetGameObject.layer = layer;
        foreach(Transform child in targetGameObject.transform) {
            SetLayerRecursive(child.gameObject, layer);
        }
    }

}
