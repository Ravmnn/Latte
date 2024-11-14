#version 330 core


uniform sampler2D tex;
uniform vec4 clipArea;
uniform vec2 resolution;


void main()
{
    vec2 normPos = gl_FragCoord.xy / resolution.xy;
    
    if (gl_FragCoord.x >= clipArea.x && gl_FragCoord.x <= clipArea.x + clipArea.z && gl_FragCoord.y >= clipArea.y && gl_FragCoord.y <= clipArea.y + clipArea.w)
        gl_FragColor = texture(tex, normPos.xy);
    else
        discard;
}