shader_type canvas_item;

uniform sampler2D SCREEN_TEXTURE : hint_screen_texture;
uniform float blur_radius : hint_range(0, 32);
uniform vec2 resolution;

void fragment() {
    vec2 uv = SCREEN_UV;
    vec3 color = vec3(0.0);
    int blur_amount = int(blur_radius);
    for (int i = -blur_amount; i <= blur_amount; ++i) {
        for (int j = -blur_amount; j <= blur_amount; ++j) {
            vec2 offset = vec2(float(i), float(j)) / resolution;
            color += texture(SCREEN_TEXTURE, uv + offset).rgb;
        }
    }
    color /= (2.0 * blur_radius + 1.0) * (2.0 * blur_radius + 1.0);
    COLOR.rgb = color;
}