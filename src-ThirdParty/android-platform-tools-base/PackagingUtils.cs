/*
 * Copyright (C) 2013 The Android Open Source Project
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *      http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 * 
 * Original source: https://android.googlesource.com/platform/tools/base/+/d41d662dbf89f9b60ca6256415a059c0107749b8/sdk-common/src/main/java/com/android/ide/common/packaging/PackagingUtils.java
 */

using System;
using System.Linq;
using System.IO;
using System.Text.RegularExpressions;

namespace Xamarin.Android.Tasks
{
	/// <summary>
	/// Utility class for packaging.
	/// </summary>
	internal class PackagingUtils
	{
		static readonly Regex pathRegex = new Regex (@"\\|\/", RegexOptions.Compiled);

		/// <summary>
		/// Checks if a zip entry is valid for packaging into the .apk as standard Java resource.
		/// </summary>
		/// <param name="entryName">the name of the zip entry from Xamarin.Tools.Zip.ZipEntry.FullName.</param>
		/// <returns>true if the entry is valid for packaging.</returns>
		public static bool CheckEntryForPackaging (string entryName)
		{
			var segments = pathRegex.Split (entryName);
			for (int i = 0; i < segments.Length - 1; i++) {
				if (!CheckFolderForPackaging (segments [i])) {
					return false;
				}
			}

			var fileName = segments [segments.Length - 1];
			if (string.IsNullOrEmpty (fileName))
				return false;
			var extension = Path.GetExtension (fileName);
			return CheckSignatureFile (segments [0], extension) &&
				CheckFileForPackaging (fileName, extension);
		}

		/// <summary>
		/// Checks whether a folder and its content is valid for packaging into the .apk as standard Java resource.
		/// </summary>
		/// <param name="folderName">the name of the folder.</param>
		/// <returns>true if the folder is valid for packaging.</returns>
		static bool CheckFolderForPackaging (string folderName)
		{
			return !EqualsIgnoreCase (folderName, "CVS") &&
				!EqualsIgnoreCase (folderName, ".svn") &&
				!EqualsIgnoreCase (folderName, "SCCS") &&
				!folderName.StartsWith ("_", StringComparison.Ordinal);
		}

		/// <summary>
		/// Checks for signing related files such as META-INF/MANIFEST.MF or META-INF/GOOG.RSA
		/// </summary>
		/// <param name="folderName">the name of the folder.</param>
		/// <param name="extension">the extension of the file (including '.')</param>
		/// <returns></returns>
		static bool CheckSignatureFile (string folderName, string extension)
		{
			return folderName != "META-INF" ||
				!EqualsIgnoreCase (".SF", extension) &&
				!EqualsIgnoreCase (".MF", extension) &&
				!EqualsIgnoreCase (".RSA", extension);
		}

		/// <summary>
		/// Checks a file to make sure it should be packaged as standard resources.
		/// </summary>
		/// <param name="fileName">the name of the file (including extension)</param>
		/// <param name="extension">the extension of the file (including '.')</param>
		/// <returns>true if the file should be packaged as standard java resources.</returns>
		static bool CheckFileForPackaging (string fileName, string extension)
		{
			// ignore hidden files and backup files
			return !(fileName [0] == '.' || fileName [fileName.Length - 1] == '~') &&
				!EqualsIgnoreCase (".aidl", extension) &&        // Aidl files
				!EqualsIgnoreCase (".rs", extension) &&          // RenderScript files
				!EqualsIgnoreCase (".fs", extension) &&          // FilterScript files
				!EqualsIgnoreCase (".rsh", extension) &&         // RenderScript header files
				!EqualsIgnoreCase (".d", extension) &&           // Dependency files
				!EqualsIgnoreCase (".java", extension) &&        // Java files
				!EqualsIgnoreCase (".scala", extension) &&       // Scala files
				!EqualsIgnoreCase (".class", extension) &&       // Java class files
				!EqualsIgnoreCase (".scc", extension) &&         // VisualSourceSafe
				!EqualsIgnoreCase (".swp", extension) &&         // vi swap file
				!EqualsIgnoreCase ("thumbs.db", fileName) &&     // image index file
				!EqualsIgnoreCase ("picasa.ini", fileName) &&    // image index file
				!EqualsIgnoreCase ("about.html", fileName) &&    // Javadoc
				!EqualsIgnoreCase ("package.html", fileName) &&  // Javadoc
				!EqualsIgnoreCase ("overview.html", fileName);   // Javadoc
		}

		static bool EqualsIgnoreCase (string a, string b)
		{
			return string.Compare (a, b, StringComparison.OrdinalIgnoreCase) == 0;
		}
	}
}
