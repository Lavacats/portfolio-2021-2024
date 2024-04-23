using UnityEngine;

namespace PB.ClientParts
{
    public partial class  ChattingManager 
    {
        #region OnChattingPopupEnterHandler
        public delegate void OnChattingPopupEnterHandler();

        private OnChattingPopupEnterHandler onChattingPopupEnterHandler = null;

        public void AddOnChattingPopupEnterHandler(OnChattingPopupEnterHandler handler)
        {
            if (handler == null) return;
            
            onChattingPopupEnterHandler += handler;
        }
        public void RemoveOnChattingPopupEnterHandler(OnChattingPopupEnterHandler handler)
        {
            if (handler == null) return;
            onChattingPopupEnterHandler -= handler;
        }

        public void ClearOnChattingPopupEnterHandler()
        {
            onChattingPopupEnterHandler = null;
        }
        public void TriggerOnChattingPopupEnterHandler()
        {
            onChattingPopupEnterHandler?.Invoke();
        }
        #endregion
        
        #region OnReceiveChatEventHandler - InGame
        
        public delegate void OnReceiveChatEventHandler(ChatData msg);

        private OnReceiveChatEventHandler onReceiveChatEventHandler = null;

        public void AddOnReceiveChatEventHandler(OnReceiveChatEventHandler handler)
        {
            if (handler == null) return;
            onReceiveChatEventHandler += handler;
        }

        public void RemoveOnReceiveChatEventHandler(OnReceiveChatEventHandler handler)
        {
            if (handler == null) return;
            onReceiveChatEventHandler -= handler;
        }

        public void ClearOnReceiveChatEventHandler()
        {
            onReceiveChatEventHandler = null;
        }

        public void TriggerOnReceiveChatEventHandler(ChatData msg)
        {
            onReceiveChatEventHandler?.Invoke(msg);
        }

        #endregion
        
        #region OnChattingTabCountHandler

        public delegate void OnChattingTabCountHandler();

        private OnChattingTabCountHandler onChattingTabCountHandler = null;

        public void AddOnChattingTabCountHandler(OnChattingTabCountHandler handler)
        {
            if (handler == null) return;
            
            onChattingTabCountHandler += handler;
        }

        public void RemoveOnChattingTabCountHandler(OnChattingTabCountHandler handler)
        {
            if (handler == null) return;
            onChattingTabCountHandler -= handler;
        }

        public void ClearOnChattingTabCountHandler()
        {
            onChattingTabCountHandler = null;
        }

        public void TriggerOnChattingTabCountHandler()
        {
            onChattingTabCountHandler?.Invoke();
        }

        #endregion
        
        #region OnReceiveAddInputChattingChannelEventHandler
        
        public delegate int OnReceiveAddInputChattingChannelEventHandler(eChatType type, ulong userId = 0);

        private OnReceiveAddInputChattingChannelEventHandler onReceiveAddInputChattingChannelEventHandler = null;

        public void AddOnReceiveAddChattingChannelEventHandler(OnReceiveAddInputChattingChannelEventHandler handler)
        {
            if (handler == null) return;
            onReceiveAddInputChattingChannelEventHandler += handler;
        }

        public void RemoveOnReceiveAddChattingChannelEventHandler(OnReceiveAddInputChattingChannelEventHandler handler)
        {
            if (handler == null) return;
            onReceiveAddInputChattingChannelEventHandler -= handler;
        }

        public void TriggerOnReceiveAddInputChattingChannelEventHandler(eChatType type, ulong userId = 0)
        {
            onReceiveAddInputChattingChannelEventHandler?.Invoke(type, userId);
        }

        #endregion

        #region OnReceiveWhisperUserChattingChannelEventHandler
        
        public delegate void OnReceiveWhisperUserChattingChannelEventHandler(eChatType type, ulong userId = 0);

        private OnReceiveWhisperUserChattingChannelEventHandler onReceiveWhisperUserChattingChannelEventHandler = null;

        public void AddOnReceiveWhisperUserChattingChannelEventHandler(OnReceiveWhisperUserChattingChannelEventHandler handler)
        {
            if (handler == null) return;
            onReceiveWhisperUserChattingChannelEventHandler += handler;
        }

        public void RemoveOnReceiveWhisperUserChattingChannelEventHandler(OnReceiveWhisperUserChattingChannelEventHandler handler)
        {
            if (handler == null) return;
            onReceiveWhisperUserChattingChannelEventHandler -= handler;
        }

        public void TriggerOnReceiveWhisperUserInputChattingChannelEventHandler(eChatType type, ulong userId = 0)
        {
            onReceiveWhisperUserChattingChannelEventHandler?.Invoke(type, userId);
        }

        #endregion
    }
}
