using Framework.EventSystem;
using NETWORK.GAME;
using System;
using System.Collections.Generic;
using UnityEngine;

public partial class GUI_BookmarkPopup : PopupDisplay
{
    [SerializeField] private GameObject go_registerPage = null;

    [SerializeField] UIInput input_name = null;
    [SerializeField] UILabel lbl_position = null;
    [SerializeField] UIButtonEx btn_register = null;
    [SerializeField] RadioObjects radio_register = null;
    [SerializeField] RadioObjects radio_iconGuild = null;       // 0 None , 1 공격 , 2 방어 , 3 집결 , 4 타겟 
    [SerializeField] UILabel[] lbl_iconGuildName = null;
    [SerializeField] UISprite[] spr_arrayIconDimmed = null;
    [SerializeField] UIButtonEx[] btn_arrayRegisterType = null;
    [SerializeField] UIButtonEx[] btn_arrayIconGuildType = null;

    private readonly Tuple<Int32, EnumBookmarkOwnerKind, EnumBookmarkKind>[] dicSelectData =
    {
        new Tuple<Int32, EnumBookmarkOwnerKind, EnumBookmarkKind>(0, EnumBookmarkOwnerKind.User,   EnumBookmarkKind.Preference),
        new Tuple<Int32, EnumBookmarkOwnerKind, EnumBookmarkKind>(1, EnumBookmarkOwnerKind.Guild,  EnumBookmarkKind.None),
        new Tuple<Int32, EnumBookmarkOwnerKind, EnumBookmarkKind>(2, EnumBookmarkOwnerKind.User,   EnumBookmarkKind.Friend),
        new Tuple<Int32, EnumBookmarkOwnerKind, EnumBookmarkKind>(3, EnumBookmarkOwnerKind.User,   EnumBookmarkKind.Enermy)
    };

    private readonly Tuple<Int32, EnumGuildBookmarkKind>[] dicSelectGuildIcon =
    {
        new Tuple<Int32, EnumGuildBookmarkKind>(0, EnumGuildBookmarkKind.Attack),
        new Tuple<Int32, EnumGuildBookmarkKind>(1, EnumGuildBookmarkKind.Deffence),
        new Tuple<Int32, EnumGuildBookmarkKind>(2, EnumGuildBookmarkKind.Rally),
        new Tuple<Int32, EnumGuildBookmarkKind>(3, EnumGuildBookmarkKind.Target)
    };

    private enum FailedReasonRegister
    {
        None = 0, 
        Invalid_Position = 1 << 1,
        Name_LimitInvalid = 1 << 2,
        Name_Empty = 1 << 3,
        Data_CountMax = 1 << 4,
        Data_GuildCountMax = 1 << 5,
        Data_ExistPos = 1 << 6,
        Data_ExistType = 1 << 7,
        Data_ExistKind = 1 << 8, 
        Data_EnoughRank = 1 << 9,
        Guild_Empty = 1 << 10,
    }

    private ObjID regi_targetObjID = ObjID.Empty;
    private int regi_posX = 0;
    private int regi_posY = 0;
    private string regi_targetName = string.Empty;
    private Tuple<Int32, EnumBookmarkOwnerKind, EnumBookmarkKind> regiSelectData = new Tuple<Int32, EnumBookmarkOwnerKind, EnumBookmarkKind>(0, EnumBookmarkOwnerKind.User, EnumBookmarkKind.Preference);
    private Tuple<Int32, EnumGuildBookmarkKind> regiSelectGuildIcon = new Tuple<Int32, EnumGuildBookmarkKind>(0, EnumGuildBookmarkKind.Attack);

    private UInt64 nFailedReasonRegister = (UInt64)FailedReasonRegister.None;

    #region Bookmark Frame Func

    private void InitializeRegisterPage()
    {
        if (go_registerPage)
            go_registerPage.SetActive(false);

        if (radio_iconGuild)
            radio_iconGuild.gameObject.SetActive(false);

        if (btn_register)
        {
            btn_register.allowOnClickEventWhenDisabled = true;
            BindControlEvent(btn_register, btn_register.onClick, OnClickRegister);
        }

        if (input_name)
        {
            BindControlEvent(input_name, input_name.onChange, OnChangeInputRegister);
            BindControlEvent(input_name, input_name.onSubmit, OnChangeInputRegister);
        }

        if (null != btn_arrayRegisterType)
        {
            for (int i = 0; i < btn_arrayRegisterType.Length; ++i)
            {
                if (!btn_arrayRegisterType[i])
                    continue;

                var param = dicSelectData[i];
                btn_arrayRegisterType[i].tweenTarget = null;
                BindControlEvent(btn_arrayRegisterType[i], btn_arrayRegisterType[i].onClick, OnClickRadioRegisterType, param);
            }
        }

        if (null != btn_arrayIconGuildType)
        {
            for (int i = 0; i < btn_arrayIconGuildType.Length; ++i)
            {
                if (!btn_arrayIconGuildType[i])
                    continue;

                var param = dicSelectGuildIcon[i];
                btn_arrayIconGuildType[i].tweenTarget = null;
                BindControlEvent(btn_arrayIconGuildType[i], btn_arrayIconGuildType[i].onClick, OnClickIconGuild, param);
            }
        }
    }

    private void EnterRegisterPage()
    {
        if (go_registerPage)
            go_registerPage.SetActive(true);

        // 데이터 초기화
        regiSelectData = dicSelectData[0];
        regiSelectGuildIcon = dicSelectGuildIcon[0];

        // 데이터 갱신
        RefreshData_FailedReasonRegister();

        // UI 갱신 
        RefreshUI_Default();
        RefreshUI_RegisterType();
        RefreshUI_GuildIcon();
        RefreshUI_RegisterButton();

        // 추가 처리
        _on_enter_register_page_init_input_();
    }

    private void _on_enter_register_page_init_input_()
    {
        string strCoord = Util.TextFormat("MAIL_UI_POSITION", regi_posX, regi_posY);

        if (input_name)
        {
            if (string.IsNullOrEmpty(regi_targetName))
                input_name.value = string.Format("[{0}]", strCoord);
            else
                input_name.value = regi_targetName;
        }
    }

    private void UpdateRegisterPage()
    {
        // UI 갱신 
        RefreshUI_Default();
        RefreshUI_RegisterType();
        RefreshUI_GuildIcon();
        RefreshUI_RegisterButton();
    }

    private void ExitRegisterPage()
    {
        if (go_registerPage)
            go_registerPage.SetActive(false);
    }

    private void OnUpdateGameEvent_RegisterPage(HashSet<string> eventIDs)
    {

    }

    private void OnDispatchGameEvent_RegisterPage(string eventID, IEventDispatchParam param)
    {
        // 데이터 갱신
        RefreshData_FailedReasonRegister();

        // UI 갱신 
        RefreshUI_GuildIcon();
        RefreshUI_RegisterButton();
    }
    #endregion

    #region Callback Func

    private void OnClickRegister(UIButtonEx button, object param)
    {
        if (Util.IsFlag(nFailedReasonRegister, (ulong)FailedReasonRegister.Invalid_Position))
            return;

        if (Util.IsFlag(nFailedReasonRegister, (ulong)FailedReasonRegister.Name_LimitInvalid))
        {
            ToastMessage.Show(Util.Text("E_BOOKMARK_LETTER_LIMIT_OVER"));
            return;
        }

        if (Util.IsFlag(nFailedReasonRegister, (ulong)FailedReasonRegister.Name_Empty))
        {
            ToastMessage.Show(Util.Text("COMMON_BOOKMARK_NON_NAME"));
            return;
        }

        if (Util.IsFlag(nFailedReasonRegister, (ulong)FailedReasonRegister.Data_GuildCountMax))
        {
            ToastMessage.Show(Util.Text("COMMON_GUILD_BOOKMARK_NOTICE_BOOKMARK_FULL"));
            return;
        }

        if (Util.IsFlag(nFailedReasonRegister, (ulong)FailedReasonRegister.Data_CountMax))
        {
            ToastMessage.Show(Util.Text("COMMON_BOOKMARK_NOTICE_BOOKMARK_FULL"));
            return;
        }

        if (Util.IsFlag(nFailedReasonRegister, (ulong)FailedReasonRegister.Data_ExistPos))
        {
            ToastMessage.Show(Util.Text("E_GUILD_BOOKMARK_ALREADY_EXIST_POS"));
            return;
        }

        if (Util.IsFlag(nFailedReasonRegister, (ulong)FailedReasonRegister.Data_ExistType))
        {
            ToastMessage.Show(Util.Text("E_GUILD_BOOKMARK_ALREADY_EXIST_TYPE"));
            return;
        }

        if (Util.IsFlag(nFailedReasonRegister, (ulong)FailedReasonRegister.Data_EnoughRank))
        {
            ToastMessage.Show(Util.Text("E_BOOKMARK_NOT_ENOUGH_RANK"));
            return;
        }
        if (Util.IsFlag(nFailedReasonRegister, (ulong)FailedReasonRegister.Guild_Empty))
        {
            ToastMessage.Show(Util.Text("E_GUILD_NOT_FOUND"));
            return;
        }

        if (regiSelectData.Item2 == EnumBookmarkOwnerKind.Guild)
            SendHandle.SendGS_GUILD_BOOKMARK_CREATE_REQ((int)regiSelectGuildIcon.Item2, input_name.value, regi_posX, regi_posY, regi_targetObjID);
        else
            SendHandle.SendGS_USER_BOOKMARK_CREATE_REQ((int)regiSelectData.Item3, input_name.value, regi_posX, regi_posY, regi_targetObjID);
    }

    private void OnClickIconGuild(UIButtonEx button, object param)
    {
        var pData = (Tuple<Int32, EnumGuildBookmarkKind>)param;
        if (regiSelectGuildIcon == pData)
            return;

        // 데이터 갱신 
        regiSelectGuildIcon = pData;
        RefreshData_FailedReasonRegister();

        // UI 갱신
        RefreshUI_GuildIcon();
        RefreshUI_RegisterButton();
    }
    private void OnClickRadioRegisterType(UIButtonEx button, object param)
    {
        var pData = (Tuple<Int32, EnumBookmarkOwnerKind, EnumBookmarkKind>)param;
        if (regiSelectData == pData)
            return;

        // 데이터 갱신
        regiSelectData = pData;
        RefreshData_FailedReasonRegister();

        // UI 갱신
        RefreshUI_RegisterType();
        RefreshUI_GuildIcon();
        RefreshUI_RegisterButton();
    }

    private void OnChangeInputRegister(UIInput input, object param)
    {
        // 데이터 갱신 
        RefreshData_FailedReasonRegister();

        // UI 갱신 
        RefreshUI_RegisterButton();
    }
    #endregion

    private void RefreshData_FailedReasonRegister()
    {
        // 초기화
        nFailedReasonRegister = (UInt64)FailedReasonRegister.None;

        if (!Util.IsValidFieldCoord(regi_posX, regi_posY))
            Util.SetFlag(ref nFailedReasonRegister, (ulong)FailedReasonRegister.Invalid_Position);

        if (input_name.value.Length >= COMMON_CONST_INFO.BOOKMARK_NAME_LENGTH_MAX)
            Util.SetFlag(ref nFailedReasonRegister, (ulong)FailedReasonRegister.Name_LimitInvalid);

        if (string.IsNullOrEmpty(input_name.value))
            Util.SetFlag(ref nFailedReasonRegister, (ulong)FailedReasonRegister.Name_Empty);

        if (regiSelectData.Item2 == EnumBookmarkOwnerKind.Guild)
        {
            if (GuildManager.Instance.MyGuild.GuildID.IsEmpty())
            {
                Util.SetFlag(ref nFailedReasonRegister, (ulong)FailedReasonRegister.Guild_Empty);
                return; // 길드가 없으므로 return;
            }

            if (COMMON_CONST_INFO.BOOKMARK_GUILD_COUNT_MAX < GuildManager.Instance.MyGuild.GuildBookmarkContainer.Count + 1)
                Util.SetFlag(ref nFailedReasonRegister, (ulong)FailedReasonRegister.Data_GuildCountMax);

            var vKeyPos = new Vector2(regi_posX, regi_posY);
            if (GuildManager.Instance.MyGuild.GuildBookmarkContainer.GetDataByPos(vKeyPos) != null)
                Util.SetFlag(ref nFailedReasonRegister, (ulong)FailedReasonRegister.Data_ExistPos);

            if (GuildManager.Instance.MyGuild.GuildBookmarkContainer.GetDataByBookmarkKind(regiSelectGuildIcon.Item2) != null)
                Util.SetFlag(ref nFailedReasonRegister, (ulong)FailedReasonRegister.Data_ExistKind);

            var pMemember = GuildManager.Instance.MyGuild.MemberContainer.pMyMemberObj;
            if (pMemember == null)
            {
                Util.SetFlag(ref nFailedReasonRegister, (ulong)FailedReasonRegister.Data_EnoughRank);
            }
            else if (pMemember.Rank < (int)EnumGuildMemberRank.R4 )
            {
                Util.SetFlag(ref nFailedReasonRegister, (ulong)FailedReasonRegister.Data_EnoughRank);
            }


        }
        else
        {
            if (COMMON_CONST_INFO.BOOKMARK_COUNT_MAX < User.BookMarkContainer.Count + 1)
                Util.SetFlag(ref nFailedReasonRegister, (ulong)FailedReasonRegister.Data_CountMax);
        }
    }

    private void RefreshUI_Default()
    {
        string strCoord = Util.TextFormat("MAIL_UI_POSITION", regi_posX, regi_posY);

        if (lbl_position)
            lbl_position.text = strCoord;
    }

    private void RefreshUI_RegisterType()
    {
        if (radio_register)
            radio_register.turnon = regiSelectData.Item1;
    }

    private void RefreshUI_GuildIcon()
    {
        if (regiSelectData.Item2 != EnumBookmarkOwnerKind.Guild)
        {
            if (radio_iconGuild)
                radio_iconGuild.gameObject.SetActive(false);

            return;
        }

        if (radio_iconGuild)
        {
            radio_iconGuild.gameObject.SetActive(true);
            radio_iconGuild.turnon = regiSelectGuildIcon.Item1;
        }

        bool isBtnEnable = true;
        foreach (var data in dicSelectGuildIcon)
        {
            isBtnEnable = true;
            if (GuildManager.Instance.MyGuild.GuildBookmarkContainer.GetDataByBookmarkKind(data.Item2) != null)
                isBtnEnable = false;

            if (btn_arrayIconGuildType[data.Item1])
            {
                btn_arrayIconGuildType[data.Item1].isEnabled = isBtnEnable;
                btn_arrayIconGuildType[data.Item1].SetState(isBtnEnable ? UIButtonColor.State.Normal : UIButtonColor.State.Disabled, true);
            }
            
            if (spr_arrayIconDimmed[data.Item1])
                spr_arrayIconDimmed[data.Item1].gameObject.SetActive(!isBtnEnable);
        }
    }

    private void RefreshUI_RegisterButton()
    {
        if (!btn_register)
            return;

        btn_register.isEnabled = nFailedReasonRegister == (ulong)FailedReasonRegister.None;
        btn_register.SetState(btn_register.isEnabled ? UIButtonColor.State.Normal : UIButtonColor.State.Disabled, true);
    }
}
