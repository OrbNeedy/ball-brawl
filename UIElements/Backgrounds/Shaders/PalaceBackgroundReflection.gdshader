shader_type canvas_item;

uniform sampler2D Target;
uniform vec2 Offset = vec2(0);
uniform vec2 Center = vec2(0.5);
uniform float Scale : hint_range(0.0, 2.0) = 1.2;

void vertex() {
	// Called for every vertex the material is visible on.
}

void fragment() {
	//textureSize(Target, 1.0)
	vec2 uv = SCREEN_UV;
	
	uv -= Center;
	uv /= Scale;
	uv += Center;
	
	if (COLOR.a > 0.0)
		COLOR = texture(Target, uv + Offset);
	COLOR.a *= 0.4;
}

//void light() {
//	// Called for every pixel for every light affecting the CanvasItem.
//	// Uncomment to replace the default light processing function with this one.
//}
