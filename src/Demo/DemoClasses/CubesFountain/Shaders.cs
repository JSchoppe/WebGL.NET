namespace Demo.DemoClasses
{
    public sealed partial class CubesEnvironment
    {
        private const string VERTEX_SHADER_CODE = @"
attribute vec3 position;
attribute vec3 color;

uniform mat4 pMatrix;
uniform mat4 vMatrix;
uniform mat4 wMatrix;

varying vec3 vColor;

void main(void)
{
    gl_Position = pMatrix * vMatrix * wMatrix * vec4(position, 1.0);
    vColor = color;
}
";
        private const string FRAGMENT_SHADER_CODE = @"
precision mediump float;

varying vec3 vColor;

void main(void)
{
    gl_FragColor = vec4(vColor, 1.0);
}
";
    }
}
