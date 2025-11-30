using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Transcript : MonoBehaviour {
    Dictionary<string, TranscriptEntry> entries;
    public static Transcript instance;

    void Start() {
        instance = this;
        entries = new Dictionary<string, TranscriptEntry>();
        entries.Add("test_01", new TranscriptEntry("Test/test_01", "This is a test subtitle. waaaa"));
    }

    public TranscriptEntry GetEntry(string key) {
        if(entries.ContainsKey(key)) {
            return entries[key];
        }
        return null;
    }
}

public class TranscriptEntry {
    public string filename;
    public string subtitle;

    public TranscriptEntry(string t, string d) {
        filename = t;
        subtitle = d;
    }
}
