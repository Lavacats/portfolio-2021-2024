using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_LoadingView : UIComponent
{
    private bool isDefault = false;

    public static float Show(bool isDefault, int mapId)
    {
        //return OnShow(isDefault, mapId);
        return 0;
    }

    private float OnShow(bool isDefault, int mapId)
    {
        this.isDefault = isDefault;
        if (isDefault)
        {
            //return DefaultShow();
        }
        else
        {
            //return ModeShow(mapId);
        }
        return 0;
    }



}
