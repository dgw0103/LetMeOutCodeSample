using UnityEngine.InputSystem;
using UnityEngine;
using System.Collections.Generic;
using System;

public abstract class Selection : MonoBehaviour, IInteractionDown
{
    public event Action OnSelect;
    public event Action OnUnselect;
    private static Stack<Selection> selections = new Stack<Selection>();



    public static Stack<Selection> Selections { get => selections; }
    public void OnInteractionDown()
    {
        if (selections.TryPeek(out Selection selection) == true)
        {
            selection.KeepUnselection();
        }

        KeyInput.PushAsInteraction();
        Select();
        selections.Push(this);
        StageManager.Instance.PushSelectionState();

        OnSelect?.Invoke();
    }
    public void OnUnselection()
    {
        if (selections.Count > 0)
        {
            KeyInput.PopKeyLockStack();
            Unselect();
            selections.Pop();
            StageManager.Instance.PopState();

            if (selections.TryPeek(out Selection selection))
            {
                selection.KeepSelection();
            }

            OnUnselect?.Invoke();
        }
    }
    public virtual void Select()
    {
        KeepSelection();
    }
    public virtual void Unselect()
    {
        KeepUnselection();
    }
    protected virtual void KeepSelection()
    {
        StageManager.Instance.Player.Stop();
        StageManager.Instance.Player.FixRotation();
    }
    protected virtual void KeepUnselection()
    {
        StageManager.Instance.Player.Resume();
    }
    public static void UnselectRecentSelection()
    {
        if (selections.TryPeek(out Selection selection))
        {
            selection.OnUnselection();
        }
    }
    public static void UnselectAll()
    {
        while (selections.TryPeek(out Selection selection))
        {
            UnselectRecentSelection();
        }
    }
    public static void ClearSelections()
    {
        selections.Clear();
    }
    [ContextMenu(nameof(LogSelections))]
    private void LogSelections()
    {
        foreach (var item in selections)
        {
            Debug.Log(item.name);
        }
    }
}