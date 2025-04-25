using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class SceneBaseControllerParam { }

public class SceneBaseController : MonoBehaviour
{
    protected eSceneType sceneType = eSceneType.None;
    public void SetSceneType(eSceneType type) { sceneType = type; }
    public virtual void OnSetup(SceneBaseControllerParam param) { }
    public virtual IEnumerator CoOnLoadData() { yield break; }
    public virtual void OnLoadCompleted(SceneBaseControllerParam param) { }
    public virtual void OnDestroyCompleted() { }
}
