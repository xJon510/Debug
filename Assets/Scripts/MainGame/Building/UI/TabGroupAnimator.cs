// TabGroupAnimator.cs  (diff-style summary)
using UnityEngine;
using UnityEngine.EventSystems;

public class TabGroupAnimator : MonoBehaviour
{
    [System.Serializable]
    public class Tab
    {
        public Animator animator;
        public string activeParam = "IsActive";     // bool
        public string hoverEnterParam = "HoverEnter";   // trigger
        public string hoverExitParam = "HoverExit";    // trigger
        public GameObject buttonObject;

        [HideInInspector] public int activeHash;
        [HideInInspector] public int hoverEnterHash;
        [HideInInspector] public int hoverExitHash;
    }

    public Tab[] tabs;

    public TabPanelSlider slider;

    void Awake()
    {
        for (int i = 0; i < tabs.Length; i++)
        {
            var t = tabs[i];
            t.activeHash = Animator.StringToHash(t.activeParam);
            t.hoverEnterHash = Animator.StringToHash(t.hoverEnterParam);
            t.hoverExitHash = Animator.StringToHash(t.hoverExitParam);

            if (t.buttonObject)
            {
                var helper = t.buttonObject.GetComponent<TabHoverHelper>();
                if (!helper) helper = t.buttonObject.AddComponent<TabHoverHelper>();
                helper.Setup(this, i);
            }
        }
    }

    void Start()
    {
        // Set the first tab as active by default
        if (tabs.Length > 0)
            OnTabClicked(0);
    }

    public void OnTabClicked(int tabIndex)
    {
        for (int i = 0; i < tabs.Length; i++)
        {
            bool makeActive = (i == tabIndex);
            var tab = tabs[i];

            tab.animator.SetBool(tab.activeHash, makeActive);

            if (makeActive)
            {
                // Only clear hover on the tab we are activating
                tab.animator.ResetTrigger(tab.hoverEnterHash);
                tab.animator.SetTrigger(tab.hoverExitHash);
            }
            // IMPORTANT: do NOT send HoverExit to the other (still-inactive) tab.
            // If the pointer is still over it, it should remain in InactiveHover
            // until OnPointerExit fires and we explicitly send HoverExit there.
        }

        if (slider != null)
        {
            if (tabIndex == 0) slider.ShowInfo();
            else slider.ShowUpgrade();
        }
    }

    // Called by helper on pointer enter/exit
    public void OnTabHover(int tabIndex, bool isEntering)
    {
        var tab = tabs[tabIndex];

        if (isEntering)
        {
            // Only enter hover when not Active
            if (!tab.animator.GetBool(tab.activeHash))
                tab.animator.SetTrigger(tab.hoverEnterHash);
        }
        else
        {
            // Always allow exit hover to clear the state, even if it just became active
            tab.animator.SetTrigger(tab.hoverExitHash);
            tab.animator.ResetTrigger(tab.hoverEnterHash);
        }
    }
}

public class TabHoverHelper : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private TabGroupAnimator group;
    private int index;

    public void Setup(TabGroupAnimator g, int i) { group = g; index = i; }

    public void OnPointerEnter(PointerEventData e) => group.OnTabHover(index, true);
    public void OnPointerExit(PointerEventData e) => group.OnTabHover(index, false);
}
