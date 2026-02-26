using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FairyGUI;
using Game.CityBattle.Entity;

namespace Game.CityBattle.UI
{
    public class MapEntityUIComponent
    {
        protected MapEntity parent = null;
        protected string packageName = string.Empty;
        protected string uiComponentName = string.Empty;
        protected GComponent panel;

        private ILimitUpdator cLookAtUpdator;

        //percent of scale after pinch
        protected float fPercentOfPinch = 0f;

        public float pinchScale = 1.5f;

        public Vector3 initPosition { get; set; }

        public Vector3 initScale { get; set; }

        public int instanceId { get; private set; }

        public MapEntityUIComponent()
        {
        }

        public MapEntityUIComponent(string package, string uicomponent)
        {
            initScale = new Vector3(1, 1, 1);
            packageName = package;
            uiComponentName = uicomponent;
            this.pinchScale = pinchScale;
        }

        public void Init(MapEntity entity)
        {
            parent = entity;
            instanceId = entity.Id;

            panel = UIPackage.CreateObject(packageName, uiComponentName).asCom;

            UIManager.instance.cEntityUIRoot.AddChild(panel);

            panel.scale = initScale;

            OnCreateView();

            cLookAtUpdator = UpdateFacade.AddFrameLimitUpdator(0, OnUpdate);

            //NotificationDelegCenter.Instance.addEventListener(NotifyConsts.CAMERA_ON_PINCH, OnCameraPinchNotify);
        }

        public virtual void Release()
        {
            if (cLookAtUpdator != null)
            {
                UpdateFacade.RemoveLimitUpdator(cLookAtUpdator);
                cLookAtUpdator = null;
            }

            panel.Dispose();

            //NotificationDelegCenter.Instance.removeEventListener(NotifyConsts.CAMERA_ON_PINCH, OnCameraPinchNotify);
        }

        public virtual void OnCreateView()
        {
            //AdjustScale();
        }

        public bool visible
        {
            get
            {
                if (panel != null) return panel.visible;
                return false;
            }
            set
            {
                if (panel != null && panel.parent != null) panel.visible = value;
            }
        }

        //如果要同步屏幕位置的话子类需要调用父类OnUpdate
        protected virtual void OnUpdate(object o)
        {
            FollowTargetWorld2ScreenPosition();
        }

        //follow target world position on screen
        protected virtual void FollowTargetWorld2ScreenPosition()
        {

            // var cameraPos = CityBattleManager.Instance.cameraController.localPos * pinchScale;
            // var followTargetScreenPos = Camera.main.WorldToScreenPoint(parent.position) + initPosition + new Vector3(0,
            //     cameraPos
            //         .z, 0);
            var followTargetScreenPos = Camera.main.WorldToScreenPoint(parent.position+initPosition);
            followTargetScreenPos.y = Screen.height - followTargetScreenPos.y;
            Vector2 pt = GRoot.inst.GlobalToLocal(followTargetScreenPos);

            panel.position = pt;
        }

        //while camera pinching we should scale our ui
/*    protected void OnCameraPinchNotify(object n)
    {
        AdjustScale();
    }*/

        //adjust ui scale
/*    protected void AdjustScale()
    {
        var cameraLocalPos = MapProcessor.instance.cMainCameraLocalPos;
        var min = CameraManager.MIN_Z;
        var max = CameraManager.MAX_Z;

        fPercentOfPinch = (cameraLocalPos.z - min) / (max - min);
        var scale = initScale * fPercentOfPinch;
    }*/
    }
}