using UnityEngine;

public class Padlock : CombinationLock
{
    [SerializeField] private int answer;
    [SerializeField] private AudioSource rotationSound;
    private int[] currentNumber;
    private const float rotationAngle = 40f;



    protected new void Awake()
    {
        base.Awake();
        currentNumber = new int[4] { 1, 1, 1, 1 };
    }



    public override void Select()
    {
        base.Select();
        AddAllOfButtonEvents();
        StageManager.Instance.UIs.PadlockPanel.SetActive(true);





        void AddAllOfButtonEvents()
        {
            for (int i = 0; i < StageManager.Instance.UIs.PadlockRotationButtons.Length; i++)
            {
                int index = i;
                int tmp = (i % 4) + 1;
                if (i < 4)
                {
                    tmp *= -1;
                }
                StageManager.Instance.UIs.PadlockRotationButtons[index].onClick.AddListener(() => RotateLockNumber(tmp));
            }
        }
        void RotateLockNumber(int directionAndIndexNum)
        {
            rotationSound.Play();
            transform.GetChild(Mathf.Abs(directionAndIndexNum)).Rotate(0, 0, rotationAngle * (directionAndIndexNum / Mathf.Abs(directionAndIndexNum)));
            SetCurrentNumber(directionAndIndexNum / Mathf.Abs(directionAndIndexNum), Mathf.Abs(directionAndIndexNum) - 1);
            if (CheckAnswer())
            {
                Unlock();
            }





            bool CheckAnswer()
            {
                int currentNumber = 0;
                for (int i = 0; i < this.currentNumber.Length; i++)
                {
                    currentNumber += this.currentNumber[i] * (int)Mathf.Pow(10, this.currentNumber.Length - i - 1);
                }

                return currentNumber == answer;
            }
            void SetCurrentNumber(int value, int index)
            {
                if (currentNumber[index] + value == 10)
                {
                    currentNumber[index] = 1;
                }
                else if (currentNumber[index] + value == 0)
                {
                    currentNumber[index] = 9;
                }
                else
                {
                    currentNumber[index] += value;
                }
            }
        }
    }
    public override void Unselect()
    {
        base.Unselect();
        RemoveAllOfButtonEvents();
        StageManager.Instance.UIs.PadlockPanel.SetActive(false);





        void RemoveAllOfButtonEvents()
        {
            for (int i = 0; i < StageManager.Instance.UIs.PadlockRotationButtons.Length; i++)
            {
                StageManager.Instance.UIs.PadlockRotationButtons[i].onClick.RemoveAllListeners();
            }
        }
    }
}