using Framework.EventSystem;
using NETWORK.GAME;
using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 해당 GUI_BookmarkPopup.cs 파일은 북마크 팝업창의 탭 페이징 기능만 담고 있습니다.
/// 
/// 각각의 탭은 partial class로 나누어져 있습니다.
/// GUI_BookmarkPopup_BookmarkPage.cs       <= ALL, PREFERENCE, FRIEND, ENEMY
/// GUI_BookmarkPopup_MovePage.cs           <= MOVE
/// GUI_BookmarkPopup_RegisterPage.cs       <= REGISTER
/// GUI_BookmarkPopup_SharePage.cs          <= SHARE
/// </summary>
public partial class GUI_BookmarkPopup : PopupDisplay
{
    /// <summary>
    /// 탭 종류
    /// </summary>
	public enum TAB_MENU { ALL, PREFERENCE,GUILD, FRIEND, ENEMY, MOVE, REGISTER, SHARE, END }

    /// <summary>
    /// 각각의 탭 페이지는 INIT, ENTER, UPDATE, EXIT 함수를 가지고 있습니다.
    /// </summary>
    public enum MENU_FUNC { INIT, ENTER, UPDATE, EXIT, END }


    // dicBookmarkTranslate ( Language / Text /  TransText ) [ 언어 / 원문 / 번역문 ]
    private static Dictionary<ObjID, Tuple<string, string,string>> dicBookmarkTranslate = new Dictionary<ObjID, Tuple<string, string,string>>();

    public class PARAM_COORDBASE : IGUIOnOpenedParam, IGUIOnRefreshedParam
	{
		public ObjID objID = ObjID.Empty;
		public int lv = 1;
		public string name = string.Empty;
		public string subName = string.Empty;
		public string guildNick = string.Empty;
		public int posX = 0;
		public int posY = 0;
		public TAB_MENU selectMenu = TAB_MENU.END;

        public PARAM_COORDBASE(ObjID ID, int xPos, int yPos, TAB_MENU selectMenuType = TAB_MENU.MOVE)
        {
			objID = ID;
			this.posX = xPos;
            this.posY = yPos;
            this.selectMenu = selectMenuType;
        }

        public PARAM_COORDBASE(ObjID ID, int level, string targetName, string targetSubName, string guildNick, int xPos, int yPos, TAB_MENU selectMenuType = TAB_MENU.MOVE)
		{
			if(!ObjID.IsEmpty(ID))
			{
				if((EnumCategory)ID.Category != EnumCategory.CellCluster)
				{
					this.objID = ID;
					this.lv = level;
					this.name = targetName;
					this.subName = targetSubName;
					this.guildNick = guildNick;
				}
			}
			
			this.posX = xPos;
			this.posY = yPos;
			this.selectMenu = selectMenuType;
		}

		public string GetNameTAGString()
		{
			if (string.IsNullOrEmpty(guildNick) && string.IsNullOrEmpty(name))
				return string.Empty;
			return Util.MakeNameTagText(lv.ToString(), guildNick, name, false, EnumNameTagType.Field_UI);
		}
	}
	
	[SerializeField] private UIButtonEx btn_close = null;

	[SerializeField] private UISheet sheetList_tab = null;
	
	private TAB_MENU m_eSelectMenuType = TAB_MENU.END;
    private Action[][] MenuFunc;

	protected override void OnBuilt()
	{
		base.OnBuilt();

        // --------------------------------------------- Bind Event
        if (btn_close)
            BindControlEvent(btn_close, btn_close.onClick, OnClickClose);

        if (btn_move)
            BindControlEvent(btn_move, btn_move.onClick, OnClickMove);

        if (btn_share2Server)
            BindControlEvent(btn_share2Server, btn_share2Server.onClick, OnClickShare2Server);

        if (btn_share2Guild)
            BindControlEvent(btn_share2Guild, btn_share2Guild.onClick, OnClickShare2Guild);

        if (btn_share2Cpy)
            BindControlEvent(btn_share2Cpy, btn_share2Cpy.onClick, OnClickShare2Cpy);
		

        MenuFunc = new Action[(int)TAB_MENU.END][]
        {
            new Action[(int)MENU_FUNC.END] { InitializeDefaultPage, EnterDefaultPage, UpdateDefaultPage, ExitDefaultPage },     // ALL
            new Action[(int)MENU_FUNC.END] { InitializeDefaultPage, EnterDefaultPage, UpdateDefaultPage, ExitDefaultPage },     // PREFERENCE
            new Action[(int)MENU_FUNC.END] { InitializeDefaultPage, EnterDefaultPage, UpdateDefaultPage, ExitDefaultPage },     // GUILD
            new Action[(int)MENU_FUNC.END] { InitializeDefaultPage, EnterDefaultPage, UpdateDefaultPage, ExitDefaultPage },     // FRIEND
            new Action[(int)MENU_FUNC.END] { InitializeDefaultPage, EnterDefaultPage, UpdateDefaultPage, ExitDefaultPage },     // ENEMY
            new Action[(int)MENU_FUNC.END] { InitializeMovePage, EnterMovePage, UpdateMovePage, ExitMovePage },                 // MOVE
            new Action[(int)MENU_FUNC.END] { InitializeRegisterPage, EnterRegisterPage, UpdateRegisterPage, ExitRegisterPage }, // REGISTER
            new Action[(int)MENU_FUNC.END] { InitializeSharePage, EnterSharePage, UpdateSharePage, ExitSharePage },             // SHARE
        };

        // --------------------------------------------- Initialize
        if (sheetList_tab)
        {
            sheetList_tab.Initialize(OnTabSheetEvent);
            sheetList_tab.RegisterHandlerValidator<TabSheetList.Block>(TabSheetList.Block.OBJECTID);
        }

        for (var menu = TAB_MENU.ALL; menu < TAB_MENU.END; ++menu)
            CallMenuFunc(menu, MENU_FUNC.INIT);

        // --------------------------------------------- Set Control
	}

	protected override void OnOpened(IGUIOnOpenedParam param)
	{
		base.OnOpened(param);

        if (!StageManager.Instance.IsCurrentStage<StageField>())
        {
            Close();
            return;
        }

        int guildIconIndex = 0;
        foreach (var pair in COMMON_GUILD_INFO.Instance.GetBOOKMARKICON())
        {
            var bookMarkValue = pair.Value;
            if (!lbl_iconGuildName[guildIconIndex])
                continue;
            lbl_iconGuildName[guildIconIndex].text = Util.Text(bookMarkValue.Name);
            guildIconIndex++;
        }

        // 데이터 갱신

        // 기본 페이지는 이동
        TAB_MENU open_menu = TAB_MENU.MOVE;

        // 기본 이동 위치는 내 영지 위치
        move_posX = User.Instance.PosX;
        move_posY = User.Instance.PosY;

        if (param is PARAM_COORDBASE)
        {
            var openParam = param as PARAM_COORDBASE;
            open_menu = openParam.selectMenu;
            regi_targetObjID = openParam.objID;
            regi_posX = openParam.posX;
            regi_posY = openParam.posY;
            regi_targetName = openParam.GetNameTAGString();
            str_Name = openParam.name;
            str_SubName = openParam.subName;
            str_GuildNick = openParam.guildNick;
            n_LV = openParam.lv;
            objID = openParam.objID;
        }

        m_eSelectMenuType = open_menu;

        // UI 갱신
        InsertNewSheet_Tab();

        // 추가 처리
        CallMenuFunc(m_eSelectMenuType, MENU_FUNC.ENTER);
    }

	protected override void OnRefreshed(IGUIOnRefreshedParam param, bool opening)
	{
		base.OnRefreshed(param, opening);

        if (opening)
        {

        }
        else
        {
            // UI 갱신 
            RefreshSheet_Tab();
        }
    }

    protected override void OnClosed(bool forced)
    {
        base.OnClosed(forced);

        // 데이터 갱신
        var ePrev = m_eSelectMenuType;
        m_eSelectMenuType = TAB_MENU.END;

        // 추가 처리
        CallMenuFunc(ePrev, MENU_FUNC.EXIT);
    }

    private void OnClickClose(UIButton button, object param)
	{
		Close();
	}

    private void CallMenuFunc(TAB_MENU menu, MENU_FUNC func)
    {
        if (menu < TAB_MENU.END && func < MENU_FUNC.END)
            MenuFunc[(int)menu][(int)func]();
    }

	[ReceiveGameEvent(GameEvent.ID.DATA_BASE_INFO_BOOKMARK,
                      GameEvent.ID.DATA_BASE_INFO_GUILD_BOOKMARK)]
	protected override void OnUpdateGameEvent(HashSet<string> eventIDs)
	{
		base.OnUpdateGameEvent(eventIDs);

        switch (m_eSelectMenuType)
        {
            case TAB_MENU.ALL:
            case TAB_MENU.PREFERENCE:
            case TAB_MENU.GUILD:
            case TAB_MENU.FRIEND:
            case TAB_MENU.ENEMY:
                OnUpdateGameEvent_DefaultPage(eventIDs);
                break;

            case TAB_MENU.MOVE:
                OnUpdateGameEvent_MovePage(eventIDs);
                break;

            case TAB_MENU.REGISTER:
                OnUpdateGameEvent_RegisterPage(eventIDs);
                break;

            case TAB_MENU.SHARE:
                OnUpdateGameEvent_SharedPage(eventIDs);
                break;
        }
	}

    [ReceiveGameEvent(GameEvent.ID.DATA_BASE_INFO_BOOKMARK,
                      GameEvent.ID.DATA_BASE_INFO_GUILD_BOOKMARK)]
    protected override void OnDispatchGameEvent(string eventID, IEventDispatchParam param)
    {
        base.OnDispatchGameEvent(eventID, param);

        switch (m_eSelectMenuType)
        {
            case TAB_MENU.ALL:
            case TAB_MENU.PREFERENCE:
            case TAB_MENU.GUILD:
            case TAB_MENU.FRIEND:
            case TAB_MENU.ENEMY:
                OnDispatchGameEvent_DefaultPage(eventID, param);
                break;

            case TAB_MENU.MOVE:
                OnDispatchGameEvent_MovePage(eventID, param);
                break;

            case TAB_MENU.REGISTER:
                OnDispatchGameEvent_RegisterPage(eventID, param);
                break;

            case TAB_MENU.SHARE:
                OnDispatchGameEvent_SharedPage(eventID, param);
                break;
        }
    }

    #region UISheet

    private void InsertNewSheet_Tab()
    {
        var sheet = sheetList_tab;
        if (!sheet)
            return;

        sheet.ClearAllItems();

        int selectIdx = 0;
        sheet.AddItems<TabSheetList.ItemData>((int)TAB_MENU.END, (idx, data) =>
        {
            data.Reset();
            data.eMenuType = (TAB_MENU)idx;
            data.funcIsSelected = _is_selected_tab_;

            if (data.funcIsSelected(data.eMenuType))
                selectIdx = idx;
        });

        sheet.UpdateAllItems();
        sheet.FocusItemAt(0);
        sheet.SelectItemAt(selectIdx);
    }

    private void RefreshSheet_Tab()
    {
        var sheet = sheetList_tab;
        if (!sheet)
            return;

        sheet.ForEachInData((itemData)=>
        {
            sheet.UpdateItem(itemData.item);
        });
    }

    private void RefreshSheet_Tab_Selected()
    {
        var sheet = sheetList_tab;
        if (!sheet)
            return;

        var listTemp = new List<int>();
        if (sheet.GetItemsInViewport(listTemp) > 0)
        {
            listTemp.ForEach((itemidx) =>
            {
                var h = sheet.GetSheetBlockHandler<TabSheetList.Block>(itemidx);
                if (null != h) h.RefreshUI_Selected();
            });
        }
    }

    private bool _is_selected_tab_(TAB_MENU eMenuType)
    {
        return m_eSelectMenuType == eMenuType;
    }

    private void OnTabSheetEvent(eSheetEvent eventType, UISheet uiSheet, UISheet.ItemData itemData, UISheetBlock sheetBlock, GameObject invokedBlockChild, int param)
    {
        var data = itemData as TabSheetList.ItemData;
        if (null == data)
            return;

        switch (eventType)
        {
            case eSheetEvent.ItemClicked:
                {
                    // 데이터 갱신
                    var ePrev = m_eSelectMenuType;
                    m_eSelectMenuType = data.eMenuType;

                    if (ePrev == m_eSelectMenuType)
                        break;

                    // UI 갱신
                    RefreshSheet_Tab_Selected();

                    // 추가 처리
                    CallMenuFunc(ePrev, MENU_FUNC.EXIT);
                    CallMenuFunc(m_eSelectMenuType, MENU_FUNC.ENTER);
                }
                break;
        }
    }

    public class TabSheetList
	{
		public class Block : UISheetBlockHandler
		{
			public const string OBJECTID = "sheetblock";

			[SerializeField] private UIButtonEx btn_button = null;
			[SerializeField] private UILabel lbl_button = null;

            protected override bool useAutoBindingByRootBinder { get { return true; } }

			protected override void OnInitialized()
			{
				base.OnInitialized();

                if (btn_button)
                    btn_button.tweenTarget = null;
            }

			protected override void OnUpdatedBlock(bool changedItem)
			{
				base.OnUpdatedBlock(changedItem);

                RefreshUI_Tab();
                RefreshUI_Selected();
            }

			protected void RefreshUI_Tab()
			{
                var data = GetItemData<ItemData>();
                if (null == data)
                    return;

                switch (data.eMenuType)
				{
					case TAB_MENU.ALL:
						lbl_button.text = Util.Text("COMMON_BOOKMARK_TAB_ALL");
                        break;

                    case TAB_MENU.PREFERENCE:
						lbl_button.text = Util.Text("COMMON_BOOKMARK_TAB_POSITION");
						break;

                    case TAB_MENU.GUILD:
                        lbl_button.text = Util.Text("COMMON_BOOKMARK_ADD_GUILD");
                        break;

                    case TAB_MENU.FRIEND:
						lbl_button.text = Util.Text("COMMON_BOOKMARK_TAB_FRIEND");
						break;

					case TAB_MENU.ENEMY:
						lbl_button.text = Util.Text("COMMON_BOOKMARK_TAB_ENEMY");
						break;

					case TAB_MENU.MOVE:
						lbl_button.text = Util.Text("COMMON_BOOKMARK_MOVE");
						break;

					case TAB_MENU.REGISTER:
						lbl_button.text = Util.Text("COMMON_BOOKMARK_ADD_BOOKMARK");
						break;

					case TAB_MENU.SHARE:
						lbl_button.text = Util.Text("COMMON_SHARE_BOOKMARK");
						break;

					default:
                        lbl_button.text = data.eMenuType.ToString();
                        break;
				}
			}

            public void RefreshUI_Selected()
            {
                var data = GetItemData<ItemData>();
                if (null == data)
                    return;

                if (btn_button)
                    btn_button.SetState(data.IsSelected() ? UIButtonColor.State.Normal : UIButtonColor.State.Disabled, true);
            }
		}

		public class ItemData : UISheet.ItemData
		{
            // 외부 입력
            public TAB_MENU eMenuType = TAB_MENU.END;
            public Func<TAB_MENU, bool> funcIsSelected = null;

			public override void Reset()
			{
				base.Reset();
				eMenuType = TAB_MENU.END;
                funcIsSelected = null;
            }

            public bool IsSelected()
            {
                if (null != funcIsSelected)
                    return funcIsSelected(eMenuType);

                return false;
            }
		}
	}

	#endregion UISheet
}
