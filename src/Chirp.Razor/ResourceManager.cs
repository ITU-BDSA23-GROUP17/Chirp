using System;
using System.IO;
using System.Reflection;
//this code is by chatgpt
//source https://stackoverflow.com/questions/3314140/how-to-read-embedded-resource-text-file
public class ResourceManager
{
    public static string GetEmbeddedResource(string resourceName)
    {
        var assembly = Assembly.GetExecutingAssembly();
        using (Stream stream = assembly.GetManifestResourceStream(resourceName))
        {
            if (stream == null)
            {
                throw new ArgumentException($"Resource {resourceName} not found in assembly.");
            }

            using (StreamReader reader = new StreamReader(stream))
            {
                return reader.ReadToEnd();
            }
        }
    }
}
