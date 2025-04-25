using System.Collections.Generic;
using UnityEngine;

namespace PB.ClientParts
{
    public struct ChatData
    {
        public eChatType ChatType;
        public string Message;
        public string Nickname;
        public UserData UserInfoData;
    }
    public class ChatChannelData
    {
        public eChatType channelChatType;
        public ulong userId;
    }
    public enum eChatCommand
    {
        None,
        Team,
        Reply,
        Group,
        HideChat,
        Surrender,
    }
    public partial class ChattingManager : MonoSingleton<ChattingManager>
    {
        private int unReadChatCount = 0;
        private Queue<ChatData> chatQueue = new Queue<ChatData>();
        private ChatData lastChatMsg;
        private UI_Popup_Chatting popupChatting = null;
        private List<ChatChannelData> chatChannelList = new List<ChatChannelData>();
        private readonly int channelLimitCount = 30;
        private int currentChannelIdx = 0;
        
        private int ReplyChannelCount
        {
            get
            {
                List<ChatChannelData> replyChannels = chatChannelList.FindAll(x => x.channelChatType == eChatType.ReceiveWhisper);
                return replyChannels.Count;
            }
        }
        
        public int GetUnReadChatQueueCount()
        {
            return chatQueue.Count;
        }
        public ChatData GetDequeChat()
        {
            ChatData deqChatData = new ChatData();
            if (chatQueue.Count > 0)
            {
                deqChatData = chatQueue.Dequeue();
            }
            return deqChatData;
        }
        public void AddChatQueue(ChatData data)
        {
            chatQueue.Enqueue(data);
        }
        public void RefreshChatCount(bool isAddCount)
        {
            if (isAddCount)
            {
                unReadChatCount++;
            }
            else
            {
                unReadChatCount = 0;
            }
        }

        public void OnOpenUserWhisperChatting(eChatType type, ulong userId = 0)
        {
            // 채팅 팝업 오픈
            OnViewChattingPopup(true,false);
            
            // 채널 추가
            currentChannelIdx = AddChannel(type, userId);
            
            // 채널 이동
            TriggerOnReceiveWhisperUserInputChattingChannelEventHandler(type,userId);
        }
        
        #region Chat Event Handler

        public void AddOnReceiveChatEventListHandler()
        {
            // Scene에서 사용할 채팅 관련 EventHandler를 추가
            
            // Lobby 기준
            AddOnReceiveChatEventHandler(OnViewLastChattingPopup);
            AddOnReceiveAddChattingChannelEventHandler(AddChannel);
        }

        public void RemoveOnReceiveChatEventListHandler()
        {
            // Scene에서 사용한 관련 EventHandler들 제거
            
            // Lobby 기준
            RemoveOnReceiveChatEventHandler(OnViewLastChattingPopup);
            RemoveOnReceiveAddChattingChannelEventHandler(AddChannel);
        }
        #endregion

        #region Channel Function

        public void ClearAllChannel()
        {
            chatChannelList.Clear();
        }
        public int AddChannel(eChatType type, ulong userId = 0)
        {
            if (userId > 0)
            {
                int idx = chatChannelList.FindIndex(x => x.userId == userId);
                if (idx >= 0)// 이미 있는 귓속말 상대
                {
                    return idx;
                }
            }
            if (type != eChatType.ReceiveWhisper)
            {
                int typeIdx = chatChannelList.FindIndex(x => x.channelChatType == type);
                if (typeIdx >= 0)// 이미 있는 채널 타입
                {
                    return typeIdx;
                }
            }
            ChatChannelData channel = new ChatChannelData();
            channel.channelChatType = type;
            channel.userId = userId;
            chatChannelList.Add(channel);
            
            // 귓속말 채널 최대 개수 초과시, 오래된 채널부터 제거
            if (ReplyChannelCount > channelLimitCount)
            {
                int index = chatChannelList.FindIndex(x => x.channelChatType == eChatType.ReceiveWhisper);
                if (index != -1)
                {
                    chatChannelList.RemoveAt(index);
                }
            }
            return chatChannelList.Count - 1;
        }
        public int RemoveChannel(eChatType type, ulong userId = 0)
        {
            int idx = -1;
            if (userId > 0)
            {
                idx = chatChannelList.FindIndex(x => x.userId == userId);
            }
            else
            {
                idx = chatChannelList.FindIndex(x => x.channelChatType == type); 
            }
            if (idx >= 0)
            {
                chatChannelList.RemoveAt(idx);
                if (idx < chatChannelList.Count)
                {
                    return idx;
                }
            }
            return 0;
        }

        public eChatType GetCurChannelChatType()
        {
            if (chatChannelList.Count > 0)
            {
                return GetChannelData(currentChannelIdx).channelChatType;
            }
            else
            {
                return eChatType.Group;
            }
        }
        public ChatChannelData GetChannelData(int idx)
        {
            ChatChannelData idxChannelData = null;
            if (idx < chatChannelList.Count)
            {
                idxChannelData = chatChannelList[idx];
            }
            return idxChannelData;
        }
        public int GetChannelIdx(eChatType type, ulong userId = 0)
        {
            int idx = -1;
            if (userId > 0 && type == eChatType.ReceiveWhisper)
            {
                idx = chatChannelList.FindIndex(x => x.userId == userId);
            }
            else
            {
                idx = chatChannelList.FindIndex(x => x.channelChatType == type);   
            }
            return idx;
        }
        /// <summary>
        /// 귓말용, userId에 대응하는 친구 목록이 없을 시 return false
        /// </summary>
        /// <param name="type"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        private bool IsExistReplyChannel(ulong userId)
        {
            if (LobbyUserData.Instance.GetSocialDataInFriend(userId) is null)
            {
                return false;
            }
            return true;
        }
        public ChatChannelData OnChangeChatChannel(int direction)
        {
            if (chatChannelList.Count > 0)
            {
                currentChannelIdx += direction;
                if (currentChannelIdx < 0)
                {
                    currentChannelIdx = chatChannelList.Count - 1;
                }
                if (currentChannelIdx > chatChannelList.Count - 1)
                {
                    currentChannelIdx = 0;
                }
                ChatChannelData channel = chatChannelList[currentChannelIdx];
                if (channel.channelChatType == eChatType.ReceiveWhisper)
                {
                    while (!ChattingManager.Instance.IsExistReplyChannel(channel.userId))
                    {
                        currentChannelIdx = ChattingManager.Instance.RemoveChannel(channel.channelChatType, channel.userId);
                        channel = chatChannelList[currentChannelIdx];
                        if (channel.channelChatType != eChatType.ReceiveWhisper)
                        {
                            break;
                        }
                    }   
                }
                return channel;
            }
            return null;
        }
        public void SetChannelIdx(int idx)
        {
            currentChannelIdx = idx;
        }
        public ChatChannelData GetCurrentChatChannelData()
        {
            ChatChannelData channelData = new ChatChannelData();
            if (currentChannelIdx >= 0 && currentChannelIdx<chatChannelList.Count)
            {
                channelData = chatChannelList[currentChannelIdx];
            }
            else
            {
                channelData.channelChatType = eChatType.Group;
                channelData.userId = 0;
            }
            return channelData;
        }
       
        #endregion
        
        #region  UI_Popup_Chatting Function
        public UI_Popup_Chatting OnViewChattingPopup(
            bool isShowUserPopup = false,
            bool isShowLastMessage=false)
        {
            popupChatting = UIHandler.Instance.LoadUI<UI_Popup_Chatting>(new UIPopupChattingParam()
            {
                IsSystemPopup = true,
                isShowUserPopup = isShowUserPopup,
                sortingOrder = UIHandler.CalcSortingOrder(70),
            }, null, true);
            
            popupChatting.OnViewChattingPopup(isShowLastMessage);
            AddChannel(eChatType.Group);   
            
            return popupChatting;
        }
        public void OnViewLastChattingPopup(ChatData msg)
        {
            // 마지막으로 도착한 메시지를 출력하는 팝업
            if (msg.ChatType == eChatType.Group || msg.ChatType == eChatType.ReceiveWhisper)
            {
                if (popupChatting == null)
                {
                    popupChatting = OnViewChattingPopup(false, true);
                    popupChatting.RefreshLastChatMsg(msg);
                }
                else
                {
                    if (!popupChatting.IsChatting)
                    {
                        popupChatting = OnViewChattingPopup(false, true);
                        popupChatting.RefreshLastChatMsg(msg);
                    }
                }
            }
        }
        
        #endregion

        #region  ChatIcon Function
        public Color32 GetChatColor(eChatType type)
        {
            switch (type)
            {
                case eChatType.Ally:
                case eChatType.AllyPing:
                case eChatType.SocialVoice:
                    return ClientConst.UI.CHAT_COLOR_TEAM;
                case eChatType.ReceiveWhisper:
                case eChatType.SendWhisper:
                    return ClientConst.UI.CHAT_COLOR_WHISPER;
                case eChatType.System:
                case eChatType.SystemServer:
                case eChatType.SystemMessageWithParam:
                    return ClientConst.UI.CHAT_COLOR_SYSTEM;
                default:
                    return ClientConst.UI.CHAT_COLOR_GROUP;
            }
        }
        public Sprite GetIconImage(eChatType type)
        {
            string spriteName = "";
            switch (type)
            {
                case eChatType.Ally:
                case eChatType.AllyPing:
                case eChatType.SocialVoice:
                    spriteName = ClientConst.UI.CHAT_TEAM_ICON;
                    break;
                case eChatType.ReceiveWhisper:
                case eChatType.SendWhisper:
                    spriteName = ClientConst.UI.CHAT_WHISPER_ICON;
                    break;
                case eChatType.System:
                case eChatType.SystemServer:
                case eChatType.SystemMessageWithParam:
                    spriteName = ClientConst.UI.CHAT_SYSTEM_ICON;
                    break;
                case eChatType.Group:
                default:
                    spriteName = ClientConst.UI.CHAT_GROUP_ICON;
                    break;
            }
            return ClientResourceManager.Instance.GetChatIcon(spriteName);
        }
        public string GetToChatTypeName(eChatType type)
        {
            switch (type)
            {
                case eChatType.Ally:
                    return "UI_CHAT_TEAM";
                case eChatType.Group:
                    return "UI_CHAT_GROUP";
                case eChatType.ReceiveWhisper:
                    return "UI_CHAT_WHISPER_REPLY";
                default:
                    return "UI_CHAT_GROUP";
            }
        }
        
        #endregion
    }
}