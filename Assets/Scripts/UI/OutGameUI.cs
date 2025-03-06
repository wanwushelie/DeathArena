using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class OutGameUI : MonoBehaviour
{
    public static OutGameUI instance;

    public GameObject panel;
    public Button newGameBtn;
    public Button continueGameBtn;
    public Button exitGameBtn;
    public Button yesBtn;
    public Button noBtn;
    public bool isNewGame;

    [SerializeField] private AudioData audioData;
    void Awake()
    {
        if (!instance)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
        SoundManager.Instance.Init();
        SoundManager.Instance.Init(audioData);
    }

    void Start()
    {
        //SoundManager.Instance.Play("BGM/OutGame", SoundType.BGM);
        SoundManager.Instance.Play("背景音乐");

        GameObject inventoryObj = new GameObject("InventorySave");
        SaveData inventorySave = inventoryObj.AddComponent<SaveData>();

        if (!inventorySave.HasSavedData())
        {
            isNewGame = true;
            continueGameBtn.gameObject.SetActive(false);
            newGameBtn.onClick.AddListener(() =>
            {
                newGameBtn.interactable = false;
                //SoundManager.Instance.Play("EFFECT/Click2", SoundType.EFFECT);
                SoundManager.Instance.Play("主菜单界面的按钮点击音效");
                StartCoroutine(StartGameWithFadeOut());
            });
        }
        else
        {
            isNewGame = false;
            continueGameBtn.gameObject.SetActive(true);
            continueGameBtn.onClick.AddListener(() =>
            {
                continueGameBtn.interactable = false;
                //SoundManager.Instance.Play("EFFECT/Click2", SoundType.EFFECT);
                SoundManager.Instance.Play("主菜单界面的按钮点击音效");
                StartCoroutine(StartGameWithFadeOut());
            });

            newGameBtn.onClick.AddListener(() =>
            {
                //SoundManager.Instance.Play("EFFECT/Click2", SoundType.EFFECT);
                SoundManager.Instance.Play("主菜单界面的按钮点击音效");
                // 기존 리스너 제거 (중복 등록 방지)
                yesBtn.onClick.RemoveAllListeners();
                noBtn.onClick.RemoveAllListeners();

                panel.SetActive(true);

                yesBtn.onClick.AddListener(() =>
                {
                    //SoundManager.Instance.Play("EFFECT/Click2", SoundType.EFFECT);
                    SoundManager.Instance.Play("主菜单界面的按钮点击音效");
                    panel.SetActive(false);
                    isNewGame = true;
                    StartCoroutine(StartGameWithFadeOut());
                });
                noBtn.onClick.AddListener(() =>
                {
                    // SoundManager.Instance.Play("EFFECT/Click2", SoundType.EFFECT);
                    SoundManager.Instance.Play("主菜单界面的按钮点击音效");
                    panel.SetActive(false);
                });
            });
        }

        exitGameBtn.onClick.AddListener(() =>
        {
            // SoundManager.Instance.Play("EFFECT/Click2", SoundType.EFFECT);
            SoundManager.Instance.Play("主菜单界面的按钮点击音效");
            SoundManager.Instance.StopAll();
            Application.Quit();
        });
    }

    // FadeOut과 게임 시작을 동시에 실행
    private IEnumerator StartGameWithFadeOut()
    {
        SoundManager.Instance.FadeOut(3.0f, "BGM/OutGame");
        //SoundManager.Instance.FadeOut(3.0f, "背景音乐");

        FadeEffect.instance.FadeAndLoadScene("InGameScene");

        yield return new WaitForSeconds(2.0f);
    }
}
