using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BaseTable;
using NLibCs;
using DEFINE;
using Contents.User;


public class SeasonEventMissionInfo : UIForm
{
    [SerializeField] private GameObject clearMark_1;                       // 시즌 미션 1 클리어 UI 표시 여부
    [SerializeField] private GameObject clearMark_2;                       // 시즌 미션 2 클리어 UI 표시 여부
    [SerializeField] private GameObject clearMark_3;                       // 시즌 미션 3 클리어 UI 표시 여부


    [SerializeField] private UIPlaySound playSound_1;                         // 보상 이펙트
    [SerializeField] private UIPlaySound playSound_2;                         // 보상 이펙트
    [SerializeField] private UIPlaySound playSound_3;                         // 보상 이펙트

    [SerializeField] private UILabel seasonMission_1;                      // 시즌 미션 1 라벨
    [SerializeField] private UILabel seasonMission_2;                      // 시즌 미션 2 라벨
    [SerializeField] private UILabel seasonMission_3;                      // 시즌 미션 3 라벨

    [SerializeField] private UILabel seasonMission_1_Progress;             // 시즌 미션 1 라벨 진행도
    [SerializeField] private UILabel seasonMission_2_Progress;             // 시즌 미션 2 라벨 진행도
    [SerializeField] private UILabel seasonMission_3_Progress;             // 시즌 미션 3 라벨 진행도

    [SerializeField] private UISprite rewardImage;                         // 보상 이미지
    [SerializeField] private UILabel rewardLabelVolume;                    // 보상 개수
    [SerializeField] private UILabel rewardLabelValue;                     // 보상 값
    [SerializeField] private GameObject rewardEffect;                      // 보상 이펙트
    public void SetMissionLabel(int index, string MissionTextKey,int curProcess, int targetProcess)
    {
        if (index == 1) 
        { 
            if(seasonMission_1)seasonMission_1.text = NTextManager.Instance.GetText(MissionTextKey) ;
            if(seasonMission_1_Progress)seasonMission_1_Progress.text = curProcess.ToString() + "/" + targetProcess.ToString();
        }
        else if (index == 2)
        {
            if(seasonMission_2)seasonMission_2.text = NTextManager.Instance.GetText(MissionTextKey);
            if(seasonMission_2_Progress)seasonMission_2_Progress.text = curProcess.ToString() + "/" + targetProcess.ToString();
        }
        else if (index == 3)
        { 
            if(seasonMission_3)seasonMission_3.text = NTextManager.Instance.GetText(MissionTextKey);
            if(seasonMission_3_Progress)seasonMission_3_Progress.text = curProcess.ToString() + "/" + targetProcess.ToString();
        }
    }

    public override void BindUIEvents()
    {
        base.BindUIEvents();
    }

    public void SetActiveMissionClearMark(int index, bool setClear)
    {
        if (index == 1) clearMark_1.SetActive(setClear);
        else if (index == 2) clearMark_2.SetActive(setClear);
        else if (index == 3) clearMark_3.SetActive(setClear);
    }
    public void RewardEffect()
    {
        rewardEffect.SetActive(true);
    }
    public void SetRewardItem(int  kind, int value)
    {
        var itemInfo = TableItemInfo.Instance.Get(kind);
        if (itemInfo == null) return;

        if(rewardImage)SetSprite(ref rewardImage, itemInfo.atlasName, itemInfo.itemIcon);
        if(rewardLabelValue) SetText(ref rewardLabelValue, value.ToString());
        if(rewardLabelVolume) SetText(ref rewardLabelVolume, PublicUIMethod.GetString_ItemVolumeText(itemInfo));
    }

    public void FinishSound()
    {
        if (playSound_1) playSound_1.enabled = false;
        if (playSound_2) playSound_2.enabled = false;
        if (playSound_3) playSound_3.enabled = false;
    }

}
