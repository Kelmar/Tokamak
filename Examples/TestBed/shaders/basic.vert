#version 450

// Basic lit vertex shader

uniform mat4 projection;
uniform mat4 view;
uniform mat4 model;
uniform float gamma;
uniform vec3 camera;

layout(location = 0) in vec3 Point;
layout(location = 1) in vec3 Normal;
layout(location = 2) in vec4 Color;
layout(location = 3) in vec2 TexCoord;

layout(location = 0) out vec4 fsin_Color;
layout(location = 1) out vec2 fsin_TexCoord;
layout(location = 2) out vec3 fsin_Normal;
layout(location = 3) out vec3 fsin_Position;
layout(location = 4) out float fsin_Gamma;
layout(location = 5) out vec3 fsin_CameraPosition;

void main()
{
    //gl_Position = projection * view * model * vec4(Point, 1.0);
    gl_Position = vec4(Point, 1.0) * model * view * projection;

    // Get the position in world space.
    fsin_Position = vec3(model * vec4(Point, 1.0));

    fsin_Normal = Normal * mat3(transpose(inverse(model)));

    fsin_Color = Color;
    fsin_TexCoord = TexCoord;

    fsin_Gamma = gamma;
    fsin_CameraPosition = camera;
}
