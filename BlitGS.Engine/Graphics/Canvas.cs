using System;
using System.Collections.Concurrent;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using static bottlenoselabs.SDL;

namespace BlitGS.Engine;

public static unsafe class Canvas
{
    public static int Width { get; private set; }

    public static int Height { get; private set; }
    
    public static StretchMode StretchMode { get; private set; }

    internal static void Init(GameConfig config)
    {
        Width = config.CanvasWidth;
        Height = config.CanvasHeight;
        
        StretchMode = config.StretchMode;

        _canvasPixmap = new Pixmap(Width, Height);

        _targetPixmap = _canvasPixmap;

        _drawColor = ColorRGB.Black;

        _clipRectangle = new Rectangle(0, 0, Width, Height);
        
        const uint rendererFlags = (uint)(SDL_RendererFlags.SDL_RENDERER_ACCELERATED |
                                     SDL_RendererFlags.SDL_RENDERER_PRESENTVSYNC);
        
        _state.Renderer = SDL_CreateRenderer(Platform.WindowPtr, null, rendererFlags);

        _state.CanvasTexture = SDL_CreateTexture(
            _state.Renderer,
            (uint)SDL_PixelFormatEnum.SDL_PIXELFORMAT_ABGR8888,
            (int)SDL_TextureAccess.SDL_TEXTUREACCESS_STREAMING,
            Width,
            Height
        );

        var sdlRenderLogicalMode = StretchMode switch
        {
            StretchMode.Integer => SDL_RendererLogicalPresentation.SDL_LOGICAL_PRESENTATION_INTEGER_SCALE,
            StretchMode.Stretch => SDL_RendererLogicalPresentation.SDL_LOGICAL_PRESENTATION_STRETCH,
            StretchMode.LetterBox => SDL_RendererLogicalPresentation.SDL_LOGICAL_PRESENTATION_LETTERBOX,
            StretchMode.Overscan => SDL_RendererLogicalPresentation.SDL_LOGICAL_PRESENTATION_OVERSCAN,
            _ => SDL_RendererLogicalPresentation.SDL_LOGICAL_PRESENTATION_INTEGER_SCALE
        };

        _ = SDL_SetRenderLogicalPresentation(_state.Renderer, Width, Height,
            sdlRenderLogicalMode,
            SDL_ScaleMode.SDL_SCALEMODE_NEAREST);

        _ = SDL_SetTextureScaleMode(_state.CanvasTexture, SDL_ScaleMode.SDL_SCALEMODE_NEAREST);

    }

    internal static void Terminate()
    {
        SDL_DestroyTexture(_state.CanvasTexture);
        SDL_DestroyRenderer(_state.Renderer);
    }
    

    public static void BeginTarget(Pixmap target)
    {
        _targetPixmap = target;
        _clipRectangle = new Rectangle(0, 0, target.Width, target.Height);
    }

    public static void EndTarget()
    {
        _targetPixmap = _canvasPixmap;
        _clipRectangle = new Rectangle(0, 0, _canvasPixmap.Width, _canvasPixmap.Height);
    }

    public static void Color(uint color)
    {
        _drawColor = color;
    }

    public static void Clip(int x=0, int y=0, int w=0, int h=0)
    {
        _clipRectangle = new Rectangle(x, y, w, h);

        if (_clipRectangle.IsEmpty)
        {
            _clipRectangle = new Rectangle(0, 0, _targetPixmap.Width, _targetPixmap.Height);
        }

        if (_clipRectangle.Width > _targetPixmap.Width || _clipRectangle.Height > _targetPixmap.Height)
        {
            _clipRectangle = new Rectangle(0, 0, _targetPixmap.Width, _targetPixmap.Height);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void PutPixel(uint* buffer, int x, int y, uint c)
    {
        if (
            x >= _clipRectangle.Left && 
            x < _clipRectangle.Right && 
            y >= _clipRectangle.Top && 
            y < _clipRectangle.Bottom)
        {
            int idx = x + y * _targetPixmap.Width;
            *(buffer + idx) = c;
        }
        
    }

    public static void Pixel(int x, int y)
    {
        var c = _drawColor;
        
        fixed (uint* ptr = _targetPixmap.PixelBuffer)
        {
            PutPixel(ptr, x, y, c);
        }
    }

    public static ColorRGB Get(int x, int y)
    {
        fixed (uint* ptr = _targetPixmap.PixelBuffer)
        {
            int idx = x + y * _targetPixmap.Width;
            
            return *(ptr + idx);
        }
    }



    public static void FillRect(int x=0, int y=0, int w=0, int h=0)
    {
        if (w == 0 || h == 0)
        {
            x = 0;
            y = 0;
            w = Width;
            h = Height;
        }
        
        var c = _drawColor;
        fixed (uint* ptr = _targetPixmap.PixelBuffer)
        {
            for (var ry = y; ry < y + h; ++ry)
            {
                for (var rx = x; rx < x + w; ++rx)
                {
                    PutPixel(ptr, rx, ry, c);
                }
            }
        }
    }

    public static void Rect(int x, int y, int w, int h)
    {
        var c = _drawColor;
        fixed (uint* ptr = _targetPixmap.PixelBuffer)
        {
            HLine(ptr, x, x + w, y, c); // Top
            HLine(ptr, x, x + w, y + h, c); // Down
            VLine(ptr, y, y + h, x, c); // Left
            VLine(ptr, y, y + h, x + w, c); // Right
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void HLine(uint* ptr, int sx, int ex, int y, uint c)
    {
        for (int x = sx; x <= ex; ++x)
        {
            PutPixel(ptr, x, y, c);
        }
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void VLine(uint* ptr, int sy, int ey, int x, uint c)
    {
        for (int y = sy; y <= ey; ++y)
        {
            PutPixel(ptr, x, y, c);
        }
    }
    
    public static void Line(int x0, int y0, int x1, int y1)
    {
        int dx = Math.Abs(x1 - x0), sx = x0 < x1 ? 1 : -1;
        int dy = -Math.Abs(y1 - y0), sy = y0 < y1 ? 1 : -1;
        int err = dx + dy;

        var c = _drawColor;
        fixed (uint* ptr = _targetPixmap.PixelBuffer)
        {
            while (true)
            {
                PutPixel(ptr, x0, y0, c);
                if (x0 == x1 && y0 == y1) break;
                var e2 = 2 * err;
                if (e2 >= dy)
                {
                    err += dy;
                    x0 += sx;
                }

                if (e2 <= dx)
                {
                    err += dx;
                    y0 += sy;
                }
            }
        }
    }
    
    public static void Circle(int centerX, int centerY, int radius)
    {
        var c = _drawColor;
        fixed (uint* ptr = _targetPixmap.PixelBuffer)
        {
            if (radius > 0)
            {
                int x = -radius, y = 0, err = 2 - 2 * radius;
                do
                {
                    PutPixel(ptr,centerX - x, centerY + y, c);
                    PutPixel(ptr,centerX - y, centerY - x, c);
                    PutPixel(ptr,centerX + x, centerY - y, c);
                    PutPixel(ptr,centerX + y, centerY + x, c);

                    radius = err;
                    if (radius <= y) err += ++y * 2 + 1;
                    if (radius > x || err > y) err += ++x * 2 + 1;
                } while (x < 0);
            }
            else
            {
                PutPixel(ptr, centerX, centerY, c);
            }
        }
    }
    
    public static void FillCircle(int centerX, int centerY, int radius)
    {
        if (radius < 0 || centerX < -radius || centerY < -radius)
        {
            return;
        }

        var c = _drawColor;
        fixed (uint* ptr = _targetPixmap.PixelBuffer)
        {
            if (radius > 0)
            {
                int x0 = 0;
                int y0 = radius;
                int d = 3 - 2 * radius;

                while (y0 >= x0)
                {
                    HLine(ptr,centerX - y0, centerX + y0, centerY - x0, c);

                    if (x0 > 0)
                    {
                        HLine(ptr, centerX - y0, centerX + y0, centerY + x0, c);
                    }

                    if (d < 0)
                    {
                        d += 4 * x0++ + 6;
                    }
                    else
                    {
                        if (x0 != y0)
                        {
                            HLine(ptr,centerX - x0, centerX + x0, centerY - y0, c);
                            HLine(ptr,centerX - x0, centerX + x0, centerY + y0, c);
                        }

                        d += 4 * (x0++ - y0--) + 10;
                    }
                }
            }
            else
            {
                PutPixel(ptr, centerX, centerY, c);
            }
        }
    }

    public static void Triangle(int x1, int y1, int x2, int y2, int x3, int y3)
    {
        Line(x1, y1, x2, y2);
        Line(x2, y2, x3, y3);
        Line(x3, y3, x1, y1);
    }

    public static void Blit(Pixmap pixmap, int x, int y)
    {
        int x2 = x + pixmap.Width;
        int y2 = y + pixmap.Height;
        int pixW = pixmap.Width;
        
        fixed (uint* ptr = _targetPixmap.PixelBuffer)
        fixed (uint* srcPtr = pixmap.PixelBuffer)    
        {
            for (int px = x; px < x2; ++px)
            {
                for (int py = y; py < y2; ++py)
                {
                    int srcIdx = (px - x) + (py - y) * pixW;
                    uint c = *(srcPtr + srcIdx);

                    if (c == ColorRGB.Empty)
                    {
                        continue;
                    }
                    
                    PutPixel(ptr, px, py, c);
                }
            }
        }
    }

    public static void BlitEx(
        Pixmap pixmap,
        int x, int y,
        Rectangle region = default,
        int width = -1,
        int height = -1,
        bool flip = false
    )
    {
        if (region.IsEmpty)
        {
            region = new Rectangle(0, 0, pixmap.Width, pixmap.Height);
        }

        if (width < 0 || height < 0)
        {
            width = region.Width;
            height = region.Height;
        }

        int x2 = x + width;
        int y2 = y + height;

        int pw = pixmap.Width;
        
        fixed (uint* ptr = _targetPixmap.PixelBuffer)
        fixed (uint* srcPtr = pixmap.PixelBuffer)
        {
            float factorW = (float)width / region.Width;
            float factorH = (float)height / region.Height;

            if (!flip)
            {
                for (int px = x; px < x2; ++px)
                {
                    for (int py = y; py < y2; ++py)
                    {
                        int srcIdx = (region.X + (int)((px - x) / factorW)) +
                                      (region.Y + (int)((py - y) / factorH)) * pixmap.Width;

                        uint c = *(srcPtr + srcIdx);

                        if (c == ColorRGB.Empty)
                        {
                            continue;
                        }
                        
                        PutPixel(ptr, px, py, c);
                        
                    }
                }
            }
            else
            {
                int startingPixel = region.Right - 1;
                
                for (int px = x; px < x2; ++px)
                {
                    for (int py = y; py < y2; ++py)
                    {
                        int srcIdx = (startingPixel - (int)((px - x) / factorW)) +
                                     (region.Top + (int)((py - y) / factorH)) * pixmap.Width;
            
                        uint c = *(srcPtr + srcIdx);

                        if (c == ColorRGB.Empty)
                        {
                            continue;
                        }
                        
                        PutPixel(ptr, px, py, c);
                    }
                }
                
            }
        }
    }

    public static void Text(Font font, int x, int y, ReadOnlySpan<char> text)
    {
        var offset = new Point(0, 0);

        for (int i = 0; i < text.Length; ++i)
        {
            var c = text[i];

            switch (c)
            {
                case '\r':
                    continue;
                case '\n':
                    offset.X = 0;
                    offset.Y += font.GlyphHeight + font.LineSpacing;
                    continue;
                default:
                {
                    ref var glyphRect = ref font.GetGlyphRegion(c);
            
                    BlitEx(font, x + offset.X, y + offset.Y, glyphRect, glyphRect.Width, glyphRect.Height);
                    break;
                }
            }
            
            offset.X += font.GlyphSpacing;
        }
    }

    #region :::::::::::::::::::::::::::: EFFECTS ::::::::::::::::::::::::::::::::::::::

    public enum PixelColorOp
    {
        Set,
        Add,
        Mult
    }

    public enum ValueFilterChannel
    {
        All,
        Red,
        Green,
        Blue
    }

    public static void ColorFilter(float value, PixelColorOp op, ValueFilterChannel channel = ValueFilterChannel.All)
    {
        fixed (uint* ptr = _targetPixmap.PixelBuffer)
        {
            int px = _clipRectangle.Left;
            int px2 = _clipRectangle.Right;
            int py = _clipRectangle.Top;
            int py2 = _clipRectangle.Bottom;
            
            int columns = px2 - px;
            int lines = py2 - py;
            int line = 0;
            
            int index = px + py * Width;

            switch (op)         
            {
                case PixelColorOp.Set:
                    while (line < lines)
                    {
                        LineValueSet(ptr + index, columns, value, channel);
                        index += Width;
                        ++line;
                    }
                    break;
                case PixelColorOp.Add:
                    while (line < lines)
                    {
                        LineValueAdd(ptr + index, columns, value, channel);
                        index += Width;
                        ++line;
                    }
                    break;
                case PixelColorOp.Mult:
                    while (line < lines)
                    {
                        LineValueMult(ptr + index, columns, value, channel);
                        index += Width;
                        ++line;
                    }
                    break;
            }
        }
    }

    public static void ColorFilter(ColorRGB color, PixelColorOp op)
    {
        fixed (uint* ptr = _targetPixmap.PixelBuffer)
        {
            int px = _clipRectangle.Left;
            int px2 = _clipRectangle.Right;
            int py = _clipRectangle.Top;
            int py2 = _clipRectangle.Bottom;
            
            int columns = px2 - px;
            int lines = py2 - py;
            int line = 0;
            
            int index = px + py * Width;

            switch (op)         
            {
                case PixelColorOp.Set:
                    while (line < lines)
                    {
                        LineColorSet(ptr + index, columns, ref color);
                        index += Width;
                        ++line;
                    }
                    break;
                case PixelColorOp.Add:
                    while (line < lines)
                    {
                        LineColorAdd(ptr + index, columns, ref color);
                        index += Width;
                        ++line;
                    }
                    break;
                case PixelColorOp.Mult:
                    while (line < lines)
                    {
                        LineColorMult(ptr + index, columns, ref color);
                        index += Width;
                        ++line;
                    }
                    break;
            }
        }
    }

    internal static void ConvertWindowCoordinatesToCanvas(float x, float y, out float canvasX, out float canvasY)
    {
        fixed (float* outX = &canvasX)
        fixed (float* outY = &canvasY)    
        {
            _ = SDL_RenderCoordinatesFromWindow(_state.Renderer, x, y, outX, outY);
        }
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void LineColorSet(uint* linePtr, int length, ref ColorRGB color)
    {
        for (int i = 0; i < length; ++i)
        {
            *(linePtr + i) = color;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void LineColorAdd(uint* linePtr, int length, ref ColorRGB color)
    {
        var r = color.R;
        var g = color.G;
        var b = color.B;
        
        for (int i = 0; i < length; ++i)
        {
            var srcCol = *(linePtr + i);
            
            var srcR = ColorRGB.ExtractR(srcCol);
            var srcG = ColorRGB.ExtractG(srcCol);
            var srcB = ColorRGB.ExtractB(srcCol);

            var addR = Math.Min(srcR + r, 255);
            var addG = Math.Min(srcG + g, 255);
            var addB = Math.Min(srcB + b, 255);
            
            *(linePtr + i) = ColorRGB.Build(addR, addG, addB);
        }
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void LineColorMult(uint* linePtr, int length, ref ColorRGB color)
    {
        var r = color.R / 255f;
        var g = color.G / 255f;
        var b = color.B / 255f;
        
        for (int i = 0; i < length; ++i)
        {
            var srcCol = *(linePtr + i);

            var srcR = ColorRGB.ExtractR(srcCol) / 255f;
            var srcG = ColorRGB.ExtractG(srcCol) / 255f;
            var srcB = ColorRGB.ExtractB(srcCol) / 255f;

            var mR = Math.Min(srcR * r, 1.0f);
            var mG = Math.Min(srcG * g, 1.0f);
            var mB = Math.Min(srcB * b, 1.0f);

            *(linePtr + i) = ColorRGB.Build(mR, mG, mB);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void LineValueSet(uint* linePtr, int length, float value, ValueFilterChannel channel)
    {
        uint* pix;
        byte currentC1;
        byte currentC2;
        
        var colValue = (byte)(MathUtils.Clamp(value, 0f, 1f) * 255);
        
        switch (channel)
        {
            case ValueFilterChannel.All:

                for (int i = 0; i < length; ++i)
                {
                    *(linePtr + i) = ColorRGB.Build(colValue, colValue, colValue);
                }
                
                break;
            case ValueFilterChannel.Red:

                for (int i = 0; i < length; ++i)
                {
                    pix = linePtr + i;
                    currentC1 = ColorRGB.ExtractG(*pix);
                    currentC2 = ColorRGB.ExtractB(*pix);
                    *(pix) = ColorRGB.Build(colValue, currentC1, currentC2);
                }
                
                break;
            case ValueFilterChannel.Green:
                
                for (int i = 0; i < length; ++i)
                {
                    pix = linePtr + i;
                    currentC1 = ColorRGB.ExtractR(*pix);
                    currentC2 = ColorRGB.ExtractB(*pix);
                    *(pix) = ColorRGB.Build(currentC1, colValue, currentC2);
                }
                
                break;
            case ValueFilterChannel.Blue:
                
                for (int i = 0; i < length; ++i)
                {
                    pix = linePtr + i;
                    currentC1 = ColorRGB.ExtractR(*pix);
                    currentC2 = ColorRGB.ExtractG(*pix);
                    *(pix) = ColorRGB.Build(currentC1, currentC2, colValue);
                }
                
                break;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void LineValueAdd(uint* linePtr, int length, float value, ValueFilterChannel channel)
    {
        switch (channel)
        {
            case ValueFilterChannel.All:
                
                for (int i = 0; i < length; ++i)
                {
                    var srcCol = *(linePtr + i);

                    var srcR = ColorRGB.ExtractR(srcCol) / 255f;
                    var srcG = ColorRGB.ExtractG(srcCol) / 255f;
                    var srcB = ColorRGB.ExtractB(srcCol) / 255f;

                    var mR = MathUtils.Clamp(srcR + value, 0f, 1f);
                    var mG = MathUtils.Clamp(srcG + value, 0f, 1f);
                    var mB = MathUtils.Clamp(srcB + value, 0f, 1f);

                    *(linePtr + i) = ColorRGB.Build(mR, mG, mB);
                }
                
                break;
            case ValueFilterChannel.Red:
                
                for (int i = 0; i < length; ++i)
                {
                    var srcCol = *(linePtr + i);

                    var srcR = ColorRGB.ExtractR(srcCol) / 255f;
                    var srcG = ColorRGB.ExtractG(srcCol) / 255f;
                    var srcB = ColorRGB.ExtractB(srcCol) / 255f;

                    var mR = MathUtils.Clamp(srcR + value, 0f, 1f);

                    *(linePtr + i) = ColorRGB.Build(mR, srcG, srcB);
                }
                break;
            case ValueFilterChannel.Green:
                
                for (int i = 0; i < length; ++i)
                {
                    var srcCol = *(linePtr + i);

                    var srcR = ColorRGB.ExtractR(srcCol) / 255f;
                    var srcG = ColorRGB.ExtractG(srcCol) / 255f;
                    var srcB = ColorRGB.ExtractB(srcCol) / 255f;

                    var mG = MathUtils.Clamp(srcG + value, 0f, 1f);

                    *(linePtr + i) = ColorRGB.Build(srcR, mG, srcB);
                }
                break;
            case ValueFilterChannel.Blue:
                
                for (int i = 0; i < length; ++i)
                {
                    var srcCol = *(linePtr + i);

                    var srcR = ColorRGB.ExtractR(srcCol) / 255f;
                    var srcG = ColorRGB.ExtractG(srcCol) / 255f;
                    var srcB = ColorRGB.ExtractB(srcCol) / 255f;

                    var mB = MathUtils.Clamp(srcB + value, 0f, 1f);

                    *(linePtr + i) = ColorRGB.Build(srcR, srcG, mB);
                }
                
                break;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void LineValueMult(uint* linePtr, int length, float value, ValueFilterChannel channel)
    {
        switch (channel)
        {
            case ValueFilterChannel.All:
                
                for (int i = 0; i < length; ++i)
                {
                    var srcCol = *(linePtr + i);

                    var srcR = ColorRGB.ExtractR(srcCol) / 255f;
                    var srcG = ColorRGB.ExtractG(srcCol) / 255f;
                    var srcB = ColorRGB.ExtractB(srcCol) / 255f;

                    var mR = MathUtils.Clamp(srcR * value, 0f, 1f);
                    var mG = MathUtils.Clamp(srcG * value, 0f, 1f);
                    var mB = MathUtils.Clamp(srcB * value, 0f, 1f);

                    *(linePtr + i) = ColorRGB.Build(mR, mG, mB);
                }
                
                break;
            case ValueFilterChannel.Red:
                
                for (int i = 0; i < length; ++i)
                {
                    var srcCol = *(linePtr + i);

                    var srcR = ColorRGB.ExtractR(srcCol) / 255f;
                    var srcG = ColorRGB.ExtractG(srcCol) / 255f;
                    var srcB = ColorRGB.ExtractB(srcCol) / 255f;

                    var mR = MathUtils.Clamp(srcR * value, 0f, 1f);

                    *(linePtr + i) = ColorRGB.Build(mR, srcG, srcB);
                }
                
                break;
            case ValueFilterChannel.Green:
                
                for (int i = 0; i < length; ++i)
                {
                    var srcCol = *(linePtr + i);

                    var srcR = ColorRGB.ExtractR(srcCol) / 255f;
                    var srcG = ColorRGB.ExtractG(srcCol) / 255f;
                    var srcB = ColorRGB.ExtractB(srcCol) / 255f;

                    var mG = MathUtils.Clamp(srcG * value, 0f, 1f);

                    *(linePtr + i) = ColorRGB.Build(srcR, mG, srcB);
                }
                
                break;
            case ValueFilterChannel.Blue:
                
                for (int i = 0; i < length; ++i)
                {
                    var srcCol = *(linePtr + i);

                    var srcR = ColorRGB.ExtractR(srcCol) / 255f;
                    var srcG = ColorRGB.ExtractG(srcCol) / 255f;
                    var srcB = ColorRGB.ExtractB(srcCol) / 255f;

                    var mB = MathUtils.Clamp(srcB * value, 0f, 1f);

                    *(linePtr + i) = ColorRGB.Build(srcR, srcG, mB);
                }
                
                break;
        }
    }

    #endregion

    public static void TakeScreenshot(string file)
    {
        var renderer = _state.Renderer;
    
        SDL_Rect viewport;

        _ = SDL_GetRenderViewport(renderer, &viewport);
    
        var screenSurface = SDL_CreateSurface(viewport.w, viewport.h, (uint)SDL_PixelFormatEnum.SDL_PIXELFORMAT_ABGR8888);
    
        if (screenSurface == null)
        {
            Console.WriteLine($"Canvas::TakeScreenshot: Could not create surface: {SDL_GetError()}");
            return;
        }
    
        if (SDL_RenderReadPixels(renderer, null, screenSurface->format->format, screenSurface->pixels, screenSurface->pitch) != 0)
        {
            Console.WriteLine($"Canvas::TakeScreenshot: Could not read pixels from Renderer: {SDL_GetError()}");
            SDL_free(screenSurface);
            return;
        }
        
        using var outputStream = File.OpenWrite(file);
        
        ImageWriter.Save(screenSurface->pixels, screenSurface->w, screenSurface->h, outputStream);
        
        SDL_free(screenSurface);
    }

    internal static void Flip()
    {
        fixed (void* ptr = _canvasPixmap.PixelBuffer)
        {
            _ = SDL_UpdateTexture(_state.CanvasTexture, null, ptr, _canvasPixmap.Pitch);
        }
        
        _ = SDL_RenderTexture(_state.Renderer, _state.CanvasTexture, null, null);
        _ = SDL_RenderPresent(_state.Renderer);
    }

    
    private static Pixmap _canvasPixmap = null!;
    private static Pixmap _targetPixmap = null!;

    private static uint _drawColor;
    
    private struct State
    {
        public SDL_Renderer* Renderer;
        public SDL_Texture* CanvasTexture;
    }

    private static State _state;
    private static Rectangle _clipRectangle;
}