using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SkillJoystick : MonoBehaviour
{
//    TouchDirType touchDirType = TouchDirType.Right;

//    private SkillBtnCD skillBtn;
//    public SkillBtnCD SkillBtn
//    {
//        set
//        {
//            skillBtn = value;
//        }
//    }

//    private SkillNode skillNode;
//    public SkillNode SkillNode { set { skillNode = value; ConstraintSetUseState(); } }

//    GameObject currentHeroGo;

//    Vector3 defaultVector3 = new Vector3(10000, 10000);
//    bool isPress = false;
//    bool mobileTouchActive = false;
//    float h, v;
//    public float bigCircleRadius = 100;
//    bool joystickMoved = false;     // 技能摇杆按下后是否位移了

//    Vector3 targetVector;      // 技能范围附近查找到的敌人
//    Vector3 targetVectorTemp;   // 用于临时存储技能范围附近查找到的敌人

//    Transform bigCircleTrans;
//    Transform smallCircleTrans;

//    Vector2 bigCircleStartWorldPos = Vector2.zero;
//    Vector2 smallCircleStartLocalPos = Vector2.zero;

//    Vector2 startPressPos;      // 技能摇杆按下时的初始位置 即第一帧位置

//    UISprite bigSprite;
//    UISprite smallSprite;

//    private Vector2 offset;     // 摇杆的偏移量  -1到1

//    #region 对外属性
//    private Vector3 lookAtTarget;       // 英雄的朝向
//    public Vector3 LookAtTarget { get { return lookAtTarget; } }

//    private bool ifCancel;  // 是否取消了摇杆
//    public bool IfCancel { get { return ifCancel; } }

//    private bool canUse;    // 是否可以使用
//    public bool CanUse { get { return canUse && CharacterManager.playerCS != null && !CharacterManager.playerCS.pm.isAutoMode; } }

//    /// <summary>
//    /// 通过传入长度 获取摇杆偏移后的对应地图内长度
//    /// </summary>
//    public float GetDistance(float totalDistance)
//    {
//        float distance = Vector2.Distance(offset, Vector2.zero);
//        return canUse == false ? totalDistance : totalDistance * distance;
//    }
//    #endregion

//    void Awake()
//    {
//        bigCircleTrans = transform;
//        smallCircleTrans = transform.GetChild(0);
//        smallCircleStartLocalPos = smallCircleTrans.localPosition;

//        bigSprite = GetComponent<UISprite>();
//        smallSprite = smallCircleTrans.GetComponent<UISprite>();

//        InitSKillAreaType();
//    }

//    public void InitAction()
//    {
//#if UNITY_EDITOR

//#elif UNITY_ANDROID || UNITY_IPHONE
//        //TouchManager.Instance.onTouchBegan += OnTouchBegan;
//        //TouchManager.Instance.onTouchEnded += OnTouchEnded;
//#endif
//    }

//    void OnDestroy()
//    {
//#if UNITY_EDITOR

//#elif UNITY_ANDROID || UNITY_IPHONE
//        //TouchManager.Instance.onTouchBegan -= OnTouchBegan;
//        //TouchManager.Instance.onTouchEnded -= OnTouchEnded;
//#endif
//    }

//    void FixedUpdate()
//    {
//        if (isPress)
//        {
//            if (!GameLibrary.VRMode)
//            {
//                CheckHeroChange();

//                PressIsTrue();

//                CheckElementState();
//            }
//        }
//    }

//    private void LateUpdate()
//    {
//        if (isPress)
//        {
//            if (!GameLibrary.VRMode)
//                UpdateElement();
//        }
//    }

//    /// <summary>
//    /// 技能按钮按下和松开时调用
//    /// </summary>
//    /// <param name="id"></param>
//    /// <param name="isPress"></param>
//    public void OnJoystickPress(int id, bool isPress, Vector3 pos)
//    {
//        mobileTouchActive = isPress;

//        this.isPress = isPress;
//        if (isPress)
//        {// && !gameObject.activeSelf
//            OnTouchBeganEvent(pos);
//        }
//        else if (!isPress)
//        {// && gameObject.activeSelf
//            OnTouchEndedEvent();
//        }
//    }

//    Vector3 dragPos;
//    public void OnTouchDrag(Vector3 pos)
//    {
//        dragPos = pos;
//    }

//    Vector2 GetPosition()
//    {
//        return dragPos;
//    }

//    /// <summary>
//    /// 监听手指按下
//    /// </summary>
//    public void OnTouchBegan(Vector3 pos)
//    {
//        //      if (dirType != touchDirType)
//        //          return;
//        //if (mobileTouchActive == false) 	// 如果手机端手指按下事件没有执行，则返回
//        //	return;

//        isPress = true;

//        OnTouchBeganEvent(pos);
//    }

//    /// <summary>
//    /// 监听手指松开
//    /// </summary>
//    public void OnTouchEnded()
//    {//TouchDirType dirType
//     //if (dirType != touchDirType)
//     //    return;
//        if (isPress == false)		// 如果手机端手指松开没有调用OnPress的松开，则不返回 执行下面方法
//            return;

//        isPress = false;
//        mobileTouchActive = false;

//        OnTouchEndedEvent();
//    }

//    /// <summary>
//    /// 手指按下时数据操作
//    /// </summary>
//    void OnTouchBeganEvent(Vector3 pos)
//    {
//        gameObject.SetActive(true);
//        joystickMoved = false;
//        elementParent = null;
//        targetVectorTemp = defaultVector3;
//        onCancelUI = false;
//        startPressPos = pos;// TouchManager.GetPosition(touchDirType);
//        dragPos = pos;
//        currentHeroGo = CharacterManager.playerCS.gameObject;
//        FightTouch._instance.skillJoystickCancelGo.SetActive(true);
//        CheckHasTarget();
//        SetOffsetIfHasTarget();
//        CreateSkillArea();
//    }

//    /// <summary>
//    /// 手指按下时数据操作
//    /// </summary>
//    void OnTouchEndedEvent()
//    {
//        SetValue();
//        if (smallCircleTrans)
//            smallCircleTrans.localPosition = smallCircleStartLocalPos;
//        gameObject.SetActive(false);
//        FightTouch._instance.skillJoystickCancelGo.SetActive(false);
//        // 鼠标抬起时 将 h,v归零
//        h = 0;
//        v = 0;
//        HideElements();
//    }

//    float lastAttackDist = 0;
//    /// <summary>
//    /// 攻击按钮持续按下和松开时调用
//    /// </summary>
//    /// <param name="isPress"></param>
//    public void OnAttackPress(bool isPress)
//    {
//        if (isPress && lastAttackDist != skillNode.rangenValue.outerRadius)
//        {
//            lastAttackDist = skillNode.rangenValue.outerRadius;
//            elementParent = null;
//            currentHeroGo = CharacterManager.playerCS.gameObject;
//            HideElement(SKillAreaElement.OuterCircle);
//            CreateAttackArea();
//        }
//        else if (!isPress)
//        {
//            lastAttackDist = 0;
//            HideElement(SKillAreaElement.OuterCircle);
//        }
//    }

//    /// <summary>
//    /// 强制隐藏所有技能按钮创建的物体  当被眩晕时会触发
//    /// </summary>
//    public void ForceHideAllSkill()
//    {
//        OnTouchEnded();
//    }

//    /// <summary>
//    /// 强制隐藏所有攻击按钮创建的物体  当被眩晕时会触发
//    /// </summary>
//    public void ForceHideAllAttack()
//    {
//        if (currentHeroGo == null)
//            return;

//        lastAttackDist = 0;
//        HideElement(SKillAreaElement.OuterCircle);
//    }

//    void SetValue()
//    {
//        // 如果在取消按钮上松开，则执行取消命令
//        if (onCancelUI)
//        {
//            ifCancel = true;
//            offset = Vector2.zero;
//            lookAtTarget = Vector3.zero;
//            canUse = false;
//        }
//        else
//        {
//            ifCancel = false;
//            offset = GetOffset();
//            lookAtTarget = GetLookAtTarget();
//            canUse = true;
//            ConstraintSetUseState();
//        }
//    }

//    void CheckHeroChange()
//    {
//        if (CharacterManager.playerCS == null || currentHeroGo != CharacterManager.playerCS.gameObject)
//        {
//            ForceHideAllSkill();
//            ForceHideAllAttack();
//        }
//    }

//    /// <summary>
//    /// 强制设置摇杆是否可以使用状态
//    /// </summary>
//    /// <returns></returns>
//    void ConstraintSetUseState()
//    {
//        if (skillNode != null)
//        {
//            switch (skillNode.target)
//            {
//                case TargetState.Need:
//                    canUse = false;
//                    break;
//            }
//        }
//    }

//    /// <summary>
//    /// 获取偏移量
//    /// </summary>
//    /// <returns></returns>
//    Vector2 GetOffset()
//    {
//        return new Vector2(h, v);
//    }

//    /// <summary>
//    /// 获取旋转值
//    /// </summary>
//    /// <returns></returns>
//    Vector3 GetLookAtTarget()
//    {
//        if (currentHeroGo == null) return Vector3.zero;

//        Vector3 targetDir = new Vector3(h, 0, v);

//        float y = Camera.main.transform.rotation.eulerAngles.y;
//        targetDir = Quaternion.Euler(0, y, 0) * targetDir;


//        return targetDir + currentHeroGo.transform.position;
//    }

//    /// <summary>
//    /// 查找技能范围内是否有敌人
//    /// </summary>
//    void CheckHasTarget()
//    {
//        CharacterManager.playerCS.SetAttackTargetTo(null);
//        GameLibrary.Instance().SetCsAttackTargetByChoseTarget(skillNode, CharacterManager.playerCS);
//        if (CharacterManager.playerCS.attackTarget != null)
//        {
//            targetVector = CharacterManager.playerCS.attackTarget.transform.position;
//        }
//        else
//        {
//            targetVector = defaultVector3;
//        }
//    }

//    /// <summary>
//    /// 如果范围内有敌人，则设置偏移量
//    /// </summary>
//    void SetOffsetIfHasTarget()
//    {
//        if (targetVector != defaultVector3)
//        {
//            if (skillNode.rangenValue.outerRadius == 0)
//            {
//                targetVector = defaultVector3;
//            }
//            else
//            {
//                if (currentHeroGo == null) return;
//                float y = Camera.main.transform.rotation.eulerAngles.y;
//                Vector3 tempVec = targetVector - currentHeroGo.transform.position;

//                Vector3 tempTargetVec = Quaternion.Euler(0, -y, 0) * tempVec;

//                h = tempTargetVec.x / skillNode.rangenValue.outerRadius;
//                v = tempTargetVec.z / skillNode.rangenValue.outerRadius;
//            }
//        }
//    }

//    // 按下时 触发此方法
//    void PressIsTrue()
//    {
//        if (!joystickMoved)
//        {
//            // 检测按下后是否移动了摇杆
//            if (Vector2.Distance(GetPosition(), startPressPos) > 5)
//            {
//                joystickMoved = true;
//            }
//        }

//        if (joystickMoved == false)
//            return;

//        // UICamera.lastTouchPosition 为当前鼠标按下时的坐标（Vector2类型）
//        if (bigCircleStartWorldPos == Vector2.zero)
//        {
//            bigCircleStartWorldPos = NGUITools.FindCameraForLayer(gameObject.layer).WorldToScreenPoint(bigCircleTrans.position);
//        }
//        Vector2 touchPos = GetPosition() - bigCircleStartWorldPos;
//        // 当鼠标拖动的位置与中心位置大于bigCircleRadius时，则固定按钮位置不会超过bigCircleRadius。  bigCircleRadius为背景图片半径长度
//        if (smallCircleTrans == null)
//        {
//            return;
//        }
//        if (Vector2.Distance(touchPos, Vector2.zero) > bigCircleRadius)
//        {
//            // 按钮位置为 鼠标方向单位向量 * bigCircleRadius
//            smallCircleTrans.localPosition = touchPos.normalized * bigCircleRadius;
//        }
//        else
//        {
//            // 按钮位置为鼠标位置
//            smallCircleTrans.localPosition = touchPos;
//        }

//        if (targetVector != defaultVector3 && joystickMoved == false)
//            return;

//        // 按钮位置x轴 / 半径 的值为0-1的横向偏移量
//        h = smallCircleTrans.localPosition.x / bigCircleRadius;

//        // 按钮位置y轴 / 半径 的值为0-1的纵向偏移量
//        v = smallCircleTrans.localPosition.y / bigCircleRadius;
//    }


//    #region 英雄身边的技能范围展示
//    enum SKillAreaElement
//    {
//        OuterCircle,    // 外圆
//        InnerCircle,    // 内圆
//        Cube,           // 矩形 读aoe_long aoe_with
//        CubeLong,        // 超长矩形 读aoe_long aoe_with
//        Sector60,        // 扇形
//        Sector90,        // 扇形
//        Sector120,        // 扇形
//        Sector180,        // 扇形
//        None
//    }

//    string path = "Effect/Prefabs/Hero_skillarea/";  // 路径
//    string circle = "quan_hero";    // 圆形
//    string cube = "chang_hero";     // 矩形
//    string cube_long = "chaochang_hero";        // 超长的矩形 小黑大
//    string sector60 = "shan_hero_60";    // 扇形60度
//    string sector90 = "shan_hero_90";    // 扇形90度
//    string sector120 = "shan_hero_120";    // 扇形120度
//    string sector180 = "shan_hero_180";    // 扇形180度

//    Dictionary<SKillAreaElement, string> allElementPath;
//    Dictionary<SKillAreaElement, Transform> allElementTrans;


//    void InitSKillAreaType()
//    {
//        allElementPath = new Dictionary<SKillAreaElement, string>();
//        allElementPath.Add(SKillAreaElement.OuterCircle, circle);
//        allElementPath.Add(SKillAreaElement.InnerCircle, circle);
//        allElementPath.Add(SKillAreaElement.Cube, cube);
//        allElementPath.Add(SKillAreaElement.CubeLong, cube_long);
//        allElementPath.Add(SKillAreaElement.Sector60, sector60);
//        allElementPath.Add(SKillAreaElement.Sector90, sector90);
//        allElementPath.Add(SKillAreaElement.Sector120, sector120);
//        allElementPath.Add(SKillAreaElement.Sector180, sector180);

//        allElementTrans = new Dictionary<SKillAreaElement, Transform>();
//        allElementTrans.Add(SKillAreaElement.OuterCircle, null);
//        allElementTrans.Add(SKillAreaElement.InnerCircle, null);
//        allElementTrans.Add(SKillAreaElement.Cube, null);
//        allElementTrans.Add(SKillAreaElement.CubeLong, null);
//        allElementTrans.Add(SKillAreaElement.Sector60, null);
//        allElementTrans.Add(SKillAreaElement.Sector90, null);
//        allElementTrans.Add(SKillAreaElement.Sector120, null);
//        allElementTrans.Add(SKillAreaElement.Sector180, null);
//    }

//    /// <summary>
//    /// 获取摇杆元素物体
//    /// </summary>
//    /// <param name="skillnode"></param>
//    /// <returns></returns>
//    public Transform SkillJoystickElement(SkillNode skillnode)
//    {
//        if (skillnode.rangenValue == null) return null;
//        SKillAreaElement element = SKillAreaElement.None;
//        switch (skillnode.rangenValue.type)
//        {
//            case RangenType.OuterCircle:
//                break;
//            case RangenType.OuterCircle_InnerCube:
//                element = SKillAreaElement.Cube;
//                break;
//            case RangenType.OuterCircle_InnerSector:
//                switch ((int)skillnode.rangenValue.angle)
//                {
//                    case 60:
//                        element = SKillAreaElement.Sector60;
//                        break;
//                    case 90:
//                        element = SKillAreaElement.Sector90;
//                        break;
//                    case 120:
//                        element = SKillAreaElement.Sector120;
//                        break;
//                    case 180:
//                        element = SKillAreaElement.Sector180;
//                        break;
//                }
//                break;
//            case RangenType.OuterCircle_InnerCircle:
//                element = SKillAreaElement.InnerCircle;
//                break;
//            case RangenType.InnerCube:
//                element = SKillAreaElement.CubeLong;
//                break;
//            default:
//                break;
//        }
//        if (allElementTrans != null && allElementTrans.ContainsKey(element))
//        {
//            return allElementTrans[element];
//        }
//        return null;
//    }
//    /// <summary>
//    /// 创建攻击按钮的区域展示
//    /// </summary>
//    void CreateAttackArea()
//    {
//        CreateElement(SKillAreaElement.OuterCircle, skillNode.rangenValue.outerRadius);
//    }

//    /// <summary>
//    /// 创建技能区域展示
//    /// </summary>
//    void CreateSkillArea()
//    {
//        if (skillNode.rangenValue == null)
//            return;

//        switch (skillNode.rangenValue.type)
//        {
//            case RangenType.OuterCircle:
//                CreateElement(SKillAreaElement.OuterCircle, skillNode.rangenValue.outerRadius);
//                break;
//            case RangenType.OuterCircle_InnerCube:
//                CreateElement(SKillAreaElement.OuterCircle, skillNode.rangenValue.outerRadius);
//                CreateElement(SKillAreaElement.Cube, skillNode.rangenValue.width, skillNode.rangenValue.length);
//                break;
//            case RangenType.OuterCircle_InnerSector:
//                CreateElement(SKillAreaElement.OuterCircle, skillNode.rangenValue.outerRadius);
//                switch ((int)skillNode.rangenValue.angle)
//                {
//                    case 60:
//                        CreateElement(SKillAreaElement.Sector60, skillNode.rangenValue.outerRadius);
//                        break;
//                    case 90:
//                        CreateElement(SKillAreaElement.Sector90, skillNode.rangenValue.outerRadius);
//                        break;
//                    case 120:
//                        CreateElement(SKillAreaElement.Sector120, skillNode.rangenValue.outerRadius);
//                        break;
//                    case 180:
//                        CreateElement(SKillAreaElement.Sector180, skillNode.rangenValue.outerRadius);
//                        break;
//                    default:
//                        break;
//                }
//                break;
//            case RangenType.OuterCircle_InnerCircle:
//                CreateElement(SKillAreaElement.OuterCircle, skillNode.rangenValue.outerRadius);
//                CreateElement(SKillAreaElement.InnerCircle, skillNode.rangenValue.innerRadius);
//                break;
//            case RangenType.InnerCube:
//                CreateElement(SKillAreaElement.CubeLong, skillNode.rangenValue.width, skillNode.rangenValue.length);
//                break;
//            default:
//                break;
//        }
//    }

//    /// <summary>
//    /// 创建技能区域展示元素
//    /// </summary>
//    /// <param name="element"></param>
//	void CreateElement(SKillAreaElement element, params float[] values)
//    {
//        Transform elementTrans = GetElement(element);
//        if (elementTrans == null) return;
//        allElementTrans[element] = elementTrans;
//        CharacterState cs = currentHeroGo.GetComponent<CharacterState>();
//        if (cs == null)
//        {
//            return;
//        }
//        switch (element)
//        {
//            case SKillAreaElement.OuterCircle:
//                elementTrans.localScale = new Vector3(values[0] * 2, 0, values[0] * 2) / currentHeroGo.transform.localScale.x * cs.changeScale;
//                elementTrans.gameObject.SetActive(true);
//                break;
//            case SKillAreaElement.InnerCircle:
//                elementTrans.localScale = new Vector3(values[0] * 2, 0, values[0] * 2) / currentHeroGo.transform.localScale.x * cs.changeScale;
//                break;
//            case SKillAreaElement.Cube:
//                elementTrans.localScale = new Vector3(values[0], 0, values[1]) / currentHeroGo.transform.localScale.x * cs.changeScale;
//                break;
//            case SKillAreaElement.CubeLong:
//                //elementTrans.localScale = new Vector3(values[0], 0, values[1]) / currentHeroGo.transform.localScale.x;
//                break;
//            case SKillAreaElement.Sector60:
//            case SKillAreaElement.Sector90:
//            case SKillAreaElement.Sector120:
//            case SKillAreaElement.Sector180:
//                elementTrans.localScale = new Vector3(values[0], 0, values[0]) / currentHeroGo.transform.localScale.x * cs.changeScale;
//                break;
//            default:
//                break;
//        }
//    }

//    Transform elementParent;
//    float hight = 0;
//    /// <summary>
//    /// 获取元素的父对象
//    /// </summary>
//    /// <returns></returns>
//    Transform GetParent()
//    {
//        if (currentHeroGo == null) return null;
//        CharacterState cs = currentHeroGo.GetComponent<CharacterState>();
//        if (cs == null)
//        {
//            return null;
//        }
//        if (elementParent == null)
//        {
//            if (cs.skilljoyParent != null)
//            {
//                elementParent = cs.skilljoyParent.transform.Find("SkillArea");
//            }
//            else
//            {
//                elementParent = currentHeroGo.transform.Find("SkillArea");
//            }
//        }
//        if (elementParent == null)
//        {
//            elementParent = new GameObject("SkillArea").transform;
//            elementParent.parent = currentHeroGo.transform;
//            elementParent.localEulerAngles = Vector3.zero;
//            elementParent.localPosition = Vector3.zero;
//            elementParent.localScale = Vector3.one;
//        }
//        if (cs.skilljoyParent != null)
//        {
//            elementParent.parent = cs.skilljoyParent.transform;
//            elementParent.localPosition = new Vector3(0, -cs.skilljoyParent.transform.localPosition.y + 0.1f, 0);
//            hight = elementParent.localPosition.y;
//        }
//        else
//        {
//            hight = 0;
//        }
//        return elementParent;
//    }

//    /// <summary>
//    /// 获取元素物体
//    /// </summary>
//    Transform GetElement(SKillAreaElement element)
//    {
//        if (currentHeroGo == null) return null;
//        string name = element.ToString();
//        Transform parent = GetParent();
//        Transform elementTrans = parent.Find(name);
//        if (elementTrans == null)
//        {
//            GameObject elementGo = Resource.CreatPrefabs(allElementPath[element], parent.gameObject, Vector3.zero, path);
//            elementGo.gameObject.SetActive(false);
//            elementGo.name = name;
//            elementTrans = elementGo.transform;
//        }
//        elementTrans.localEulerAngles = Vector3.zero;
//        elementTrans.localPosition = Vector3.zero;
//        elementTrans.localScale = Vector3.one;
//        return elementTrans;
//    }

//    /// <summary>
//    /// 隐藏所有元素
//    /// </summary>
//    void HideElements()
//    {
//        if (currentHeroGo == null) return;
//        Transform parent = GetParent();
//        if (parent == null) return;
//        for (int i = 0, length = parent.childCount; i < length; i++)
//        {
//            if (parent.GetChild(i) != null)
//                parent.GetChild(i).gameObject.SetActive(false);
//        }
//        SetElementState(true, true);
//    }

//    /// <summary>
//    /// 隐藏指定元素
//    /// </summary>
//    /// <param name="element"></param>
//    void HideElement(SKillAreaElement element)
//    {
//        if (currentHeroGo == null) return;
//        Transform parent = GetParent();
//        if (parent == null) return;
//        Transform elementTrans = parent.Find(element.ToString());
//        if (elementTrans != null)
//            elementTrans.gameObject.SetActive(false);
//    }

//    /// <summary>
//    /// 每帧更新元素
//    /// </summary>
//    void UpdateElement()
//    {
//        switch (skillNode.rangenValue.type)
//        {
//            case RangenType.OuterCircle:
//                UpdateElementPosition(SKillAreaElement.OuterCircle, skillNode.rangenValue.outerRadius);
//                break;
//            case RangenType.OuterCircle_InnerCube:
//                UpdateElementPosition(SKillAreaElement.OuterCircle, skillNode.rangenValue.outerRadius);
//                UpdateElementPosition(SKillAreaElement.Cube, skillNode.rangenValue.width, skillNode.rangenValue.length);
//                break;
//            case RangenType.OuterCircle_InnerSector:
//                UpdateElementPosition(SKillAreaElement.OuterCircle, skillNode.rangenValue.outerRadius);
//                switch ((int)skillNode.rangenValue.angle)
//                {
//                    case 60:
//                        UpdateElementPosition(SKillAreaElement.Sector60, skillNode.rangenValue.outerRadius);
//                        break;
//                    case 90:
//                        UpdateElementPosition(SKillAreaElement.Sector90, skillNode.rangenValue.outerRadius);
//                        break;
//                    case 120:
//                        UpdateElementPosition(SKillAreaElement.Sector120, skillNode.rangenValue.outerRadius);
//                        break;
//                    case 180:
//                        UpdateElementPosition(SKillAreaElement.Sector180, skillNode.rangenValue.outerRadius);
//                        break;
//                    default:
//                        break;
//                }
//                break;
//            case RangenType.OuterCircle_InnerCircle:
//                UpdateElementPosition(SKillAreaElement.OuterCircle, skillNode.rangenValue.outerRadius);
//                UpdateElementPosition(SKillAreaElement.InnerCircle, skillNode.rangenValue.innerRadius);
//                break;
//            case RangenType.InnerCube:
//                UpdateElementPosition(SKillAreaElement.CubeLong, skillNode.rangenValue.width, skillNode.rangenValue.length);
//                break;
//            default:
//                break;
//        }
//    }

//    /// <summary>
//    /// 每帧更新元素位置
//    /// </summary>
//    /// <param name="element"></param>
//    void UpdateElementPosition(SKillAreaElement element, params float[] values)
//    {
//        if (allElementTrans[element] == null)
//            return;
//        CharacterState cs = currentHeroGo.GetComponent<CharacterState>();
//        if (cs == null)
//        {
//            return;
//        }
//        switch (element)
//        {
//            case SKillAreaElement.OuterCircle:
//                allElementTrans[element].localScale = new Vector3(values[0] * 2, 0, values[0] * 2) / currentHeroGo.transform.localScale.x * cs.changeScale;
//                break;
//            case SKillAreaElement.InnerCircle:
//                allElementTrans[element].localScale = new Vector3(values[0] * 2, 0, values[0] * 2) / currentHeroGo.transform.localScale.x * cs.changeScale;
//                allElementTrans[element].position = GetCirclePosition(skillNode.rangenValue.outerRadius);
//                break;
//            case SKillAreaElement.Cube:
//                allElementTrans[element].localScale = new Vector3(values[0], 0, values[1]) / currentHeroGo.transform.localScale.x * cs.changeScale;
//                allElementTrans[element].LookAt(GetCubeSectorLookAt());
//                break;
//            case SKillAreaElement.CubeLong:
//                allElementTrans[element].LookAt(GetCubeSectorLookAt());
//                break;
//            case SKillAreaElement.Sector60:
//            case SKillAreaElement.Sector90:
//            case SKillAreaElement.Sector120:
//            case SKillAreaElement.Sector180:
//                allElementTrans[element].localScale = new Vector3(values[0], 0, values[0]) / currentHeroGo.transform.localScale.x * cs.changeScale;
//                allElementTrans[element].LookAt(GetCubeSectorLookAt());
//                break;
//            default:
//                break;
//        }
//        if (!allElementTrans[element].gameObject.activeSelf)
//            allElementTrans[element].gameObject.SetActive(true);
//    }

//    /// <summary>
//    /// 获取InnerCircle元素位置
//    /// </summary>
//    /// <returns></returns>
//    Vector3 GetCirclePosition(float dist)
//    {
//        if (currentHeroGo == null) return Vector3.zero;

//        Vector3 targetDir = new Vector3(h, 0, v) * dist;

//        if (Camera.main == null)
//        {
//            return Vector3.zero;
//        }
//        float y = Camera.main.transform.rotation.eulerAngles.y;
//        targetDir = Quaternion.Euler(0, y, 0) * targetDir;

//        return targetDir + currentHeroGo.transform.position;
//    }

//    /// <summary>
//    /// 获取Cube、Sector元素朝向
//    /// </summary>
//    /// <returns></returns>
//    Vector3 GetCubeSectorLookAt()
//    {
//        if (currentHeroGo == null) return Vector3.zero;

//        if (targetVector == defaultVector3 && joystickMoved == false)   // 如果摇杆未移动且没有目标时，方向为英雄前方
//        {
//            return elementParent.position + currentHeroGo.transform.forward;
//        }
//        Vector3 targetDir = new Vector3(h, 0, v);

//        float y = Camera.main.transform.rotation.eulerAngles.y;
//        targetDir = Quaternion.Euler(0, y, 0) * targetDir;

//        return targetDir + elementParent.position;
//    }

//    bool onCancelUI;    // 是否手指在取消技能按钮上
//    RaycastHit hit;
//    /// <summary>
//    /// 检查元素的状态 施放状态和取消施放状态
//    /// </summary>
//    void CheckElementState()
//    {
//        // 射线的碰撞检测  
//        if (Physics.Raycast(UICamera.mainCamera.ScreenPointToRay(GetPosition()), out hit, 200))
//        {
//            if (hit.collider.gameObject == FightTouch._instance.skillJoystickCancelGo)
//            {
//                SetElementState(false);
//                onCancelUI = true;
//            }
//            else
//            {
//                SetElementState(true);
//                onCancelUI = false;
//            }
//        }
//        else
//        {
//            SetElementState(true);
//            onCancelUI = false;
//        }
//    }

//    bool colorState = false;
//    Color blue = new Color(8f / 255, 78f / 255, 255f / 255, 190f / 255);
//    Color red = new Color(255f / 255, 0f / 255, 0f / 255, 160f / 255);
//    string blueSprite = "jinengyaogan";
//    string redSprite = "jinengyaogan_hong";
//    Renderer[] renderers;
//    /// <summary>
//    /// 设置元素的状态 施放状态和取消施放状态
//    /// </summary>
//    void SetElementState(bool normal, bool loosen = false)
//    {
//        if (colorState == normal)
//            return;
//        colorState = normal;

//        bigSprite.spriteName = normal == true ? blueSprite : redSprite;

//        if (renderers == null && currentHeroGo != null)
//        {
//            renderers = GetParent().GetComponentsInChildren<Renderer>();
//        }

//        for (int i = 0; i < renderers.Length; i++)
//        {
//            if (renderers[i] != null && renderers[i].material != null)
//                renderers[i].material.SetColor("_TintColor", normal == true ? blue : red);
//        }

//        if (loosen)
//        {
//            renderers = null;
//        }
//    }

//    #endregion
}