using System;
using WebAssembly;
using WebGLDotNET;
using Demo.DemoClasses;

namespace Demo
{
    /// <summary>
    /// Program is used to initialize how code will interface with page.
    /// </summary>
    public static class Program
    {
        #region Private Fields
        // Links to the javascript object in the browser.
        private static JSObject window;
        private static JSObject canvas;
        // This implements our "scene" behavior.
        private static CanvasEnvironment canvasBehavior;
        // It is required to expose as an action for cross language.
        private static Action<double> drawLoop = new Action<double>(DrawLoop);
        private static Action<object> onResize = new Action<object>(OnResize);
        private static double previousMilliseconds;
        #endregion
        #region Main
        /// <summary>
        /// Entrypoint that is called in index.html.
        /// </summary>
        private static void Main()
        {
            // Check to see if the browser supports WebGL2.
            if (!WebGL2RenderingContextBase.IsSupported)
            {
                HtmlHelper.AddParagraph("This browser does not support WebGL2!!!");
                return;
            }
            // Retrieve a reference to the window in javascript.
            // Get the window inner width and height.
            window = (JSObject)Runtime.GetGlobalObject();
            int width = (int)window.GetObjectProperty("innerWidth");
            int height = (int)window.GetObjectProperty("innerHeight");

            // Create the canvas element and tie it into our custom behavior.
            canvas = HtmlHelper.AddCanvas("demo", "demoCanvas", width, height);
            canvasBehavior = new CubesEnvironment(canvas);
            canvasBehavior.Start();
            // Hook into the browser event that is called
            // when the user rescales the window.
            window.Invoke("addEventListener", "resize", onResize);
            // Hooks into the javascript animation frame API.
            // This lets the browser drive the update cycle.
            RequestAnimationFrame();
        }
        #endregion
        #region Browser Size Change
        private static void OnResize(object eventArgs)
        {
            // Get the new inner dimensions of the window.
            int width = (int)window.GetObjectProperty("innerWidth");
            int height = (int)window.GetObjectProperty("innerHeight");
            // Apply these new dimensions to the canvas.
            canvas.SetObjectProperty("width", width);
            canvas.SetObjectProperty("height", height);
            // Update the behavior of this state change.
            canvasBehavior.Resize(width, height);
        }
        #endregion
        #region Browser Update Loop
        private static void DrawLoop(double milliseconds)
        {
            // Calculate the passed time.
            var elapsedMilliseconds = milliseconds - previousMilliseconds;
            previousMilliseconds = milliseconds;
            // Notify the canvas behavior to update and draw.
            canvasBehavior.Update((float)elapsedMilliseconds / 1000f);
            canvasBehavior.Draw();
            // Repeat cycle.
            RequestAnimationFrame();
        }
        private static void RequestAnimationFrame()
        {
            // Web API that gets the next moment a frame will be drawn.
            window.Invoke("requestAnimationFrame", drawLoop);
        }
        #endregion
    }
}
