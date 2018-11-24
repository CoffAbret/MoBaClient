using UnityEngine;
using System.Collections.Generic;
public class MobaMiniMap : MonoBehaviour
{
    public static List<Player> mapElements = new List<Player>();
    public static List<Tower> mapTowerElements = new List<Tower>();
    public static MobaMiniMap instance;

    public UISprite heroHead;
    public UISprite map1v1;
    public UISprite map1v1Hero;
    public UISprite mapDragBox;

    Transform mapTrans;
    float mapSize;
    float mapScale = 380;
    float mapRotZ = 5;
    float mapPosOffsetZ = 1.2f;
    Vector2 mapDragConstrain;
    Vector2 mapDragCenter;
    int baseDepth;
    Vector3 cameraPlayerOffset = Vector3.zero;

    float mPosRatio;

    void Awake()
    {
        instance = this;
        mapElements.Clear();

        GameObject mapGo = GameObject.Find("Map");
        if (mapGo != null)
        {
            mapTrans = mapGo.transform;
            SphereCollider mapCol = mapGo.GetComponent<SphereCollider>();
            if (mapCol != null)
                mapSize = mapCol.radius;
        }

        mapDragBox.gameObject.SetActive(false);
        baseDepth = map1v1.depth + 1;
    }
    void RefreshIcon(Player element)
    {
        if (element != null && element.m_MapIcon != null)
        {
            element.m_MapIcon.transform.localPosition = GetMapPos(element.m_VGo.transform.position);
            if (!element.m_MapIcon.gameObject.activeSelf)
                element.m_MapIcon.gameObject.SetActive(true);
        }
    }

    void RefreshTowerIcon(Tower element)
    {
        if (element != null && element.m_MapIcon != null)
        {
            element.m_MapIcon.transform.localPosition = GetMapPos(element.m_VGo.transform.position);
            if (!element.m_MapIcon.gameObject.activeSelf)
                element.m_MapIcon.gameObject.SetActive(true);
        }
    }

    Vector3 GetMapPos(Vector3 worldPos)
    {
        Vector3 ret = Vector3.zero;
        float xOff = mapTrans != null ? worldPos.x - mapTrans.position.x : worldPos.x;
        float zOff = mapTrans != null ? worldPos.z - mapTrans.position.z : worldPos.z;
        float xRatio = xOff / mapSize;
        float zRatio = zOff / mapSize;
        ret.x = mPosRatio * xRatio * mapScale;
        ret.y = 0.5f * zRatio * mapScale;
        ret = Quaternion.Euler(0f, 0f, mapRotZ) * ret;
        return ret;
    }

    Vector3 GetWorldPos(Vector2 mapPos)
    {
        Vector3 ret = Vector3.zero;
        mapPos = Quaternion.Euler(0f, 0f, -mapRotZ) * mapPos;
        float xRatio = mapPos.x / (mPosRatio * mapScale);
        float yRatio = mapPos.y / (0.5f * mapScale);
        float xPos = mapTrans != null ? xRatio * mapSize + mapTrans.position.x : xRatio * mapSize;
        float zPos = mapTrans != null ? yRatio * mapSize + mapTrans.position.z : yRatio * mapSize;
        ret = new Vector3(xPos, 0f, zPos);
        return ret;
    }

    void OnPress(bool b)
    {
        GameData.m_IsDragMinMap = b;
        mapDragBox.gameObject.SetActive(b);
        if (b)
        {
            Vector2 touchPos = NGUIMath.ScreenToPixels(UICamera.currentTouch.pos, transform);
            Vector3 camTrans = GetWorldPos(touchPos);
            mapDragBox.transform.localPosition = touchPos;
            Camera.main.transform.position = new Vector3(camTrans.x, Camera.main.transform.position.y, camTrans.z - mapPosOffsetZ) + cameraPlayerOffset;
        }
    }

    void OnDrag(Vector2 delta)
    {
        Vector2 touchPos = NGUIMath.ScreenToPixels(UICamera.currentTouch.pos, transform);
        touchPos.x = Mathf.Clamp(touchPos.x, -0.5f * (mapDragConstrain.x - mapDragBox.width) + mapDragCenter.x, 0.5f * (mapDragConstrain.x - mapDragBox.width) + mapDragCenter.x);
        touchPos.y = Mathf.Clamp(touchPos.y, -0.5f * (mapDragConstrain.y - mapDragBox.height) + mapDragCenter.y, 0.5f * (mapDragConstrain.y - mapDragBox.height) + mapDragCenter.y);
        Vector3 camTrans = GetWorldPos(touchPos);
        mapDragBox.transform.localPosition = touchPos;
        Camera.main.transform.position = new Vector3(camTrans.x, Camera.main.transform.position.y, camTrans.z - mapPosOffsetZ) + cameraPlayerOffset;
    }

    void Update()
    {
        for (int i = 0; i < mapElements.Count; i++)
        {
            RefreshIcon(mapElements[i]);
        }

        for (int i = 0; i < mapTowerElements.Count; i++)
        {
            RefreshTowerIcon(mapTowerElements[i]);
        }
    }

    string GetMapIconNameByState(Player cs)
    {
        switch (cs.m_PlayerData.m_Type)
        {
            case 1:
                return cs.m_PlayerData.m_HeroAttrNode.icon_name;
            case 2:
            case 3:
                return cs.m_PlayerData.m_CampId == 1 ? "moba_lvbingbing" : "moba_hongbing";
        }
        return string.Empty;
    }

    string GetMapIconNameByTowerState(Tower cs)
    {
        return cs.m_CampId == 1 ? "moba_lanta" : "moba_hongjia";
    }

    public UISprite AddMapIconByType(Player cs)
    {
        map1v1.gameObject.SetActive(true);
        mapScale = 300;
        mapRotZ = 0;
        mapDragConstrain = new Vector2(320, 80);
        mapDragCenter = new Vector2(0, 0);
        mPosRatio = 0.6f;
        cameraPlayerOffset = new Vector3(-2f, 0, -2f);

        UISprite icon = NGUITools.AddSprite(gameObject, cs.m_PlayerData.m_Type == 1 ? map1v1Hero.atlas : map1v1.atlas, GetMapIconNameByState(cs));
        icon.transform.localEulerAngles = new Vector3(0, 0, -35);
        icon.gameObject.SetActive(false);
        icon.depth = map1v1.depth + 1;
        if (cs.m_PlayerData.m_Type == 1)
        {
            icon.depth = baseDepth + 1;
            string borderName = cs.m_PlayerData.m_CampId == 2 ? "hongdiankuang" : "landiankuang";
            UISprite iconGroupBorder = NGUITools.AddSprite(icon.gameObject, map1v1Hero.atlas, borderName);
            iconGroupBorder.width = iconGroupBorder.height = 41;
            iconGroupBorder.depth = icon.depth + 1;
            baseDepth += 2;
            icon.width = icon.height = 36;
        }
        else
            icon.width = icon.height = 6;
        cs.m_MapIcon = icon;
        cs.m_DestoryMinMapCallback = RemoveMapIcon;
        if (!mapElements.Contains(cs)) mapElements.Add(cs);
        return icon;
    }

    public UISprite AddMapIconByTower(Tower cs)
    {
        map1v1.gameObject.SetActive(true);
        mapScale = 300;
        mapRotZ = 0;
        mapDragConstrain = new Vector2(320, 80);
        mapDragCenter = new Vector2(0, 0);
        mPosRatio = 0.6f;
        cameraPlayerOffset = new Vector3(-2f, 0, -2f);

        UISprite icon = NGUITools.AddSprite(gameObject, map1v1.atlas, GetMapIconNameByTowerState(cs));
        icon.transform.localEulerAngles = new Vector3(0, 0, -35);
        icon.gameObject.SetActive(false);
        icon.depth = map1v1.depth + 1;
        icon.width = 14;
        icon.height = 20;
        cs.m_MapIcon = icon;
        cs.m_DestoryMinMapCallback = RemoveTowerMapIcon;
        if (!mapTowerElements.Contains(cs)) mapTowerElements.Add(cs);
        return icon;
    }

    public void RemoveMapIcon(Player player)
    {
        if (mapElements.Contains(player)) mapElements.Remove(player);
        if (player.m_MapIcon != null) GameObject.DestroyImmediate(player.m_MapIcon.gameObject);
    }

    public void RemoveTowerMapIcon(Tower tower)
    {
        if (mapTowerElements.Contains(tower)) mapTowerElements.Remove(tower);
        if (tower.m_MapIcon != null) GameObject.DestroyImmediate(tower.m_MapIcon.gameObject);
    }

    private void OnDestroy()
    {
        for (int i = 0; i < mapElements.Count; i++)
        {
            RemoveMapIcon(mapElements[i]);
        }
        mapElements.Clear();

        for (int i = 0; i < mapTowerElements.Count; i++)
        {
            RemoveTowerMapIcon(mapTowerElements[i]);
        }
        mapTowerElements.Clear();
    }
}