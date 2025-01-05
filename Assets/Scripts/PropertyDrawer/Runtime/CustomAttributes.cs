using UnityEngine;
using System;

namespace EditorTeaching
{
    #region Basic Attributes
    /// <summary>
    /// 自定义范围属性：用于创建最小/最大值范围滑块
    /// </summary>
    public class MinMaxRangeAttribute : PropertyAttribute
    {
        public float minValue;
        public float maxValue;

        public MinMaxRangeAttribute(float min, float max)
        {
            minValue = min;
            maxValue = max;
        }
    }

    /// <summary>
    /// 只读属性：使字段在 Inspector 中只读
    /// </summary>
    public class ReadOnlyAttribute : PropertyAttribute { }

    /// <summary>
    /// 条件显示属性：基于条件显示或隐藏字段
    /// </summary>
    public class ConditionalAttribute : PropertyAttribute
    {
        public string conditionField;
        public object compareValue;

        public ConditionalAttribute(string field, object value)
        {
            conditionField = field;
            compareValue = value;
        }
    }
    #endregion

    #region Visual Attributes
    /// <summary>
    /// 预览属性：为纹理添加预览功能
    /// </summary>
    public class PreviewAttribute : PropertyAttribute
    {
        public float previewSize = 64f;
        public PreviewAttribute(float size = 64f)
        {
            previewSize = size;
        }
    }

    /// <summary>
    /// 按钮属性：添加可点击按钮
    /// </summary>
    public class ButtonAttribute : PropertyAttribute
    {
        public string buttonText;
        public ButtonAttribute(string text = "Execute")
        {
            buttonText = text;
        }
    }

    /// <summary>
    /// 进度条属性：将float或int显示为进度条
    /// </summary>
    public class ProgressBarAttribute : PropertyAttribute
    {
        public string title;
        public float maxValue;
        public Color barColor;

        public ProgressBarAttribute(string title = "", float maxValue = 100f)
        {
            this.title = title;
            this.maxValue = maxValue;
            this.barColor = Color.green;
        }
    }

    /// <summary>
    /// 装饰器属性：在字段上方显示标题和说明
    /// </summary>
    public class HeaderDecoratorAttribute : PropertyAttribute
    {
        public string header;
        public string description;
        public MessageType messageType;

        public HeaderDecoratorAttribute(string header, string description = "", MessageType messageType = MessageType.Info)
        {
            this.header = header;
            this.description = description;
            this.messageType = messageType;
        }
    }
    #endregion

    #region Validation Attributes
    /// <summary>
    /// 验证属性：确保字符串不为空
    /// </summary>
    public class NotNullOrEmptyAttribute : PropertyAttribute { }

    /// <summary>
    /// 验证属性：确保数值在指定范围内
    /// </summary>
    public class ValidateNumberAttribute : PropertyAttribute
    {
        public float min;
        public float max;
        public string message;

        public ValidateNumberAttribute(float min = float.MinValue, float max = float.MaxValue, string message = "")
        {
            this.min = min;
            this.max = max;
            this.message = message;
        }
    }
    #endregion

    #region Layout Attributes
    /// <summary>
    /// 布局属性：在一行内显示多个字段
    /// </summary>
    public class InlinePropertyAttribute : PropertyAttribute
    {
        public float[] weights;
        public InlinePropertyAttribute(params float[] columnWeights)
        {
            weights = columnWeights.Length > 0 ? columnWeights : new float[] { 1 };
        }
    }

    /// <summary>
    /// 分组属性：将多个字段组合在一起
    /// </summary>
    public class GroupAttribute : PropertyAttribute
    {
        public string groupName;
        public bool expanded;

        public GroupAttribute(string name, bool startExpanded = true)
        {
            groupName = name;
            expanded = startExpanded;
        }
    }
    #endregion

    #region Helper Enums
    public enum MessageType
    {
        None,
        Info,
        Warning,
        Error
    }
    #endregion
}