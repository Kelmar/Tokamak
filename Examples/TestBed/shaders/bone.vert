#version 450

// Basic lit vertex shader with bones

// Scene parameters
uniform mat4 projection;
uniform mat4 view;

// Object transforms
uniform mat4 model;
uniform mat4 bones[128];

// Mesh input parameters
layout(location = 0) in vec3 Point;
layout(location = 1) in vec3 Normal;
layout(location = 2) in vec4 Color;
layout(location = 3) in vec2 TexCoord;

layout(location = 4) in int BoneIndices[4];
layout(location = 5) in float BoneWeights[4];

// Material parameters (pass through)
layout(location = 0) out vec4 fsin_Color;

// Mesh parameters to fragment (transformed output)
layout(location = 1) out vec2 fsin_TexCoord;
layout(location = 2) out vec3 fsin_Normal;
layout(location = 3) out vec3 fsin_Position;

void main()
{
    mat4 m = model *
        bones[BoneIndices[0]] * BoneWeights[0] +
        bones[BoneIndices[1]] * BoneWeights[1] +
        bones[BoneIndices[2]] * BoneWeights[2] +
        bones[BoneIndices[3]] * BoneWeights[3];

    //gl_Position = view * projection * model * vec4(Point, 1.0);
    //gl_Position = vec4(Point, 1.0) * model * projection * view;
    gl_Position = vec4(Point, 1.0) * m * projection * view;

    // Get the position in world space.
    fsin_Position = vec3(model * vec4(Point, 1.0));

    fsin_Normal = Normal * mat3(transpose(inverse(model)));

    fsin_Color = Color;
    fsin_TexCoord = TexCoord;
}
