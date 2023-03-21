using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class TestSceneManager : MonoBehaviour
{
    private string _openAIApiKey;
    private string _defaultSetting;
    private string _requestText;

    [SerializeField]
    private Image fumanImage;

    [SerializeField]
    private Sprite[] emotionSprites;

    [SerializeField]
    private Text fromChatBotText;

    [SerializeField]
    private Text[] emotionTexts;

    // 設定用のテキストフィールド
    [SerializeField]
    private InputField inputSettingField;

    void Start()
    {
        StartCoroutine(StreamingAssetsLoader.LoadTextFile("OpenAIApiKey.txt", result =>
        {
            _openAIApiKey = result;
        }));

        StartCoroutine(StreamingAssetsLoader.LoadTextFile("defaultSetting2.txt", result =>
        {
            _defaultSetting = result;
        }));

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void TalkingButton()
    {
        _requestText = inputSettingField.text;
        inputSettingField.text = "";
        ChangeEmotionImage(5);
        _ = SendButtonAsync();
    }


    /// <summary>
    /// 送信ボタン押下後の処理
    /// </summary>
    /// <returns></returns>
    async Task SendButtonAsync()
    {
        var connection = new ConnectionChatGPT(_openAIApiKey, _defaultSetting);
        var returnChatGPTText = await connection.RequestAsync(_requestText);

        var returnMessage = returnChatGPTText.choices[0].message.content;

        Debug.Log(returnMessage);

        ChatGPTResponse myData = JsonUtility.FromJson<ChatGPTResponse>(returnMessage);

        SetFromChatGPTText(myData.message);
        SetEmotionText(myData.emotion);

        /*
        // Emotionオブジェクトから各値を取得する
        int joy = myData.emotion.joy;
        int fun = myData.emotion.fun;
        int anger = myData.emotion.anger;
        int sad = myData.emotion.sad;

        // または、次のようにして値を出力することができます
        Debug.Log("Joy: " + joy);
        Debug.Log("Fun: " + fun);
        Debug.Log("Anger: " + anger);
        Debug.Log("Sad: " + sad);

        Debug.Log("-------------------------");
        */
    }

    void SetFromChatGPTText(string text)
    {
        fromChatBotText.text = text;
    }

    void SetEmotionText(Emotion emotion)
    {
        emotionTexts[0].text = "喜 : " + emotion.joy.ToString();
        emotionTexts[1].text = "怒 : " + emotion.anger.ToString();
        emotionTexts[2].text = "哀 : " + emotion.sad.ToString();
        emotionTexts[3].text = "楽 : " + emotion.fun.ToString();

        if (emotion.joy >= 4)
        {
            ChangeEmotionImage(0);
        }
        else if(emotion.anger >= 4)
        {
            ChangeEmotionImage(1);
        }
        else if (emotion.sad >= 4)
        {
            ChangeEmotionImage(2);
        }
        else if (emotion.fun >= 4)
        {
            ChangeEmotionImage(3);
        }
        else
        {
            ChangeEmotionImage(4);
        }
    }

    void ChangeEmotionImage(int index)
    {
        fumanImage.sprite = emotionSprites[index];
    }
}
