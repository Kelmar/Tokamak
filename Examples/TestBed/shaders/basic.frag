#version 450

// Basic lit fragment shader.

layout(location = 0) in vec4 fsin_Color;
layout(location = 1) in vec2 fsin_TexCoord;
layout(location = 2) in vec3 fsin_Normal;
layout(location = 3) in vec3 fsin_Position;
layout(location = 4) in float fsin_Gamma;
layout(location = 5) in vec3 fsin_CameraPosition;

layout(location = 0) out vec4 fsout_Color;

uniform int is8Bit;
uniform vec3 lightColor;
uniform vec3 lightPosition;
uniform sampler2D texture0;

void main()
{
    float ambientStrength = 0.1;
    float specularStrength = 0.5;

    vec4 tx = texture(texture0, fsin_TexCoord);
    vec4 clr = is8Bit != 0 ? vec4(fsin_Color.rgb, fsin_Color.a * tx.r) : tx * fsin_Color;

    vec4 ambient = vec4(lightColor * ambientStrength, 1);

    vec3 normal = normalize(fsin_Normal);
    vec3 lightDirection = normalize(lightPosition - fsin_Position);

    vec3 viewDirection = normalize(fsin_CameraPosition - fsin_Position);
    vec3 reflectDirection = reflect(-lightDirection, normal);

    float spec = pow(max(dot(viewDirection, reflectDirection), 0.0), 32);
    vec4 specular = vec4(specularStrength * spec * lightColor, 1);

    float diff = max(dot(normal, lightDirection), 0.0);
    vec4 diffuse = vec4(diff * lightColor, 1);

    vec4 result = (ambient + diffuse + specular) * clr;

    result.r = pow(result.r, fsin_Gamma);
    result.g = pow(result.g, fsin_Gamma);
    result.b = pow(result.b, fsin_Gamma);

    fsout_Color = result;
}
