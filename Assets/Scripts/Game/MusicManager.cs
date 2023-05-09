using System.Collections;
using System.Collections.Generic;
using CoffeyUtils;
using CoffeyUtils.Sound;
using UnityEngine;

namespace Game.SoundSystem
{
    public class MusicManager : MonoBehaviour
    {
        public Vector2 timeBetweenSongs = new Vector2(10f, 40f); // The amount of time to wait before changing songs
        [SerializeField] public List<MusicTrack> _peacefulTracks;
        //SoundManager _soundManager;

        [SerializeField, ReadOnly] private int currentSongIndex = 0; // The index of the currently playing song
        [SerializeField, ReadOnly] private bool changingSong = false;


        private void Awake()
        {
            SoundManager.StopAllMusic();
        }
        private void Update()
        {
            if (GameManager.Data.ChaseThemePlaying && GameManager.Data.MonstersWatchingPlayer?.Count <= 0)
            {
                SoundManager.StopAllMusic();
                GameManager.Data.ChaseThemePlaying = false;
                StopAllCoroutines();
                StartCoroutine(ChangeSongs());
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
            yield return new WaitForSeconds(Random.Range(timeBetweenSongs.x, timeBetweenSongs.y));

            // Set the next song to play if no monster watching - these are peaceful songs
            
            SoundManager.PlayMusicNow(_peacefulTracks[currentSongIndex]);
            //_peacefulTracks[currentSongIndex].Play();

            //if the next song in the index does not reach the end of array, queue it. Otherwise, q the beginning track
            if (currentSongIndex + 1 < _peacefulTracks.Count)
            {
                SoundManager.QueueMusic(_peacefulTracks[currentSongIndex + 1]);
            }
            else
            {
                SoundManager.QueueMusic(_peacefulTracks[0]);
            }

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
