using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundLibrary : MonoBehaviour{

    public SoundGroup[] groups;

    Dictionary<string, AudioClip[]> groupDict = new Dictionary<string, AudioClip[]>();

    void Awake(){
        foreach (SoundGroup group in groups){
            groupDict.Add (group.groupID, group.group);
        }
    }

    public AudioClip GetClipByName(string name){
        if(groupDict.ContainsKey(name)){
            AudioClip[] sounds = groupDict[name];
            return sounds[Random.Range(0, sounds.Length)]; 
        }
        return null;
    }

    [System.Serializable]
    public class SoundGroup {
        public string groupID;
        public AudioClip[] group;
    }
}