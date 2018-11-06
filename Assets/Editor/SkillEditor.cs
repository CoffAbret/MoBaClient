using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Reflection;
using System;
using System.IO;
using System.Collections.Generic;
using UnityEditor.Animations;

/// <summary>
/// 技能节点
/// </summary>
public enum SkillItemType
{
    None,
    Root,
    SkillEvent
}

/// <summary>
/// 事件类型
/// </summary>
public enum SkillEventType
{
    AnimaEvent
}

public class SkillBase
{
    public SkillItemType type = SkillItemType.None;
    public string resPath = "";
    public int skillId;
}

public class SkillEditor : EditorWindow
{
    //存储每个状态添加的事件
    private List<AnimationEvent> m_Events = new List<AnimationEvent>();
    //树形控件
    private static TreeViewControl m_treeViewControl = null;
    //技能树形父节点
    private static SkillTreeViewItem _root = null;
    //技能树形当前选择的节点
    private static SkillTreeViewItem _curItem = null;
    //技能数据读取位置
    private static string FixPath = "Assets/Resources/SkillFightData/Skill/";
    //技能数据写入位置
    private static string ResPath = "SkillFightData/Skill/";
    //当前模型控制器对应的配置技能数据
    private static Dictionary<int, Skill> _skillDic = new Dictionary<int, Skill>();
    [MenuItem("Tools/技能编辑器")]
    public static void ShowSkillTreeViewPanel()
    {
        _skillDic.Clear();
        GetSkillData();
        CreateTreeView();
        RefreshPanel();
    }

    static SkillEditor m_instance = null;

    public static SkillEditor GetPanel()
    {
        if (null == m_instance)
        {
            m_instance = EditorWindow.GetWindow<SkillEditor>(false, "技能编辑器", false);
        }
        return m_instance;
    }

    public static void RefreshPanel()
    {
        SkillEditor panel = GetPanel();
        panel.Repaint();
    }

    /// <summary>
    /// 创建技能树
    /// </summary>
    static void CreateTreeView()
    {
        m_treeViewControl = TreeViewInspector.AddTreeView();
        m_treeViewControl.DisplayInInspector = false;
        m_treeViewControl.DisplayOnGame = false;
        m_treeViewControl.DisplayOnScene = false;
        m_treeViewControl.X = 600;
        m_treeViewControl.Y = 500;

        _root = m_treeViewControl.RootItem;
        _root.Header = "所有技能";
        SkillBase data1 = new SkillBase();
        data1.type = SkillItemType.None;
        _root.DataContext = data1;
        _curItem = _root;
        AddEvents(_root);
        CreateSkillItem();
    }

    static void AddEvents(SkillTreeViewItem item)
    {
        AddHandlerEvent(out item.Selected);
    }

    public static void Handler(object sender, System.EventArgs args)
    {
        _curItem = sender as SkillTreeViewItem;
        Selection.activeObject = Resources.Load((_curItem.DataContext as SkillBase).resPath);
    }

    static void AddHandlerEvent(out System.EventHandler handler)
    {
        handler = new System.EventHandler(Handler);
    }

    void OnEnable()
    {
        wantsMouseMove = true;
    }

    int skillId = 0; //技能id
    int selectIdx = 0; //选择的事件
    void OnGUI()
    {
        if (null == m_treeViewControl)
        {
            return;
        }

        if (_curItem == null)
        {
            return;
        }

        wantsMouseMove = true;
        if (null != Event.current &&
            Event.current.type == EventType.MouseMove)
        {
            Repaint();
        }
        m_treeViewControl.DisplayTreeView(TreeViewControl.DisplayTypes.USE_SCROLL_VIEW);

        if ((_curItem.DataContext as SkillBase).type == SkillItemType.None)
        {
            skillId = EditorGUILayout.IntField("技能id:", skillId);
            GUILayout.BeginVertical();
            if (GUILayout.Button("创建技能"))
            {
                AddSkill(skillId);
            }
            GUILayout.EndVertical();
        }
        else if ((_curItem.DataContext as SkillBase).type == SkillItemType.Root)
        {
            GUILayout.BeginHorizontal();
            string[] list = new string[] { SkillEventType.AnimaEvent.ToString() };
            selectIdx = EditorGUILayout.Popup("选择事件", selectIdx, list);
            if (GUILayout.Button("添加事件"))
            {
                AddSkillEventNode(_curItem, list[selectIdx]);
            }
            if (GUILayout.Button("删除技能"))
            {
                DeleteSkill(_curItem);
            }
            GUILayout.EndHorizontal();
        }
        else if ((_curItem.DataContext as SkillBase).type == SkillItemType.SkillEvent)
        {
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("删除事件"))
            {
                DeleteSkillEventNode(_curItem);
            }
            GUILayout.EndHorizontal();
        }
    }

    /// <summary>
    /// 添加技能
    /// </summary>
    /// <param name="newSkillId">技能id</param>
    /// <param name="isCreateAsset">是否创建新的资源</param>
    static SkillTreeViewItem AddSkill(int newSkillId, bool isCreateAsset = true)
    {
        Skill skill = ScriptableObject.CreateInstance<Skill>();
        _skillDic[skill.skillId] = skill;
        skill.skillId = newSkillId;
        if (isCreateAsset)
        {
            AssetEditor.CreateAsset(skill, FixPath + newSkillId, "Skill");
        }

        SkillTreeViewItem skillItem = _root.AddItem(newSkillId.ToString());
        SkillBase data = new SkillBase();
        data.type = SkillItemType.Root;
        data.resPath = ResPath + newSkillId + "/Skill";
        data.skillId = newSkillId;
        skillItem.DataContext = data;
        AddEvents(skillItem);
        return skillItem;
    }

    /// <summary>
    /// 删除技能
    /// </summary>
    /// <param name="newSkillId">技能id</param>
    static void DeleteSkill(SkillTreeViewItem item)
    {
        _root.Items.Remove(item);
        int key = int.Parse(item.Header);
        _skillDic.Remove(key);
        AssetDatabase.DeleteAsset(FixPath + item.Header);
    }

    /// <summary>
    /// 添加事件
    /// </summary>
    /// <param name="item">父节点</param>
    /// <param name="name">事件名</param>
    /// <param name="isCreateAsset">是否创建新的资源</param>
    /// <returns></returns>
    static SkillTreeViewItem AddSkillEventNode(SkillTreeViewItem item, string name, bool isCreateAsset = true)
    {
        if (name == String.Empty)
        {
            return null;
        }
        Skill skill = RegetditSkill(item);
        if (skill == null)
        {
            return null;
        }
        string evtName = name;
        Assembly ass = typeof(AnimaEvent).Assembly;
        string[] nameList = name.Split('_');
        System.Type type = ass.GetType(nameList[0]);
        AnimaEvent evt = System.Activator.CreateInstance(type) as AnimaEvent;
        SkillBase evtData = new SkillBase();
        if (skill.animaEvent.Count > 0)
        {
            evtName = name + "_" + skill.animaEvent.Count;
        }
        if (isCreateAsset)
        {
            skill.animaEvent.Add(evt as AnimaEvent);
        }
        if (isCreateAsset)
        {
            AssetEditor.CreateAsset(evt, FixPath + skill.skillId + "/" + evtName, evtName);
        }
        else
        {
            evtName = name;
        }
        SkillTreeViewItem evtItem = item.AddItem(evtName);
        evtData.type = SkillItemType.SkillEvent;
        evtData.skillId = skill.skillId;
        evtData.resPath = ResPath + skill.skillId + "/" + evtName + "/" + evtName;
        evtItem.DataContext = evtData;
        AddEvents(evtItem);
        EditorUtility.SetDirty(skill);
        return evtItem;
    }

    /// <summary>
    /// 删除事件
    /// </summary>
    /// <param name="item">节点</param>
    /// <returns></returns>
    static void DeleteSkillEventNode(SkillTreeViewItem item)
    {
        Skill skill = RegetditSkill(item);
        if (skill == null)
        {
            return;
        }
        for (int i = 0; i < skill.animaEvent.Count; i++)
        {
            if (skill.animaEvent[i].name == item.Header)
            {
                skill.animaEvent.RemoveAt(i);
            }
        }
        item.Parent.Items.Remove(item);
        string path = FixPath + skill.skillId + "/" + item.Header;
        AssetDatabase.DeleteAsset(path);
        EditorUtility.SetDirty(skill);
    }

    void OnDestroy()
    {
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        GC.Collect();
    }

    static Skill RegetditSkill(SkillTreeViewItem item)
    {
        SkillBase data = item.DataContext as SkillBase;
        Skill skill = null;
        if (_skillDic.ContainsKey(data.skillId))
        {
            skill = _skillDic[data.skillId];
        }
        else
        {
            skill = Resources.Load(data.resPath) as Skill;
            if (skill != null)
            {
                _skillDic[data.skillId] = skill;
            }
        }
        return skill;
    }

    static void CreateSkillItem()
    {
        List<Skill> skills = new List<Skill>();
        foreach (var node in _skillDic)
        {
            if (node.Value != null)
            {
                skills.Add(node.Value);
            }
        }
        for (int i = 0; i < skills.Count; ++i)
        {
            SkillTreeViewItem skillItem = AddSkill(skills[i].skillId, false);
            skillItem.IsExpanded = false;
            if (skillItem == null)
            {
                continue;
            }
            for (int n = 0; n < skills[i].animaEvent.Count; n++)
            {
                SkillTreeViewItem evtItem = AddSkillEventNode(skillItem, skills[i].animaEvent[n].name, false);
            }
        }
    }

    /// <summary>
    /// 获取技能配置数据
    /// </summary>
    static void GetSkillData()
    {
        try
        {
            DirectoryInfo parentFolder = new DirectoryInfo(FixPath);
            //遍历文件夹
            foreach (DirectoryInfo folder in parentFolder.GetDirectories())
            {
                Skill skill = Resources.Load("SkillFightData/Skill/" + folder.Name + "/Skill") as Skill;
                if (skill == null)
                    continue;
                _skillDic[skill.skillId] = skill;
            }
        }
        catch (Exception e)
        {
            Debug.LogError(e);
        }
    }
}
