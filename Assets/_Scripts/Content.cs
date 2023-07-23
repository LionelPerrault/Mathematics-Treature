using System.Collections.Generic;
using System.IO;
using System;
using UnityEngine;
using UnityEngine.Networking;

#nullable enable

[Serializable]
public class Content : MonoBehaviour {
    string? jsonPath;
    public const string filename = "Text_Content.json";
    
    Dictionary<uint, TextSegment> textDict = new Dictionary<uint, TextSegment>();

    public void Start() {
        jsonPath = System.IO.Path.Combine(Application.streamingAssetsPath, filename);
        StartCoroutine( makeRequest() );
    }

    IEnumerator<UnityWebRequestAsyncOperation> makeRequest() {
        UnityWebRequest req = UnityWebRequest.Get(jsonPath);
        yield return req.SendWebRequest();

        if( !(req.isNetworkError || req.isHttpError) )
            parseJsonString(req.downloadHandler.text, ref textDict);
        else
            Debug.LogError(req.error);
    }

    public void parseJsonString(string json, ref Dictionary<uint, TextSegment> temp) {
        TextCollector collector = JsonUtility.FromJson<TextCollector>(json);

        if (collector.segments is null) return;
        
        foreach(TextSegment segment in collector.segments)
            textDict.Add(segment.id, segment);
    }

    /// read the Text_Content.json and parses it into the textDict
    public void readParseJson(string path, ref Dictionary<uint, TextSegment> temp) {
        using (StreamReader sr = new StreamReader(path)) {
            string json = sr.ReadToEnd();
            parseJsonString(json, ref temp);
        }
    }

    /// write to Text_Content.json a TextCollector
    void writeToJson(string path, TextCollector content) {
        string json = JsonUtility.ToJson(content, true);
        File.WriteAllText(path, json);
    }

    /// returns a Textsegment or null for a given id
    public TextSegment? GetTextSegment(uint segmentID) {
        return textDict.ContainsKey(segmentID) ? textDict[segmentID] : null;
    }

    public int GetSegmentAmount() {
        return textDict.Count;
    }

    [Serializable]
    class TextCollector {
        public TextSegment[] segments;

        public TextCollector(TextSegment[] s) { segments = s; }
        public override string ToString() {
            string output = "";
            foreach (TextSegment ts in segments) output += ts.ToString() + "\n";
            return output;
        }
    }
}

[Serializable]
public class TextSegment {
    public uint id;             /// scalar identifier, segments will be worked thru in chronologically order
    public string content;      /// string containing the Text/Content of that Segment
    public string textAnswer;   /// string containing the Text to which the user gives an anwser
    public Answer answer;       /// this Segments Answer
    
    public TextSegment(uint xid, string c, string tA, Answer a) {
        id=xid; content=c; textAnswer=tA; answer=a;
    }

    public override string ToString() {
        return $"id: {id}, content: {content}, textAnswer: {textAnswer}, answer: {{\n{answer}\n}}";
    }
}

[Serializable]
public class Answer {
    public bool      isMultipleChoice;  /// indicating of this Answer instance is a single sentence or multiple Choice Answer
    public SingleAnswer? singleAnswer;      /// single Answer Sentence which is to be expected
    public Choice?[] multipleChoices;   /// Multiple Choice Answer which is to be expected

    public Answer(bool iMC, SingleAnswer? sA=null, Choice?[] mCs=null) {
        isMultipleChoice = iMC;
        singleAnswer     = sA;
        multipleChoices  = mCs;
    }

    public override string ToString() {
        if(!isMultipleChoice) return singleAnswer.ToString();
        else {
            string output = "\tChoice: [\n\t\t";
            foreach(Choice choi in multipleChoices) output += choi.ToString() + ";\n\t\t";
            return output.Remove(output.Length-4, 4) + "\n\t]";
        }
    }
}

[Serializable]
public class SingleAnswer {
    public string singleAnswer;
    public KeyboardMode mode;

    public SingleAnswer(string sA, KeyboardMode m) {
        singleAnswer = sA; mode = m;
    }

    public override string ToString() {
        return $"\tsingleAnswer: {singleAnswer}, mode: {mode}";
    }
}

[Serializable]
public class Choice {
    public string sentence;  /// displays an Text which the user can select
    public bool isCorrect;   /// determins if this Choice is to be selected (i.e. correct) or not 

    public Choice(string s, bool i) {
        sentence=s; isCorrect=i;
    }

    public override string ToString() {
        string t = "True "; string f = "False";
        return $"isCorrect: {(isCorrect?t:f)}, sentence: {sentence}";
    }
}