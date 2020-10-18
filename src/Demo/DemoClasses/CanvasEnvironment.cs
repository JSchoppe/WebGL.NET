using System.Numerics;
using WebAssembly;
using WebGLDotNET;
using SkiaSharp;

namespace Demo.DemoClasses
{
    /// <summary>
    /// Defines behavior for a WebGL driven canvas element.
    /// </summary>
    public abstract class CanvasEnvironment
    {
        #region Fields
        /// <summary>
        /// Contains the methods related to web graphics library.
        /// </summary>
        protected readonly WebGLRenderingContextBase WebGL;
        private Vector4 clearColor;
        #endregion
        #region Constructor
        public CanvasEnvironment(JSObject canvas)
        {
            // Define the WebGL attributes.
            // TODO do more research into what this specifies.
            WebGLContextAttributes attributes =
                new WebGLContextAttributes
                {
                    Stencil = true
                };
            // Create a new context using the page canvas and attributes.
            WebGL = new WebGL2RenderingContext(canvas, attributes);
            // Initialize width and height from the canvas element.
            Width = (int)canvas.GetObjectProperty("clientWidth");
            Height = (int)canvas.GetObjectProperty("clientWidth");
            // Define a default clear color.
            ClearColor = new SKColor(0, 0, 0, 255);
        }
        #endregion
        #region Properties
        /// <summary>
        /// The width of the canvas in pixels.
        /// </summary>
        protected int Width { get; private set; }
        /// <summary>
        /// The height of the canvas in pixels.
        /// </summary>
        protected int Height { get; private set; }
        /// <summary>
        /// The color used to clear the canvas between draws.
        /// </summary>
        protected SKColor ClearColor
        {
            set
            {
                clearColor = new Vector4(
                    value.Red / 255f,
                    value.Green / 255f,
                    value.Blue / 255f,
                    value.Alpha / 255f
                );
            }
        }
        #endregion
        #region Methods
        /// <summary>
        /// This method should implement initial states of WebGL objects.
        /// </summary>
        public abstract void Start();
        /// <summary>
        /// This should update the WebGL objects relative to time.
        /// </summary>
        /// <param name="deltaTime">The change in time.</param>
        public abstract void Update(float deltaTime);
        /// <summary>
        /// This should handle the draw calls for the WebGL objects.
        /// </summary>
        public virtual void Draw()
        {
            // Turn on the depth testing.
            // TODO: do more research on exactly what this does.
            WebGL.Enable(WebGLRenderingContextBase.DEPTH_TEST);
            // Set the viewport to be the entire screen.
            WebGL.Viewport(0, 0, Width, Height);
            // Fill the canvas with the clear color.
            WebGL.ClearColor(clearColor.X, clearColor.Y, clearColor.Z, clearColor.W);
            WebGL.Clear(WebGLRenderingContextBase.COLOR_BUFFER_BIT);
        }
        /// <summary>
        /// Override this to define behavior change when the canvas is resized.
        /// </summary>
        /// <param name="newWidth">The new pixel width of the canvas.</param>
        /// <param name="newHeight">The new pixel height of the canvas.</param>
        public virtual void Resize(int newWidth, int newHeight)
        {
            Width = newWidth;
            Height = newHeight;
        }
        #endregion
    }
}
