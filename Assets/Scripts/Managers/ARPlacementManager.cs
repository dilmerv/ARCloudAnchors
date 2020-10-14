using System.Collections.Generic;
using DilmerGames.Core.Singletons;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

[RequireComponent(typeof(ARRaycastManager))]
public class ARPlacementManager : Singleton<ARPlacementManager>
{ 
    [SerializeField]
    private Camera arCamera;

    [SerializeField]
    private GameObject placedPrefab = null;

    private GameObject placedGameObject = null;

    private ARRaycastManager arRaycastManager = null;

    static List<ARRaycastHit> hits = new List<ARRaycastHit>();

    private ARAnchorManager arAnchorManager = null;

    void Awake() 
    {
        arRaycastManager = GetComponent<ARRaycastManager>();
        arAnchorManager = GetComponent<ARAnchorManager>();
    }

    bool TryGetTouchPosition(out Vector2 touchPosition)
    {
        if(Input.touchCount > 0)
        {
            var touch = Input.GetTouch(0);

            if(touch.phase == TouchPhase.Began)
            {
                touchPosition = touch.position;

                bool isOverUI = touchPosition.IsPointOverUIObject();

                return isOverUI ? false : true;
            }
        }

        touchPosition = default;

        return false;
    }

    public void RemovePlacements()
    {
        Destroy(placedGameObject);
        placedGameObject = null;
    }

    void Update()
    {
        if(!TryGetTouchPosition(out Vector2 touchPosition))
            return;

        if(placedGameObject != null)
            return;

        if(arRaycastManager.Raycast(touchPosition, hits, UnityEngine.XR.ARSubsystems.TrackableType.PlaneWithinPolygon))
        {
            var hitPose = hits[0].pose;
            placedGameObject = Instantiate(placedPrefab, hitPose.position, hitPose.rotation);
            var anchor = arAnchorManager.AddAnchor(new Pose(hitPose.position, hitPose.rotation));
            placedGameObject.transform.parent = anchor.transform;

            // this won't host the anchor just add a reference to be later host it
            ARCloudAnchorManager.Instance.QueueAnchor(anchor);
        }
    }

    public void ReCreatePlacement(Transform transform)
    {
        placedGameObject = Instantiate(placedPrefab, transform.position, transform.rotation);
        placedGameObject.transform.parent = transform;
    }
}