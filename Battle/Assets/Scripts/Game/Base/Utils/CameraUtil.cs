using UnityEngine;
using DG.Tweening;

public class CameraUtil
{
    public enum DragMode
    {
        None,
        Drag,
        Scroll
    }

    private static Plane GROUND_PLANE = new Plane(Vector3.up, Vector3.zero);
    private static Plane XYGROUND_PLANE = new Plane(Vector3.forward, Vector3.zero);
    // private static Plane XYGROUND_PLANE = new Plane(new Vector3(0,0,1.09f), Vector3.zero);

    /**3d城市，建筑z轴升高的高度, 相机更改y轴 CameraUtil == CITYCONST.CITY_Z_HEI*/
    public static float CITY_Z_HEI_CAMERA = 0f;

    public static Vector3 GetWorldPos(Camera camera, Vector2 screenPos, float distance)
    {
        ///camera.ScreenPointToRay 官方给的范围是0 - max-1
        int screenWidth = Screen.width;
        int screenHeight = Screen.height;
        screenPos.x = Mathf.Clamp(screenPos.x, 0, screenWidth - 1);
        screenPos.y = Mathf.Clamp(screenPos.y, 0, screenHeight - 1);
        Ray ray = camera.ScreenPointToRay(screenPos);
        return ray.GetPoint(distance);
    }

    /**获取屏幕对应的世界点*/
    public static Vector3 GetWorldPos(Camera camera, Vector2 screenPos, bool useXYPlane = false)
    {
        int screenWidth = Screen.width;
        int screenHeight = Screen.height;
        //screenPos.x = Mathf.Clamp(screenPos.x, 0, screenWidth - 1);
        //screenPos.y = Mathf.Clamp(screenPos.y, 0, screenHeight - 1);
        Ray ray = camera.ScreenPointToRay(screenPos);

        float t;

        Plane plane = useXYPlane ? XYGROUND_PLANE : GROUND_PLANE;
        plane.Raycast(ray, out t);
        var tmpPos = ray.GetPoint(t);
        tmpPos.y -= CITY_Z_HEI_CAMERA;
        // Debug.Log("[gk] GetWorldPos:scrfeenPos=" + screenPos + ";worldPos=" + tmpPos + ";z_height_camera="+CITY_Z_HEI_CAMERA);

        return tmpPos;
    }

    public static Vector3 GetGroundWorldPos(Camera camera, Vector3 targePos, bool useXYPlane = false)
    {
        Ray ray = new Ray(camera.transform.position, targePos - camera.transform.position);
        Plane plane = useXYPlane ? XYGROUND_PLANE : GROUND_PLANE;
        float t;
        plane.Raycast(ray, out t);
        return ray.GetPoint(t);
    }

    public static Vector3 GetScreenPos(Camera camera, Camera uiCamera, Vector3 worldPos)
    {
        Vector3 pos = camera.WorldToViewportPoint(worldPos);
        pos.z = 0;
        return uiCamera.ViewportToWorldPoint(pos);
    }

    public static Vector3 GetScreenPosEx(Camera camera, Camera uiCamera, Vector3 worldPos, out bool isReverse)
    {
        Vector3 pos = camera.WorldToViewportPoint(worldPos);
//        //Debug.Log(pos);
        if (pos.z <= 0)
        {
            isReverse = true;
        }
        else
        {
            isReverse = false;
        }

        pos.z = 0;
        return uiCamera.ViewportToWorldPoint(pos);
    }

    public static void ShakeScreen(Camera camera, float time, float amountX = 1f, float amountY = 1f,
        float amountZ = 1f, float delay = 0f)
    {
        //		paramTbl["amount"] = new Vector3(amountX, amountY, amountZ);
        //		paramTbl["islocal"] = true;
        //		paramTbl["time"] = time;
        //		paramTbl["space"] = Space.Self;
        //		iTween.ShakePosition(Camera.main.gameObject, paramTbl);
        if (camera != null)
        {
            camera.DOShakePosition(time, new Vector3(amountX, amountY, amountZ)).SetDelay(delay);
        }
    }

    public static void GetViewPortWorldPos(Camera camera, int screenWidth, int screenHeight, ref float worldPosXMin,
        ref float worldPosYMin, ref float worldPosXMax, ref float worldPosYMax)
    {
        Vector3 pt = CameraUtil.GetWorldPos(camera, new Vector2(0, 0), true);
        // worldPosXMin = Mathf.Min(worldPosXMin, pt.x);
        // worldPosXMax = Mathf.Max(worldPosXMax, pt.x);
        worldPosYMin = Mathf.Min(worldPosYMin, pt.y);
        worldPosYMax = Mathf.Max(worldPosYMax, pt.y);
        pt = CameraUtil.GetWorldPos(camera, new Vector2(0, screenHeight), true);
        worldPosXMin = Mathf.Min(worldPosXMin, pt.x);
        worldPosXMax = Mathf.Max(worldPosXMax, pt.x);
        worldPosYMin = Mathf.Min(worldPosYMin, pt.y);
        worldPosYMax = Mathf.Max(worldPosYMax, pt.y);
        pt = CameraUtil.GetWorldPos(camera, new Vector2(screenWidth, screenHeight), true);
        worldPosXMin = Mathf.Min(worldPosXMin, pt.x);
        worldPosXMax = Mathf.Max(worldPosXMax, pt.x);
        worldPosYMin = Mathf.Min(worldPosYMin, pt.y);
        worldPosYMax = Mathf.Max(worldPosYMax, pt.y);
        worldPosXMin -= 10;
        worldPosXMax += 10;
        worldPosYMin -= 10;
        worldPosYMax += 10;
    }
}