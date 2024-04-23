using System.Collections;
using System.Collections.Generic;
using PB.ClientParts.Platform;
using UnityEngine;

namespace PB.ClientParts
{
    public delegate void OnSetAndChangeChannelEventHandler(eChatCommand command);
    public class ChatCommandInfoData
    {
        public eChatCommand chatCommand;
        public string commandString;
        public OnSetAndChangeChannelEventHandler onSetAndChageChannelEventHandler;
    }
    public class UI_ChattingController : UIRecycleViewController<UIChattingItemParam>
    {
        [Header("---Define Command---")]
        
        [SerializeField]
        private UI_ChattingItem_Renewal chattingItem;
        [SerializeField]
        private UI_ChattingInputFieldItem chattingInputFieldItem;
        [SerializeField]
        private List<string> changeChannelToGroupCommandList;   // 그룹 채팅 단축키
        [SerializeField]
        private List<string> changeChannelToReplyCommandList;   // 귓속말 단축키
        
        private UI_ChattingInputFieldItemParam chattingInputFieldItemParam;
        private List<ChatCommandInfoData> allChangeChannelCommandList = new List<ChatCommandInfoData>();  // 모든 단축키
        
        private bool isChatBan = false;
        private const int chatBanTime = 4;
        private float chatBanTimePassed = 0f;
        private bool isFocusChattingItem = false;
        private bool isShowUserPopup = false;
        
        protected Rect visibleRectSize; 

        public bool IsFocusChattingItem => isFocusChattingItem;
        
        public void SetData(bool isShowUserPopup)
        {
            this.isShowUserPopup = isShowUserPopup;
            chattingInputFieldItem.SetData();
            ChattingManager.Instance.AddOnReceiveWhisperUserChattingChannelEventHandler(ChangeInputFieldTypeChannel);
            chattingInputFieldItemParam = GetChattingInputFieldItemParam(ChattingManager.Instance.GetCurChannelChatType());
        }
        public virtual void RemoveEventHandler()
        {
            chattingInputFieldItem.RemoveEventHandler();
            ChattingManager.Instance.RemoveOnReceiveWhisperUserChattingChannelEventHandler(ChangeInputFieldTypeChannel);
        }
        protected override void Start()
        {
            base.Start();
            allChangeChannelCommandList.Clear();
            
            for (int i = 0; i < changeChannelToGroupCommandList.Count; i++)
            {
                AddChangeCommandData(eChatCommand.Group, changeChannelToGroupCommandList[i],allChangeChannelCommandList);
            }
            for (int i = 0; i < changeChannelToReplyCommandList.Count; i++)
            {
                AddChangeCommandData(eChatCommand.Reply, changeChannelToReplyCommandList[i],allChangeChannelCommandList);
            }
            visibleRectSize.position = CachedRectTransform.position + new Vector3(Screen.width * 0.5f, Screen.height * 0.5f);
            visibleRectSize.size = CachedRectTransform.rect.size;
            
            tableData.Clear();
        }
        protected virtual void Update()
        {
            CheckBanTimeUpdate();
            if (ChattingManager.Instance.GetUnReadChatQueueCount() > 0)
            {
                AddNewRow(ChattingManager.Instance.GetDequeChat());
                ChattingManager.Instance.TriggerOnChattingTabCountHandler();
            }
        }
        
        public void Enter()
        {
            ChattingManager.Instance.RefreshChatCount(false);
            ChatChannelData curChannelData = ChattingManager.Instance.GetCurrentChatChannelData();
            chattingInputFieldItemParam = GetChattingInputFieldItemParam(curChannelData.channelChatType,curChannelData.userId);
            
            ChattingManager.Instance.TriggerOnChattingTabCountHandler();
            UIHandler.UIEvenHelper.TriggerOnReceiveSocialDataEventHandler();
            chattingInputFieldItem.SetData(chattingInputFieldItemParam);
            chattingInputFieldItem.FocusChatInputField();
        }
        private void CheckBanTimeUpdate()
        {
            if (isChatBan)
            {
                chatBanTimePassed += Time.deltaTime;
                if (chatBanTimePassed > chatBanTime)
                {
                    isChatBan = false;
                    chatBanTimePassed = 0f;
                }
            }
        }
        private void AddNewRow(ChatData msg)
        {
            chattingItem.SetChattingItemChatData(msg,isShowUserPopup);
            UIChattingItemParam param = chattingItem.GetChattingItemParam();
            tableData.Add(param);
            InitializeTableView();
        }
        
        public void ShowUserPopup()
        {
            if (isShowUserPopup)
            {
                if (GetPreviousSelectCell() is UI_ChattingItem_Renewal item)
                {
                    if (item.IsChattingItemSelected)
                    {
                        item.ShowUserPopup();
                    }
                }
            }
        }
        public void OnDeselectItem()
        {
            // prev Item
            if (GetPreviousSelectCell() is UI_ChattingItem_Renewal item)
            {
                item.OnDeselectedItem();   
            }
            InitSelectIdx();
            isFocusChattingItem = false;
        }
        public void OnFocusChatInputField()
        {
#if UNITY_GAMECORE
            PlatformManager.Instance.PlatformVirtualKeyboard(string.Empty, string.Empty, string.Empty, 68,0, (hresult, resultString) =>
            {
                chattingInputFieldItem.SetMessageValue(resultString);
                SendChatMessage();
            });
#elif UNITY_PS5
            PlatformManager.Instance.PlatformVirtualKeyboard(string.Empty, string.Empty, string.Empty, (int)Sony.PS5.Dialog.Ime.Option.DEFAULT, 0,
                (result, resultString) =>
                {
                    chattingInputFieldItem.SetMessageValue(resultString);
                    SendChatMessage();
                });
#else
            chattingInputFieldItem.FocusChatInputField();
#endif
        }
        public void OnItemSelectAtFirst(int direction)
        {
            InitSelectIdx();
            SelectIdx += direction;
            OnChattingItemSelect();
        }
        public void OnChattingItemSelect()
        {
            if (tableData.Count <= SelectIdx || SelectIdx < 0)
            {
                return;
            }
            if (!isFocusChattingItem)
            {
                isFocusChattingItem = true;
            }
            // prev Item
            if (GetPreviousSelectCell() is UI_ChattingItem_Renewal item)
            {
                item.OnDeselectedItem();   
            }
            // current Item
            item = GetSelectCell(SelectIdx, (int)(chattingItem.textBuffer.top + chattingItem.textBuffer.bottom)) as UI_ChattingItem_Renewal;
            if (item is not null)
            {
                item.OnSelectedItem();
            }
        }
        private UI_ChattingInputFieldItemParam GetChattingInputFieldItemParam(eChatType type,ulong userID=0)
        {
            UI_ChattingInputFieldItemParam param = new UI_ChattingInputFieldItemParam
            {
                ChatSprite = ChattingManager.Instance.GetIconImage(type), ToName = ChattingManager.Instance.GetToChatTypeName(type),
                ChatColor32 = ChattingManager.Instance.GetChatColor(type), ChatType = type,TargetUserId = userID,
            };
            return param;
        }
        
        public void OnChangeInputChannel(int direction)
        {
            ChatChannelData changedChannel = ChattingManager.Instance.OnChangeChatChannel(direction);
            ChangeInputFieldTypeChannel(changedChannel.channelChatType, changedChannel.userId);
        }
        private void SetChannelIdxAndChangeChannel(eChatType type)
        {
            int idx = ChattingManager.Instance.GetChannelIdx(type);
            if (idx < 0)//not exist channel
            {
                switch (type)
                {
                    case eChatType.Group:
                    case eChatType.Ally:
                    case eChatType.ReceiveWhisper://채널이 없음을 알려줘야 함.
                        ChatModule.SendSystemChatNotExist();
                        break;
                    default:
                        CGLog.LogVerbose($"Strange data came in!! ChatType : {type.ToString()}");
                        break;
                }
            }
            else
            {
                ChattingManager.Instance.SetChannelIdx(idx);
                ChangeInputFieldTypeChannel(type, ChattingManager.Instance.GetChannelData(idx).userId);
            }
        }
        private void ChangeInputFieldTypeChannel(eChatType type, ulong userId = 0)
        {
            chattingInputFieldItem.FocusChatInputField();
            chattingInputFieldItemParam.ChatSprite = ChattingManager.Instance.GetIconImage(type);
            chattingInputFieldItemParam.ChatColor32 = ChattingManager.Instance.GetChatColor(type);
            chattingInputFieldItemParam.ToName = ChattingManager.Instance.GetToChatTypeName(type);
            chattingInputFieldItemParam.ChatType = type;
            chattingInputFieldItemParam.TargetUserId = userId;
            chattingInputFieldItem.SetData(chattingInputFieldItemParam);
        }

        
        public void SendChatMessage()
        {
            chattingInputFieldItem.SetStartInputChattingPos();
            StartCoroutine(CoSendChatMessage());
        }
        private IEnumerator CoSendChatMessage()
        {
            chattingInputFieldItem.UnfocusChatInputField();
            yield return null;
            chattingInputFieldItem.SendChatMessage();
            yield return null;
            chattingInputFieldItem.InitChatInputField();
        }
        
        private void AddChangeCommandData(eChatCommand command, string commandString, List<ChatCommandInfoData> commandDataList)
        {
            ChatCommandInfoData eachInfoData = new ChatCommandInfoData();
            eachInfoData.chatCommand = command;
            eachInfoData.onSetAndChageChannelEventHandler = SetCommandCallback;
            eachInfoData.commandString = commandString;
            commandDataList.Add(eachInfoData);   
        }
        private void SetCommandCallback(eChatCommand command)
        {
            switch (command)
            {
                case eChatCommand.Team:
                    SetChannelIdxAndChangeChannel(eChatType.Ally);
                    break;
                case eChatCommand.Group:
                    SetChannelIdxAndChangeChannel(eChatType.Group);
                    break;
                case eChatCommand.Reply:
                    SetChannelIdxAndChangeChannel(eChatType.ReceiveWhisper);
                    break;
                default:
                    break;
            }
        }
        public void OnStartChatCommandAction()
        {
            if (chattingInputFieldItem.IsCommand())
            {
                StartCoroutine(CheckCommandAndRunAction());
            }
        }  
        private IEnumerator CheckCommandAndRunAction()
        {
            yield return null;
            chattingInputFieldItem.RemoveFirstSpace(); 
            
            for (int i = 0; i < allChangeChannelCommandList.Count; i++)
            {
                if (allChangeChannelCommandList[i].commandString == chattingInputFieldItem.Chat.ToLower())
                {
                    chattingInputFieldItem.InitChatInputField(RemoveFirstOccurrence(chattingInputFieldItem.Chat,
                        allChangeChannelCommandList[i].commandString));
                    allChangeChannelCommandList[i].onSetAndChageChannelEventHandler(allChangeChannelCommandList[i].chatCommand);
                }
            }
        }
        private string RemoveFirstOccurrence(string text, string target)
        {
            int index = text.IndexOf(target);
            if (index >= 0)
            {
                return text.Remove(index, target.Length);
            }
            else
            {
                return text;
            }
        }
        
        protected override float GetCellHeightAtIndex(int index)
        {
            return tableData[index].cellSize;
        }
    }
}
