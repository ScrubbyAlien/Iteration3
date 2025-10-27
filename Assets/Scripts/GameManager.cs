using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    private float endX;
    [SerializeField]
    private Transform player;
    [SerializeField]
    private string mainMenuScene;
    [SerializeField]
    private float restartTime;
    
    private bool gameEnded;

    public UnityEvent OnWin;

    private void Start() {
        if (!player) return;
        player.GetComponent<CollisionHandler>().OnDie += () => StartCoroutine(RestartLevel(restartTime));
    }

    private void Update() {
        if (!player) return;
        if (!gameEnded && player.position.x > endX) {
            gameEnded = true;
            OnWin?.Invoke();
            StartCoroutine(EndLevel(restartTime));
        }
    }

    private IEnumerator EndLevel(float time) {
        yield return new WaitForSeconds(time);
        SceneManager.LoadScene(mainMenuScene);
    }

    private IEnumerator RestartLevel(float time) {
        yield return new WaitForSeconds(time);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void LoadLevel(string name) {
        SceneManager.LoadScene(name);
    }
}
