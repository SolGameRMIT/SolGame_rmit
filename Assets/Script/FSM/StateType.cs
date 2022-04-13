using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum StateType
{
    Enter,           //     enter the scene
    SpawnAnimation, //      display Star Animation
    Patrolling,
    ChaseA,
    Chase,          //  
    Die,            //      Death Hunter or predator or Flock or player
}