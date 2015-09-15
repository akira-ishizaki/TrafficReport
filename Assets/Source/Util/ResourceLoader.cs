﻿    using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;

namespace TrafficReport.Util
{
    public class ResourceLoader
    {

        public static Assembly ResourceAssembly
        {
            get {
                //return null;
                return Assembly.GetAssembly(typeof(ResourceLoader));
            }
        }

        public static byte[] loadResourceData(string name)
        {
#if BuildingModDll
            name = "TrafficReport.Assets." + name.Replace("/",".");

            UnmanagedMemoryStream stream  = (UnmanagedMemoryStream)ResourceAssembly.GetManifestResourceStream(name);
            if (stream == null)
            {
                Log.error("Could not find resource: " + name);
                return null;
            }

            Log.debug("Found resource: " + name);
            BinaryReader read = new BinaryReader(stream);
            return read.ReadBytes((int)stream.Length);
#else             
            string resolvedName = "Assets/" + name;
            Log.info("Loading: " + resolvedName);
            return File.ReadAllBytes(resolvedName);
#endif
        }

        public static string loadResourceString(string name)
        {
           return System.Text.Encoding.UTF8.GetString(loadResourceData(name));
        }

        public static Texture2D loadTexture(string filename)
        {
            try
            {
                Texture2D texture = new Texture2D(100,100, TextureFormat.ARGB32, true); //These values make no difference
                texture.filterMode = FilterMode.Trilinear;
                texture.LoadImage(loadResourceData(filename));
                return texture;
            }
            catch (Exception e)
            {
                Log.error("LoadTexture() The file could not be read:" + e.Message);                
            }

            return null;
        }

    }
}
