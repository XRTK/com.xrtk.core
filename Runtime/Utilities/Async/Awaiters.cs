// MIT License

// Copyright(c) 2016 Modest Tree Media Inc

// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:

// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.

// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

using System;
using System.Threading;
using System.Threading.Tasks;
using XRTK.Utilities.Async.AwaitYieldInstructions;

namespace XRTK.Utilities.Async
{
    /// <summary>
    /// Utility class to provide custom awaiters.
    /// </summary>
    public static class Awaiters
    {
        /// <summary>
        /// Use this awaiter to continue execution on the main thread.
        /// </summary>
        /// <remarks>Brings the execution back to the main thead on the next engine update.</remarks>
        public static UnityMainThread UnityMainThread { get; } = new UnityMainThread();

        /// <summary>
        /// Use this awaiter to continue execution on the background thread.
        /// </summary>
        public static BackgroundThread BackgroundThread { get; } = new BackgroundThread();

        /// <summary>
        /// Use this awaiter to wait until the condition is met.<para/>
        /// Author: Oguzhan Soykan<para/>
        /// Source: https://stackoverflow.com/questions/29089417/c-sharp-wait-until-condition-is-true
        /// </summary>
        /// <remarks>Passing in -1 will make this wait indefinitely for the condition to be met.</remarks>
        /// <typeparam name="T"></typeparam>
        /// <param name="element"></param>
        /// <param name="predicate">The predicate condition to meet.</param>
        /// <param name="timeout">The number of seconds before timing out and throwing an exception. (-1 is indefinite)</param>
        /// ReSharper disable once ExceptionNotThrown
        /// <exception cref="TimeoutException">A <see cref="TimeoutException"/> can be thrown when the condition isn't satisfied after timeout.</exception>
        public static async Task WaitUntil<T>(this T element, Func<T, bool> predicate, int timeout = 10)
        {
            if (timeout == -1)
            {
                await WaitUntil_Indefinite(element, predicate);
            }
            else
            {
                using (var cancellationTokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(timeout)))
                {
                    var tcs = new TaskCompletionSource<object>();

                    void Exception()
                    {
                        tcs.TrySetException(new TimeoutException());
                        tcs.TrySetCanceled();
                    }

                    cancellationTokenSource.Token.Register(Exception);
#if UNITY_EDITOR
                    var editorCancelled = false;
                    UnityEditor.EditorApplication.playModeStateChanged += playModeStateChanged => editorCancelled = true;
#endif

                    while (!cancellationTokenSource.IsCancellationRequested)
                    {
#if UNITY_EDITOR
                        if (editorCancelled)
                        {
                            tcs.TrySetCanceled(CancellationToken.None);
                        }
#endif
                        try
                        {
                            if (!predicate(element))
                            {
                                await Task.Delay(1, cancellationTokenSource.Token);
                                continue;
                            }
                        }
                        catch (Exception e)
                        {
                            tcs.TrySetException(e);
                        }

                        tcs.TrySetResult(Task.CompletedTask);
                        break;
                    }

                    await tcs.Task;
                }
            }
        }

        private static async Task WaitUntil_Indefinite<T>(T element, Func<T, bool> predicate)
        {
            var tcs = new TaskCompletionSource<object>();

#if UNITY_EDITOR
            var editorCancelled = false;
            UnityEditor.EditorApplication.playModeStateChanged += playModeStateChanged => editorCancelled = true;
#endif
            while (true)
            {
#if UNITY_EDITOR
                if (editorCancelled)
                {
                    tcs.TrySetCanceled(CancellationToken.None);
                }
#endif
                try
                {
                    if (!predicate(element))
                    {
                        await Task.Delay(1);
                        continue;
                    }
                }
                catch (Exception e)
                {
                    tcs.TrySetException(e);
                }

                tcs.TrySetResult(Task.CompletedTask);
                break;
            }

            await tcs.Task;
        }
    }
}
