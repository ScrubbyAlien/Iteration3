using System;
using System.Collections;
using System.Linq;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using Object = UnityEngine.Object;

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
    
    protected static IEnumerator WaitForNumberOfFixedUpdates(int updates) {
        for (int i = 0; i < updates; i++) {
            yield return new WaitForFixedUpdate();
        }
    }
    
    protected static IEnumerator WaitForNumberOfFrames(int frames) {
        for (int i = 0; i < frames; i++) {
            yield return null;
        }
    }
    
    protected static Vector3 GetObjectPositionByName(string name) { 
        return Object.FindObjectsByType<Transform>(FindObjectsSortMode.None)
                     .First(g => g.gameObject.name == name)
                     .transform.position;
    }

}