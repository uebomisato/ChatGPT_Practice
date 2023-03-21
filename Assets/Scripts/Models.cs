using System;
using System.Collections.Generic;
using UnityEngine;

//ChatGPT APIのmessageに含まれる要素
[Serializable]
public class ChatGPTMessageModel
{
    public string role;
    public string content;
}

//ChatGPT APIにRequestを送るためのJSON用クラス
//modelとmessagesについては必須項目
[Serializable]
public class RequestModel
{
    public string model;
    public List<ChatGPTMessageModel> messages;
    public int max_tokens;
    public int top_p;

}

//ChatGPT APIからのResponseを受け取るためのクラス
//ChatGPTからのResponse例は下記に記載
[Serializable]
public class ChatGPTResponseModel
{
    public string id;
    public string @object;
    public int created;
    public Choice[] choices;
    public Usage usage;

    [Serializable]
    public class Choice
    {
        public int index;
        public ChatGPTMessageModel message;
        public string finish_reason;
    }

    [Serializable]
    public class Usage
    {
        public int prompt_tokens;
        public int completion_tokens;
        public int total_tokens;
    }
}

[Serializable]
public class ChatGPTResponse
{
    public Emotion emotion;
    public string message;
}

[Serializable]
public class Emotion
{
    public int joy;
    public int fun;
    public int anger;
    public int sad;
}

//ChatGPTからのResponse例
/*
{
  "id": "chatcmpl-123",
  "object": "chat.completion",
  "created": 1677652288,
  "choices": [{
    "index": 0,
    "message": {
      "role": "assistant",
      "content": "\n\nHello there, how may I assist you today?",
    },
    "finish_reason": "stop"
  }],
  "usage": {
    "prompt_tokens": 9,
    "completion_tokens": 12,
    "total_tokens": 21
  }
}
*/