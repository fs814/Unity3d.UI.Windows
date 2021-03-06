﻿using System.IO;

namespace UnityEngine.UI.Windows.Plugins.Resources {

	public static class Utilities {

		public static ResourceAuto GetResource(string groupName, string filename, string resourcesPathMask, string webPathMask) {

			//Debug.Log("Load: " + groupName + " :: " + filename + " :: " + resourcesPathMask + " :: " + webPathMask);

			ResourceAuto resource = null;

			// Look up built-in storage
			if (resource == null) {

				var path = string.Format("{0}/{1}", groupName, string.Format(resourcesPathMask, filename));
				//Debug.Log("RES: " + path);
				if (UnityEngine.Resources.Load(path) != null) {

					resource = ResourceAuto.CreateResourceRequest(path);

				}

			}

			// Look up cache storage
			if (resource == null) {

				var path = Utilities.GetCachePath(groupName, filename);
				//Debug.Log("CCH: " + path);
				if (File.Exists(path) == true) {

					resource = ResourceAuto.CreateWebRequest(path);

				}

			}

			// Create web request
			if (resource == null) {

				var path = string.Format(webPathMask, filename);
				//Debug.Log("WEB: " + path);
				resource = ResourceAuto.CreateWebRequest(path);

			}

			return resource;

		}

		public static void SaveResource(string groupName, string filename, byte[] bytes) {
			
			var path = Utilities.GetCachePath(groupName, filename);
			File.WriteAllBytes(path, bytes);

		}

		private static string GetCachePath(string groupName, string filename) {
			
			#if UNITY_TVOS
			var cachePath = Application.temporaryCachePath;
			#else
			var cachePath = Application.persistentDataPath;
			#endif

			var dir = string.Format("{0}/{1}", cachePath, groupName);
			var path = string.Format("{0}/{1}.cache", dir, filename);

			try {
				
				if (Directory.Exists(dir) == false) {

					Directory.CreateDirectory(dir);

				}

			} catch (System.Exception) {



			}

			return path;

		}

	}
}

