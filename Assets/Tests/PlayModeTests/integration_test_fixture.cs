using System;
using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

[TestFixture]
public abstract class integration_test_fixture
{
    protected abstract string sceneName { get; }
    private Scene scene;
    
    [UnitySetUp]
    protected virtual IEnumerator Setup() {
        yield return SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
        scene = SceneManager.GetSceneByName(sceneName);
        SceneManager.SetActiveScene(scene);
        yield return null;
    }
    
    [UnityTearDown]
    protected IEnumerator TearDown() {
        yield return SceneManager.UnloadSceneAsync(scene);
        yield return null;
    }
    
    protected static void SimulateUpdatesInDuration(
        ITestable testable, 
        float duration, 
        float deltaTime, 
        Action EveryUpdate = null) 
    {
        int numberOfUpdates = Mathf.RoundToInt(duration / deltaTime);
        for (int i = 0; i < numberOfUpdates; i++) {
            testable.UpdateWithDelta(deltaTime);
            EveryUpdate?.Invoke();
        }
    }
}