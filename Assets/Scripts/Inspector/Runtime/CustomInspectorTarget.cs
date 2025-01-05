using UnityEngine;
using System;
using System.Collections.Generic;

namespace EditorTeaching.Inspector
{
    /// <summary>
    /// 武器类型枚举
    /// </summary>
    public enum WeaponType
    {
        None,
        Sword,
        Bow,
        Staff,
        Shield
    }

    /// <summary>
    /// 技能数据
    /// </summary>
    [Serializable]
    public class SkillData
    {
        [Tooltip("技能名称")]
        public string skillName;

        [Tooltip("技能伤害")]
        public float damage;

        [Tooltip("冷却时间")]
        public float cooldown;

        [Tooltip("技能图标")]
        public Sprite icon;
    }

    /// <summary>
    /// 示例目标组件，用于演示自定义检查器功能
    /// </summary>
    public class CustomInspectorTarget : MonoBehaviour
    {
        #region Basic Settings
        [Header("Basic Settings")]

        [Tooltip("玩家名称")]
        public string playerName = "Player";

        [Tooltip("生命值")]
        [Range(0, 200)]
        public int health = 100;

        [Tooltip("移动速度")]
        [Range(1f, 10f)]
        public float moveSpeed = 5f;

        [Tooltip("是否无敌")]
        [SerializeField]
        private bool isInvincible;
        #endregion

        #region Advanced Settings
        [Header("Advanced Settings")]

        [Tooltip("武器类型")]
        public WeaponType weaponType;

        [Tooltip("装备的武器预制体")]
        public GameObject weaponPrefab;

        [Tooltip("武器材质")]
        public Material weaponMaterial;

        [Tooltip("武器特效")]
        public ParticleSystem weaponEffect;

        [Tooltip("武器音效")]
        public AudioClip weaponSound;
        #endregion

        #region Visual Settings
        [Header("Visual Settings")]

        [Tooltip("角色模型")]
        public Mesh characterMesh;

        [Tooltip("角色贴图")]
        public Texture2D characterTexture;

        [Tooltip("角色颜色")]
        public Color characterColor = Color.white;

        [Tooltip("自定义着色器")]
        public Shader customShader;
        #endregion

        #region Skill System
        [Header("Skill System")]

        [Tooltip("技能列表")]
        public List<SkillData> skills = new List<SkillData>();

        [Tooltip("技能点")]
        [Range(0, 10)]
        public int skillPoints = 0;

        [Tooltip("技能冷却时间修正")]
        [Range(0.5f, 2f)]
        public float cooldownModifier = 1f;
        #endregion

        #region Physics Settings
        [Header("Physics Settings")]

        [Tooltip("重力缩放")]
        [Range(0f, 3f)]
        public float gravityScale = 1f;

        [Tooltip("跳跃力度")]
        [Range(1f, 20f)]
        public float jumpForce = 10f;

        [Tooltip("地面检测层")]
        public LayerMask groundLayer;

        [Tooltip("碰撞检测范围")]
        public Vector3 collisionBounds = new Vector3(1f, 2f, 1f);
        #endregion

        #region Debug Settings
        [Header("Debug Settings")]

        [Tooltip("显示调试信息")]
        public bool showDebugInfo;

        [Tooltip("调试颜色")]
        public Color debugColor = Color.yellow;

        [Tooltip("调试范围")]
        [Range(0.1f, 5f)]
        public float debugRange = 1f;
        #endregion

        #region Properties
        /// <summary>
        /// 获取或设置无敌状态
        /// </summary>
        public bool IsInvincible
        {
            get => isInvincible;
            set => isInvincible = value;
        }
        #endregion

        #region Unity Events
        [Header("Unity Events")]

        [Tooltip("受伤事件")]
        public UnityEngine.Events.UnityEvent onDamaged;

        [Tooltip("治疗事件")]
        public UnityEngine.Events.UnityEvent onHealed;

        [Tooltip("等级提升事件")]
        public UnityEngine.Events.UnityEvent onLevelUp;
        #endregion

        #region Vector Fields
        [Header("Transform Settings")]

        [Tooltip("本地位置")]
        public Vector3 localPosition;

        [Tooltip("本地旋转")]
        public Vector3 localRotation;

        [Tooltip("本地缩放")]
        public Vector3 localScale = Vector3.one;

        [Tooltip("锚点偏移")]
        public Vector2 anchorOffset;
        #endregion

        #region Curve Settings
        [Header("Animation Settings")]

        [Tooltip("动画曲线")]
        [SerializeField]
        private AnimationCurve animationCurve = new AnimationCurve(
            new Keyframe(0, 0, 0, 1),    // 起始帧：时间0，值0，入切线0，出切线1
            new Keyframe(0.5f, 1, 2, 2), // 中间帧：时间0.5，值1，入切线2，出切线2
            new Keyframe(1, 0, 1, 0)     // 结束帧：时间1，值0，入切线1，出切线0
        );

        [Tooltip("渐变曲线")]
        public Gradient colorGradient = new Gradient()
        {
            colorKeys = new GradientColorKey[]
            {
                new GradientColorKey(Color.red, 0.0f),
                new GradientColorKey(Color.yellow, 0.5f),
                new GradientColorKey(Color.green, 1.0f)
            },
            alphaKeys = new GradientAlphaKey[]
            {
                new GradientAlphaKey(1.0f, 0.0f),
                new GradientAlphaKey(1.0f, 1.0f)
            }
        };
        #endregion
    }
}