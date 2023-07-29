using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class FSM : MonoBehaviour
{
    [SerializeField]
    protected State myState;

    //해당 상태인지 비교
    public bool CompareState(State state)
    {
        return myState == state;
    }

    private void Update()
    {
        StateProcess();
    }

    //상태가 바뀌었을때 할 행동들
    public void ChangeState(State s)
    {
        if (this.myState == s) return;
        this.myState = s;

        switch (myState)
        {
            case State.Move:
                {
                    ChangeMoveState();
                }
                break;

            case State.Fight:
                {
                    ChangeFightState();
                }
                break;

            case State.Death:
                {
                    ChangeDeathState();
                }
                break;
        }
    }

    protected abstract void ChangeMoveState();
    protected abstract void ChangeFightState();
    protected abstract void ChangeDeathState();

    //특정 상태일때 행동할 동작들
    private void StateProcess()
    {

        switch (myState)
        {
            case State.Move:
                {
                    MoveStateRoutine();
                }
                break;
            case State.Fight:
                {
                    FightStateRoutine();
                }
                break;
        }
    }

    protected abstract void MoveStateRoutine();
    protected abstract void FightStateRoutine();

}
