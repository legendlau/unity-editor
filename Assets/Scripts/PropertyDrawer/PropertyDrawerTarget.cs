using UnityEngine;
using System;

namespace EditorTeaching
{
    /// <summary>
    /// 示例：可序列化的复杂类型
    /// </summary>
    [Serializable]
    public class ComplexType
    {
        public string name = "Default";
        public Color color = Color.white;
        public Vector3 position;
    }

    /// <summary>
    /// 示例：带有自定义属性的数据类
    /// 此类展示了各种自定义属性绘制器的使用方法
    /// </summary>
    public class PropertyDrawerTarget : MonoBehaviour
    {
        [Header("Basic Properties")]
        [Tooltip("使用自定义范围滑块控制的健康值范围")]
        [MinMaxRange(0f, 100f)]
        public Vector2 healthRange = new Vector2(20f, 80f);

        [Tooltip("只读字段示例")]
        [ReadOnly]
        public string identifier = "READ_ONLY_ID";

        [Tooltip("控制高级选项显示的开关")]
        [Conditional("showAdvanced", true)]
        public bool showAdvanced = false;

        [Tooltip("仅在showAdvanced为true时显示")]
        [Conditional("showAdvanced", true)]
        public float advancedValue = 0f;

        [Header("Visual Properties")]
        [Tooltip("带有预览功能的纹理字段")]
        [Preview(128f)]
        public Texture2D previewTexture;

        [Tooltip("点击按钮重置所有值")]
        [Button("Reset Values")]
        public bool resetTrigger;

        [Header("Complex Type Example")]
        [Tooltip("自定义复杂类型示例")]
        public ComplexType complexData = new ComplexType();

        [Header("Array Example")]
        [Tooltip("复杂类型数组示例")]
        public ComplexType[] dataArray = new ComplexType[0];

        /// <summary>
        /// 重置所有值到默认状态
        /// </summary>
        public void Reset()
        {
            identifier = "READ_ONLY_ID_" + UnityEngine.Random.Range(1000, 9999);
            healthRange = new Vector2(20f, 80f);
            advancedValue = 0f;
            showAdvanced = false;

            // 初始化复杂类型
            if (complexData == null)
            {
                complexData = new ComplexType
                {
                    name = "Default",
                    color = Color.white,
                    position = Vector3.zero
                };
            }

            // 清空数组
            dataArray = new ComplexType[0];
        }

        private void OnValidate()
        {
            // 确保复杂类型不为空
            if (complexData == null)
            {
                complexData = new ComplexType();
            }

            // 确保数组不为空
            if (dataArray == null)
            {
                dataArray = new ComplexType[0];
            }
        }
    }
}