﻿using System;
using System.IO;
using System.Linq;
using System.Reflection;

namespace SharpCpp
{
    public static class FileUtils
    {
        /// <summary>
        /// Extend PATH
        /// </summary>
        private static readonly string[] PATH = new[] {
            "/usr/local/bin" // For MacOS
        };

        /// <summary>
        /// Finds a command in the PATH environment variable or throws FileNotFoundException.
        /// </summary>
        public static string FindExecutable(string command)
        {
            if (File.Exists(command))
                return Path.GetFullPath(command);

            var pathVariable = Environment.GetEnvironmentVariable("PATH");

            foreach (var path in PATH.Concat(pathVariable.Split(Path.PathSeparator))) {
                var fullPath = Path.Combine(path, command);
                if (File.Exists(fullPath))
                    return fullPath;
            }

            throw new FileNotFoundException();
        }

        /// <summary>
        /// Copies resource identified by resourceId to the application's temporary folder.
        /// </summary>
        /// <returns>Path to the resource or throws an exception</returns>
        public static string CopyResourceToTmpFolder(string resourceId)
        {
            var assembly = Assembly.GetExecutingAssembly();
            var parts = resourceId.Split('.');

            if (parts == null || parts.Length < 2)
                throw new TFatalException("Illegal resourceId");

            var tmpName = Path.Combine(
                Path.GetTempPath(),
                parts[parts.Length - 2] + "." + parts[parts.Length - 1]
            );

            using (Stream stream = assembly.GetManifestResourceStream(resourceId)) {
                if (stream == null)
                    throw new TFatalException("Resource[" + resourceId + "] not found");
                
                using (StreamReader reader = new StreamReader(stream)) {
                    File.WriteAllText(tmpName, reader.ReadToEnd());
                }
            }

            return tmpName;
        }
    }
}
