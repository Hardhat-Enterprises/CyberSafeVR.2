using UnityEngine;
using UnityEngine.UI;

namespace Smishing01
{
    /// <summary>
    /// Adds a vertical gradient effect to a UI Image by tinting its vertices.
    /// Useful for richer-looking panels without textures.
    /// Attach to a GameObject with a Graphic component.
    /// </summary>
    [RequireComponent(typeof(Graphic))]
    public class UIGradient : BaseMeshEffect
    {
        public Color topColor    = new Color(0.18f, 0.20f, 0.28f);
        public Color bottomColor = new Color(0.07f, 0.08f, 0.11f);

        public override void ModifyMesh(VertexHelper vh)
        {
            if (!IsActive() || vh.currentVertCount == 0) return;

            int count = vh.currentVertCount;
            UIVertex v = new UIVertex();

            // First find min/max Y to normalize
            float minY = float.MaxValue, maxY = float.MinValue;
            for (int i = 0; i < count; i++)
            {
                vh.PopulateUIVertex(ref v, i);
                if (v.position.y < minY) minY = v.position.y;
                if (v.position.y > maxY) maxY = v.position.y;
            }
            float range = Mathf.Max(0.0001f, maxY - minY);

            for (int i = 0; i < count; i++)
            {
                vh.PopulateUIVertex(ref v, i);
                float t = (v.position.y - minY) / range;
                v.color = Color.Lerp(bottomColor, topColor, t);
                vh.SetUIVertex(v, i);
            }
        }
    }
}
