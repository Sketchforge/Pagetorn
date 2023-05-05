using System.Collections;
using System.Collections.Generic;
using CoffeyUtils.Sound;
using UnityEngine;

namespace Game.SoundSystem
{
    public class MusicManager : MonoBehaviour
    {
        public float timeBetweenSongs = 60f; // The amount of time to wait before changing songs
        [SerializeField] public List<MusicTrack> _peacefulTracks;
        //SoundManager _soundManager;

        private int currentSongIndex = 0; // The index of the currently playing song
        private bool changingSong = false;

        private void Update()
        {
            if ((GameManager.Data.ChaseThemePlaying == true && GameManager.Data.MonstersWatchingPlayer?.Count <= 0))
            {
                if (!changingSong)
                {
                    SoundManager.StopAllMusic();
                    GameManager.Data.ChaseThemePlaying = false;
                    StartCoroutine(ChangeSongs());
                }
                   
            }

            else
            {
                if (!changingSong)
                    StartCoroutine(ChangeSongs());
            }
        }
        //(GameManager.Data.currentRoom.HalfRoomSize.x <= 15 || GameManager.Data.currentRoom.HalfRoomSize.y <= 15) ||
        public IEnumerator ChangeSongs()
        {
            Debug.Log("ChangeSongsActivated");
            // Wait for the specified amount of time
            changingSong = true;
            yield return new WaitForSeconds(timeBetweenSongs);

            // Set the next song to play if no monster watching - these are peaceful songs
            
            SoundManager.PlayMusicNow(_peacefulTracks[currentSongIndex]);
            _peacefulTracks[currentSongIndex].Play();
            SoundManager.QueueMusic(_peacefulTracks[currentSongIndex + 1]);

            // Increase the song index, or loop back to the start if we've reached the end of the array
            currentSongIndex++;
            if (currentSongIndex >= _peacefulTracks.Count)
            {
                currentSongIndex = 0;
            }

            yield return new WaitForSeconds(_peacefulTracks[currentSongIndex].Clip.length);

            
            changingSong = false;

        }
    }
}
