using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class NewButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerClickHandler
{
    [SerializeField]
    private BUTTONANIMTYPE buttonType = BUTTONANIMTYPE.NONE;
    [SerializeField]
    private BUTTONSOUNDTYPE soundType = BUTTONSOUNDTYPE.NORMAL;

    [Space(15)]
    [SerializeField]
    private UnityEvent clickEvents;

    private Graphic[] m_cGraphicArr = null;
    private IEnumerator buttonAnimCoroutine = null;

    private void Awake()
    {
        if(m_cGraphicArr == null)
        {
            m_cGraphicArr = GetComponentsInChildren<Graphic>();
        }
    }

    private void OnDisable()
    {
        Clear();
    }

    //버튼이 애니메이션 플레이 도중 비활성화 되었을때 초기값으로 리셋
    private void Clear()
    {
        if (m_cGraphicArr != null)
        {
            if((buttonType & BUTTONANIMTYPE.COLOR) == BUTTONANIMTYPE.COLOR)
            {
                ResetColor();
            }

            if((buttonType & BUTTONANIMTYPE.SIZE) == BUTTONANIMTYPE.SIZE)
            {
                ResetSize();
            }
        }
    }

    private void ResetColor()
    {
        for (int i = 0; i < m_cGraphicArr.Length; i++)
        {
            m_cGraphicArr[i].color = Color.white;
        }
    }

    private void ResetSize()
    {
        m_cGraphicArr[(int)BUTTONINDEX.PARENT].rectTransform.localScale = Vector3.one;
        m_cGraphicArr[(int)BUTTONINDEX.PARENT].raycastPadding = Vector4.zero;
    }

    //버튼 비활성화 표시할때 사용. (버튼 타입 Size만 사용할 것)
    public virtual void GrayScale(Sprite _grayScaleSprite = null)
    {
        for (int i = 0; i < m_cGraphicArr.Length; i++)
        {
            m_cGraphicArr[i].color = Color.gray;
        }
    }

    //버튼 활성화 표시할때 사용. (버튼 타입 Size만 사용할 것)
    public virtual void WhiteScale(Sprite _whiteScaleSprite = null)
    {
        for (int i = 0; i < m_cGraphicArr.Length; i++)
        {
            m_cGraphicArr[i].color = Color.white;
        }
    }

    public void OnPointerClick(PointerEventData _data)
    {
        //SoundManager.Instance.PlayButtonSound(soundType);

        if (clickEvents != null)
        {
            clickEvents.Invoke();
        }
    }

    public void OnPointerDown(PointerEventData _data)
    {
        if (buttonType != BUTTONANIMTYPE.NONE)
        {
            PlayButtonAnim(SELECTED.SELECTED);
        }
    }

    public void OnPointerUp(PointerEventData _data)
    {
        if (buttonType != BUTTONANIMTYPE.NONE)
        {
            PlayButtonAnim(SELECTED.UNSELECTED);
        }
    }

    private void PlayButtonAnim(SELECTED _selected)
    {
        if (buttonAnimCoroutine != null)
        {
            StopCoroutine(buttonAnimCoroutine);
            buttonAnimCoroutine = null;
        }

        buttonAnimCoroutine = GetAnimation(_selected);

        if (buttonAnimCoroutine != null)
        {
            StartCoroutine(buttonAnimCoroutine);
        }
    }

    #region Animation Coroutines
    private IEnumerator GetAnimation(SELECTED selected)
    {
        switch (buttonType)
        {
            case BUTTONANIMTYPE.COLOR:
                return PlayColorAnim(selected);
            case BUTTONANIMTYPE.SIZE:
                return PlaySizeAnim(selected);
            case BUTTONANIMTYPE.ALL:
                return PlayAllAnim(selected);
            default:
                return null;
        }
    }

    private IEnumerator PlayColorAnim(SELECTED _selected)
    {
        float lerpTime = 0f;
        Color startColor = GetStartColor(_selected);
        Color targetColor = GetTargetColor(_selected);

        while (true)
        {
            lerpTime = CalcLerpTime(lerpTime);
            ChangeGraphicsColor(Color.Lerp(startColor, targetColor, lerpTime));

            if (lerpTime.Equals((int)EXITTIME.BUTTON))
            {
                break;
            }

            yield return null;
        }
    }

    private IEnumerator PlaySizeAnim(SELECTED selected)
    {
        float lerpTime = 0f;
        Vector2 startScale = GetStartScale(selected);
        Vector2 targetScale = GetTargetScale(selected);

        while (true)
        {
            lerpTime = CalcLerpTime(lerpTime);
            ChangeParentGraphicSize(Vector2.Lerp(startScale, targetScale, lerpTime));

            if (lerpTime.Equals((int)EXITTIME.BUTTON))
            {
                break;
            }

            yield return null;
        }
    }

    private IEnumerator PlayAllAnim(SELECTED selected)
    {
        float lerpTime = 0f;
        Vector2 startScale = GetStartScale(selected);
        Vector2 targetScale = GetTargetScale(selected);
        Color startColor = GetStartColor(selected);
        Color targetColor = GetTargetColor(selected);

        while (true)
        {
            lerpTime = CalcLerpTime(lerpTime);
            ChangeParentGraphicSize(Vector2.Lerp(startScale, targetScale, lerpTime));
            ChangeGraphicsColor(Color.Lerp(startColor, targetColor, lerpTime));
            
            if (lerpTime.Equals((int)EXITTIME.BUTTON))
            {
                break;
            }

            yield return null;
        }
    }

    private void ChangeParentGraphicSize(Vector2 lerpVec2)
    {
        m_cGraphicArr[(int)BUTTONINDEX.PARENT].rectTransform.localScale = lerpVec2;
        m_cGraphicArr[(int)BUTTONINDEX.PARENT].raycastPadding =
            CalculatePadding(m_cGraphicArr[(int)BUTTONINDEX.PARENT].rectTransform);
    }

    private void ChangeGraphicsColor(Color lerpColor)
    {
        for (int i = 0; i < m_cGraphicArr.Length; i++)
        {
            m_cGraphicArr[i].color = lerpColor;
        }
    }

    private Vector4 CalculatePadding(RectTransform rect)
    {
        //x = left, y = bottom, z = right, w = top
        float width = rect.sizeDelta.x * rect.localScale.x - rect.sizeDelta.x;
        float height = rect.sizeDelta.y * rect.localScale.y - rect.sizeDelta.y;

        float v4x = width * rect.pivot.x;
        float v4y = height * rect.pivot.y;
        float v4z = width * (1 - rect.pivot.x);
        float v4w = height * (1 - rect.pivot.y);

        return new Vector4(v4x, v4y, v4z, v4w);
    }

    private Color GetStartColor(SELECTED _selected)
    {
        return CheckBtnSelected(_selected) ? Color.white : Color.gray;
    }

    private Color GetTargetColor(SELECTED _selected)
    {
        return CheckBtnSelected(_selected) ? Color.gray : Color.white;
    }

    private Vector2 GetStartScale(SELECTED _selected)
    {
        return CheckBtnSelected(_selected) ? Vector2.one : Vector2.one * 0.9f;
    }

    private Vector2 GetTargetScale(SELECTED _selected)
    {
        return CheckBtnSelected(_selected) ? Vector2.one * 0.9f : Vector2.one;
    }

    private bool CheckBtnSelected(SELECTED _selected)
    {
        return _selected == SELECTED.SELECTED;
    }

    private float CalcLerpTime(float lerpTime, float speed = (float)BUTTONSPEED.ANIMATION)
    {
        lerpTime += Time.unscaledDeltaTime * speed;
        return lerpTime = Mathf.Clamp01(lerpTime);
    }
    #endregion
}
