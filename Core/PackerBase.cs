using System;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Threading;

namespace Sacristan.Ahhnold.IO
{
    public static partial class SaveFile
    {
        public abstract class PackerBase
        {
            protected const string InvalidHashMessage = "InvalidHash";
            protected virtual string FileName { get; }
            protected virtual string Extension => ".dat";
            protected virtual string Salt => "17t5j010Z611KIx";
            protected string FileNameWithExtension => FileName + Extension;
            public virtual bool HasSaveFile => File.Exists(SaveFilePath);
            public string SaveFilePath => GetDataPath(FileNameWithExtension);

            public virtual void Delete()
            {
                CancelPendingSave();
                string path = SaveFilePath;
                if (HasSaveFile) File.Delete(path);
                DeleteTempFile(path);
            }

            protected string GetHash(string data)
            {
                return Sha256Sum(data + "(╯°□°）╯︵ ┻━┻" + Salt);
            }

            protected bool IsHashValid(string data, string hash)
            {
                return GetHash(data).Equals(hash);
            }

            private static string Sha256Sum(string str)
            {
                System.Security.Cryptography.SHA256Managed crypt = new System.Security.Cryptography.SHA256Managed();
                StringBuilder hash = new StringBuilder();
                byte[] cryptedBytes = crypt.ComputeHash(Encoding.UTF8.GetBytes(str), 0, Encoding.UTF8.GetByteCount(str));

                for (int i = 0; i < cryptedBytes.Length; i++)
                {
                    byte cryptedByte = cryptedBytes[i];
                    hash.Append(cryptedByte.ToString("x2"));
                }

                return hash.ToString().ToLower();
            }

            protected const string TempFileExtension = ".tmp";

            // Waits until the save is physically on storage before swapping it in, so it survives power loss or an
            // OS crash. An app kill doesn't need it (the OS still writes out what it was handed), so packers that
            // save very often can turn it off to skip that wait and keep the crash-safe swap.
            protected virtual bool FlushToDisk => true;

            protected static string GetTempFilePath(string path) => path + TempFileExtension;

            // Saves are written here, then swapped in with CommitTempFile, so a crash never leaves a truncated save.
            protected static FileStream CreateTempFile(string path) =>
                new FileStream(GetTempFilePath(path), FileMode.Create, FileAccess.Write, FileShare.None);

            protected void WriteTempFile(string path, string contents) => WriteTempFile(path, contents, FlushToDisk);

            protected static void WriteTempFile(string path, string contents, bool flushToDisk) =>
                WriteTempFile(path, new UTF8Encoding(false).GetBytes(contents), flushToDisk);

            protected static void WriteTempFile(string path, byte[] bytes, bool flushToDisk)
            {
                bool written = false;

                try
                {
                    using (FileStream stream = CreateTempFile(path))
                    {
                        stream.Write(bytes, 0, bytes.Length);
                        stream.Flush(flushToDisk);
                    }

                    written = true;
                }
                finally
                {
                    // A failed write (fe disk full) shouldn't leave a partial temp taking up the space.
                    if (!written) TryDeleteTempFile(path);
                }
            }

            protected static void CommitTempFile(string path)
            {
                string tempPath = GetTempFilePath(path);

                if (!File.Exists(path))
                {
                    File.Move(tempPath, path);
                    return;
                }

                try
                {
                    File.Replace(tempPath, path, null);
                }
                catch (System.PlatformNotSupportedException)
                {
                    File.Delete(path);
                    File.Move(tempPath, path);
                }
            }

            protected static void DeleteTempFile(string path)
            {
                string tempPath = GetTempFilePath(path);
                if (File.Exists(tempPath)) File.Delete(tempPath);
            }

            // For cleanup after a failed save, where the original exception is the one worth reporting.
            protected static void TryDeleteTempFile(string path)
            {
                try { DeleteTempFile(path); }
                catch (IOException) { }
                catch (System.UnauthorizedAccessException) { }
            }

            #region Background saving

            // Opt in to write saves on a worker thread: the save is still built on the calling thread (so it's a
            // snapshot), only the file write, flush and swap move off it. Platforms without threads save inline.
            protected virtual bool SaveInBackground => false;

            public static bool SupportsBackgroundSave
            {
                get
                {
#if UNITY_WEBGL && !UNITY_EDITOR
                    return false;
#else
                    return true;
#endif
                }
            }

            protected bool UseBackgroundSave => SaveInBackground && SupportsBackgroundSave;

            // Keyed by save file path rather than per packer, so any packer for a file waits for its writes.
            private static readonly Dictionary<string, WriteQueue> writeQueues = new Dictionary<string, WriteQueue>();
            private static bool waitsOnQuit;

            private sealed class WriteQueue
            {
                private readonly object queueLock = new object();
                private readonly ManualResetEvent idle = new ManualResetEvent(true);
                private Action pending;
                private bool writing;

                public bool IsWriting
                {
                    get { lock (queueLock) return writing; }
                }

                // One write at a time; writes queued meanwhile replace each other, so only the newest runs.
                public void Enqueue(Action write)
                {
                    lock (queueLock)
                    {
                        pending = write;
                        if (writing) return;
                        writing = true;
                        idle.Reset();
                    }

                    ThreadPool.QueueUserWorkItem(_ => Run());
                }

                public void DropPending()
                {
                    lock (queueLock) pending = null;
                }

                public bool Wait(int millisecondsTimeout) => idle.WaitOne(millisecondsTimeout);

                private void Run()
                {
                    while (true)
                    {
                        Action write;

                        lock (queueLock)
                        {
                            write = pending;
                            pending = null;

                            if (write == null)
                            {
                                writing = false;
                                idle.Set();
                                return;
                            }
                        }

                        try { write(); }
                        catch (Exception e) { UnityEngine.Debug.LogError($"Ahhnold.IO background save failed: {e}"); }
                    }
                }
            }

            private WriteQueue FindWriteQueue(bool create)
            {
                string path = SaveFilePath;

                lock (writeQueues)
                {
                    if (!writeQueues.TryGetValue(path, out WriteQueue queue) && create)
                    {
                        queue = new WriteQueue();
                        writeQueues.Add(path, queue);
                    }

                    return queue;
                }
            }

            public bool IsSaving
            {
                get
                {
                    WriteQueue queue = FindWriteQueue(create: false);
                    return queue != null && queue.IsWriting;
                }
            }

            protected void QueueWrite(Action write)
            {
                if (!UseBackgroundSave)
                {
                    write();
                    return;
                }

                if (!waitsOnQuit)
                {
                    waitsOnQuit = true;
                    UnityEngine.Application.quitting += () => WaitForAllPendingSaves();
                }

                FindWriteQueue(create: true).Enqueue(write);
            }

            /// <summary>Blocks until this file's background save (if any) is on disk. True unless it timed out.</summary>
            public bool WaitForPendingSave(int millisecondsTimeout = Timeout.Infinite)
            {
                WriteQueue queue = FindWriteQueue(create: false);
                return queue == null || queue.Wait(millisecondsTimeout);
            }

            /// <summary>
            /// Blocks until every background save is on disk. Runs on Application.quitting by itself; call it when the
            /// app is paused too (fe OnApplicationPause(true)), as mobile and standalone VR apps are often killed
            /// while paused without quitting. True unless it timed out.
            /// </summary>
            public static bool WaitForAllPendingSaves(int millisecondsTimeout = Timeout.Infinite)
            {
                WriteQueue[] queues;
                lock (writeQueues)
                {
                    queues = new WriteQueue[writeQueues.Count];
                    writeQueues.Values.CopyTo(queues, 0);
                }

                bool allDone = true;
                foreach (WriteQueue queue in queues)
                    allDone &= queue.Wait(millisecondsTimeout);

                return allDone;
            }

            // Drops a queued save that hasn't started and waits for the one in progress.
            protected void CancelPendingSave()
            {
                WriteQueue queue = FindWriteQueue(create: false);
                if (queue == null) return;

                queue.DropPending();
                queue.Wait(Timeout.Infinite);
            }

            #endregion

        }

    }
}