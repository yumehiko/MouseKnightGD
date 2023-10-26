shader_type canvas_item;
uniform sampler2D SCREEN_TEXTURE : hint_screen_texture, filter_linear_mipmap;

// Radius that the shader was designed for.
const float DEFAULT_RADIUS = 10.000000;

// Unit vector: (1, 0) or (0, 1)
uniform vec2 step;
// Desired blur radius.
uniform float radius = 10.000000;

void fragment() {
	vec2 s = radius / DEFAULT_RADIUS * step / vec2(textureSize(SCREEN_TEXTURE, 0));
	COLOR.rgb =
		0.011194727 * texture(SCREEN_TEXTURE, SCREEN_UV - 10.000000000 * s).rgb +
		0.039368696 * texture(SCREEN_TEXTURE, SCREEN_UV - 8.415809477 * s).rgb +
		0.071308558 * texture(SCREEN_TEXTURE, SCREEN_UV - 6.435363708 * s).rgb +
		0.110237219 * texture(SCREEN_TEXTURE, SCREEN_UV - 4.455121108 * s).rgb +
		0.145451038 * texture(SCREEN_TEXTURE, SCREEN_UV - 2.475020813 * s).rgb +
		0.163798995 * texture(SCREEN_TEXTURE, SCREEN_UV - 0.495000167 * s).rgb +
		0.157439298 * texture(SCREEN_TEXTURE, SCREEN_UV + 1.485004498 * s).rgb +
		0.129158204 * texture(SCREEN_TEXTURE, SCREEN_UV + 3.465057055 * s).rgb +
		0.090434685 * texture(SCREEN_TEXTURE, SCREEN_UV + 5.445220765 * s).rgb +
		0.054043977 * texture(SCREEN_TEXTURE, SCREEN_UV + 7.425557483 * s).rgb +
		0.027564604 * texture(SCREEN_TEXTURE, SCREEN_UV + 9.406126897 * s).rgb;
	COLOR.a = 1.0;
}
