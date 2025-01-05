using UnityEngine;

public class CustomGizmosExample : MonoBehaviour
{
    [Header("Basic Settings")]
    [SerializeField] private Color m_WireframeColor = Color.green;
    [SerializeField] private Color m_SolidColor = new Color(1f, 0f, 0f, 0.5f);
    [SerializeField] private float m_Radius = 1f;
    [SerializeField] private Vector3 m_Size = Vector3.one;
    [SerializeField] private float m_ArrowLength = 2f;
    [SerializeField] private Mesh m_CustomMesh;
    [SerializeField] private Material m_CustomMaterial;
    [SerializeField] private Texture2D m_Icon;

    [Header("Advanced Settings")]
    [SerializeField] private bool m_ShowWireframe = true;
    [SerializeField] private bool m_ShowSolid = true;
    [SerializeField] private bool m_ShowIcons = true;
    [SerializeField] private bool m_ShowMesh = true;
    [SerializeField] private float m_FrustumFOV = 60f;
    [SerializeField] private float m_FrustumMaxRange = 10f;
    [SerializeField] private float m_FrustumMinRange = 0.3f;
    [SerializeField] private float m_FrustumAspect = 1.6f;

    private void OnDrawGizmos()
    {
        // 保存原始颜色和矩阵
        Color originalColor = Gizmos.color;
        Matrix4x4 originalMatrix = Gizmos.matrix;

        DrawWireframeGizmos();
        DrawSolidGizmos();
        DrawCustomMeshGizmos();
        DrawIconGizmos();
        DrawFrustumGizmos();

        // 恢复原始设置
        Gizmos.color = originalColor;
        Gizmos.matrix = originalMatrix;
    }

    private void DrawWireframeGizmos()
    {
        if (!m_ShowWireframe) return;

        Gizmos.color = m_WireframeColor;

        // 线框球体
        Gizmos.DrawWireSphere(transform.position, m_Radius);

        // 线框立方体
        Vector3 cubePos = transform.position + Vector3.right * 3f;
        Gizmos.DrawWireCube(cubePos, m_Size);

        // 射线
        Vector3 rayStart = transform.position + Vector3.up * 2f;
        Gizmos.DrawRay(new Ray(rayStart, transform.forward * m_ArrowLength));

        // 线段
        Vector3 lineStart = transform.position + Vector3.left * 2f;
        Vector3 lineEnd = lineStart + Vector3.up * m_ArrowLength;
        Gizmos.DrawLine(lineStart, lineEnd);
    }

    private void DrawSolidGizmos()
    {
        if (!m_ShowSolid) return;

        Gizmos.color = m_SolidColor;

        // 实心球体
        Vector3 spherePos = transform.position + Vector3.right * -3f;
        Gizmos.DrawSphere(spherePos, m_Radius);

        // 实心立方体
        Vector3 cubePos = transform.position + Vector3.forward * 3f;
        Gizmos.DrawCube(cubePos, m_Size);

        // 使用矩阵变换绘制旋转的立方体
        Vector3 rotatedCubePos = transform.position + Vector3.forward * -3f;
        Gizmos.matrix = Matrix4x4.TRS(
            rotatedCubePos,
            Quaternion.Euler(45f, 45f, 45f),
            Vector3.one
        );
        Gizmos.DrawCube(Vector3.zero, m_Size);
    }

    private void DrawCustomMeshGizmos()
    {
        if (!m_ShowMesh || m_CustomMesh == null) return;

        Gizmos.color = m_WireframeColor;
        Vector3 meshPos = transform.position + Vector3.up * 3f;

        // 绘制网格线框
        Gizmos.DrawWireMesh(m_CustomMesh, meshPos, transform.rotation, m_Size);

        if (m_CustomMaterial != null)
        {
            // 绘制带材质的网格
            Gizmos.color = m_SolidColor;
            Gizmos.DrawMesh(m_CustomMesh, meshPos, transform.rotation, m_Size);
        }
    }

    private void DrawIconGizmos()
    {
        if (!m_ShowIcons || m_Icon == null) return;

        // 在不同位置绘制图标
        Vector3 iconPos = transform.position + Vector3.up * 4f;
        Gizmos.DrawIcon(iconPos, m_Icon.name, true);
    }

    private void DrawFrustumGizmos()
    {
        // 绘制视锥体
        Vector3 frustumPos = transform.position + Vector3.right * 5f;
        Gizmos.matrix = Matrix4x4.TRS(frustumPos, transform.rotation, Vector3.one);
        Gizmos.DrawFrustum(
            Vector3.zero,
            m_FrustumFOV,
            m_FrustumMaxRange,
            m_FrustumMinRange,
            m_FrustumAspect
        );
    }

    private void OnDrawGizmosSelected()
    {
        // 当物体被选中时绘制的额外 Gizmos
        Gizmos.color = Color.yellow;

        // 绘制方向指示器
        DrawDirectionIndicator();

        // 绘制边界框
        DrawBoundingBox();
    }

    private void DrawDirectionIndicator()
    {
        float arrowSize = m_ArrowLength * 0.2f;
        Vector3 direction = transform.forward * m_ArrowLength;
        Vector3 right = transform.right * arrowSize;
        Vector3 up = transform.up * arrowSize;
        Vector3 arrowTip = transform.position + direction;

        Gizmos.DrawRay(transform.position, direction);
        Gizmos.DrawRay(arrowTip, -right - up);
        Gizmos.DrawRay(arrowTip, -right + up);
        Gizmos.DrawRay(arrowTip, right - up);
        Gizmos.DrawRay(arrowTip, right + up);
    }

    private void DrawBoundingBox()
    {
        // 绘制包围盒
        Bounds bounds = new Bounds(transform.position, m_Size * 2f);
        Gizmos.DrawWireCube(bounds.center, bounds.size);
    }
}
