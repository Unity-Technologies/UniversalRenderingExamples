using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using Unity.PerformanceTesting;
using NUnit.Framework;
using UnityEditor;
using System.Collections.Generic;

namespace Tests
{
    public class PerformanceRunnerTests
    {
        private string testScene = "SampleScene";
        private int measureFrames = 1000;
        private int warmupFrames = 100;

        private float startTime;
        private int startFrameCount;
        private float renderedFps;


        static List<string> editorBuildSettingsScenes = new List<string>();

        [RuntimeInitializeOnLoadMethod]
        static void LoadSceneNames()
        {
            int sceneCount = SceneManager.sceneCountInBuildSettings;

            for (int i = 0; i < sceneCount; i++)
            {
                string sceneName = SceneUtility.GetScenePathByBuildIndex(i);
                if (!sceneName.Contains("Init"))
                {
                    editorBuildSettingsScenes.Add(sceneName);
                }
            }
        }


        [UnityTest, Performance]
        public IEnumerator FrameTimeTest([ValueSource("editorBuildSettingsScenes")] string scene)
        {
            LogAssert.ignoreFailingMessages = true;
            // Load the Test Scene
            SceneManager.LoadScene(scene);
            yield return null;

            for (int i = 0; i < warmupFrames; i++)
            {
                yield return new WaitForEndOfFrame();
            }

            ResetFrameTime();
            yield return null;

            SampleGroup ms = new SampleGroup("FrameTime, MS");
            SampleGroup fps = new SampleGroup("FPS", SampleUnit.Undefined, true);

            for (int i = 0; i < measureFrames; i++)
            {
                // Measure FrameTime in MS 
                Measure.Custom(ms, Time.unscaledDeltaTime * 1000);

                // Measure FPS
                renderedFps = CalculateAverageFps();
                Measure.Custom(fps, renderedFps);

                yield return new WaitForEndOfFrame();
            }
        }

        [UnityTest, Performance]
        public IEnumerator MemoryUsageTests([ValueSource("editorBuildSettingsScenes")] string scene)
        {
            LogAssert.ignoreFailingMessages = true;
            // Load the Test Scene
            yield return null;

            for (int i = 0; i < warmupFrames; i++)
            {
                yield return new WaitForEndOfFrame();
            }

            var allocated = new double[measureFrames];
            var reserved = new double[measureFrames];

            SceneManager.LoadScene(scene);

            // Wait a few frames to make sure any allocations done on the first use while rendering/animating
            // the scene are included
            for (int i = 0; i < warmupFrames; i++)
            {
                yield return new WaitForEndOfFrame();
            }

            // Measure for a few frames in case memory usage fluctuates over the frames
            for (var i = 0; i < measureFrames; i++)
            {
                System.GC.Collect();
                allocated[i] = UnityEngine.Profiling.Profiler.GetTotalAllocatedMemoryLong();
                reserved[i] = UnityEngine.Profiling.Profiler.GetTotalReservedMemoryLong();
                yield return null;
            }

            // We wait until the end to report the results to UTF, in case it allocates memory to hold them
            var allocatedSampleGroup = new SampleGroup("TotalAllocatedMemory", SampleUnit.Megabyte);
            var reservedSampleGroup = new SampleGroup("TotalReservedMemory", SampleUnit.Megabyte);
            for (var i = 0; i < measureFrames; i++)
            {
                Measure.Custom(allocatedSampleGroup, allocated[i] / 1024.0 / 1024.0);
                Measure.Custom(reservedSampleGroup, reserved[i] / 1024.0 / 1024.0);
            }
        }

        private float CalculateAverageFps()
        {
            return ((Time.renderedFrameCount - startFrameCount) / (Time.time - startTime));
        }

        private void ResetFrameTime()
        {
            startTime = Time.time;
            startFrameCount = Time.renderedFrameCount;
        }
    }
}
