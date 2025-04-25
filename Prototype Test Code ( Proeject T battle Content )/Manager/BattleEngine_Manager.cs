using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleEngine_Manager : SingleTon<BattleEngine_Manager>
{
    public BattleEngine Engine;
    public Battle_MapDirector MapDirector;
    public Battle_Pathfinder_Controller Pathfinder;
}
