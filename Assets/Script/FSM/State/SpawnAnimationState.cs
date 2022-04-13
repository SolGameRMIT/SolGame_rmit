using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class SpawnAnimationState : IState
{
    private FSM m_FSM;
    float spawnAnimationDestoryTime = 2f;
    float spawnAnimationTime;
    Transform m_transform;
    GameObject m_gameObject;
    public GameObject spawnAnimation;
    GameObject m_spawnAnimationObject;
    public SpawnAnimationState(FSM fsm, GameObject _gameObject,GameObject _spawnExplosion)
    {
        m_FSM = fsm;
        m_gameObject = _gameObject;
        m_transform = m_gameObject.transform;
        spawnAnimation = _spawnExplosion;
        
    }
    public SpawnAnimationState()
    {

    }

    public void OnEnter()   //  The method that should be performed to enter this state
    {
        spawnAnimationTime = Time.time;
        var postion = new Vector3(m_transform.position.x, 0, m_transform.position.z);
        m_spawnAnimationObject = MonoSub.Instantiate(spawnAnimation, postion, m_transform.rotation); //Spawn in the broken version
    }
    public void OnUpdate() //The method that should be executed to maintain this state
    {
        if(Time.time > spawnAnimationTime + spawnAnimationDestoryTime)
        {
            MonoSub.Destroy(m_spawnAnimationObject);
            m_FSM.TransitionState(StateType.Patrolling);
        }
    }
    public void OnExit() //The method that should be executed to exit this state
    {            
        // call OnExit() method by FSM
        MonoSub.Destroy(m_spawnAnimationObject);

    }

    private class MonoSub : MonoBehaviour
    {

    }
}