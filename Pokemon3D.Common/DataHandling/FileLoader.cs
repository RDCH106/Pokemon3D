﻿using Pokemon3D.Common.DataHandling;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;

namespace Pokemon3D.Common.DataHandling
{
    public class FileLoader : AsyncDataLoader, FileProvider
    {
        private readonly Dictionary<string, byte[]> _fileCache;

        public FileLoader()
        {
            _fileCache = new Dictionary<string, byte[]>();
        }

        protected override DataLoadResult OnRequestData(string resourcePath)
        {
            byte[] existing;
            if (_fileCache.TryGetValue(resourcePath, out existing)) return DataLoadResult.Succeeded(existing);

            try
            {
                existing = File.ReadAllBytes(resourcePath);
            }
            catch (Exception ex)
            {
                return DataLoadResult.Failed(ex.Message);
            }
            _fileCache.Add(resourcePath, existing);

            return DataLoadResult.Succeeded(existing);
        }

        protected override DataLoadResult[] OnMultiCastRequestData(string resourcePath)
        {
            var requestResults = new List<DataLoadResult>();
            if (Directory.Exists(resourcePath))
            {
                foreach (var filePath in Directory.GetFiles(resourcePath))
                {
                    requestResults.Add(OnRequestData(filePath));
                }
            }
            return requestResults.ToArray();
        }

        public void GetFileAsync(string filePath, Action<DataLoadResult> onDataReceived)
        {
            LoadAsync(filePath, onDataReceived);
        }

        public void GetFilesAsync(string[] filePaths, Action<DataLoadResult[]> onDataReceived)
        {
            LoadAsync(filePaths, onDataReceived);
        }

        public void GetFilesOfFolderAsync(string folderPath, Action<DataLoadResult[]> onDataReceived)
        {
            LoadMulticastAsync(folderPath, onDataReceived);
        }

        public byte[] GetFile(string filePath)
        {
            var data = OnRequestData(filePath);

            return data.Data;
        }
    }
}
