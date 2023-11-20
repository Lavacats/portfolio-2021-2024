using UnityEngine;
using BaseTable;
using NLibCs;
using Contents.User;
using FlatBuffers;
using PROTOCOL.FLATBUFFERS;
using PROTOCOL.GAME.ID;
using GameEvent;

public class SeasonEventMission : UIForm
{
    public const string OBJECTID = "SheetBlock_EventList";

    [SerializeField] private GameObject seasonMissionInfo;              // 시즌 미션 상세보기 오브젝트
    [SerializeField] private GameObject seasonMissionClearMark;           // 미션 올클리어 여부
    [SerializeField] private UILabel seasonMissionName;                 // 시즌 이벤트 이름
    [SerializeField] private UISprite seasonMissionSprite;              // 시즌 이벤트  이미지
    [SerializeField] private UIButton buttonMission;                    // 미션 클릭 버튼 ( 사진 )
    [SerializeField] private GameObject seasonDataObject;               // 메모지

    [SerializeField] private UILabel labelReward;                   // 보상 버튼 라벨
    [SerializeField] private UIButton buttonReward;                 // 보상 버튼
    [SerializeField] private GameObject receivedRewardEffect;               // 메모지

    [SerializeField] private GameObject rewardNotice;                      // 보상 레드닷

    public SeasonEventMissionInfo seasonData;           //UI
    public NrChloeEventDataInfo seasonDataInfo;         //DATA
    public EventSeasonDlg eventSeasonDlg;

    public bool completeMission = false;

    public override void Init()
    {
        base.Init();
    }
    public override void BindUIEvents()
    {
        base.BindUIEvents();
        if (buttonMission) EventDelegate.Add(buttonMission.onClick, OnClick_EventIcon);

        if (buttonReward) EventDelegate.Add(buttonReward.onClick, OnClick_RewardButtion);
    }
    public void RefreshSeasonEvent()
    {
        SetSeasonEventInfo(false,seasonDataInfo);
    }
    public void SetActiveMissionClearMark(bool setClear)
    {
        seasonMissionClearMark.SetActive(setClear);
    }
    public void SetMissionLabel(string MissionTextKey)
    {
        seasonMissionName.text = NTextManager.Instance.GetText(MissionTextKey);
    }
    public void SetSpriteKey(string spriteKey)
    {
        if (null == seasonMissionSprite)
            return;

        seasonMissionSprite.spriteName = spriteKey;
        seasonMissionSprite.grayscale = true;
    }

    public void SetMissionPaperOff()
    {
        seasonDataObject.SetActive(false);
    }

    private void OnClick_EventIcon()
    {
        // 카드 클릭해서 종이 열기
        seasonDataObject.SetActive(true);
        if (null != seasonDataInfo)
        {
            eventSeasonDlg.AllSeasonEventMissionPaperOff();
            SetSeasonEventInfo(true,seasonDataInfo);
        }
    }
    void SetSeasonEventInfo(bool activeInfo,NrChloeEventDataInfo missionINfo)
    {
        // 종이활성화
        seasonDataObject.SetActive(activeInfo);

        labelReward.text = NTextManager.Instance.GetText("UI_SEASONEVENT_REWARD_TEXT");

        // 목표값 달성 미션 체크
        int missionCount = 0;

        if (missionINfo != null)
        {
            if (missionINfo.SeasonEventMission_1 != null)
            {
                if (missionINfo.SeasonEventMission_1.MissionTitle != null) 
                { 
                    // 미션정보 dicSeasionMission에서 현재 Kind에 맞는 유저의 값을 가져옴
                    var curValue = ChloeStoryEventManager.Instance.dicSeasonMission[missionINfo.SeasonEventMission_1_Kind];
                    var targetValue = missionINfo.SeasonEventMission_1.TargetValue;

                    if (curValue >= targetValue)
                    {
                        missionCount++;
                        seasonData.SetActiveMissionClearMark(1, true);
                        curValue = targetValue;
                    }

                    // 미션 타이틀 세팅
                    seasonData.SetMissionLabel(
                        1,                                                  // 인덱스
                        missionINfo.SeasonEventMission_1.MissionTitle,      // 타이틀
                        curValue,                                           // 현재 미션 수행값
                        targetValue                                         // 미션 최종 목표값
                        );
                }
            }
            if (missionINfo.SeasonEventMission_2 != null)
            {
                if (missionINfo.SeasonEventMission_2.MissionTitle != null)
                {
                    var curValue = ChloeStoryEventManager.Instance.dicSeasonMission[missionINfo.SeasonEventMission_2_Kind];
                    var targetValue = missionINfo.SeasonEventMission_2.TargetValue;

                    if (curValue >= missionINfo.SeasonEventMission_2.TargetValue)
                    {
                        missionCount++;
                        seasonData.SetActiveMissionClearMark(2, true);
                        curValue = targetValue;
                    }

                    seasonData.SetMissionLabel(
                        2,                                                  // 인덱스
                        missionINfo.SeasonEventMission_2.MissionTitle,      // 타이틀
                        curValue,                                           // 현재 미션 수행값
                        targetValue                                         // 미션 최종 목표값
                        );
                }
            }
            if (missionINfo.SeasonEventMission_3 != null)
            {
                if (missionINfo.SeasonEventMission_3.MissionTitle != null)
                {
                    var curValue = ChloeStoryEventManager.Instance.dicSeasonMission[missionINfo.SeasonEventMission_3_Kind];
                    var targetValue = missionINfo.SeasonEventMission_3.TargetValue;

                    if (curValue >= missionINfo.SeasonEventMission_3.TargetValue)
                    {
                        missionCount++;
                        seasonData.SetActiveMissionClearMark(3, true);
                        curValue = targetValue;
                    }

                    seasonData.SetMissionLabel(
                        3,                                                 // 인덱스
                        missionINfo.SeasonEventMission_3.MissionTitle,     // 타이틀
                        curValue,                                          // 현재 미션 수행값
                        targetValue                                        // 미션 최종 목표값
                        );
                }
            }

            if (missionINfo.SeasonRewards.Count > 0)
            {
                // NDT테이블에서 정보를 읽어와 아이템 값을 세팅한다
                // 22.09.02 기준 1개 아이템만 사용하기로 했으므로 0 사용
                seasonData.SetRewardItem(missionINfo.SeasonRewards[0].Kind, missionINfo.SeasonRewards[0].Quantity);
            }

            if (missionCount >= 3)
            {
                seasonMissionSprite.grayscale = false;
                Refresh_RewardEffect(missionINfo.SeasonEventKind);
                if (ChloeStoryEventManager.Instance.dicRewardEvent[missionINfo.SeasonEventKind] == (int)ChloeStoryEventManager.SeasonEventRewardState.ReceivedRewad)
                {
                    SetActiveMissionClearMark(true);
                }
                
                if(ChloeStoryEventManager.Instance.dicRewardEvent[missionINfo.SeasonEventKind] == (int)ChloeStoryEventManager.SeasonEventRewardState.CanReceiveReward)
                {
                    rewardNotice.SetActive(true);
                }
                else
                {
                    rewardNotice.SetActive(false);
                }
            }
            else
            {
                rewardNotice.SetActive(false);
            }
        }
    }
    public void Refresh_RewardEffect(int SeasonKind)
    {
        if (ChloeStoryEventManager.Instance.dicRewardEvent.ContainsKey(SeasonKind))
        {
            // dicRewardEvent에 존재한다 : 이미 관련 처리가 된 이벤트 
            if (ChloeStoryEventManager.Instance.dicRewardEvent[SeasonKind] == (int)ChloeStoryEventManager.SeasonEventRewardState.ReceivedRewad)
            {
                seasonData.RewardEffect();
            }
        }
        else
        {
            // dicRewardEvent에 존재하지않는다 : 새로 받을 수 있게된 이벤트 
            ChloeStoryEventManager.Instance.dicRewardEvent.Add(SeasonKind, (int)ChloeStoryEventManager.SeasonEventRewardState.CanReceiveReward);
        }

    }

    public void OnClick_RewardButtion()
    {
        // 서버에 보상 수령 가능 여부 확인
        if (seasonDataInfo != null)
        {
            if (seasonDataInfo.SeasonEventKind != 0)
            {
                if (ChloeStoryEventManager.Instance.dicRewardEvent.ContainsKey(seasonDataInfo.SeasonEventKind)) 
                { 
                    // 보상 수령 처리를 위한 패킷 서버로 송신
                    if (ChloeStoryEventManager.Instance.dicRewardEvent[seasonDataInfo.SeasonEventKind] == (int)ChloeStoryEventManager.SeasonEventRewardState.CanReceiveReward)
                    {
                        NFlatBufferBuilder.SendBytes<GS_SEASON_EVENT_REWARD_REQ>(ePACKET_ID.GS_SEASON_EVENT_REWARD_REQ, () => GS_SEASON_EVENT_REWARD_REQ.CreateGS_SEASON_EVENT_REWARD_REQ(FlatBuffers.NFlatBufferBuilder.FBB,
                         UserBase.User.UID,
                        (int)ChloeStoryEventManager.SeasonEventRewardState.ReceivedRewad,
                        seasonDataInfo.SeasonEventKind
                        ));

                        return;
                    }
                }
                var iteminfo = BaseTable.TableItemInfo.Instance.Get(seasonDataInfo.SeasonRewards[0].Kind);
                if (null == iteminfo)
                    return;
                var titleText = NLibCs.NTextManager.Instance.GetText(iteminfo.itemName);
                var descText = NLibCs.NTextManager.Instance.GetText(iteminfo.itemInstruction);

                ToolTipDlg.SetToolTipDlg(titleText, descText);
            }
        }
    }
}
