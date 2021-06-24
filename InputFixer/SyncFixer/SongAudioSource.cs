using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace NoStopMod.InputFixer.SyncFixer
{
    class SongAudioSource : MonoBehaviour
    {
        
        //public AudioSource m_audioSource;

        //private bool m_playbackUnmutePending;
        //private double? m_startTime;

        //public void PlayScheduled(double dspStartTime)
        //{
        //    m_startTime = dspStartTime;
        //    m_playbackUnmutePending = true; // weird bug, plays an artifact before scheduled time?
        //    m_audioSource.PlayScheduled(dspStartTime);
        //}

        //void OnAudioFilterRead(float[] data, int channels)
        //{
        //    if (!m_playbackUnmutePending)
        //    {
        //        return;
        //    }

        //    if (AudioSettings.dspTime >= m_startTime)
        //    {
        //        m_playbackUnmutePending = false;
        //        return;
        //    }

        //    for (int i = 0; i < data.Length; i++)
        //    {
        //        data[i] = 0f; // mute until start time cos PlaySchedule artifact.
        //    }
        //}
        
    }
}
