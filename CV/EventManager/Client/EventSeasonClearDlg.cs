using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BaseTable;
using NLibCs;
using DEFINE;
using Contents.User;
using FlatBuffers;
using PROTOCOL.COMMON;
using PROTOCOL.FLATBUFFERS;
using PROTOCOL.GAME.ID;

public class EventSeasonClearDlg : UIForm
{
    [SerializeField] private UISprite ClearImage;              
    [SerializeField] private UILabel ClearTitle;               
    [SerializeField] private UILabel ClearPhrase;
    [SerializeField] private UIButton closeButton;

    public override void Open(IUIParamBase param)
    {
        IsExceptionBlur = true;

        TotalEventListDlg.FirstOpen = false;

        SetPanelDepth_ByLastDepth();
        base.Open(param);

        var clearImageKey = string.Empty;
        var clearTitlekey = string.Empty;
        var clearDescKey = string.Empty;

        var seasonInfo = TableChloeEventInfo.Instance.GetChloeEventSeasonInfo(SeasonPassManager.Instance.GetCurSeason(), SeasonPassManager.Instance.GetCurPeriod(), 0);
        if (seasonInfo != null)
        {
            clearImageKey = seasonInfo.CompleteSprite;
            clearTitlekey = seasonInfo.SeasonTitle;
            clearDescKey = seasonInfo.CompleteText;

        }
        if (ClearImage) ClearImage.spriteName = clearImageKey;
        if (ClearTitle)SetText(ref ClearTitle, NTextManager.Instance.GetText(clearTitlekey));
        if(ClearPhrase)SetText(ref ClearPhrase, NTextManager.Instance.GetText(clearDescKey));

        if (closeButton) EventDelegate.Add(closeButton.onClick, Onclick_Close);

    }

    public void Onclick_Close()
    {
        if(UIFormManager.Instance.IsOpenUIForm<EventSeasonClearDlg>())
        {
            UIFormManager.Instance.CloseUIForm<EventSeasonClearDlg>();
        }


        if (false == UIFormManager.Instance.IsOpenUIForm<EventSeasonDlg>()
            && UIFormManager.Instance.IsOpenUIForm<TotalEventListDlg>())
        {
            NFlatBufferBuilder.SendBytes<GS_SEASON_EVENT_INFO_REQ>(ePACKET_ID.GS_SEASON_EVENT_INFO_REQ, () => GS_SEASON_EVENT_INFO_REQ.CreateGS_SEASON_EVENT_INFO_REQ(FlatBuffers.NFlatBufferBuilder.FBB, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1));
        }
    }
}
