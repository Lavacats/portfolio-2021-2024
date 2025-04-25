using Framework.EventSystem;
using NETWORK.GAME;
using System;
using System.Collections.Generic;
using UnityEngine;

public partial class GUI_BookmarkPopup : PopupDisplay
{
    [SerializeField] private GameObject go_bookmarkPage = null;
    [SerializeField] private UISheet sheetList_bookmark = null;

    #region Bookmark Frame Func

    private void InitializeDefaultPage()
    {
        if (sheetList_bookmark)
        {
            sheetList_bookmark.Initialize(OnBookMarkSheetEvent);
            sheetList_bookmark.RegisterHandlerValidator<BookmarkSheetList.Block>(BookmarkSheetList.Block.OBJECTID);
        }

        if (go_bookmarkPage)
            go_bookmarkPage.SetActive(false);
    }

    private void EnterDefaultPage()
    {
        // 데이터 갱신

        // UI 갱신
        if (go_bookmarkPage)
            go_bookmarkPage.SetActive(true);

        InsertNewSheet_BookmarkList();
    }

    private void UpdateDefaultPage()
    {
        // UI 갱신
        InsertNewSheet_BookmarkList();
    }

    private void ExitDefaultPage()
    {
        if (go_bookmarkPage)
            go_bookmarkPage.SetActive(false);
    }

    private void OnUpdateGameEvent_DefaultPage(HashSet<string> eventIDs)
    {
        InsertNewSheet_BookmarkList();
    }

    private void OnDispatchGameEvent_DefaultPage(string eventID, IEventDispatchParam param)
    {

    }
    #endregion

    private void InsertNewSheet_BookmarkList()
    {
        var sheet = sheetList_bookmark;
        if (!sheet)
            return;

        sheet.ClearAllItems();

        if (m_eSelectMenuType == TAB_MENU.GUILD || m_eSelectMenuType == TAB_MENU.ALL)
        {
            // 연맹 북마크 정보 추가
            var BookMarkRepo = GuildManager.Instance.MyGuild.GuildBookmarkContainer.Repository;

            foreach (var pair in BookMarkRepo) //Pair Type => key : ObjID , value : BookMarkObj
            {
                var guildBookmarObj = pair.Value;
                var data = sheet.AddItem<BookmarkSheetList.ItemData>();
                data.Reset();
                data.bookMarkObjId = guildBookmarObj.MyObjID;
                data.name = guildBookmarObj.Name;
                data.xPos = guildBookmarObj.PosX;
                data.yPos = guildBookmarObj.PosY;
                data.bookMarkKind = guildBookmarObj.Kind;

                var bookMarkIconValue = COMMON_GUILD_INFO.Instance.GetBOOKMARKICON(guildBookmarObj.Kind);
                if (bookMarkIconValue != null)
                    data.spriteKey = bookMarkIconValue.SpriteKey;

            }
        }
        if (m_eSelectMenuType != TAB_MENU.GUILD)
        {
            // 유저 북마크 정보 추가
            var BookMarkRepo = User.BookMarkContainer.Repository;

            foreach (var pair in BookMarkRepo) //Pair Type => key : ObjID , value : BookMarkObj
            {
                var userBookMarkObj = pair.Value;
                if (m_eSelectMenuType != TAB_MENU.ALL)
                {
                    var eKIND = EnumBookmarkKind.None;
                    if (m_eSelectMenuType == TAB_MENU.PREFERENCE)
                        eKIND = EnumBookmarkKind.Preference;
                    else if (m_eSelectMenuType == TAB_MENU.FRIEND)
                        eKIND = EnumBookmarkKind.Friend;
                    else if (m_eSelectMenuType == TAB_MENU.ENEMY)
                        eKIND = EnumBookmarkKind.Enermy;

                    if (userBookMarkObj.Kind != (int)eKIND)
                        continue;
                }

                var data = sheet.AddItem<BookmarkSheetList.ItemData>();
                data.Reset();
                data.bookMarkObjId = userBookMarkObj.MyObjID;
                data.name = userBookMarkObj.Name;
                data.spriteKey = "SPR_USER_BOOKMARK_ICON";
                data.xPos = userBookMarkObj.PosX;
                data.yPos = userBookMarkObj.PosY;
                data.bookMarkKind = userBookMarkObj.Kind;

            }
        }

        sheet.UpdateAllItems();
        sheet.SelectItemAt(0);
    }

    private void BookMarkPosMove(int x, int y)
    {
        if (!Util.IsValidFieldCoord(x, y))
            return;

        GameCameraController.Instance.TroopObjID = new ObjID();
        GameCameraController.Instance.SetPosition(new Vector3(x, 0f, y));

        Close();
    }

    #region UISheet
    private void OnBookMarkSheetEvent(eSheetEvent eventType, UISheet uiSheet, UISheet.ItemData itemData, UISheetBlock sheetBlock, GameObject invokedBlockChild, int param)
    {
        var data = itemData as BookmarkSheetList.ItemData;
        if (null == data)
            return;

        switch (eventType)
        {
            case eSheetEvent.ItemClicked:
                {
                    BookMarkPosMove(data.xPos, data.yPos);
                }
                break;
        }
    }

    public class BookmarkSheetList
    {
        public class Block : UISheetBlockHandler
        {
            [SerializeField] private UILabel lbl_position = null;
            [SerializeField] private UILabel lbl_markName = null;
            [SerializeField] private UIButtonEx btn_delete = null;
            [SerializeField] private UIButtonEx btn_modify = null;
            [SerializeField] private UIButtonEx btn_translation = null;
            [SerializeField] private UISprite spr_BookMark = null;

            public const string OBJECTID = "sheetblock";

            protected override bool useAutoBindingByRootBinder { get { return true; } }

            protected override void OnInitialized()
            {
                base.OnInitialized();
                EventDelegate.Add(btn_delete.onClick, OnClick_DeleteButton);
                EventDelegate.Add(btn_modify.onClick, OnClick_ModifyButton);
                EventDelegate.Add(btn_translation.onClick, OnClick_TranslationButton);
            }

            protected override void OnUpdatedBlock(bool changedItem)
            {
                base.OnUpdatedBlock(changedItem);

                var data = GetItemData<ItemData>();
                if (null == data)
                    return;

                lbl_position.text = string.Format(Util.Text("COMMON_BOOKMARK_COORDINATE_X_Y"), data.xPos, data.yPos);
                lbl_markName.text = data.name;

                if (data.bookMarkObjId.KIND == (int)EnumBookmarkOwnerKind.Guild)
                {
                    Util.SpriteTo(spr_BookMark, data.spriteKey);
                    btn_translation.gameObject.SetActive(true);
                }
                else if (data.bookMarkObjId.KIND == (int)EnumBookmarkOwnerKind.User)
                {
                    Util.SpriteTo(spr_BookMark, "SPR_USER_BOOKMARK_ICON");
                    btn_translation.gameObject.SetActive(false);
                }
            }

            private void OnClick_DeleteButton()
            {
                var data = GetItemData<ItemData>();
                if (null == data)
                    return;

                if (data.bookMarkObjId.KIND == (int)EnumBookmarkOwnerKind.Guild)
                {
                    var pMemember = GuildManager.Instance.MyGuild.MemberContainer.pMyMemberObj;
                    if (pMemember == null)
                        return;

                    if (pMemember.Rank < (int)EnumGuildMemberRank.R4)
                    {
                        ToastMessage.Show(Util.Text("E_BOOKMARK_NOT_ENOUGH_RANK"));
                        return;
                    }
                }

                MessageBox.Show(Util.Text("COMMON_BOOKMARK_UI_DELETE_POPUP_NAME"), Util.Text("COMMON_BOOKMARK_UI_DELETE_POPUP_DESC"),(MessageBoxDefine.IFeedback feedback) =>
                {
                    switch (feedback.result)
                    {
                        case MessageBoxDefine.RESULT_OK:
                            {
                                SoundManager.Instance.Play_SFX(Defines.SFX_UI_BUTTON_ELIMINATION);

                                if (data.bookMarkObjId.KIND == (int)EnumBookmarkOwnerKind.Guild)
                                {
                                    if (!ObjID.IsEmpty(data.bookMarkObjId))
                                        SendHandle.SendGS_GUILD_BOOKMARK_DELETE_REQ(data.bookMarkObjId);
                                }
                                else if (data.bookMarkObjId.KIND == (int)EnumBookmarkOwnerKind.User)
                                {
                                    if (!ObjID.IsEmpty(data.bookMarkObjId))
                                        SendHandle.GS_USER_BOOKMARK_DELETE_REQ(data.bookMarkObjId);
                                }
                            }
                            break;

                        case MessageBoxDefine.RESULT_CANCEL:
                            break;
                    }
                }, Util.Text("COMMON_UI_GUILD_UP_GROUP_DELETE"), Util.Text("COMMON_UI_TEXT_010"));
            }
            private void OnClick_ModifyButton()
            {
                var data = GetItemData<ItemData>();
                if (null == data) return;

                if (data.bookMarkObjId.KIND == (int)EnumBookmarkOwnerKind.Guild)
                {
                    var pMemember = GuildManager.Instance.MyGuild.MemberContainer.pMyMemberObj;
                    if (pMemember == null)
                        return;

                    if (pMemember.Rank < (int)EnumGuildMemberRank.R4)
                    {
                        ToastMessage.Show(Util.Text("E_BOOKMARK_NOT_ENOUGH_RANK"));
                        return;
                    }
                }

                SoundManager.Instance.Play_SFX(Defines.SFX_UI_BUTTON_CLICK);

                GUIScene.Instance.OpenDisplay<GUI_BookmarkModifiedPopup>(new GUI_BookmarkModifiedPopup.PARAM(data.bookMarkObjId));
            }
            private void OnClick_TranslationButton()
            {
                var data = GetItemData<ItemData>();
                if (data == null) return;
                On_Translation(data);
            }
            void On_Translation(ItemData data)
            {
                string tCode = TranslateHelper.Instance.GetTranslateCode(); //szTranslateCode
                string strBookMarkName = data.name;
                ObjID tObjID = ObjID.Empty;

                if (data.bookMarkObjId.IsEmpty()) return;

                tObjID = data.bookMarkObjId;

                // 기존 번역 검색
                if (dicBookmarkTranslate.ContainsKey(tObjID))
                {
                    Tuple<string, string,string> value; // item1 = 언어 , item2 : 원문 , itme3 : 번역문
                    dicBookmarkTranslate.TryGetValue(tObjID, out value);
                    if (tCode == value.Item1 && data.name == value.Item2) 
                    {
                        Util.TextTo(lbl_markName, value.Item3);
                        return; // 기존 번역값
                    }
                }
                Action<TransData> onComplete = (transData) =>
                {
                    if (null == transData)
                        return;

                    if (dicBookmarkTranslate.ContainsKey(tObjID))
                    {
                        dicBookmarkTranslate.Remove(tObjID);
                    }
                    dicBookmarkTranslate.Add(tObjID, Tuple.Create(transData.dstLanguage, strBookMarkName, transData.dstText));
                    Util.TextTo(lbl_markName, transData.dstText);
                };
                TranslateHelper.Instance.Translate(strBookMarkName, onComplete);
                return;
            }
        }
        public class ItemData : UISheet.ItemData
        {
            public int xPos;
            public int yPos;
            public int bookMarkKind;
            public string name;
            public string spriteKey;
            public ObjID bookMarkObjId = ObjID.Empty;
            public override void Reset()
            {
                xPos = 0;
                yPos = 0;
                bookMarkKind = 0;
                name = string.Empty;
                spriteKey = string.Empty;
                bookMarkObjId = ObjID.Empty;
            }
        }
    }
    #endregion UISheet
}
