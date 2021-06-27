using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour{

    public AudioClip ChantThing;
    public AudioClip dudtest3;
    public AudioClip gameBgm;
    public AudioClip HappyCheer;
    public AudioClip HappyWin;
    public AudioClip mouseOver;
    public AudioClip OwwThroat;
    public AudioClip PapyrusMouseover;
    public AudioClip SadCheer;
    public AudioClip SadLoss;
    public AudioClip SnekShake;
    public AudioClip Songthing;
    public AudioClip woosh;
    public AudioClip writing;

    public static void PlaySound(AudioClip clip) {
        GameObject soundGameObject = new GameObject("Sound");
        AudioSource audioSource = soundGameObject.AddComponent<AudioSource>();
        audioSource.PlayOneShot(clip);
    }
}

