using HoJin;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class SimpleDoor : Door
{
    [SerializeField] private SnapAxis axis;
    [SerializeField] private DoorMovingPropertyType doorMovingPropertyType;
    [SerializeField] private AnimationCurve[] curves;
    private Vector3[] originalEulerAngles;
    private Vector3[] originalPositions;
    private float movingTime;
    private float time = 0;
    private DoorState doorState;



    protected new void Awake()
    {
        base.Awake();
        movingTime = curves.Max((x) => x.keys.Last().time - x.keys.First().time);
        originalEulerAngles = new Vector3[Doors.Length];
        originalPositions = new Vector3[Doors.Length];
        for (int i = 0; i < Doors.Length; i++)
        {
            originalEulerAngles[i] = Doors[i].transform.localEulerAngles;
            originalPositions[i] = Doors[i].transform.localPosition;
        }
    }
    protected new void Start()
    {
        base.Start();
        doorState = DoorState.Closed;
    }



    public override DoorState DoorState
    { 
        get => doorState;
        set
        {
            doorState = value;
        }
    }
    protected override float OpeningTime { get => movingTime; }
    protected override float ClosingTime { get => movingTime; }
    public override void MoveAsOpening(float speed)
    {
        StartCoroutine(Open_Coroutine(speed));
    }
    public override void MoveAsClosing(float speed)
    {
        StartCoroutine(Close_Coroutine(speed));
    }
    private IEnumerator Open_Coroutine(float speed)
    {
        float maxTime = curves.Max((x) => x.keys.Last().time);

        base.openingSound.Play();

        yield return Move_CoroutineMultiplyingDeltaTime(maxTime, speed);
    }
    private IEnumerator Close_Coroutine(float speed)
    {
        float minTime = curves.Min((x) => x.keys.First().time);



        yield return Move_CoroutineMultiplyingDeltaTime(minTime, -speed);
    }
    private IEnumerator Move_Coroutine(float destinationTime, float speed)
    {
        float previousTime = time;



        switch (doorMovingPropertyType)
        {
            case DoorMovingPropertyType.Rotation:
                while (time * (speed / Mathf.Abs(speed)) < destinationTime)
                {
                    time += speed;
                    for (int i = 0; i < curves.Length; i++)
                    {
                        Doors[i].transform.localEulerAngles += (curves[i].Evaluate(time) * axis.GetUnitVectorBySnapAxis()) -
                            curves[i].Evaluate(previousTime) * axis.GetUnitVectorBySnapAxis();
                    }
                    previousTime = time;
                    yield return null;
                }

                for (int i = 0; i < curves.Length; i++)
                {
                    Doors[i].transform.localEulerAngles = originalEulerAngles[i] + (curves[i].Evaluate(destinationTime) * axis.GetUnitVectorBySnapAxis());
                }
                break;
            case DoorMovingPropertyType.Position:
                while (time * (speed / Mathf.Abs(speed)) < destinationTime)
                {
                    time += speed;
                    for (int i = 0; i < curves.Length; i++)
                    {
                        Doors[i].transform.localPosition += (curves[i].Evaluate(time) * axis.GetUnitVectorBySnapAxis()) -
                            curves[i].Evaluate(previousTime) * axis.GetUnitVectorBySnapAxis();
                    }
                    previousTime = time;
                    yield return null;
                }

                for (int i = 0; i < curves.Length; i++)
                {
                    Doors[i].transform.localPosition = originalPositions[i] + (curves[i].Evaluate(destinationTime) * axis.GetUnitVectorBySnapAxis());
                }
                break;
            default:
                break;
        }
    }
    private IEnumerator Move_CoroutineMultiplyingDeltaTime(float destinationTime, float speed)
    {
        float previousTime = time;



        switch (doorMovingPropertyType)
        {
            case DoorMovingPropertyType.Rotation:
                while (time * (speed / Mathf.Abs(speed)) < destinationTime)
                {
                    time += speed * Time.deltaTime / defaultSpeed;
                    for (int i = 0; i < curves.Length; i++)
                    {
                        Doors[i].transform.localEulerAngles += (curves[i].Evaluate(time) * axis.GetUnitVectorBySnapAxis()) -
                            curves[i].Evaluate(previousTime) * axis.GetUnitVectorBySnapAxis();
                    }
                    previousTime = time;
                    yield return null;
                }

                for (int i = 0; i < curves.Length; i++)
                {
                    Doors[i].transform.localEulerAngles = originalEulerAngles[i] + (curves[i].Evaluate(destinationTime) * axis.GetUnitVectorBySnapAxis());
                }
                break;
            case DoorMovingPropertyType.Position:
                while (time * (speed / Mathf.Abs(speed)) < destinationTime)
                {
                    time += speed * Time.deltaTime / defaultSpeed;
                    for (int i = 0; i < curves.Length; i++)
                    {
                        Doors[i].transform.localPosition += (curves[i].Evaluate(time) * axis.GetUnitVectorBySnapAxis()) -
                            curves[i].Evaluate(previousTime) * axis.GetUnitVectorBySnapAxis();
                    }
                    previousTime = time;
                    yield return null;
                }

                for (int i = 0; i < curves.Length; i++)
                {
                    Doors[i].transform.localPosition = originalPositions[i] + (curves[i].Evaluate(destinationTime) * axis.GetUnitVectorBySnapAxis());
                }
                break;
            default:
                break;
        }
    }
    public override void Rattle(float rattlingTime)
    {
        StartCoroutine(Rattle_Coroutine(rattlingTime));
    }
    private IEnumerator Rattle_Coroutine(float rattlingTime, float speed = defaultSpeed)
    {
        CanInteract = false;
        yield return Move_Coroutine(rattlingTime, speed);
        yield return Move_Coroutine(curves.Min((x) => x.keys.First().time), -speed);
        CanInteract = true;
    }
}