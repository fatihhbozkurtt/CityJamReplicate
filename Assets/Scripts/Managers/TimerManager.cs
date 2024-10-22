using TMPro;
using UnityEngine;

namespace Managers
{
    public class TimerManager : MonoBehaviour
    {
        [Header("References")] public TextMeshProUGUI timerText;
        [Header("Config")] public int totalSeconds;
        [Header("Debug")] private float _timeRemaining;
        private bool _timerIsRunning;

        private void Start()
        {
            _timeRemaining = totalSeconds;
            _timerIsRunning = true;
        }

        private void Update()
        {
            if (!GameManager.instance.isLevelActive) return;
            if (!_timerIsRunning) return;

            if (_timeRemaining > 0)
            {
                _timeRemaining -= Time.deltaTime;
                DisplayTime(_timeRemaining);
            }
            else
            {
                _timeRemaining = 0;
                _timerIsRunning = false;
                GameManager.instance.EndGame(false);
            }
        }

        void DisplayTime(float timeToDisplay)
        {
            timeToDisplay = Mathf.Max(timeToDisplay, 0);

            int minutes = Mathf.FloorToInt(timeToDisplay / 60);
            int seconds = Mathf.FloorToInt(timeToDisplay % 60);

            timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
        }
    }
}