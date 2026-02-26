using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.CityBattle
{
    public partial class CityBattleManager
    {
        void AddEasyTouchListener()
        {
            EasyTouch.On_SimpleTap += EasyTouch_On_SimpleTap;
            EasyTouch.On_Swipe += EasyTouch_On_Swipe;
            EasyTouch.On_SwipeStart += EasyTouch_On_SwipeStart;
            EasyTouch.On_SwipeEnd += EasyTouch_On_SwipeEnd;
            EasyTouch.On_Drag += EasyTouch_On_Drag;
            EasyTouch.On_Pinch += EasyTouch_On_Pinch;
        }

        void RemoveEasyTouchListener()
        {
            EasyTouch.On_SimpleTap -= EasyTouch_On_SimpleTap;
            EasyTouch.On_Swipe -= EasyTouch_On_Swipe;
            EasyTouch.On_SwipeStart -= EasyTouch_On_SwipeStart;
            EasyTouch.On_SwipeEnd -= EasyTouch_On_SwipeEnd;
            EasyTouch.On_Drag -= EasyTouch_On_Drag;
            EasyTouch.On_Pinch -= EasyTouch_On_Pinch;
        }

        void EasyTouch_On_SimpleTap(Gesture gesture)
        {
            if (gesture.touchCount != 1) return;
            //if (FairyGUI.Stage.isEasyTouchOnUI(gesture.position)) return;

            /*if (selectStaticLogicObj != null)
            {
                ResetSelected();
            }

            var worldPos = CameraUtil.GetWorldPos(Camera.main, gesture.position);
            var coor = MapCoords.WorldPosToCoor(worldPos);
            var staticObj = GetStaticLogicObj(coor);

            if (staticObj != null)
            {
                switch (staticObj.type)
                {
                    case MapLogicObjType.LOCK_AREA:
                        // 弹出UI
                        SelectObj(staticObj);
                        UIManager.instance.OpenEnforceWindow(EnforceWindowPage.LockAreaWindow, staticObj);

                        break;
                    case MapLogicObjType.BUILDING:
                        SelectObj(staticObj);
                        UIManager.instance.OpenEnforceWindow(EnforceWindowPage.BuildingWindow, staticObj);
                        break;
                }
            }
            else
            {
                ResetSelected();
            }*/
        }

        /*public void SelectObj(MapStaticLogicObj logicObj)
        {
            selectStaticLogicObj = logicObj;
            mainCamera.CameraTweenFocusTo(
                selectStaticLogicObj.entity.position + MapCoords.CoorToWorldPos(Settings.cameraOffsetBuilding), 0.2f);
        }*/

        public void ResetSelected()
        {
            //selectStaticLogicObj = null;
            UIManager.instance.CloseAllEnforceWindow();
        }


        void EasyTouch_On_SwipeStart(Gesture gesture)
        {
            //if (FairyGUI.Stage.isEasyTouchOnUI(gesture.position)) return;

            if (gesture.touchCount != 1) return;

            cameraController.OnSwipeStart(gesture);
        }

        void EasyTouch_On_Swipe(Gesture gesture)
        {
            //if (FairyGUI.Stage.isEasyTouchOnUI(gesture.position)) return;

            if (gesture.touchCount != 1) return;

            cameraController.OnSwipe(gesture);
        }

        void EasyTouch_On_SwipeEnd(Gesture gesture)
        {
        }

        void EasyTouch_On_Drag(Gesture gesture)
        {
        }

        void EasyTouch_On_Pinch(Gesture gesture)
        {
            if (gesture.touchCount == 2)
            {
                cameraController.OnPinch(gesture);
                /*int oldLod = mainCamera.lod;
                mainCamera.OnPinch(gesture);
                int newLod = mainCamera.lod;
                if (oldLod != newLod)
                {
                    // 修改建筑
                    var enumer = buildingList.GetEnumerator();
                    while (enumer.MoveNext())
                    {
                        enumer.Current.Value.OnLodChange(newLod);
                    }
                }*/
            }
        }
    }
}