#version 330 core


uniform sampler2D texture;
uniform vec4 clipArea;


void main()
{
    vec2 pos = gl_FragCoord.xy;
    
    if (pos.x >= clipArea.x && pos.x <= clipArea.x + clipArea.z && pos.y >= clipArea.y && pos.y <= clipArea.y + clipArea.w)
        gl_FragColor = texture2D(texture, gl_TexCoord[0].xy);
    else
        discard; // Descarte pixels fora da área de recorte
}