# Unity PropertyDrawer 示例

本示例展示了 Unity PropertyDrawer 和 CustomPropertyDrawer 的各种功能和用法。主要通过 `PropertyDrawerExample.cs` 脚本实现，提供了丰富的自定义属性绘制器功能。

## 实现过程

### 1. 自定义属性 (Attributes)

创建了多个自定义属性类：

```csharp
// 范围滑块属性
public class MinMaxRangeAttribute : PropertyAttribute
{
    public float minValue;
    public float maxValue;
}

// 只读属性
public class ReadOnlyAttribute : PropertyAttribute { }

// 条件显示属性
public class ConditionalAttribute : PropertyAttribute
{
    public string conditionField;
    public object compareValue;
}

// 预览属性
public class PreviewAttribute : PropertyAttribute
{
    public float previewSize = 64f;
}

// 按钮属性
public class ButtonAttribute : PropertyAttribute
{
    public string buttonText;
}
```

### 2. 可序列化的复杂类型

```csharp
[Serializable]
public class ComplexType
{
    public string name = "Default";
    public Color color = Color.white;
    public Vector3 position;
}
```

### 3. 目标组件类

创建了一个包含各种自定义属性的组件类：

```csharp
public class PropertyDrawerTarget : MonoBehaviour
{
    [MinMaxRange(0f, 100f)]
    public Vector2 healthRange;

    [ReadOnly]
    public string identifier;

    [Conditional("showAdvanced", true)]
    public bool showAdvanced;

    [Preview(128f)]
    public Texture2D previewTexture;

    [Button("Reset Values")]
    public bool resetTrigger;
}
```

### 4. 属性绘制器实现

#### MinMaxRange 绘制器
- 实现自定义范围滑块
- 显示当前最小值和最大值
- 支持拖动调节

#### ReadOnly 绘制器
- 禁用字段编辑
- 保持字段显示

#### Conditional 绘制器
- 基于条件显示/隐藏字段
- 支持布尔值和枚举类型条件
- 动态调整布局

#### Preview 绘制器
- 显示纹理预览
- 可配置预览大小
- 支持所有 Texture 类型

#### Button 绘制器
- 创建可点击按钮
- 触发自定义操作
- 支持 Undo/Redo

#### ComplexType 绘制器
- 自定义复杂类型布局
- 优化显示效果
- 支持多字段编辑

## 功能特性

1. **范围控制**
   - 自定义最小/最大值范围
   - 可视化滑块控制
   - 实时数值显示

2. **字段保护**
   - 只读字段支持
   - 防止意外修改

3. **条件显示**
   - 基于其他字段值显示/隐藏
   - 动态布局调整
   - 支持多种条件类型

4. **资源预览**
   - 内联纹理预览
   - 可配置预览大小
   - 实时更新

5. **交互控制**
   - 自定义按钮
   - 操作响应
   - 状态重置

6. **复杂类型支持**
   - 自定义类型布局
   - 多字段组合显示
   - 直观的编辑界面

## 使用方法

1. 将脚本放在 Editor 文件夹中
2. 创建使用这些属性的组件
3. 在 Inspector 中查看效果

示例：
```csharp
[MinMaxRange(0, 100)]
public Vector2 health;

[ReadOnly]
public string id;

[Preview(128)]
public Texture2D icon;
```

## 注意事项

1. 所有自定义绘制器都支持：
   - Undo/Redo 操作
   - 预制体修改
   - 多对象编辑

2. 性能考虑：
   - 避免在 OnGUI 中进行耗时操作
   - 合理使用缓存
   - 注意绘制区域的计算

3. 布局注意：
   - 正确处理缩进级别
   - 计算准确的属性高度
   - 处理好多行显示

## 扩展建议

1. 添加更多条件类型支持
2. 实现更复杂的预览功能
3. 添加自定义验证规则
4. 实现更多交互控制
5. 优化多对象编辑体验

## 参考文档

- [Unity PropertyDrawer Documentation](https://docs.unity3d.com/ScriptReference/PropertyDrawer.html)
- [Unity CustomPropertyDrawer Documentation](https://docs.unity3d.com/ScriptReference/CustomPropertyDrawer.html)
- [Unity SerializedProperty Documentation](https://docs.unity3d.com/ScriptReference/SerializedProperty.html)
- [Unity Editor GUI Documentation](https://docs.unity3d.com/ScriptReference/EditorGUI.html)