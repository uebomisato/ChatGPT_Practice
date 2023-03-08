using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
//using static Models;

public class SceneManager : MonoBehaviour
{

    private string _requestText = "";
    private string _openAIApiKey;
    private string _defaultSetting;

    // ユーザーからの質問入力用テキストフィールド
    [SerializeField]
    private InputField inputQuestionField;

    // API叩いて返ってきたテキスト表示用
    [SerializeField]
    private Text returnTextFromApi;

    // ***** Debug用に表示する設定画面 *****
    // 設定画面オブジェクト
    [SerializeField]
    private GameObject SettingsObject;

    // 設定画面を開いているかのbool
    private bool _isOpen = false;

    // 設定用のテキストフィールド
    [SerializeField]
    private InputField inputSettingField;
    private string _nowSettingText = "";

    // デフォルト設定を適用しているかのON・OFF
    public Toggle isDefaultSetting;

    [SerializeField]
    private GameObject Blur;

    [SerializeField]
    private VideoClip[] videos;
    private VideoPlayer _videoPlayer;

    [SerializeField]
    private RenderTexture[] renderTextures;
    private RawImage _rawImage;

    [SerializeField]
    private GameObject AICharactor;

    [SerializeField]
    private Text _totalToken;
    private int _TotalToken = 0;


    void Start()
    {
        _videoPlayer = AICharactor.GetComponent<VideoPlayer>();
        _rawImage = AICharactor.GetComponent<RawImage>();

        StartCoroutine(StreamingAssetsLoader.LoadTextFile("OpenAIApiKey.txt", result =>
        {
            _openAIApiKey = result;
        }));

        StartCoroutine(StreamingAssetsLoader.LoadTextFile("defaultSetting.txt", result =>
        {
            _defaultSetting = result;
            // textFieldにも表示させておく
            inputSettingField.text = _defaultSetting;
            // 現在の設定内容として保持する
            _nowSettingText = _defaultSetting;
        }));


        ChangeAICharactor(1);

        SettingsObject.SetActive(_isOpen);
        Blur.SetActive(_isOpen);
    }

    /// <summary>
    /// デフォルト設定のON・OFF切り替え
    /// </summary>
    public void OnToggleChanged()
    {
        // デフォルト設定がONの時、defaultSetting.txtを読み込む
        if (isDefaultSetting.isOn)
        {
            // textFieldにも表示させておく
            inputSettingField.text = _defaultSetting;
            // 現在の設定内容として保持する
            _nowSettingText = _defaultSetting;
        }
        else
        {
            // デフォルト設定がOFFの時、設定内容を削除
            inputSettingField.text = "";
            _nowSettingText = "";
        }
    }

    /// <summary>
    /// 設定用テキストを入力後、反映させる処理
    /// </summary>
    public void TapSettingButton()
    {
        _nowSettingText = inputSettingField.text;
    }

    /// <summary>
    /// 送信ボタン押下
    /// </summary>
    public void OnTap()
    {
        ChangeAICharactor(0);
        _ = SendButtonAsync();
    }

    /// <summary>
    /// 送信ボタン押下後の処理
    /// </summary>
    /// <returns></returns>
    async Task SendButtonAsync()
    {
        _requestText = inputQuestionField.text;
 
        var connection = new Connection(_openAIApiKey, _nowSettingText);
        var returnChatGPTText = await connection.RequestAsync(_requestText);
        _TotalToken += returnChatGPTText.usage.total_tokens;

        _totalToken.text = "現在の累計使用トークン数 : " + _TotalToken.ToString();
        returnTextFromApi.text = returnChatGPTText.choices[0].message.content;
        ChangeAICharactor(1);
    }

    /// <summary>
    /// キャラクターの動作を変更
    /// </summary>
    /// <param name="num">配列の何番目のものに差し替えるか</param>
    void ChangeAICharactor(int num)
    {
        _videoPlayer.clip = videos[num];
        _videoPlayer.targetTexture = renderTextures[num];
        _rawImage.texture = renderTextures[num];
    }

    /// <summary>
    /// デバッグボタン押下で設定ポップアップ表示の切り替え
    /// </summary>
    public void DebugModeButton()
    {
        _isOpen = !_isOpen;
        SettingsObject.SetActive(_isOpen);
        Blur.SetActive(_isOpen);
    }
}
