
using UnityEngine;

public class GameFlow : MonoBehaviour
{
    [SerializeField]
    private bool isTest;
    // [SerializeField]
    // private bool isLanguageTest = false;

    //[SerializeField]
    //private eSupportedLanguages languages;

    // [SerializeField]
    // private bool isShowJoystick = false;
    //
    // [SerializeField]
    // private bool isShowConsoleLog = false;
    //
    // [SerializeField]
    // private bool isShowFrameChecker = false;

    [SerializeField]
    private Texture2D mouseTex = null;

    [SerializeField]
    private bool isCURSOR_FREE = false;

    private void Start()
    {
        Application.runInBackground = true;


        GameStart();
    }

    private async void GameStart()
    {


        //await ClientTableManager.Instance.LoadTableAsync();
        //GameKeyMapper.InitInGameKeyMapper();
        // SkillSlotUIMapper.InitSkillSlotUIMapper();
        // InputOption.Instance.Init();

        // InputHandler.Instance.SetDefaultActionMap();

        //ClientConst.IsUseSecondWaveMouse = !isCURSOR_FREE;

        SceneLoadData firstScene = new SceneLoadData();
        firstScene.isDefaultLoadingView = false;
        firstScene.loadSceneType =eSceneType.IntroScene;



        PBSceneManager.Instance.LoadScene(firstScene);


    }

}
