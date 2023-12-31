using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[AddComponentMenu("UI/CurvedSurfaceImage")]
public class CurvedSurfaceImage : Image
{
    [SerializeField]
    public float m_Curvature = 20;
    float m_Width;
    float m_Height;
    float curvature => -m_Height * (m_Curvature / 100);
    protected override void OnPopulateMesh(VertexHelper toFill)
    {
        GenerateCurvedSprite(toFill);
    }
    void GenerateCurvedSprite(VertexHelper toFill)
    {
        var color32 = color;

        toFill.Clear();
        m_Width = rectTransform.rect.width;
        m_Height = rectTransform.rect.height;
        var xPoint = m_Width / 2;
        var yPoint = m_Height / 2;

        var leftStartPos = new Vector3(-xPoint, -yPoint);
        var leftMiddlePos = new Vector3(-xPoint, 0, curvature);
        var leftEndPos = new Vector3(-xPoint, yPoint);
        List<Vector3> leftLinePoints = BezierCurve.GetCurvePoints(new Vector3[] { leftStartPos, leftMiddlePos, leftEndPos }, 0.01f);
        var rightStartPos = new Vector3(xPoint, -yPoint);
        var rightMiddlePos = new Vector3(xPoint, 0, curvature);
        var rightEndPos = new Vector3(xPoint, yPoint);
        List<Vector3> rightLinePoints = BezierCurve.GetCurvePoints(new Vector3[] { rightStartPos, rightMiddlePos, rightEndPos }, 0.01f);

        List<UIVertex> vertices = new List<UIVertex>();
        vertices.AddRange(GetUIVertex(leftLinePoints, color32, m_Width, m_Height));
        vertices.AddRange(GetUIVertex(rightLinePoints, color32, m_Width, m_Height));

        List<int> indices = new List<int>();
        for (int i = 0; i < leftLinePoints.Count - 1; i++)
        {
            indices.Add(i + leftLinePoints.Count);
            indices.Add(i + 1);
            indices.Add(i);
        }
        for (int i = rightLinePoints.Count; i < rightLinePoints.Count * 2 - 1; i++)
        {
            indices.Add(i);
            indices.Add(i + 1);
            indices.Add(i - leftLinePoints.Count + 1);
        }

        toFill.AddUIVertexStream(vertices, indices);
    }

    List<UIVertex> GetUIVertex(List<Vector3> points, Color color, float width, float hight)
    {
        List<UIVertex> uiVertices = new List<UIVertex>();
        for (int i = 0; i < points.Count; i++)
        {
            UIVertex v = new UIVertex();
            v.color = color;
            v.position = points[i];
            v.uv0 = new Vector2(points[i].x / width + 0.5f, points[i].y / hight + 0.5f);
            uiVertices.Add(v);
        }
        return uiVertices;
    }
}
