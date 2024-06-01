#ifndef YAPCG_HELPER_INCLUDED
#define YAPCG_HELPER_INCLUDED
#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Macros.hlsl"

float cos_norm (float x)
{
    return cos(x) * 0.5 + 0.5;
}
float sin_norm (float x)
{
    return cos(x + PI) * 0.5 + 0.5;
}

#endif