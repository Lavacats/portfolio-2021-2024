using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoadData
{
    public eSceneType loadSceneType;
    public SceneBaseControllerParam setupSceneParam;
    public SceneBaseControllerParam loadCompleteSceneParam;
    public bool isDefaultLoadingView = true;
    public bool isShowLoadingView = true;
}

public class PBSceneManager : DontDestroySingleton<PBSceneManager>
{
    public int mapID = 1;
    public int matchingCount = 2;
    //public ePlayType playType = ePlayType.None;

    public bool isTest = false;

    private class SceneData
    {
        public SceneBaseController scene = null;
    }

    private SceneData currentSceneData = new SceneData();

    private bool isLoading = false;


    public void LoadScene(SceneLoadData data)
    {
        if (data == null) return;

        if (isLoading)
        {
            return;
        }

        isLoading = true;
        StopAllCoroutines();
        StartCoroutine(CoLoadScene(data));
    }
    private IEnumerator CoLoadScene(SceneLoadData data)
    {
#if !BUILD_SERVER
        if (data.isShowLoadingView)
        {
            int checkMapId = 0;
            if (data.setupSceneParam is GameSceneParam param)
            {
                checkMapId = param.mapID;
            }

            float len = 0;
            //if (!data.isDefaultLoadingView)
            {
                len = UI_LoadingView.Show(data.isDefaultLoadingView, checkMapId);
            }
            //else
            //{
            //    len = UI_Popup_Loading_Normal.Show();
            //}
            yield return new WaitForSeconds(len);
            yield return null;
        }
#endif

        if (currentSceneData == null)
        {
            currentSceneData = new SceneData();
            yield return null;
        }

        if (currentSceneData.scene != null)
        {
            currentSceneData.scene.OnDestroyCompleted();
            currentSceneData.scene = null;
        }

        yield return SceneManager.LoadSceneAsync(eSceneType.StartScene.ToString(), LoadSceneMode.Single);
        //yield return null;

        Resources.UnloadUnusedAssets();
        System.GC.Collect();
        yield return null;

        // SceneManager.LoadScene(data.loadSceneType.ToString(), LoadSceneMode.Single);
        // yield return null;

        yield return SceneManager.LoadSceneAsync(data.loadSceneType.ToString(), LoadSceneMode.Single);

        Scene scene = SceneManager.GetSceneByName(data.loadSceneType.ToString());

        if (!scene.IsValid())
        {
            isLoading = false;
            yield break;
        }

        SceneBaseController sceneBaseController = null;
        GameObject[] obj = scene.GetRootGameObjects();


        for (int i = 0; i < obj.Length; i++)
        {
            if (obj[i].TryGetComponent(out SceneBaseController sceneBaseCtrl))
            {
                sceneBaseController = sceneBaseCtrl;
                break;
            }
        }

        if (sceneBaseController == null)
        {
            isLoading = false;
            yield break;
        }

        currentSceneData.scene = sceneBaseController;

        sceneBaseController.SetSceneType(data.loadSceneType);
        yield return StartCoroutine(sceneBaseController.CoOnLoadData());
        sceneBaseController.OnSetup(data.setupSceneParam);
        sceneBaseController.OnLoadCompleted(data.loadCompleteSceneParam);

        isLoading = false;
    }
}
