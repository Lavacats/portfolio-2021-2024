using UnityEngine;

/// <summary>
/// 0. None
/// 1. 친구 초대 취소
/// 2. 수락
/// 3. 거절
/// 4. 그룹으로 초대
/// 5. 친구 초대
/// 6. 그룹장 위임
/// 7. 그룹에서 추방
/// 8. 귓속말
/// 9. 프로필보기
/// 10.친구삭제
/// 11.신고
/// 12. 차단
/// 13. 스쿼드 초대 취소
/// 14. 차단 리스트
/// </summary>

namespace PB.ClientParts
{
    public delegate void OnUserMenuItemClickEventHandler();
    
    public enum eUserMenuType
    {
        None=-1,
        CancelInvite=0,
        Accept,
        Refuse,
        InviteSquad,
        InviteFriend,
        ChangeGroupLeader,
        KickGroup,
        Whisper,
        ViewProfile,
        DeleteFriend,
        Report,
        CutOff,
        CancelSquadInvite,
        BlockList,
    }
    public class UI_UserMenuItem_Renewal : UIComponent
    {
        [SerializeField]
        private UILocalizedText menuText;
        [SerializeField]
        private UILocalizedText selectText;
        [SerializeField]
        private RectTransform focusImage;

        public eUserMenuType userMenuItemType;
        private OnUserMenuItemClickEventHandler onUserMenuItemClickEventHandler;
        
        public void SetData(string localKey, OnUserMenuItemClickEventHandler onUserMenuItemClickEventHandler)
        {
            menuText.LocalKey = localKey;
            selectText.LocalKey = localKey;
            this.onUserMenuItemClickEventHandler = onUserMenuItemClickEventHandler;
        }

        protected override void OnAction(GameObject go)
        {
            OnClick();
        }

        protected override void OnSelect(bool isSelect)
        {
            OnHover(isSelect);
        }

        protected override void OnHover(bool isHover)
        {
            if (isHover)
            {
                SoundManager.Instance.PlayUI2DOneShot(ClientConst.UI.Sound.FOCUS_LIST);
            }
            focusImage.SetActive(isHover);
        }

        protected override void OnClick()
        {
            base.OnClick();
            OnHover(false);
            SoundManager.Instance.PlayUI2DOneShot(ClientConst.UI.Sound.SELECT);
            onUserMenuItemClickEventHandler?.Invoke();
        }
        
        public void FocusVisible(bool isFocus)
        {
            OnHover(isFocus);
        }
    }
}
