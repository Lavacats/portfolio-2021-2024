using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseEventManager : SingleTon<BaseEventManager>
{
    public enum EVENT_BASE
    {
        NONE,
        MOUSE_DOWN_RIGHT,
        ONSELECT_ARMY,
        ONSELECT_ARMY_UI,
        MOUSE_DESELECT_ARMY,
        MAP_LOAD,
        ON_EVENT_SKILL,
        AMRY_STATE_SKILL_START,
        ARMY_STATE_SKILL_END,
        BUFF_START,
        BUFF_END,
        COMBAT_START,
        
    }
    private Dictionary<EVENT_BASE, List<Action<object>>> dicEventHandler = new Dictionary<EVENT_BASE, List<Action<object>>>();

    public void AddEvent(EVENT_BASE type, Action<object> action)
    {
        if (dicEventHandler.ContainsKey(type)==false)
        {
            dicEventHandler[type] = new List<Action<object>>();
        }
        dicEventHandler[type].Add(action);
    }
    public void RemoveEvent(EVENT_BASE type, Action<object> action)
    {
        if (dicEventHandler.ContainsKey(type))
        {
            if (dicEventHandler[type].Contains(action))
            {
                dicEventHandler[type].Remove(action);
            }
        }
    }
    public bool OnEvent(EVENT_BASE type,object value)
    {
        bool result = false;
        if (dicEventHandler.ContainsKey(type))
        {
            foreach(var action in dicEventHandler[type])
            {
                result = true;
                action?.Invoke(value);
            }
        }
        return result;
    }
 }
