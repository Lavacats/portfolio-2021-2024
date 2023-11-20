using System.Collections;
using System.Collections.Generic;
using NETWORK.GAME;
using UnityEngine;

public class GUI_BookmarkModifiedPopup : PopupDisplay
{
    public class PARAM : IGUIOnOpenedParam, IGUIOnRefreshedParam
    {
        public ObjID bookMarkObjID = ObjID.Empty;
        public PARAM(ObjID objID)
        {
            bookMarkObjID = objID;
        }
    }
    [SerializeField] UILabel lbl_position = null;
    [SerializeField] UIInput input_name = null;
    [SerializeField] UIButtonEx btn_register = null;

    private int bookMarkPosX = 0;
    private int bookMarkPosY = 0;
    private int bookMarkKind = 0;
    private string bookMarkName = null;
    private ObjID bookMarkObjId = ObjID.Empty;
    protected override void OnBuilt()
    {
        base.OnBuilt();

        if (null != btn_register)
            BindControlEvent(btn_register, btn_register.onClick, OnClickRegister);
    }
    protected override void OnOpened(IGUIOnOpenedParam param)
    {
        base.OnOpened(param);

        var openParam = param as PARAM;
        if (openParam == null)
        {
            Close();
            return;
        }
        if (openParam.bookMarkObjID.KIND == (int)EnumBookmarkOwnerKind.Guild)
        {
           var guildBookMark = GuildManager.Instance.MyGuild.GuildBookmarkContainer.GetData(openParam.bookMarkObjID);
            if (guildBookMark == null) 
            { 
                Close(); 
                return;
            }
            bookMarkObjId = guildBookMark.MyObjID;
            bookMarkPosX = guildBookMark.PosX;
            bookMarkPosY = guildBookMark.PosY;
            bookMarkKind = guildBookMark.Kind;
            bookMarkName = guildBookMark.Name;
        }
        else if (openParam.bookMarkObjID.KIND == (int)EnumBookmarkOwnerKind.User)
        {
            var userBookMark = User.BookMarkContainer.GetData(openParam.bookMarkObjID);
            if (userBookMark == null)
            {
                Close();
                return;
            }
            bookMarkObjId = userBookMark.MyObjID;
            bookMarkPosX = userBookMark.PosX;
            bookMarkPosY = userBookMark.PosY;
            bookMarkKind = userBookMark.Kind;
            bookMarkName = userBookMark.Name;
        }
        UpdateUI();
    }
    protected override void OnRefreshed(IGUIOnRefreshedParam param, bool opening)
    {
        base.OnRefreshed(param, opening);

        if (opening)
        {

        }
        else
        {
            UpdateUI();
        }
    }
    private void OnClickRegister(UIButtonEx button, object param)
    {
        if (!StageManager.Instance.IsCurrentStage<StageField>()) return;

        if (input_name.value.Length >= COMMON_CONST_INFO.BOOKMARK_NAME_LENGTH_MAX)
        {
            ToastMessage.Show(Util.Text("E_BOOKMARK_LETTER_LIMIT_OVER"));
            return;
        }

        if (!bookMarkObjId.IsEmpty())
        {
            if (!string.IsNullOrEmpty(input_name.value))
            {
                if (bookMarkObjId.KIND == (int)EnumBookmarkOwnerKind.Guild)
                {
                    SendHandle.SendGS_GUILD_BOOKMARK_MODIFY_REQ(bookMarkObjId, input_name.value, bookMarkPosX, bookMarkPosY);
                }
                else if (bookMarkObjId.KIND == (int)EnumBookmarkOwnerKind.User)
                { 
                    SendHandle.SendGS_USER_BOOKMARK_MODIFY_REQ(bookMarkObjId, input_name.value, bookMarkPosX, bookMarkPosY);
                }
                Close();
            }
            else
                ToastMessage.Show(Util.Text("COMMON_BOOKMARK_NON_NAME"));
        }
    }
    private void UpdateUI()
    {
        if (bookMarkObjId.IsEmpty()) return;

        if (lbl_position != null)
        {
            lbl_position.text = string.Format(Util.Text("MAIL_UI_POSITION"), bookMarkPosX, bookMarkPosY);
        }
        if (input_name != null)
        {
            input_name.value = bookMarkName;
        }
    }
}
