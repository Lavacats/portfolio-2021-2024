using System.Collections;
using DG.Tweening;
using PB.ClientParts;

namespace UnityEngine.UI
{
    [ExecuteInEditMode]
    [RequireComponent(typeof(RectTransform)), RequireComponent(typeof(LayoutElement))]
public class UISocialUserBannerAccordian :  UIComponent
    {
        public enum eTransition
        {
            Instant,
            Tween
        }
        public enum eState
        {
            Collapsed,
            Expanded
        }
        [SerializeField]
        protected bool isToggle = false;

        public float MinHeight = 18f;

        [HideInInspector]
        public float verticalPadding;

        [SerializeField]
        private eTransition transition = eTransition.Tween;

        [SerializeField]
        private float transitionDuration = 0.3f;

        [SerializeField]
        protected eState currentState = eState.Expanded;

        [SerializeField]
        protected LayoutElement layoutElement;

        protected UIButton btn;

        [SerializeField]
        protected UI_SocialUserBannerContainer accordionItem;

        [SerializeField]
        private RectTransform arrowRectTransform;

        [SerializeField]
        private Vector3 arrowRectRotate = new Vector3();

        public float targetFloat;

        /// <summary>
        /// Gets or sets the transition.
        /// </summary>
        /// <value>The transition.</value>
        public eTransition Transition
        {
            get { return transition; }
            set { transition = value; }
        }

        /// <summary>
        /// Gets or sets the duration of the transition.
        /// </summary>
        /// <value>The duration of the transition.</value>
        public float TransitionDuration
        {
            get { return transitionDuration; }
            set { transitionDuration = value; }
        }

        public virtual void SetBtnEventHandler()
        {
            if (!isToggle)
            {
                btn.SetOnClickEventHandler(OnValueChanged);
            }
        }

        protected virtual void OnValidate()
        {
            LayoutElement le = gameObject.GetComponent<LayoutElement>();

            if (le != null)
            {
                le.preferredHeight = (currentState == eState.Expanded) ? -1f : MinHeight;
            }
        }

        public void OnValueChanged(GameObject go)
        {
            if (isToggle)
            {
                if (go == btn.CachedGameObject)
                {
                    if (currentState == eState.Collapsed)
                    {
                        TransitionToState(eState.Expanded);
                    }
                    else if (currentState == eState.Expanded)
                    {
                        TransitionToState(eState.Collapsed);
                    }	
                }
                else
                {
                    if (currentState == eState.Expanded)
                    {
                        TransitionToState(eState.Collapsed);
                        accordionItem.ForceUpdateCellsTop();
                    }
                }
            }
            else
            {
                TransitionToState(currentState == eState.Collapsed ? eState.Expanded : eState.Collapsed);
            }
        }

        public void UpdateValue()
        {
            if (isToggle)
            {
                if (currentState == eState.Expanded)
                {
                    TransitionToState(eState.Expanded);
                }
            }
            else
            {
                TransitionToState(currentState == eState.Collapsed ? eState.Expanded : eState.Collapsed);
            }
        }

        protected virtual void TransitionToState(eState state)
        {
            if (layoutElement == null) return;

            // Save as current state
            currentState = state;

            // Transition
            if (transition == eTransition.Instant)
            {
                layoutElement.preferredHeight = (state == eState.Expanded) ? -1f : MinHeight;
            }
            else if (transition == eTransition.Tween)
            {
                if (state == eState.Expanded)
                {
                    StartTween(GetAccordionItemExpandedHeight());
                }
                else
                {
                    StartTween(MinHeight);
                }
            }
        }

        protected float GetExpandedHeight()
        {
            if (layoutElement == null)
                return MinHeight;

            float originalPrefH = layoutElement.preferredHeight;
            layoutElement.preferredHeight = -1f;
            float h = LayoutUtility.GetPreferredHeight(CachedRectTransform);
            layoutElement.preferredHeight = originalPrefH;

            return h;
        }

        protected float GetAccordionItemExpandedHeight()
        {
            if (accordionItem.layoutElement == null)
                return MinHeight;

            return accordionItem.targetHeight + MinHeight;
        }

        protected virtual void StartTween(float targetFloat)
        {
            Vector2 endValue = new Vector2(CachedRectTransform.sizeDelta.x, targetFloat);
            this.targetFloat = targetFloat;
            CachedRectTransform.DOSizeDelta(endValue, transitionDuration).onComplete = SetHeight;
            layoutElement.DOPreferredSize(endValue, transitionDuration);
            if (accordionItem == null)
            {
                return;
            }

            endValue = new Vector2(accordionItem.CachedRectTransform.sizeDelta.x, targetFloat - MinHeight);
            accordionItem.CachedRectTransform.DOSizeDelta(endValue, transitionDuration).onComplete = accordionItem.SetPreferredHeight;
            if (currentState == eState.Expanded)
            {
                arrowRectTransform.DORotate(Vector3.zero, transitionDuration);
            }
            else
            {
                arrowRectTransform.DORotate(arrowRectRotate, transitionDuration);
            }

            accordionItem.layoutElement.DOPreferredSize(endValue, transitionDuration);
        }

        protected virtual void SetHeight()
        {
            if (layoutElement == null)
                return;

            layoutElement.preferredHeight = targetFloat;
        }
    }
}