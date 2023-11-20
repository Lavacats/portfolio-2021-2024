using System.Collections;
using System.Collections.Generic;
using GameEvent.Parameter;
using UnityEngine;
using Framework;

using System;
using PROTOCOL.FLATBUFFERS;
using GameEvent;

public enum EnumGuildBookmarkKind
{
    None =0,
    Attack,
    Deffence,
    Rally,
    Target
}
public class GuildBookmarkObj : IBaseObj , IObjectSetData<INFO_GUILD_BOOKMARK>
{

    //--------------------------------------------------------------------
    // 공통 양식
    public ObjID MyObjID { get; set; }
    public bool Expired { get { return ObjID.IsEmpty(MyObjID) || RemoveFlag; } }
    public Int64 Count { get { return 1; } }
    //--------------------------------------------------------------------
    // 지정되지 않은 데이터의 처리

    public int Kind { get; private set; } = 0;
    public string Name { get; private set; } = string.Empty;
    public int PosX { get; private set; } = 0;
    public int PosY { get; private set; } = 0;
    public ObjID TargetObjID { get; private set; } = ObjID.Empty;

    public bool RemoveFlag { get; private set; } = false;
    public GuildBookmarkObj (in ObjID objid)
    {
        MyObjID = objid;
    }

    public void SetData(ref INFO_GUILD_BOOKMARK data)
    {
        Kind = data.Kind;
        Name = Util.GetString(data.GetNameBytes().Value);
        PosX = data.PosX;
        PosY = data.PosY;
        TargetObjID = data.TargetObjID;

        RemoveFlag = data.RemoveFlag == 1;
    }
    public void SetData<T>(ref T data) where T : struct { }
}
public class GuildBookmarkContainer : TBaseObjContainer<GuildBookmarkObj>,
        IBaseObjContainerExporter<GuildBookmarkObj,INFO_GUILD_BOOKMARK>
{
    public override string DefaultEventID => GameEvent.DATA_BASE_INFO_GUILD_BOOKMARK.CACHED.eventID;

    private Dictionary<int, ObjID> lookupByEnumGuildBookmarkKind = new Dictionary<int, ObjID>();
    public GuildBookmarkObj GetDataByBookmarkKind(EnumGuildBookmarkKind eKIND)
    {
        if (lookupByEnumGuildBookmarkKind.ContainsKey((int)eKIND))
        {
            var pData = GetData(lookupByEnumGuildBookmarkKind[(int)eKIND]);
            if (null != pData)
                return pData;
        }

        return null;
    }
    public IReadOnlyDictionary<int, ObjID> GetCollectionLookupBookmarkKind()
    {
        return lookupByEnumGuildBookmarkKind;
    }

    private Dictionary<Vector2, ObjID> lookupByPos = new Dictionary<Vector2, ObjID>();
    public GuildBookmarkObj GetDataByPos(Vector2 pos)
    {
        if (lookupByPos.ContainsKey(pos))
        {
            var pData = GetData(lookupByPos[pos]);
            if (null != pData)
                return pData;
        }

        return null;
    }
    public IReadOnlyDictionary<Vector2, ObjID> GetCollectionLookupByPos()
    {
        return lookupByPos;
    }

    protected override IEventParam CreateDispatchEvent()
    {
        var ge = GameEvent.DATA_BASE_INFO_GUILD_BOOKMARK.CACHED;
        ge.Reset();
        return ge;
    }

    protected override void OnPreProcessData(in ObjID targetObjID, IEventParam param) 
    {
        var pData = GetData(targetObjID);
        if (null != pData)
            (param as GameEvent.DATA_BASE_INFO_GUILD_BOOKMARK)?.SetPrev(pData);
    }

    protected override void OnPostProcessData(eObjContainerProcess result, in ObjID targetObjID, GuildBookmarkObj targetBaseObj, IEventParam param)
    {
        if (result == eObjContainerProcess.PostFailed)
            return;

        var removed = result == eObjContainerProcess.PostRemoved;
        var p = param as GameEvent.DATA_BASE_INFO_GUILD_BOOKMARK;
        if (null != p)
            p.SetCurr(targetBaseObj, removed);

        if (!removed)
        {
            // 추가/갱신 시
            if (!lookupByEnumGuildBookmarkKind.ContainsKey(targetBaseObj.Kind))
                lookupByEnumGuildBookmarkKind.Add(targetBaseObj.Kind, targetBaseObj.MyObjID);

            var vKeyPos = new Vector2(targetBaseObj.PosX, targetBaseObj.PosY);
            if (!lookupByPos.ContainsKey(vKeyPos))
                lookupByPos.Add(vKeyPos, targetBaseObj.MyObjID);

            if (LastHandledObjs.ContainsValue(eObjState.Enum.Added, targetBaseObj))
            {
                // 추가됨
                // 필드 뷰 오브젝트 생성
                WOC.WOFViewBookmark.Create(targetObjID);
            }
        }
        else
        {
            // 삭제 시   
            if (lookupByEnumGuildBookmarkKind.ContainsKey(targetBaseObj.Kind))
                lookupByEnumGuildBookmarkKind.Remove(targetBaseObj.Kind);

            var vKeyPos = new Vector2(targetBaseObj.PosX, targetBaseObj.PosY);
            if (lookupByPos.ContainsKey(vKeyPos))
                lookupByPos.Remove(vKeyPos);

            // 필드 뷰 오브젝트 삭제
            WOC.WOFViewBookmark.Remove(targetObjID);
        }
    }
    protected override bool AllowDispatchEvent(IEventParam param)
    {
        return true;
    }

    public ObjID ExportObjID(ref INFO_GUILD_BOOKMARK source)
    {
        return source.GameObjID;
    }

    public GuildBookmarkObj CreateBaseObj(in ObjID objID, ref INFO_GUILD_BOOKMARK source)
    {
        return new GuildBookmarkObj(objID);
    }
}
