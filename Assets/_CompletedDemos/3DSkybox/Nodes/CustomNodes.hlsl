
#ifndef CUSTOMNODES_INCLUDED
#define CUSTOMNODES_INCLUDED

void LightDirection_half(out half3 direction) {
    #if SHADERGRAPH_PREVIEW
    direction = half3(0,1,0);
    # else
    Light light = GetMainLight();
    direction = light.direction;
    #endif
}

void FindColor_float(float4 waterDeep, float4 waterShallow, float4 landLow, float4 landHigh, float waterHeight, float value, out float4 color) {
    float4 waterColor = lerp(waterDeep, waterShallow, saturate(value/waterHeight));
    float4 landColor = lerp(landLow, landHigh, saturate((value-waterHeight)/(1-waterHeight)));
    float isWater = step(value, waterHeight);
    color = waterColor * isWater+ landColor * (1-isWater);
}

#endif