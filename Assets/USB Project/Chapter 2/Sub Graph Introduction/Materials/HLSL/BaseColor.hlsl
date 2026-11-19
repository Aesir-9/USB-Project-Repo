void SetColor_half(in bool AddColor, in half3 BaseColor, in half3 Color, out half3 Out)
{
    if (AddColor) Out = saturate(BaseColor + Color); 
    else Out =  BaseColor * Color;
}