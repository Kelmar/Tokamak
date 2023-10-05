namespace Graphite.OGL
{
    public static class Shaders
    {
        public const string Vertex = @"#version 330 core

uniform mat4 projection;

layout (location = 0) in vec2 location;

void main(void)
{
    gl_Position = vec4(location, 0, 1) * projection;
}
";

        public const string Fragment = @"#version 330 core

uniform vec4 inColor;

out vec4 outputColor;

void main()
{
    outputColor = inColor;
}
";
    }
}
