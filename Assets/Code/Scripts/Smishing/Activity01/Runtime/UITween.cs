using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Smishing01
{
    /// <summary>
    /// Tiny coroutine-based tween utility. No DOTween dependency.
    /// </summary>
    public static class UITween
    {
        public static IEnumerator FadeCanvasGroup(CanvasGroup cg, float from, float to, float dur)
        {
            if (cg == null) yield break;
            float t = 0f;
            cg.alpha = from;
            while (t < dur)
            {
                t += Time.deltaTime;
                cg.alpha = Mathf.Lerp(from, to, EaseOutCubic(t / dur));
                yield return null;
            }
            cg.alpha = to;
        }

        public static IEnumerator ScalePop(Transform target, float from, float to, float dur)
        {
            if (target == null) yield break;
            float t = 0f;
            target.localScale = Vector3.one * from;
            while (t < dur)
            {
                t += Time.deltaTime;
                float k = EaseOutBack(t / dur);
                target.localScale = Vector3.one * Mathf.Lerp(from, to, k);
                yield return null;
            }
            target.localScale = Vector3.one * to;
        }

        public static IEnumerator SlideIn(RectTransform rt, Vector2 fromOffset, float dur)
        {
            if (rt == null) yield break;
            Vector2 target = rt.anchoredPosition;
            Vector2 start  = target + fromOffset;
            rt.anchoredPosition = start;
            float t = 0f;
            while (t < dur)
            {
                t += Time.deltaTime;
                rt.anchoredPosition = Vector2.Lerp(start, target, EaseOutCubic(t / dur));
                yield return null;
            }
            rt.anchoredPosition = target;
        }

        public static IEnumerator ColorFlash(Graphic g, Color flash, float dur)
        {
            if (g == null) yield break;
            Color original = g.color;
            float t = 0f;
            while (t < dur)
            {
                t += Time.deltaTime;
                g.color = Color.Lerp(flash, original, t / dur);
                yield return null;
            }
            g.color = original;
        }

        private static float EaseOutCubic(float x) => 1f - Mathf.Pow(1f - Mathf.Clamp01(x), 3f);
        private static float EaseOutBack(float x)
        {
            const float c1 = 1.70158f, c3 = c1 + 1f;
            x = Mathf.Clamp01(x);
            return 1f + c3 * Mathf.Pow(x - 1f, 3f) + c1 * Mathf.Pow(x - 1f, 2f);
        }
    }
}
