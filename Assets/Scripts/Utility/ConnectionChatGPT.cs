using System;
using System.Collections.Generic;
using System.Text;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

public class ConnectionChatGPT
{
    //APIキー
    private readonly string _apiKey;
    //会話履歴を保持するリスト
    private readonly List<ChatGPTMessageModel> _messageList = new List<ChatGPTMessageModel>();

    public ConnectionChatGPT(string apiKey,string settingText)
    {
        _apiKey = apiKey;
        //AIの応答のプロンプトを記録
        _messageList.Add(
            new ChatGPTMessageModel() { role = "system", content = settingText});
    }

    public async UniTask<ChatGPTResponseModel> RequestAsync(string userMessage)
    {
        //文章生成AIのAPIのエンドポイントを設定
        var apiUrl = "https://api.openai.com/v1/chat/completions";

        //ユーザーからのメッセージを記録
        _messageList.Add(new ChatGPTMessageModel { role = "user", content = userMessage });

        //OpenAIのAPIリクエストに必要なヘッダー情報を設定
        var headers = new Dictionary<string, string>
            {
                {"Authorization", "Bearer " + _apiKey},
                {"Content-type", "application/json"}
            };


        //利用するモデルやトークン上限、プロンプトをオプションに設定
        var options = new RequestModel()
        {
            model = "gpt-3.5-turbo",
            messages = _messageList,
            max_tokens = 200,
            top_p = 1
        };
        var jsonOptions = JsonUtility.ToJson(options);

        //HTTP（POST）の情報を設定
        using UnityWebRequest request = new UnityWebRequest(apiUrl, "POST")
        {
            uploadHandler = new UploadHandlerRaw(Encoding.UTF8.GetBytes(jsonOptions)),
            downloadHandler = new DownloadHandlerBuffer()
        };

        //HTTPヘッダーを設定
        foreach (var header in headers)
        {
            request.SetRequestHeader(header.Key, header.Value);
        }

        //ここでリクエスト送信
        await request.SendWebRequest();


        //エラーの時
        if (request.result == UnityWebRequest.Result.ConnectionError ||
           request.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.LogError(request.error);
            throw new Exception();
        }
        else
        {
            var responseString = request.downloadHandler.text;
            var responseObject = JsonUtility.FromJson<ChatGPTResponseModel>(responseString);
            _messageList.Add(responseObject.choices[0].message);

            // _messageListには今までのやりとりが追加されていくため、常に一つ前のやり取りのみ保持しておくようにする
            // _messageList[0]には、AIキャラの設定をしているプロンプトが入っている
            if (_messageList.Count > 3)
            {
                _messageList.RemoveRange(1,2);
            }
            return responseObject;
        }
    }
}
