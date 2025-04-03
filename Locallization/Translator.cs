using TMPro;
using UnityEngine;
using HoJin;
using LetMeOut;
using Michsky.UI.Dark;
using System;
using System.Linq;

public class Translator : MonoBehaviour, ITranslator
{
    [SerializeField] public string key;
    [SerializeField] public TranslatorKeywordType translatorKeywordType = TranslatorKeywordType.Default;
    [SerializeField] private FontStorage fontStorage;
    [SerializeField] private TMP_Text tMP_Text;



    public TMP_Text Text { get => tMP_Text; }
    public HorizontalAlignmentOptions HorizontalAlignmentOptions { set => tMP_Text.horizontalAlignment = value; }
    public VerticalAlignmentOptions VerticalAlignmentOptions { set => tMP_Text.verticalAlignment = value; }



    private void Reset()
    {
        TryGetComponent(out tMP_Text);
    }



    public static void TranslateAll()
    {
        foreach (var item in FindObjectsOfType<MonoBehaviour>(true).OfType<ITranslator>())
        {
            item.Translate();
        }
    }
    public void Translate(string key, TranslatorKeywordType keyword)
    {
        this.key = key;
        translatorKeywordType = keyword;
        Translate();
    }
    public void Translate()
    {
        if (string.IsNullOrEmpty(key) == false)
        {
            tMP_Text.text = GameManager.Instance.GetTranslationFromJsonFile(key, translatorKeywordType);
        }
        ChangeFontByLanguage();
    }
    public void SetText(string text)
    {
        tMP_Text.text = text;
        ChangeFontByLanguage();
    }
    private void ChangeFontByLanguage()
    {
        try
        {
            tMP_Text.font = fontStorage.GetFontBy(GameManager.Instance.PreferencesData.Language);
        }
        catch (Exception)
        {
            Debug.Log(gameObject.name);
            Debug.Log(tMP_Text);
            Debug.Log(fontStorage);
            Debug.Log(GameManager.Instance);
            Debug.Log(GameManager.Instance.PreferencesData);
        }
    }
}