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
        bool simulatePhysics = false,
        Action everyUpdate = null
    ) 
    {
        int numberOfUpdates = Mathf.RoundToInt(duration / deltaTime);
        for (int i = 0; i < numberOfUpdates; i++) {
            if (simulatePhysics) Physics2D.Simulate(deltaTime);
            UpdateTestable(testable, deltaTime);
            everyUpdate?.Invoke();
        }
    }
    
    protected static void SimulateUpdatesInDuration(
        ITestable[] testables, 
        float duration, 
        float deltaTime, 
        bool simulatePhysics = false,
        Action everyUpdate = null
    ) 
    {
        int numberOfUpdates = Mathf.RoundToInt(duration / deltaTime);
        for (int i = 0; i < numberOfUpdates; i++) {
            if (simulatePhysics) Physics2D.Simulate(deltaTime);
            UpdateTestables(testables, deltaTime);
            everyUpdate?.Invoke();
        }
    }
    
    protected static void SimulateUpdatesWhile(
        ITestable testable, 
        float deltaTime, 
        Predicate<ITestable> predicate,
        bool simulatePhysics = false,
        uint maximumIterations = 1000
    ) {
        uint iterations = 0;
        while (predicate(testable) && iterations < maximumIterations) {
            if (simulatePhysics) Physics2D.Simulate(deltaTime);
            UpdateTestable(testable, deltaTime);
            iterations++;
        }
    }
    
    protected static void SimulateUpdatesWhile(
        ITestable[] testables, 
        float deltaTime,
        Predicate<ITestable[]> predicate,
        bool simulatePhysics = false,
        uint maximumIterations = 1000
    ) {
        uint iterations = 0;
        while (predicate(testables) && iterations < maximumIterations) {
            if (simulatePhysics) Physics2D.Simulate(deltaTime);
            UpdateTestables(testables, deltaTime);
            iterations++;
        }
    }

    private static void UpdateTestable(ITestable testable, float deltaTime) {
        if (testable.enabledForTesting) testable.UpdateWithDelta(deltaTime);
    }

    private static void UpdateTestables(ITestable[] testables, float deltaTime) {
        foreach (ITestable testable in testables) {
            UpdateTestable(testable, deltaTime);
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