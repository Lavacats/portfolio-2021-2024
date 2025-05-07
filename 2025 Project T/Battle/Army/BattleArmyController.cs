using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleArmyController : MonoBehaviour
{
    [SerializeField] List< BattleArmy> DummyArmy = new List< BattleArmy>();
    [SerializeField] private BattleArmy_SkillController ArmySkilController;

    private Army_ComBatController ArmyComBatController = new Army_ComBatController();
    void Start()
    {
        List<BattleArmy> playerArmy =  new List<BattleArmy>();
        foreach(var army in DummyArmy)
        {
            army.Init();
            if (army.GetBattleArmyBattleData().IsPlayer)
            {
                playerArmy.Add(army);
            }
        }
        ArmySkilController.Init(playerArmy);
        ArmyComBatController.Init();

    }
    private void OnDestroy()
    {
        ArmyComBatController.OnDestroy();
    }

    // Update is called once per frame
    void Update()
    {
        ArmyComBatController.Update();
    }
}
