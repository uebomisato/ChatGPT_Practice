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

    string _requestText = "";
    string _openAIApiKey;
    string _defaultSetting;

    [SerializeField]
    InputField inputQuestionField;

    [SerializeField]
    Text returnText;

    [SerializeField]
    InputField inputSettingField;
    string _nowSettingText = "";

    //Toggle用のフィールド
    public Toggle isDefaultSetting;

    //Debug用
    [SerializeField]
    GameObject debugModeObject;
    bool _isOpen = false;

    [SerializeField]
    private VideoClip[] videos;
    [SerializeField]
    private RenderTexture[] rawImages;
    [SerializeField]
    private GameObject AICharactor;


    void Start()
    {
        debugModeObject.SetActive(_isOpen);
        ChangeAICharactor(1);

        StartCoroutine(StreamingAssetsLoader.LoadTextFile("OpenAIApiKey.txt", result =>
        {
            _openAIApiKey = result;
        }));

        Debug.Log(_openAIApiKey);
        isDefaultSetting.isOn = true;
    }

    public void OnToggleChanged()
    {

        if (isDefaultSetting.isOn)
        {
            StartCoroutine(StreamingAssetsLoader.LoadTextFile("defaultSetting.txt", result =>
            {
                _defaultSetting = result;
            }));

            Debug.Log(_defaultSetting);

            inputSettingField.text = _defaultSetting;
            _nowSettingText = _defaultSetting;
        }
        else
        {
            inputSettingField.text = "";
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

        returnText.text = returnChatGPTText.choices[0].message.content;
        ChangeAICharactor(1);
    }

    void ChangeAICharactor(int num)
    {
        AICharactor.GetComponent<VideoPlayer>().clip = videos[num];
        AICharactor.GetComponent<VideoPlayer>().targetTexture = rawImages[num];
        AICharactor.GetComponent<RawImage>().texture = rawImages[num];
    }

    /// <summary>
    /// デバッグボタン押下で設定ポップアップ表示の切り替え
    /// </summary>
    public void DebugModeButton()
    {
        _isOpen = !_isOpen;
        debugModeObject.SetActive(_isOpen);
    }
}
