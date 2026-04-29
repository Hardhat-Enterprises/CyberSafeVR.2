using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Smishing01
{
    /// <summary>
    /// Adds hover and press visual feedback to a Button: scales up on hover,
    /// flashes on click. Attach alongside a UI Button.
    /// </summary>
    [RequireComponent(typeof(RectTransform))]
    public class ButtonHoverEffect : MonoBehaviour,
        IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler
    {
        [SerializeField] private float hoverScale = 1.08f;
        [SerializeField] private float lerpSpeed  = 12f;

        private Vector3 _baseScale;
        private Vector3 _targetScale;
        private Graphic _graphic;

        private void Awake()
        {
            _baseScale   = transform.localScale;
            _targetScale = _baseScale;
            _graphic     = GetComponent<Graphic>();
        }

        private void Update()
        {
            transform.localScale = Vector3.Lerp(transform.localScale, _targetScale,
                Time.deltaTime * lerpSpeed);
        }

        public void OnPointerEnter(PointerEventData e) => _targetScale = _baseScale * hoverScale;
        public void OnPointerExit (PointerEventData e) => _targetScale = _baseScale;
        public void OnPointerDown (PointerEventData e)
        {
            if (_graphic != null)
                StartCoroutine(UITween.ColorFlash(_graphic, Color.white, 0.15f));
        }
    }
}
