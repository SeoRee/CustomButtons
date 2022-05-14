using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class NewSlider : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    private Image m_cFillImage;
    private RectTransform m_cSlideBtnRect;

    private float m_fBoundary = 0f;

    [SerializeField]
    private UnityEvent<float> m_onValueChanged;
    [SerializeField]
    private UnityEvent m_onPointerUp;

    void Awake()
    {
        Init();
    }

    private void Init()
    {
        if(m_cFillImage == null)
        {
            m_cFillImage = GetComponent<Image>();
        }

        if(m_cSlideBtnRect == null)
        {
            m_cSlideBtnRect = transform.GetChild(transform.childCount - 1).GetComponent<RectTransform>();
        }

        if (m_fBoundary == 0f)
        {
            m_fBoundary = m_cSlideBtnRect.sizeDelta.x / 2;
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (m_onPointerUp != null)
        {
            m_onPointerUp.Invoke();
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        SetSliderValue(GetSliderValue(eventData));
        if(m_onValueChanged != null)
        {
            m_onValueChanged.Invoke(m_cFillImage.fillAmount);
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        SetSliderValue(GetSliderValue(eventData));
        if (m_onValueChanged != null)
        {
            m_onValueChanged.Invoke(m_cFillImage.fillAmount);
        }
    }

    public void SetSliderValue(float value)
    {
        m_cFillImage.fillAmount = value;

        value *= m_cFillImage.rectTransform.sizeDelta.x;
        value = Mathf.Clamp(value, m_fBoundary, m_cFillImage.rectTransform.sizeDelta.x - m_fBoundary);
        m_cSlideBtnRect.anchoredPosition = new Vector2(value, 0);
    }

    private float GetSliderValue(PointerEventData eventData, Canvas canvas = null)
    {
        Vector2 outVec2;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(m_cFillImage.rectTransform, eventData.position, canvas.worldCamera, out outVec2);
        outVec2.x = Mathf.Clamp(outVec2.x, 0, m_cFillImage.rectTransform.sizeDelta.x);
        return outVec2.x / m_cFillImage.rectTransform.sizeDelta.x;
    }
}
