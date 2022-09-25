using System;
using System.IO;
using System.Runtime.InteropServices;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Sirius.Engine;
using UnityEngine;
using Screen = Sirius.Engine.Screen;

public class VirtualScreen : Singleton<VirtualScreen>
{
    public const int Width = Screen.Width;
    public const int Height = Screen.Height;

    private byte[] _buffer;
    private int _bufferSize;
    private int _palletNum;
    private IntPtr _pLineBuffer;

    public VirtualScreen()
    {
    }
    
    ~VirtualScreen()
    {
        Marshal.FreeHGlobal(_pLineBuffer);
    }

    public void Initialize()
    {
        _palletNum = Screen.Instance.PalletNum;
        _bufferSize = Width * Height;
        _buffer = new byte[Width * Height];
        _pLineBuffer = Marshal.AllocHGlobal(Width);
    }

    public void SetPixels(
        int posX, int posY, int width, int height, byte[] buffer,
        bool alpha, byte add, int clipX, int clipY, int clipWidth, int clipHeight,
        bool additive)
    {
        if (posX < 0) return;
        if (posX > Width) return;
        if (posX + width > Width) return;
        if (posY + height < 0) return;
        if (posY > Height) return;

        byte[] copyBuffer = new byte[buffer.Length];
        var pCopy = Marshal.AllocCoTaskMem(buffer.Length);
        Marshal.Copy(buffer, 0, pCopy, buffer.Length);
        Marshal.Copy(pCopy, copyBuffer, 0, buffer.Length);
        Marshal.FreeCoTaskMem(pCopy);
        
        if (alpha)
        {
            for (var i = 0; i < copyBuffer.Length; i++)
            {
                var x = posX + i % width;
                var y = posY + i / width;
                var index = x + y * Width;

                if (y < 0 || y >= Height)
                {
                    continue;
                }
                
                if (copyBuffer[i] == 255)
                {
                    copyBuffer[i] = _buffer[index];
                    continue;
                }

                if (add > 0)
                {
                    copyBuffer[i] += add;
                    if (copyBuffer[i] > 2)
                    {
                        copyBuffer[i] = 2;
                    }
                }
                    
                if (x <= clipX ||
                    x >= clipX + clipWidth ||
                    y <= clipY ||
                    y >= clipY + clipHeight)
                {
                    copyBuffer[i] = _buffer[index];
                    continue;
                }

                if (additive)
                {
                    copyBuffer[i] += _buffer[index];
                    if (copyBuffer[i] > 2)
                    {
                        copyBuffer[i] = 2;
                    }
                }
            }
        }

        if (height + posY > Height)
        {
            var diff = height + posY - Height;
            height -= diff;
        }

        var yStart = 0;
        if (posY - clipY < 0)
        {
            yStart = -posY + clipY;
        }

        if (yStart < 0)
        {
            yStart = 0;
        }

        var yEnd = height;
        if (clipHeight > 0 && height > clipHeight)
        {
            yEnd = clipHeight + yStart;
        }
        for (var y = yStart; y < yEnd; y++)
        {
            Marshal.Copy(copyBuffer, y * width, _pLineBuffer, width);
            var index = posX + (posY + y) * Width;
            if (index < 0) continue;
            Marshal.Copy(_pLineBuffer, _buffer, index, width);
        }
    }

    public void SetPixel(int x, int y, byte color)
    {
        var index = x + y * Width;
        if (index < 0 ||
            index >= _bufferSize)
        {
            return;
        }

        _buffer[index] = color;
    }
    
    public void AddPixel(int x, int y, byte color)
    {
        var index = x + y * Width;
        if (index < 0 ||
            index >= _bufferSize)
        {
            return;
        }

        _buffer[index] += color;
        if (_buffer[index] >= 2)
        {
            _buffer[index] = 2;
        }
    }

    public void Draw()
    {
        ScreenHandler.SetPixels(_buffer);
    }
}