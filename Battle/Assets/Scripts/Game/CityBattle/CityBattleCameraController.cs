using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using DG.Tweening;


namespace Game.CityBattle
{
    public class CityBattleCameraController
    {
        Camera mainCamera;
        Transform cameraContainer;
        Vector2 curGesturePos;

        public float MAX_Z = 50;
        public float MIN_Z = -100;

        public float Lod_Z = 10;

        Vector3 lowWorldPoint = Vector3.zero;
        Vector3 topWorldPoint = new Vector3(MapCoords.MAP_SIZE, 0, MapCoords.MAP_SIZE);
        Vector3 leftWorldPoint = new Vector3(0, 0, MapCoords.MAP_SIZE);
        Vector3 rightWorldPoint = new Vector3(MapCoords.MAP_SIZE, 0, 0);

        protected Tweener localMoveTween = null;

        public Vector3 cameraPosition
        {
            get { return cameraContainer.position; }
            set
            {
                cameraContainer.position = value;
                //GameMain.serverDataManager.cameraPosition = value;
            }
        }

        public Vector3 localPos
        {
            get { return mainCamera.transform.localPosition; }
        }

        public Camera mainCam
        {
            get { return mainCamera; }
        }

        public int lod
        {
            get { return localPos.z >= Lod_Z ? 0 : 1; }
        }

        public void Init(Camera camera)
        {
            mainCamera = Camera.main;
            cameraContainer = mainCamera.transform.parent.parent;
        }

        public void Update(float deltaTime)
        {
        }

        void CheckBounds()
        {
            Vector2 lowEdge = mainCamera.WorldToScreenPoint(lowWorldPoint);
            Vector2 topEdge = mainCamera.WorldToScreenPoint(topWorldPoint);
            Vector2 leftEdge = mainCamera.WorldToScreenPoint(leftWorldPoint);
            Vector2 rightEdge = mainCamera.WorldToScreenPoint(rightWorldPoint);

            if (lowEdge.y > topEdge.y) lowEdge.y = 0;
            Vector2 screenOffset = Vector2.zero;
            if (lowEdge.y > 0) screenOffset.y = lowEdge.y;
            if (topEdge.y < Screen.height) screenOffset.y = topEdge.y - Screen.height;
            if (leftEdge.x > 0) screenOffset.x = leftEdge.x;
            if (rightEdge.x < Screen.width) screenOffset.x = rightEdge.x - Screen.width;

            Vector3 worldPosOffect = CameraUtil.GetWorldPos(mainCamera, screenOffset);
            Vector3 worldPosPviot = CameraUtil.GetWorldPos(mainCamera, Vector2.zero);
            cameraPosition += worldPosOffect - worldPosPviot;
        }

        #region Position

        #endregion

        #region Event

        public void OnSwipeStart(Gesture gesture)
        {
            cleanLocalMoveTween();

            curGesturePos = gesture.position;
        }

        public void OnSwipe(Gesture gesture)
        {
            cleanLocalMoveTween();

            var preSwipePos = CameraUtil.GetWorldPos(mainCamera, curGesturePos);
            var curSwipePos = CameraUtil.GetWorldPos(mainCamera, gesture.position);
            cameraPosition -= curSwipePos - preSwipePos;
            curGesturePos = gesture.position;

            CheckBounds();
        }

        public float OnPinch(Gesture gesture)
        {
            cleanLocalMoveTween();

            var deltaPinch = gesture.deltaPinch;

            float z = mainCamera.transform.localPosition.z;
            var pinchValue = z;

            z += deltaPinch * 0.1f;
            if (z > MAX_Z)
                z = MAX_Z;
            else if (z < MIN_Z)
                z = MIN_Z;
            mainCamera.transform.localPosition = new Vector3(0, 0, z);

            pinchValue = pinchValue - z;
            //CAMERA_ON_PINCH
            //NotificationDelegCenter.Instance.dispatchEvent(NotifyConsts.CAMERA_ON_PINCH, new Notification(mainCamera.transform.localPosition));

            CheckBounds();
            return z;
        }

        public void OnSwipeOut(Gesture gesture)
        {
            cleanLocalMoveTween();
        }

        public void CameraTweenFocusTo(Vector3 targetPos, float duration)
        {
            cleanLocalMoveTween();

            localMoveTween = cameraContainer.DOMove(targetPos, duration);
        }

        protected void cleanLocalMoveTween()
        {
            if (localMoveTween != null)
            {
                localMoveTween.Kill();
                localMoveTween = null;
            }
        }

        #endregion
    }
}