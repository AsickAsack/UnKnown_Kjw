using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class FSM : MonoBehaviour
{
    [SerializeField]
    protected State myState;

    //�ش� �������� ��
    public bool CompareState(State state)
    {
        return myState == state;
    }

    private void Update()
    {
        StateProcess();
    }

    //���°� �ٲ������ �� �ൿ��
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

    //Ư�� �����϶� �ൿ�� ���۵�
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
