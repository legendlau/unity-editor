using UnityEngine;
using UnityEditor;

namespace EditorTeaching
{
    #region Basic Drawers
    [CustomPropertyDrawer(typeof(MinMaxRangeAttribute))]
    public class MinMaxRangeDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            MinMaxRangeAttribute range = attribute as MinMaxRangeAttribute;
            if (property.propertyType == SerializedPropertyType.Vector2)
            {
                Vector2 vector = property.vector2Value;
                float minValue = vector.x;
                float maxValue = vector.y;

                EditorGUI.MinMaxSlider(
                    new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight),
                    label,
                    ref minValue,
                    ref maxValue,
                    range.minValue,
                    range.maxValue
                );

                EditorGUI.LabelField(
                    new Rect(position.x, position.y + EditorGUIUtility.singleLineHeight, position.width, EditorGUIUtility.singleLineHeight),
                    $"Range: {minValue:F2} - {maxValue:F2}"
                );

                property.vector2Value = new Vector2(minValue, maxValue);
            }

            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUIUtility.singleLineHeight * 2;
        }
    }

    [CustomPropertyDrawer(typeof(ReadOnlyAttribute))]
    public class ReadOnlyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginDisabledGroup(true);
            EditorGUI.PropertyField(position, property, label, true);
            EditorGUI.EndDisabledGroup();
        }
    }

    [CustomPropertyDrawer(typeof(ConditionalAttribute))]
    public class ConditionalDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            ConditionalAttribute condition = attribute as ConditionalAttribute;
            SerializedProperty conditionProperty = property.serializedObject.FindProperty(condition.conditionField);

            bool showField = true;
            if (conditionProperty != null)
            {
                switch (conditionProperty.propertyType)
                {
                    case SerializedPropertyType.Boolean:
                        showField = conditionProperty.boolValue == (bool)condition.compareValue;
                        break;
                    case SerializedPropertyType.Enum:
                        showField = conditionProperty.enumValueIndex == (int)condition.compareValue;
                        break;
                }
            }

            if (showField)
            {
                EditorGUI.PropertyField(position, property, label, true);
            }
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            ConditionalAttribute condition = attribute as ConditionalAttribute;
            SerializedProperty conditionProperty = property.serializedObject.FindProperty(condition.conditionField);

            bool showField = true;
            if (conditionProperty != null && conditionProperty.propertyType == SerializedPropertyType.Boolean)
            {
                showField = conditionProperty.boolValue == (bool)condition.compareValue;
            }

            if (!showField)
                return 0f;

            return EditorGUI.GetPropertyHeight(property, label, true);
        }
    }
    #endregion

    #region Visual Drawers
    [CustomPropertyDrawer(typeof(PreviewAttribute))]
    public class PreviewDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            PreviewAttribute preview = attribute as PreviewAttribute;
            float previewSize = preview.previewSize;

            Rect propertyRect = new Rect(position.x, position.y, position.width - previewSize - 5f, EditorGUIUtility.singleLineHeight);
            EditorGUI.PropertyField(propertyRect, property, label);

            if (property.propertyType == SerializedPropertyType.ObjectReference && property.objectReferenceValue is Texture)
            {
                Rect previewRect = new Rect(
                    position.x + position.width - previewSize,
                    position.y,
                    previewSize,
                    previewSize
                );
                EditorGUI.DrawPreviewTexture(previewRect, (Texture)property.objectReferenceValue);
            }

            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            PreviewAttribute preview = attribute as PreviewAttribute;
            return Mathf.Max(preview.previewSize, EditorGUIUtility.singleLineHeight);
        }
    }

    [CustomPropertyDrawer(typeof(ButtonAttribute))]
    public class ButtonDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            ButtonAttribute buttonAttr = attribute as ButtonAttribute;

            if (GUI.Button(position, buttonAttr.buttonText))
            {
                var target = property.serializedObject.targetObject;
                var method = target.GetType().GetMethod("Reset", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
                if (method != null)
                {
                    method.Invoke(target, null);
                    EditorUtility.SetDirty(target);
                }
            }
        }
    }

    [CustomPropertyDrawer(typeof(ProgressBarAttribute))]
    public class ProgressBarDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            ProgressBarAttribute progressAttr = attribute as ProgressBarAttribute;

            if (property.propertyType == SerializedPropertyType.Float ||
                property.propertyType == SerializedPropertyType.Integer)
            {
                float value = property.propertyType == SerializedPropertyType.Float ?
                    property.floatValue : property.intValue;

                float normalizedValue = Mathf.Clamp01(value / progressAttr.maxValue);

                EditorGUI.ProgressBar(position, normalizedValue,
                    string.IsNullOrEmpty(progressAttr.title) ? label.text : progressAttr.title);
            }
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUIUtility.singleLineHeight;
        }
    }

    [CustomPropertyDrawer(typeof(HeaderDecoratorAttribute))]
    public class HeaderDecoratorDrawer : DecoratorDrawer
    {
        private UnityEditor.MessageType ConvertMessageType(EditorTeaching.MessageType type)
        {
            switch (type)
            {
                case EditorTeaching.MessageType.None:
                    return UnityEditor.MessageType.None;
                case EditorTeaching.MessageType.Info:
                    return UnityEditor.MessageType.Info;
                case EditorTeaching.MessageType.Warning:
                    return UnityEditor.MessageType.Warning;
                case EditorTeaching.MessageType.Error:
                    return UnityEditor.MessageType.Error;
                default:
                    return UnityEditor.MessageType.None;
            }
        }

        public override void OnGUI(Rect position)
        {
            HeaderDecoratorAttribute headerAttr = attribute as HeaderDecoratorAttribute;

            EditorGUI.LabelField(position, headerAttr.header, EditorStyles.boldLabel);

            if (!string.IsNullOrEmpty(headerAttr.description))
            {
                Rect helpBoxRect = position;
                helpBoxRect.y += EditorGUIUtility.singleLineHeight;
                EditorGUI.HelpBox(helpBoxRect, headerAttr.description, ConvertMessageType(headerAttr.messageType));
            }
        }

        public override float GetHeight()
        {
            HeaderDecoratorAttribute headerAttr = attribute as HeaderDecoratorAttribute;
            float height = EditorGUIUtility.singleLineHeight;

            if (!string.IsNullOrEmpty(headerAttr.description))
            {
                height += EditorGUIUtility.singleLineHeight * 2;
            }

            return height;
        }
    }
    #endregion

    #region Validation Drawers
    [CustomPropertyDrawer(typeof(NotNullOrEmptyAttribute))]
    public class NotNullOrEmptyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            bool isValid = true;
            if (property.propertyType == SerializedPropertyType.String)
            {
                isValid = !string.IsNullOrEmpty(property.stringValue);
            }
            else if (property.propertyType == SerializedPropertyType.ObjectReference)
            {
                isValid = property.objectReferenceValue != null;
            }

            Color originalColor = GUI.color;
            if (!isValid)
            {
                GUI.color = Color.red;
            }

            EditorGUI.PropertyField(position, property, label);
            GUI.color = originalColor;

            if (!isValid)
            {
                EditorGUILayout.HelpBox("This field cannot be null or empty.", UnityEditor.MessageType.Error);
            }

            EditorGUI.EndProperty();
        }
    }

    [CustomPropertyDrawer(typeof(ValidateNumberAttribute))]
    public class ValidateNumberDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            ValidateNumberAttribute validateAttr = attribute as ValidateNumberAttribute;
            bool isValid = true;
            float value = 0f;

            if (property.propertyType == SerializedPropertyType.Float)
            {
                value = property.floatValue;
                isValid = value >= validateAttr.min && value <= validateAttr.max;
            }
            else if (property.propertyType == SerializedPropertyType.Integer)
            {
                value = property.intValue;
                isValid = value >= validateAttr.min && value <= validateAttr.max;
            }

            Color originalColor = GUI.color;
            if (!isValid)
            {
                GUI.color = Color.red;
            }

            EditorGUI.PropertyField(position, property, label);
            GUI.color = originalColor;

            if (!isValid && !string.IsNullOrEmpty(validateAttr.message))
            {
                EditorGUILayout.HelpBox(validateAttr.message, UnityEditor.MessageType.Error);
            }

            EditorGUI.EndProperty();
        }
    }
    #endregion

    #region Layout Drawers
    [CustomPropertyDrawer(typeof(InlinePropertyAttribute))]
    public class InlinePropertyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            InlinePropertyAttribute inlineAttr = attribute as InlinePropertyAttribute;
            float[] weights = inlineAttr.weights;

            position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

            float totalWeight = 0f;
            foreach (float weight in weights) totalWeight += weight;

            float xPos = position.x;
            float remainingWidth = position.width;

            SerializedProperty iterator = property.Copy();
            bool enterChildren = true;
            int fieldIndex = 0;

            while (iterator.NextVisible(enterChildren) && fieldIndex < weights.Length)
            {
                enterChildren = false;
                if (iterator.depth > property.depth)
                {
                    float fieldWidth = (weights[fieldIndex] / totalWeight) * position.width - 5f;
                    Rect fieldRect = new Rect(xPos, position.y, fieldWidth, position.height);

                    EditorGUI.PropertyField(fieldRect, iterator, GUIContent.none);

                    xPos += fieldWidth + 5f;
                    remainingWidth -= fieldWidth + 5f;
                    fieldIndex++;
                }
            }

            EditorGUI.EndProperty();
        }
    }

    [CustomPropertyDrawer(typeof(GroupAttribute))]
    public class GroupDrawer : PropertyDrawer
    {
        private bool foldout;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            GroupAttribute groupAttr = attribute as GroupAttribute;

            Rect foldoutRect = position;
            foldoutRect.height = EditorGUIUtility.singleLineHeight;

            foldout = EditorGUI.Foldout(foldoutRect, groupAttr.expanded, groupAttr.groupName, true);

            if (foldout)
            {
                EditorGUI.indentLevel++;

                Rect propertyRect = position;
                propertyRect.y += EditorGUIUtility.singleLineHeight;
                propertyRect.height = EditorGUIUtility.singleLineHeight;

                EditorGUI.PropertyField(propertyRect, property, label, true);

                EditorGUI.indentLevel--;
            }

            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            if (!foldout)
                return EditorGUIUtility.singleLineHeight;

            return EditorGUIUtility.singleLineHeight +
                   EditorGUI.GetPropertyHeight(property, label, true);
        }
    }
    #endregion

    #region Custom Editor
    [CustomEditor(typeof(PropertyDrawerTarget))]
    public class PropertyDrawerTargetEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.HelpBox(
                "This example demonstrates various PropertyDrawer features:\n" +
                "1. Basic Features\n" +
                "   - MinMaxRange: Custom range slider\n" +
                "   - ReadOnly: Non-editable fields\n" +
                "   - Conditional: Show/hide based on conditions\n\n" +
                "2. Visual Features\n" +
                "   - Preview: Texture preview\n" +
                "   - Button: Custom actions\n" +
                "   - ProgressBar: Visual progress\n" +
                "   - HeaderDecorator: Custom headers\n\n" +
                "3. Validation Features\n" +
                "   - NotNullOrEmpty: Null validation\n" +
                "   - ValidateNumber: Number range validation\n\n" +
                "4. Layout Features\n" +
                "   - InlineProperty: Multiple fields in one line\n" +
                "   - Group: Collapsible groups",
                UnityEditor.MessageType.Info);

            DrawDefaultInspector();

            serializedObject.ApplyModifiedProperties();
        }
    }
    #endregion
}