# Unity Editor Handles 示例

本示例展示了 Unity Editor Handles API 的各种功能和用法。主要通过 `CustomHandlesExample.cs` 脚本实现，提供了丰富的场景编辑器交互功能。

## 实现过程

### 1. 基础结构设计

创建了两个主要类：

- `HandlesGUITarget`: 用于存储和配置所有可编辑的属性
- `CustomHandlesExample`: 继承自 `Editor`，实现具体的编辑器功能


### 2. 属性配置

#### 基础变换控制

```csharp
public Vector3 customPosition;
public Quaternion customRotation = Quaternion.identity;
public Vector3 customScale = Vector3.one;
```

#### 形状控制

```csharp
public float radius = 5f;
public Vector3 offset = Vector3.zero;
public Color areaColor = Color.yellow;
public bool showWireSphere = true;
```

#### 高级控制

```csharp
public float sliderValue = 0f;
public Vector3 bezierStart = Vector3.zero;
public Vector3 bezierEnd = Vector3.forward * 5f;
public Vector3 bezierStartTangent = Vector3.up * 2f;
public Vector3 bezierEndTangent = Vector3.up * 2f;
```


### 3. 功能实现

#### 基础变换手柄

- 位置控制 (Position Handle)
- 旋转控制 (Rotation Handle)
- 缩放控制 (Scale Handle)

#### 形状绘制

- 线框球体
- 矩形区域
- 圆锥体
- 自定义网格

#### 贝塞尔曲线编辑

- 曲线可视化
- 控制点编辑
- 切线控制

#### 光源和相机控制

- 光源范围可视化
- 光照强度控制
- 相机视锥体
- FOV 调节

#### 自定义手柄样式

- 球形手柄
- 立方体手柄
- 圆锥手柄
- 圆柱手柄


### 4. 交互功能

#### 键盘快捷键

- `G`: 切换网格吸附
- `R`: 切换旋转模式

#### 测量工具

- 距离测量
- 角度测量
- 比例可视化
- 方向指示器

## 使用方法

1. 通过菜单创建示例：
   - 选择 `Editor > Scene GUI Example`
   - 会自动创建一个带有 `HandlesGUITarget` 组件的游戏对象

2. 在 Inspector 中配置：
   - 基础变换参数
   - 形状参数
   - 显示选项
   - 视觉效果设置

3. 场景视图交互：
   - 使用各种手柄进行变换操作
   - 查看可视化效果
   - 使用快捷键进行控制

## 特性列表

- [x] 基础变换控制
- [x] 形状绘制和编辑
- [x] 贝塞尔曲线编辑
- [x] 光源控制
- [x] 相机控制
- [x] 自定义手柄样式
- [x] 测量工具
- [x] 键盘快捷键
- [x] Undo/Redo 支持
- [x] 网格吸附
- [x] 场景视图自动刷新

## 注意事项

1. 所有编辑操作都支持 Undo/Redo
2. 使用 `EditorGUI.BeginChangeCheck()` 和 `EndChangeCheck()` 跟踪变化
3. 保持场景视图的实时更新
4. 合理使用 `Handles.color` 和 `Handles.matrix` 的保存和恢复
5. 注意性能优化，避免不必要的重绘

## 扩展建议

1. 添加更多自定义手柄样式
2. 实现更复杂的交互逻辑
3. 添加更多的测量工具
4. 优化性能和内存使用
5. 添加更多的快捷键支持

## 参考文档

- [Unity Handles Documentation](https://docs.unity3d.com/ScriptReference/Handles.html)
- [Unity Editor Scripting](https://docs.unity3d.com/Manual/editor-scripting.html)
- [Custom Editors](https://docs.unity3d.com/Manual/editor-CustomEditors.html)