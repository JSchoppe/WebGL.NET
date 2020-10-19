using System;
using System.Collections.Generic;
using System.Text;
using WebGLDotNET;
using Demo.Helpers;
using System.IO;

namespace Demo.DemoClasses.Loading
{
    /// <summary>
    /// Contains methods for loading assets for WebGL.
    /// </summary>
    public static class AssetsIO
    {
        /// <summary>
        /// Loads and compiles a shader program.
        /// </summary>
        /// <param name="webGLBase">The context that will compile the shader.</param>
        /// <param name="vertexShaderPath">The path to the vertex shader code.</param>
        /// <param name="fragmentShaderPath">The path to the fragment shader code.</param>
        /// <returns></returns>
        public static WebGLProgram LoadShader(WebGLRenderingContextBase webGLBase,
            string vertexShaderPath, string fragmentShaderPath)
        {
            WebGLProgram shaderProgram;
            using (var vertexShaderStream = EmbeddedResourceHelper.Load(vertexShaderPath))
            using (var fragmentShaderStream = EmbeddedResourceHelper.Load(fragmentShaderPath))
            using (var vertexShaderReader = new StreamReader(vertexShaderStream))
            using (var fragmentShaderReader = new StreamReader(fragmentShaderStream))
            {
                // Read the given files.
                string vertexShader = vertexShaderReader.ReadToEnd();
                string fragmentShader = fragmentShaderReader.ReadToEnd();
                // Compile the new shader program.
                shaderProgram = webGLBase.InitializeShaders(vertexShader, fragmentShader);
            }
            // Dispose of the streams used to read the files.

            return shaderProgram;
        }
    }
}
