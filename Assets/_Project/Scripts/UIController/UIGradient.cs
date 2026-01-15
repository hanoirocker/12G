using UnityEngine;
using UnityEngine.UI;

namespace TwelveG.UIController
{
  [AddComponentMenu("UI/Effects/Gradient")]
  public class UIGradient : BaseMeshEffect
  {
    public Color m_color1 = Color.white;
    public Color m_color2 = new Color(1, 1, 1, 0); // Blanco transparente
    [Range(-180f, 180f)]
    public float m_angle = -90f; // Vertical de arriba a abajo

    public override void ModifyMesh(VertexHelper vh)
    {
      if (!IsActive()) return;

      int count = vh.currentVertCount;
      if (count == 0) return;

      var vertex = new UIVertex();
      float bottomY = float.MaxValue;
      float topY = float.MinValue;

      for (int i = 0; i < count; i++)
      {
        vh.PopulateUIVertex(ref vertex, i);
        bottomY = Mathf.Min(bottomY, vertex.position.y);
        topY = Mathf.Max(topY, vertex.position.y);
      }

      float uiHeight = topY - bottomY;

      for (int i = 0; i < count; i++)
      {
        vh.PopulateUIVertex(ref vertex, i);
        
        float t = (vertex.position.y - bottomY) / uiHeight;
        vertex.color *= Color.Lerp(m_color2, m_color1, t);
        vh.SetUIVertex(vertex, i);
      }
    }
  }
}