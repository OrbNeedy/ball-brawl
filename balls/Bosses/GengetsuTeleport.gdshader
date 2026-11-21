shader_type canvas_item;

uniform float progress : hint_range(0.0, 1.0, 0.05) = 0;
uniform float strength = 0.25;
uniform float time = 0;
varying vec4 color;

void vertex() {
	// Called for every vertex the material is visible on.
	//float time = TIME * 1.0;
	color = COLOR;
}

void fragment() {
	// Called for every pixel the material is visible on.
	vec2 uv = UV;
	float displacement = sin((UV.y * 20.0) - time);
	uv.x += displacement * strength * progress;
	COLOR = texture(TEXTURE, uv) * color;
}

//void light() {
//	// Called for every pixel for every light affecting the CanvasItem.
//	// Uncomment to replace the default light processing function with this one.
//}
