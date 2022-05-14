using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class NewToggleSlider : MonoBehaviour, IPointerDownHandler
{
    private Image m_cFillImage;
    private RectTransform m_cSlideBtnRect;

    private float m_fBoundary = 0f;
    private bool m_bToggleOn = true;
    private IEnumerator m_Coroutine = null;

    [SerializeField]
    private UnityEvent<bool> m_onValueChanged;

    void Awake()
    {
        Init();
    }

    private void Init()
    {
        if (m_cFillImage == null)
        {
            m_cFillImage = GetComponent<Image>();
        }

        if (m_cSlideBtnRect == null)
        {
            m_cSlideBtnRect = transform.GetChild(transform.childCount - 1).GetComponent<RectTransform>();
        }

        if (m_fBoundary == 0f)
        {
            m_fBoundary = m_cSlideBtnRect.sizeDelta.x / 2;
        }
    }

    public void SetToggle(bool turnOn)
    {
        m_bToggleOn = turnOn;
        m_cFillImage.color = m_bToggleOn ? Color.white : Color.gray;
        m_cSlideBtnRect.anchoredPosition = m_bToggleOn ? new Vector2(m_cFillImage.rectTransform.sizeDelta.x - m_fBoundary, 0)
        : new Vector2(m_fBoundary, 0);
    }

    public void Init(bool toggleOn)
    {
        Init();
        m_bToggleOn = toggleOn;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        ChangeValue();
        if(m_onValueChanged != null)
        {
            m_onValueChanged.Invoke(m_bToggleOn);
        }
    }

    public void ChangeValue()
    {
        Init();

        if(m_Coroutine != null)
        {
            return;
        }

        m_bToggleOn = !m_bToggleOn;

        m_Coroutine = ChangeCoroutine();
        StartCoroutine(m_Coroutine);
    }

    IEnumerator ChangeCoroutine()
    {
        float lerpTime = 0;
        Vector2 startPos = m_bToggleOn ? new Vector2(m_fBoundary, 0) : new Vector2(m_cFillImage.rectTransform.sizeDelta.x - m_fBoundary, 0);
        Vector2 targetPos = m_bToggleOn ? new Vector2(m_cFillImage.rectTransform.sizeDelta.x - m_fBoundary, 0) : new Vector2(m_fBoundary, 0);

        m_cFillImage.color = m_bToggleOn ? Color.white : Color.gray;

        while (true)
        {
            lerpTime += Time.unscaledDeltaTime * (float)BUTTONSPEED.ANIMATION;
            lerpTime = Mathf.Clamp01(lerpTime);

            m_cSlideBtnRect.anchoredPosition = Vector2.Lerp(startPos, targetPos, lerpTime);

            if (lerpTime.Equals((int)EXITTIME.BUTTON))
            {
                m_Coroutine = null;
                break;
            }

            yield return null;
        }
    }

    private void OnDisable()
    {
        Init();
        if (m_Coroutine != null)
        {
            m_Coroutine = null;
            SetToggle(m_bToggleOn);
        }
    }
}
