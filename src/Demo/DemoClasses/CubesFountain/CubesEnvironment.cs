using System;
using WaveEngine.Common.Math;
using WebAssembly;
using WebGLDotNET;

namespace Demo.DemoClasses
{
    /// <summary>
    /// An environment with raining cubes.
    /// </summary>
    public sealed partial class CubesEnvironment : CanvasEnvironment
    {
        // Store the indices of triangle trios.
        ushort[] indices;
        // Streams of data to be accessed in linear order
        // by the graphics device.
        WebGLBuffer vertexBuffer;
        WebGLBuffer indexBuffer;
        WebGLBuffer colorBuffer;
        // Stores accessors to variables in compiled
        // shader programs.
        WebGLUniformLocation pMatrixUniform;
        WebGLUniformLocation vMatrixUniform;
        WebGLUniformLocation wMatrixUniform;
        // Defines the operations applied to draw elements in the viewport.
        Matrix projectionMatrix;
        Matrix viewMatrix;
        Matrix worldMatrix;

        public CubesEnvironment(JSObject canvas) : base(canvas) { }

        public override void Start()
        {
            float[] vertices = new float[]
            {
                -1, -1, -1,
                 1, -1, -1,
                 1,  1, -1,

                -1,  1, -1,
                -1, -1,  1,
                 1, -1,  1,

                 1,  1,  1,
                -1,  1,  1,
                -1, -1, -1,

                -1,  1, -1,
                -1,  1,  1,
                -1, -1,  1,

                 1, -1, -1,
                 1,  1, -1,
                 1,  1,  1,

                 1, -1,  1,
                -1, -1, -1,
                -1, -1,  1,

                 1, -1,  1,
                 1, -1, -1,
                -1,  1, -1,

                -1,  1,  1,
                 1,  1,  1,
                 1,  1, -1
            };
            vertexBuffer = WebGL.CreateArrayBuffer(vertices);

            indices = new ushort[]
            {
                 0,  1,  2,
                 0,  2,  3,

                 4,  5,  6,
                 4,  6,  7,

                 8,  9, 10,
                 8, 10, 11,

                12, 13, 14,
                12, 14, 15,

                16, 17, 18,
                16, 18, 19,

                20, 21, 22,
                20, 22, 23
            };
            indexBuffer = WebGL.CreateElementArrayBuffer(indices);

            float[] colors = new float[]
            {
                1, 0, 0,
                1, 0, 0,
                1, 0, 0,
                1, 0, 0,

                0, 1, 0,
                0, 1, 0,
                0, 1, 0,
                0, 1, 0,

                0, 0, 1,
                0, 0, 1,
                0, 0, 1,
                0, 0, 1,

                1, 1, 0,
                1, 1, 0,
                1, 1, 0,
                1, 1, 0,

                0, 1, 1,
                0, 1, 1,
                0, 1, 1,
                0, 1, 1,

                1, 1, 1,
                1, 1, 1,
                1, 1, 1,
                1, 1, 1
            };
            colorBuffer = WebGL.CreateArrayBuffer(colors);

            // Ask WebGL to compile our shaders that will run
            // on the graphics device.
            var shaderProgram =
                WebGL.InitializeShaders(VERTEX_SHADER_CODE, FRAGMENT_SHADER_CODE);

            // Hey WebGL, for that program we just compiled, could you give
            // me accessors to modify some of the variables within?
            pMatrixUniform = WebGL.GetUniformLocation(shaderProgram, "pMatrix");
            vMatrixUniform = WebGL.GetUniformLocation(shaderProgram, "vMatrix");
            wMatrixUniform = WebGL.GetUniformLocation(shaderProgram, "wMatrix");

            // TODO still not 100% sure what all of this is doing.
            WebGL.BindBuffer(WebGLRenderingContextBase.ARRAY_BUFFER, vertexBuffer);
            var positionAttribute = (uint)WebGL.GetAttribLocation(shaderProgram, "position");
            WebGL.VertexAttribPointer(positionAttribute, 3, WebGLRenderingContextBase.FLOAT, false, 0, 0);
            WebGL.EnableVertexAttribArray(positionAttribute);

            WebGL.BindBuffer(WebGLRenderingContextBase.ARRAY_BUFFER, colorBuffer);
            var colorAttribute = (uint)WebGL.GetAttribLocation(shaderProgram, "color");
            WebGL.VertexAttribPointer(colorAttribute, 3, WebGLRenderingContextBase.FLOAT, false, 0, 0);
            WebGL.EnableVertexAttribArray(colorAttribute);

            WebGL.BindBuffer(WebGLRenderingContextBase.ELEMENT_ARRAY_BUFFER, indexBuffer);

            worldMatrix = Matrix.Identity;
        }

        public override void Update(float deltaTime)
        {
            float aspectRatio = Width / Height;

            // These values correspond to a camera frustum.
            projectionMatrix = Matrix.CreatePerspectiveFieldOfView(
                (float)Math.PI / 4, aspectRatio, 0.1f, 1000f);

            viewMatrix = Matrix.CreateLookAt(Vector3.UnitZ * 10, Vector3.Zero, Vector3.Up);

            // Spin the world (not the cube).
            Quaternion rotation = Quaternion.CreateFromYawPitchRoll(
                deltaTime * 2,
                deltaTime * 4,
                deltaTime * 3);
            worldMatrix *= Matrix.CreateFromQuaternion(rotation);
        }

        public override void Draw()
        {
            base.Draw();

            // Hey WebGL, can you update this data in the shader program?
            WebGL.UniformMatrix4fv(pMatrixUniform, false, projectionMatrix.ToArray());
            WebGL.UniformMatrix4fv(vMatrixUniform, false, viewMatrix.ToArray());
            WebGL.UniformMatrix4fv(wMatrixUniform, false, worldMatrix.ToArray());

            // Hey WebGL, draw me some
            WebGL.DrawElements(
                // triangles
                WebGLRenderingContextBase.TRIANGLES,
                // with the vertex indices (all of them)
                indices.Length,
                // and this is the memory size of each vert.
                WebGLRenderingContextBase.UNSIGNED_SHORT,
                // You can start at the beginning.
                0
            );
        }
    }
}
