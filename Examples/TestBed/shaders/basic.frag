#version 450

// Basic lit fragment shader.

uniform int is8Bit;
uniform sampler2D texture0;

// Scene parameters
uniform vec3 lightColor;
uniform vec3 lightPosition;
uniform float gamma;
uniform vec3 camera;

// Material parameters
layout(location = 0) in vec4 fsin_Color;

// Mesh parameters
layout(location = 1) in vec2 fsin_TexCoord;
layout(location = 2) in vec3 fsin_Normal;
layout(location = 3) in vec3 fsin_Position;

// Fragment output
layout(location = 0) out vec4 fsout_Color;

void main()
{
    float ambientStrength = 0.1;
    float specularStrength = 0.5;

    vec4 tx = texture(texture0, fsin_TexCoord);
    vec4 clr = is8Bit != 0 ? vec4(fsin_Color.rgb, fsin_Color.a * tx.r) : tx * fsin_Color;

    vec4 ambient = vec4(lightColor * ambientStrength, 1);

    vec3 normal = normalize(fsin_Normal);
    vec3 lightDirection = normalize(lightPosition - fsin_Position);

    vec3 viewDirection = normalize(camera - fsin_Position);
    vec3 reflectDirection = reflect(-lightDirection, normal);

    float spec = pow(max(dot(viewDirection, reflectDirection), 0.0), 32);
    vec4 specular = vec4(specularStrength * spec * lightColor, 1);

    float diff = max(dot(normal, lightDirection), 0.0);
    vec4 diffuse = vec4(diff * lightColor, 1);

    vec4 result = (ambient + diffuse + specular) * clr;

    result.r = pow(result.r, gamma);
    result.g = pow(result.g, gamma);
    result.b = pow(result.b, gamma);

    fsout_Color = result;
}
