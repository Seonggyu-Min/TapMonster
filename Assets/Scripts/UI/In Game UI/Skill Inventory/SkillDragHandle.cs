using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


public class SkillDragHandle :
    MonoBehaviour,
    IPointerDownHandler,
    IPointerUpHandler,
    IInitializePotentialDragHandler,
    IDragHandler,
    IEndDragHandler
{
    #region Fields and Properties

    [Header("Refs")]
    [SerializeField] private SkillUIOrchestrator _skillUIOrchestrator;
    [SerializeField] private SkillSlotView _skillSlotView;
    [SerializeField] private ScrollRect _inventoryScrollRect;
    [SerializeField] private Canvas _rootCanvas;

    [Header("Ghost")]
    [SerializeField] private SkillGhost _ghost;

    [Header("Long Press Config")]
    [SerializeField] private float _longPressTime = 0.25f;

    public SkillSlotView SourceSlot => _skillSlotView;

    private CanvasGroup _canvasGroup;
    private bool _pressed;
    private bool _longPressed;
    private Coroutine _longPressCo;

    private bool _forwardingToScroll; // 스크롤로 이벤트 전달 중인지 여부
    private bool _dragStarted; // 고스트 생성 여부

    private const LogCategory CurrentCategory = LogCategory.UI;

    #endregion


    #region Unity Methods

    private void Awake()
    {
        if (_skillSlotView == null) _skillSlotView = GetComponentInParent<SkillSlotView>();
        this.TryBindComponent(ref _canvasGroup, CurrentCategory);
        if (_skillUIOrchestrator == null) this.PrintLog("orchestrator가 비어있음", CurrentCategory, LogType.Error);
        if (_rootCanvas == null) this.PrintLog("rootCanvas가 비어있음", CurrentCategory, LogType.Error);
        if (_ghost == null) this.PrintLog("ghost고스트가 비어있음", CurrentCategory, LogType.Error);

        // 시작 시 비활성화
        if (_ghost != null) _ghost.gameObject.SetActive(false);
    }

    private void OnDisable()
    {
        if (_longPressCo != null)
        {
            StopCoroutine(_longPressCo);
            _longPressCo = null;
        }
    }

    #endregion


    #region EventSystems Methods

    public void OnPointerDown(PointerEventData eventData)
    {
        _pressed = true;
        _longPressed = false;
        _forwardingToScroll = true;

        // 빈 슬롯이면 드래그 시작 금지
        if (_skillUIOrchestrator != null && _skillSlotView != null)
        {
            int id = _skillUIOrchestrator.GetSkillIdFrom(_skillSlotView);
            if (id == SkillId.None)
            {
                // ID가 비어도 드래그 금지
                _pressed = false;
                _forwardingToScroll = true;
                return;
            }
        }

        if (_longPressCo != null)
        {
            StopCoroutine(_longPressCo);
        }
        _longPressCo = StartCoroutine(LongPressRoutine());
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        _pressed = false;

        if (_longPressCo != null)
        {
            StopCoroutine(_longPressCo);
            _longPressCo = null;
        }
    }

    public void OnInitializePotentialDrag(PointerEventData eventData)
    {
        if (_inventoryScrollRect != null)
        {
            _inventoryScrollRect.OnInitializePotentialDrag(eventData);
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        // 롱프레스가 아니면 스크롤로 이벤트 전달
        if (!_longPressed)
        {
            ForwardToScroll(eventData, ExecuteEvents.dragHandler);
            return;
        }

        // 롱프레스 완료 후 드래그 처리
        if (!_dragStarted)
        {
            _dragStarted = true;
            this.PrintLog("롱프레스 완료 및 드래그 시작", CurrentCategory);

            _canvasGroup.blocksRaycasts = false;
            ShowGhost(eventData);
        }

        UpdateGhost(eventData);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        // 롱프레스가 아니면 스크롤로 이벤트 전달
        if (!_dragStarted)
        {
            ForwardToScroll(eventData, ExecuteEvents.endDragHandler);
        }

        HideGhost();
        _canvasGroup.blocksRaycasts = true;

        _dragStarted = false;
        _longPressed = false;

        if (_inventoryScrollRect != null)
            _inventoryScrollRect.enabled = true;
    }

    #endregion


    #region Private Methods

    private void ForwardToScroll<T>(PointerEventData eventData, ExecuteEvents.EventFunction<T> func)
        where T : IEventSystemHandler
    {
        if (_inventoryScrollRect == null) return;
        if (!_forwardingToScroll) return;

        ExecuteEvents.Execute(_inventoryScrollRect.gameObject, eventData, func);
    }

    private void ShowGhost(PointerEventData eventData)
    {
        if (_ghost == null || _skillUIOrchestrator == null || _skillSlotView == null) return;

        int skillId = _skillUIOrchestrator.GetSkillIdFrom(_skillSlotView);
        Sprite icon = _skillUIOrchestrator.GetIconOrNull(skillId);

        _ghost.SetIcon(icon);
        _ghost.gameObject.SetActive(true);
        _ghost.SetScreenPosition(eventData.position, eventData.pressEventCamera, _rootCanvas);

        this.PrintLog("고스트 활성화", CurrentCategory);
    }

    private void UpdateGhost(PointerEventData eventData)
    {
        if (_ghost == null || !_ghost.gameObject.activeSelf) return;
        _ghost.SetScreenPosition(eventData.position, eventData.pressEventCamera, _rootCanvas);
    }

    private void HideGhost()
    {
        if (_ghost == null) return;
        _ghost.gameObject.SetActive(false);
    }

    private IEnumerator LongPressRoutine()
    {
        float t = 0f;
        while (_pressed && t < _longPressTime)
        {
            t += Time.unscaledDeltaTime;
            yield return null;
        }

        if (_pressed)
        {
            _longPressed = true;
            _forwardingToScroll = false;

            // 롱프레스 성공 후 스크롤 잠금
            if (_inventoryScrollRect != null)
            {
                _inventoryScrollRect.enabled = false;
            }
        }
    }

    #endregion
}
