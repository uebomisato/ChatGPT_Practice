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
    string returnFilePathText;

    [SerializeField]
    InputField inputQuestionField;

    [SerializeField]
    Text returnText;

    [SerializeField]
    InputField inputSettingField;
    string _nowSettingText = "";

    //Toggle用のフィールド
    public Toggle isDefaultSetting;

    [SerializeField]
    private VideoClip[] videos;
    [SerializeField]
    private RenderTexture[] rawImages;
    [SerializeField]
    private GameObject AICharactor;

    // Start is called before the first frame update
    void Start()
    {
        ChangeAICharactor(1);
        _openAIApiKey = ReadFile("OpenAIApiKey");
        isDefaultSetting.isOn = false;
    }

    public void OnToggleChanged()
    {

        if (isDefaultSetting.isOn)
        {
            _defaultSetting = ReadFile("defaultSetting");
            inputSettingField.text = _defaultSetting;
            _nowSettingText = _defaultSetting;
        }
        else
        {
            inputSettingField.text = "";
        }
    }

    // 読み込み関数
    string ReadFile(string filePath)
    {
        // FileReadTest.txtファイルを読み込む
        FileInfo fi = new FileInfo(Application.dataPath + "/" + filePath + ".txt");

        try
        {
            // 一行毎読み込み
            using (StreamReader sr = new StreamReader(fi.OpenRead(), Encoding.UTF8))
            {
                returnFilePathText = sr.ReadToEnd();
            }
        }
        catch (Exception e)
        {
            // 改行コード
            returnFilePathText += SetDefaultText();
        }

        return returnFilePathText;
    }

    // 改行コード処理
    string SetDefaultText()
    {
        return "C#あ\n";
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
}
