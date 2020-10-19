using System;
using WaveEngine.Common.Math;
using WebAssembly;
using WebGLDotNET;
using glTFLoader.Schema;
using Demo.Helpers;
using glTFLoader;
using Demo.DemoClasses.Loading;
using System.IO;

namespace Demo.DemoClasses
{
    /// <summary>
    /// An environment with raining cubes.
    /// </summary>
    public sealed partial class CubesEnvironment : CanvasEnvironment
    {
        // Streams of data to be accessed in linear order
        // by the graphics device.
        WebGLBuffer[] vertexBuffers;
        WebGLBuffer indexBuffer;


        int indexBufferCount;
        uint shaderNormalAttribute;
        uint shaderPositionAttribute;
        WebGLUniformLocation worldViewProjectionUniformLocation;
        Matrix worldViewProjectionMatrix;


        public CubesEnvironment(JSObject canvas) : base(canvas) { }

        public override void Start()
        {
            // Ask WebGL to compile our shaders that will run
            // on the graphics device.
            WebGLProgram shaderProgram = AssetsIO.LoadShader(WebGL,
                "VertexShader.essl", "FragmentShader.essl");
            // Ask WebGL to gives us accessors for the variables
            // in our compiled shader.
            shaderNormalAttribute = (uint)WebGL.GetAttribLocation(shaderProgram, "in_var_NORMAL");
            shaderPositionAttribute = (uint)WebGL.GetAttribLocation(shaderProgram, "in_var_POSITION");
            worldViewProjectionUniformLocation = WebGL.GetUniformLocation(shaderProgram, "worldViewProj");

            LoadGltf("testCube.glb", out Gltf model, out byte[][] buffers);

            LoadMesh(model, out BufferView indicesBufferView, out BufferView[] attributesBufferView);

            indexBufferCount = indicesBufferView.ByteLength / sizeof(ushort);
            indexBuffer = WebGL.CreateBuffer();
            WebGL.BindBuffer(WebGLRenderingContextBase.ELEMENT_ARRAY_BUFFER, indexBuffer);
            var indexBufferView = buffers[indicesBufferView.Buffer];
            var indices = new byte[indicesBufferView.ByteLength];
            Array.Copy(indexBufferView, indicesBufferView.ByteOffset, indices, 0, indicesBufferView.ByteLength);
            WebGL.BufferData(
                WebGLRenderingContextBase.ELEMENT_ARRAY_BUFFER,
                indices,
                WebGLRenderingContextBase.STATIC_DRAW);

            var vertexBufferCount = attributesBufferView.Length;
            vertexBuffers = new WebGLBuffer[vertexBufferCount];

            for (var i = 0; i < vertexBufferCount; i++)
            {
                var vertexBufferView = attributesBufferView[i];
                var buffer = WebGL.CreateBuffer();
                WebGL.BindBuffer(WebGLRenderingContextBase.ARRAY_BUFFER, buffer);
                var verticesBufferView = buffers[vertexBufferView.Buffer];
                var vertices = new byte[vertexBufferView.ByteLength];
                Array.Copy(
                    verticesBufferView,
                    vertexBufferView.ByteOffset,
                    vertices,
                    0,
                    vertexBufferView.ByteLength);
                WebGL.BufferData(
                    WebGLRenderingContextBase.ARRAY_BUFFER,
                    vertices,
                    WebGLRenderingContextBase.STATIC_DRAW);
                vertexBuffers[i] = buffer;
            }
        }

        private float angle = 0f;
        private Matrix viewProjectionMatrix;

        public override void Update(float deltaTime)
        {
            var viewMatrix = Matrix.CreateLookAt(new Vector3(0, 0, 3), new Vector3(0, 0, 0), Vector3.UnitY);
            var projectionMatrix = Matrix.CreatePerspectiveFieldOfView(
                MathHelper.PiOver4,
                (float)Width / Height,
                0.1f, 100f);
            viewProjectionMatrix = Matrix.Multiply(viewMatrix, projectionMatrix);

            angle += deltaTime;
            var offsetQuaternion = Quaternion.CreateFromAxisAngle(Vector3.Right, MathHelper.PiOver2);
            var rotationQuaternion = Quaternion.CreateFromAxisAngle(Vector3.Forward, angle);
            var worldMatrix = Matrix.CreateFromQuaternion(offsetQuaternion * rotationQuaternion);
            worldViewProjectionMatrix = worldMatrix * viewProjectionMatrix;
        }

        public override void Draw()
        {
            base.Draw();

            WebGL.BindBuffer(WebGLRenderingContextBase.ELEMENT_ARRAY_BUFFER, indexBuffer);

            // Normals
            WebGL.BindBuffer(WebGLRenderingContextBase.ARRAY_BUFFER, vertexBuffers[0]);
            WebGL.EnableVertexAttribArray(shaderNormalAttribute);
            WebGL.VertexAttribPointer(shaderNormalAttribute, 3, WebGLRenderingContextBase.FLOAT, false, 12, 0);

            // Positions
            WebGL.BindBuffer(WebGLRenderingContextBase.ARRAY_BUFFER, vertexBuffers[1]);
            WebGL.EnableVertexAttribArray(shaderPositionAttribute);
            WebGL.VertexAttribPointer(shaderPositionAttribute, 3, WebGLRenderingContextBase.FLOAT, false, 12, 0);

            WebGL.UniformMatrix4fv(worldViewProjectionUniformLocation, false, worldViewProjectionMatrix.ToArray());

            WebGL.DrawElements(
                WebGLRenderingContextBase.TRIANGLES,
                indexBufferCount,
                WebGLRenderingContextBase.UNSIGNED_SHORT,
                0);
        }






        private void LoadMesh(Gltf model, out BufferView indicesBufferView, out BufferView[] attributesBufferView)
        {
            var mesh = model.Meshes[0];
            indicesBufferView = null;
            attributesBufferView = null;

            for (var i = 0; i < mesh.Primitives.Length; i++)
            {
                var primitive = mesh.Primitives[i];

                if (primitive.Indices.HasValue)
                {
                    indicesBufferView = ReadAccessor(model, primitive.Indices.Value);
                }

                var attributesCount = primitive.Attributes.Values.Count;
                attributesBufferView = new BufferView[attributesCount];
                var insertIndex = 0;

                foreach (var attribute in primitive.Attributes)
                {
                    attributesBufferView[insertIndex++] = ReadAccessor(model, attribute.Value);
                }
            }
        }

        private void LoadGltf(string filename, out Gltf model, out byte[][] buffers)
        {
            using (var modelStream = EmbeddedResourceHelper.Load(filename))
            {
                model = Interface.LoadModel(modelStream);
            }

            var buffersLength = model.Buffers.Length;
            buffers = new byte[buffersLength][];

            for (var i = 0; i < buffersLength; i++)
            {
                byte[] bufferBytes;

                using (Stream modelStream = EmbeddedResourceHelper.Load(filename))
                {
                    bufferBytes = Interface.LoadBinaryBuffer(modelStream);
                }

                buffers[i] = bufferBytes;
            }
        }

        private BufferView ReadAccessor(Gltf model, int index)
        {
            var accessor = model.Accessors[index];

            if (!accessor.BufferView.HasValue)
            {
                return null;
            }

            return model.BufferViews[accessor.BufferView.Value];
        }

    }
}
