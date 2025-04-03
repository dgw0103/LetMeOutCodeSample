using System.Linq;
using UnityEngine;
using System;
using LetMeOut;

[RequireComponent(typeof(Examining))]
public class Paper : MonoBehaviour
{
    [SerializeField] private AudioSource pageTurningSound;
    private KeyObject key;
    private Translator[] pageTexts;
    private Animation[] pageAnimations;
    private Examining examining;
    private int page;
    private readonly string pageName = "Page";
    private readonly string turnRightPageName = "TurnRightPage";
    private readonly string turnLeftPageName = "TurnLeftPage";
    private readonly string turnUpPageName = "TurnUpPage";



    protected void Reset()
    {
        Transform pages = transform.Find("Pages");
        if (pages == null)
        {
            pages = new GameObject("Pages").transform;
        }
        pages.localPosition = Vector3.up * 0.1f;
        pages.localScale = Vector3.one;
        pages.SetParent(transform);
        pages.SetSiblingIndex(0);
        pages.gameObject.SetActive(false);
    }
    protected void Awake()
    {
        Pages = transform.GetChild(0);
        Page = 0;
        pageTexts = new Translator[Pages.childCount];
        pageAnimations = new Animation[Pages.childCount];
        for (int i = 0; i < Pages.childCount; i++)
        {
            Pages.GetChild(i).GetChild(0).TryGetComponent(out pageTexts[i]);
            pageAnimations[i] = Pages.GetChild(i).GetComponent<Animation>();
        }
        TryGetComponent(out key);
        TryGetComponent(out examining);
        examining.OnPickUp += () =>
        {
            InitPages();
            TurnOnPaperUIsByPageCount();
        };
        examining.OnPutDown += () =>
        {
            ClosePage();
            StageManager.Instance.UIs.DisappearPickingUpPanel();
        };
    }



    protected Examining Examining { get => examining; }
    public Transform Pages { get; private set; }
    private int Page { set { page = (value + Pages.childCount) % Pages.childCount; } }
    protected event Action OnPickUp { add => examining.OnPickUp += value; remove => examining.OnPickUp -= value; }
    protected event Action OnPutDown { add => examining.OnPutDown += value; remove => examining.OnPutDown -= value; }
    private void InitPages()
    {
        Pages.gameObject.SetActive(true);

        for (int i = 0; i < Pages.childCount; i++)
        {
            Pages.GetChild(i).localPosition = Vector3.up * -0.01f;
        }

        Pages.GetChild(0).localPosition = Vector3.zero;
        Page = 0;

        for (int i = 0; i < pageTexts.Length; i++)
        {
            pageTexts[i].Translate(key.Key + i, TranslatorKeywordType.Page);
        }
    }
    private void TurnOnPaperUIsByPageCount()
    {
        if (Pages.childCount > 1)
        {
            StageManager.Instance.UIs.PaperPanel.SetActive(true);
            StageManager.Instance.UIs.PaperPanelLeftButton.onClick.AddListener(TurnLeftPage);
            StageManager.Instance.UIs.PaperPanelRightButton.onClick.AddListener(TurnRightPage);
        }
    }
    private void ClosePage()
    {
        Pages.gameObject.SetActive(false);
        if (Pages.childCount > 1)
        {
            StageManager.Instance.UIs.PaperPanel.SetActive(false);
            StageManager.Instance.UIs.PaperPanelLeftButton.onClick.RemoveListener(TurnLeftPage);
            StageManager.Instance.UIs.PaperPanelRightButton.onClick.RemoveListener(TurnRightPage);
        }
    }
    /// <summary>
    /// direction(Right : 1, Left : -1)
    /// </summary>
    /// <param name="direction">Right : 1, Left : -1</param>
    private void TurnPage(int direction, string animationName)
    {
        pageTurningSound.Play();
        pageAnimations[page].Play(animationName);
        Page = page + direction;
        pageAnimations[page].Play(turnUpPageName);
    }
    private void TurnRightPage()
    {
        TurnPage(1, turnRightPageName);
    }
    private void TurnLeftPage()
    {
        TurnPage(-1, turnLeftPageName);
    }
#if UNITY_EDITOR
    [ContextMenu("Create page")]
    private void CreatePage()
    {
        GameObject page = Instantiate(Resources.FindObjectsOfTypeAll<GameObject>().Single((x) => x.name == pageName), transform.GetChild(0));

        page.name = "Page" + (transform.GetChild(0).childCount - 1);
        page.transform.localPosition = Vector3.zero;
        page.GetComponent<MeshFilter>().mesh = GetComponent<MeshFilter>().sharedMesh;
        page.GetComponent<MeshRenderer>().materials = GetComponent<MeshRenderer>().sharedMaterials;
    }
#endif
}
