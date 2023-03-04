using System;
using System.Collections.Generic;
using System.Text;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
//using static Models;

public class Connection : MonoBehaviour
{
    /*
     * TODO:
     * - ChatGPTと接続
     * - 返答を取得
     * 
     */

    private readonly string _apiKey;
    //会話履歴を保持するリスト
    private readonly List<ChatGPTMessageModel> _messageList = new List<ChatGPTMessageModel>();

    ChatGPTMessageModel setting;

    public Connection(string apiKey,string settingText)
    {
        _apiKey = apiKey;
        //AIの応答の前提条件を記録
        _messageList.Add(
            new ChatGPTMessageModel() { role = "system", content = settingText});
    }

    public async UniTask<ChatGPTResponseModel> RequestAsync(string userMessage)
    {
        //文章生成AIのAPIのエンドポイントを設定
        var apiUrl = "https://api.openai.com/v1/chat/completions";

        //ユーザの発話を記録
        _messageList.Add(new ChatGPTMessageModel { role = "user", content = userMessage });

        //OpenAIのAPIリクエストに必要なヘッダー情報を設定
        var headers = new Dictionary<string, string>
            {
                {"Authorization", "Bearer " + _apiKey},
                {"Content-type", "application/json"},
                {"X-Slack-No-Retry", "1"}
            };


        //文章生成で利用するモデルやトークン上限、プロンプトをオプションに設定
        var options = new RequestModel()
        {
            model = "gpt-3.5-turbo",
            messages = _messageList,
            max_tokens = 140,
            top_p = 1
        };
        var jsonOptions = JsonUtility.ToJson(options);

        Debug.Log("自分:" + userMessage);


        //OpenAIの文章生成(Completion)にAPIリクエストを送り、結果を変数に格納
        using UnityWebRequest request = new UnityWebRequest(apiUrl, "POST")
        {
            uploadHandler = new UploadHandlerRaw(Encoding.UTF8.GetBytes(jsonOptions)),
            downloadHandler = new DownloadHandlerBuffer()
        };

        foreach (var header in headers)
        {
            request.SetRequestHeader(header.Key, header.Value);
        }

        await request.SendWebRequest(); // ここでリクエスト送信


        //エラーの時はここに入る
        if (request.result == UnityWebRequest.Result.ConnectionError ||
           request.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.LogError(request.error);
            throw new Exception();
        }
        else
        {
            //AI側の応答を記録
            //わかりづらいですが、ここではChatGPTMessageModel {role = "user", content = apiからのレスポンス}と同等のオブジェクトが追加されています。

            var responseString = request.downloadHandler.text;
            var responseObject = JsonUtility.FromJson<ChatGPTResponseModel>(responseString);
            Debug.Log("ChatGPT:" + responseObject.choices[0].message.content);
            _messageList.Add(responseObject.choices[0].message);
            return responseObject;
        }
    }
}
