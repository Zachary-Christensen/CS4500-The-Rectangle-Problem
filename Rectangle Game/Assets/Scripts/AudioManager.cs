using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public AudioSource winSound;
    public AudioSource loseSound;
    public AudioSource wrongSolutionSound;
    public AudioSource moveSound;
    public AudioSource dropSound;
    public AudioSource rotateSound;
    public AudioSource scaleDownSound;
    public AudioSource scaleUpSound;


    public void PlayWin()
    {
        winSound.Play();
    }

    public void PlayLose()
    {
        loseSound.Play();
    }

    public void PlayWrongSolution()
    {
        wrongSolutionSound.Play();
    }

    public void PlayMove()
    {
        moveSound.Play();
    }

    public void PlayDrop()
    {
        dropSound.Play();
    }

    public void PlayRotate()
    {
        rotateSound.Play();
    }

    public void PlayScaleDown()
    {
        scaleDownSound.Play();
    }

    public void PlayScaleUp() 
    {
        scaleUpSound.Play();    
    }
}
