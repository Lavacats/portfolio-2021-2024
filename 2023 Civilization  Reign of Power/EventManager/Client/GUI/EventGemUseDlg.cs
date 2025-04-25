using System;
using UnityEngine;
using System.Collections.Generic;
using Contents.User;
using DEFINE;
using PROTOCOL.FLATBUFFERS;
using PROTOCOL.GAME.ID;
using FlatBuffers;
using BaseTable;
using NLibCs;
using EVENT;

public class EventGemUseDlg : UIForm
{
    [SerializeField] private UILabel EventTitle;                    
    [SerializeField] private UILabel EventDesc;                     
    [SerializeField] private UILabel EventLeftTime;                 
    [SerializeField] private UIButton questionMarkButton;
    [SerializeField] private UITextureEx EventBackGroundImage;

    [SerializeField] private UILabel _curPointlabel = null;
    [SerializeField] private UILabel _timelabel = null;
    [SerializeField] private UISheet _sheetFinalReward = null;

    [SerializeField] private float rewardUILength;

    [Header("sheet")]
    [SerializeField] private UIDragScrollView _scrollPoint;

    private EventGemUseSlider _useEventSlider = null;
    private int useEventGroupKind = 0;
   

    public override void Init()
    {
        base.Init();
        IsExceptionBlur = true;

        if (_scrollPoint)
        {
            var dailypointroot = _scrollPoint.gameObject;
            if (dailypointroot != null)
            {
                _useEventSlider = dailypointroot.SetComponent<EventGemUseSlider>();
                _useEventSlider.rewardUILength = rewardUILength;
                _useEventSlider.SetUI();
       
            }
        }
        useEventGroupKind = UseEventManager.Instance.selectEventGroupKind;
    }

    public override void BindUIEvents()
    {
        base.BindUIControls();

        _sheetFinalReward.Initialize();
        _sheetFinalReward.RegisterHandlerValidator<EventGemUseDlg.FinalRewardSheetBlock>(EventGemUseDlg.FinalRewardSheetBlock.OBJECTID);
        if (questionMarkButton) EventDelegate.Add(questionMarkButton.onClick, OnClick_Question);
    }

    public override void Open(IUIParamBase param)
    {
        IsExceptionBlur = true;

        TotalEventListDlg.FirstOpen = false;
        TotalEventListDlg.CloseAllSubEventDlg(this);

        SetPanelDepth_ByLastDepth();

        base.Open(param);

        SetEventUI();

    }

    public override void Close()
    {
        base.Close();
    }

    public override void Close_End()
    {
        base.Close_End();
    }

    public override void Update()
    {
        base.Update();

        SetUseEventTime();
    }
    public void SetUseEventTime()
    {
        if (UseEventManager.Instance.dicUseEventinfo.ContainsKey(useEventGroupKind))
        {
            var CurDate = PublicMethod.GetNowDate_Utc();
            var eventEndTime = UseEventManager.Instance.dicUseEventinfo[useEventGroupKind].eventEndTime;
            var TimeOut = PublicMethod.GetDueDate_Utc(eventEndTime) - CurDate;

            var hoursText = (TimeOut.Hours / 10 < 1 ? $"0{TimeOut.Hours}" : $"{TimeOut.Hours}");
            var minutesText = (TimeOut.Minutes / 10 < 1 ? $"0{TimeOut.Minutes}" : $"{TimeOut.Minutes}");
            var secondsText = (TimeOut.Seconds / 10 < 1 ? $"0{TimeOut.Seconds}" : $"{TimeOut.Seconds}");

            _timelabel.text = $"{TimeOut.Days}" + NTextManager.Instance.GetText("COMMON_MEASURE_DAY_COUNT") + $" {hoursText}" + ":" + $"{minutesText}" + ":" + $"{secondsText}";

            if (PublicMethod.GetDueDate_Utc(eventEndTime) < CurDate)
            {
                if (UIFormManager.Instance.IsOpenUIForm<TotalEventListDlg>())
                {
                    var useEventUi = UIFormManager.Instance.GetUIForm<TotalEventListDlg>();
                    useEventUi.Close();
                }
            }
        }
    }
    public void SetEventUI()
    {
        useEventGroupKind = UseEventManager.Instance.selectEventGroupKind;
        var userSelectGroupKindEvent = TableUseEventInfo.Instance.GetEventInfo(useEventGroupKind);
        int curPoint = 0;


        SetFinalReward();
        ImageLoad(EventBackGroundImage, userSelectGroupKindEvent.strTab_URL);
        if (userSelectGroupKindEvent != null)
        {
            SetText(ref EventTitle, NTextManager.Instance.GetText(userSelectGroupKindEvent.strMainTitle_Text_Key));
            SetText(ref EventDesc, NTextManager.Instance.GetText(userSelectGroupKindEvent.strSub_Title_Text_Key));
            UseEventManager.Instance.dicUseEventpoint.TryGetValue(useEventGroupKind, out curPoint);
            _curPointlabel.text = curPoint.ToString();

        }
 
    }
    public void ImageLoad(UITextureEx tex, string ImageName)
    {
        //ÀÌ¹ÌÁö ¼ÂÆÃ
        if (null != tex)
        {
            EventManager.Instance.ImageLoad(EventManager.DAILYIMAGE_KEY, ImageName, (wwwdata, error) =>
            {
                if (wwwdata != null)
                {
                    EventBackGroundImage.mainTexture = wwwdata;
                }
            });
        }
    }
    public void RefreshUI()
    {
        SetEventUI();
        _useEventSlider.RefreshUI();
    }
    public void RefreshSlider(int eventKind)
    {
        _useEventSlider.RefreshItem(eventKind);
    }
    public void SetFinalReward()
    {
        _sheetFinalReward.ClearAllItems();
        var vecReward = TableUseEventInfo.Instance.GetFinalReward(useEventGroupKind);
        for (int i = 0; i < vecReward.Count; i++)
        {
            var data = _sheetFinalReward.AddItem<EventGemUseDlg.ItemData>();

            if (data == null) continue;

            var rewardItemInfo = TableItemInfo.Instance.Get(vecReward[i].Key);
            if (rewardItemInfo == null) continue;

            data.useEventItemKind = rewardItemInfo.itemKind;
            data.useEventItemValue = rewardItemInfo.itemvalue1;
            data.useEventItemCount = vecReward[i].Value;
        }
        _sheetFinalReward.UpdateAllItems();
    }
    private void OnClick_Question()
    {
        PublicSoundMethod.PlayClickCommon(GameDefine.eCLICK_SOUND_TYPE.CLICK_SOUND_4);

        var contentskind = (int)GameDefine.eQuestionPopUpContentsType.UseEvent;
        BaseQuestionDlg.OpenQuestionDlg(contentskind);
    }

    public class FinalRewardSheetBlock : UISheetBlockHandler
    {
        public const string OBJECTID = "SheetBlock_Item";

        static int useEventRewardMaxCount = 3;
        protected override bool useAutoBindingByRootBinder => true;

        private UISprite _spriteItem = new UISprite();
        private UILabel _labelItemNum = new UILabel();
        private UILabel _labelItemVol = new UILabel();

        protected override void OnInitialized()
        {
            base.OnInitialized();
        }
        protected override void OnUpdatedBlock(bool changedItem)
        {
            var data = GetItemData<ItemData>();
            if (data == null) return;

            var itemInfo = TableItemInfo.Instance.Get(data.useEventItemKind);
            if (itemInfo == null) return;

            SetSprite(itemInfo.atlasName, itemInfo.itemIcon, data.useEventItemCount, itemInfo.itemvalue2);
        }
        private void SetSprite(string atlasName, string iconName, int itemCount, int itemVolume)
        {
            if (_spriteItem == null) return;
            if (_labelItemNum == null) return;
            if (_labelItemNum == null) return;
            if (_labelItemVol == null) return;

            if (itemVolume > 0)
                _labelItemVol.text = PublicUIMethod.GetUnitizedString(itemVolume);
            else
                _labelItemVol.text = string.Empty;
            
            _spriteItem.atlas.Change(atlasName);
            _spriteItem.spriteName = iconName;
            _labelItemNum.text = itemCount.ToString();
        }
    }
    public class ItemData : UISheet.ItemData
    {
        public int useEventItemKind;
        public int useEventItemValue;
        public int useEventItemCount;
    }
}
