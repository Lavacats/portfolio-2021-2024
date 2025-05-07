using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class SpawnObject : MonoBehaviour
{
    public BattleBaseUnit targetObejct;
    public BattleBaseUnit_Status AttackUserInfo;

    public GameObject Hit_Effect_FX;        // FX가 없어서 사용하지못하지만, 필요하다면 투사체에서 FX를 재생.

    public string ShootIdx = string.Empty;
    public string TargetIdx = string.Empty;

    public E_SpawnType SpawnType = E_SpawnType.Attack;
    void Update()
    {
        if(targetObejct!= null)
        {
            Vector3 direction = (new Vector3( targetObejct.transform.position.x,0, targetObejct.transform.position .z)- new Vector3(this.transform.position.x,0, this.transform.position.z)).normalized;

            // 2. 현재 방향에서 목표 방향으로 부드럽게 회전
            if (direction != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(direction);
                this.transform.rotation = Quaternion.Slerp(this.transform.rotation, targetRotation, 5 * Time.deltaTime);
            }
            // 3. 앞으로 이동
            this.transform.position += direction * 5f * Time.deltaTime;

            // 충돌
            float distance = Vector3.Distance(targetObejct.transform.position, this.transform.position);
            if (distance < 1)
            {
                OnInvoke();
            }
        }
    }
    void OnInvoke()
    {
        switch (SpawnType)
        {
            case E_SpawnType.Attack:
                {

                    ArmyDataManager.Instance.Engine.GetEngine_Damage().OnHit_SpawnObjectDamage(this);
                }
                break;
            case E_SpawnType.Skill:
                {
                    ArmyDataManager.Instance.Engine.GetEngine_SKill().OnHit_SkillDamage(ShootIdx,TargetIdx);
                }
                break;
        }
  
        this.gameObject.SetActive(false);
        targetObejct = null;
    }
}
