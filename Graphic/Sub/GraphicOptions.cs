using MiMFa.Service;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiMFa.Graphic
{
    [Serializable]
    public class GraphicOptions
    {
        public bool BrightSwitch { get; set; } = true;
        public bool ColorSwitch { get;  set; } = true;
        public bool FlipHorizontalSwitch { get;  set; } = true;
        public bool FlipVerticalSwitch { get;  set; } = true;
        public bool RotateSwitch { get;  set; } = true;
        public bool ScaleSwitch { get;  set; } = true;
        public bool PositionSwitch { get;  set; } = true;

        public float ScaleX { get; set; } = 1;
        public float ScaleY { get; set; } = 1;
        public float ScaleZ { get; set; } = 1;

        public float PositionX { get; set; } = 0;
        public float PositionY { get; set; } = 0;
        public float PositionZ { get; set; } = 0;

        public float AngleVolume { get; set; } = 0;
        public int ContrastVolume { get; set; } = 0;
        public int GrayscaleVolume { get; set; } = 0;
        public int LightVolume { get; set; } = 0;
        public int BrightVolume { get; set; } = 0;
        public int RedVolume { get; set; } = 0;
        public int GreenVolume { get; set; } = 0;
        public int BlueVolume { get; set; } = 0;
        public int OpacityVolume { get; set; } = 0;

        public GraphicOptions() { }
        public GraphicOptions(GraphicOptions baseOption)
        {
            BrightSwitch = baseOption.BrightSwitch;
            ColorSwitch = baseOption.ColorSwitch;
            FlipHorizontalSwitch = baseOption.FlipHorizontalSwitch;
            FlipVerticalSwitch = baseOption.FlipVerticalSwitch;
            RotateSwitch = baseOption.RotateSwitch;
            ScaleSwitch = baseOption.ScaleSwitch;
            PositionSwitch = baseOption.PositionSwitch;
            ScaleX = baseOption.ScaleX;
            ScaleY = baseOption.ScaleY;
            ScaleZ = baseOption.ScaleZ;
            PositionX = baseOption.PositionX;
            PositionY = baseOption.PositionY;
            PositionZ = baseOption.PositionZ;
            AngleVolume = baseOption.AngleVolume;
            ContrastVolume = baseOption.ContrastVolume;
            GrayscaleVolume = baseOption.GrayscaleVolume;
            LightVolume = baseOption.LightVolume;
            BrightVolume = baseOption.BrightVolume;
            RedVolume = baseOption.RedVolume;
            GreenVolume = baseOption.GreenVolume;
            BlueVolume = baseOption.BlueVolume;
            OpacityVolume = baseOption.OpacityVolume;
        }

        public virtual void Reset()
        {
            BrightSwitch = true;
            ColorSwitch = true;
            FlipHorizontalSwitch = true;
            FlipVerticalSwitch = true;
            RotateSwitch = true;
            ScaleSwitch = true;
            PositionSwitch = true;
            ScaleX = 1;
            ScaleY = 1;
            ScaleZ = 1;
            PositionX = 0;
            PositionY = 0;
            PositionZ = 0;
            AngleVolume = 0;
            ContrastVolume = 0;
            GrayscaleVolume = 0;
            LightVolume = 0;
            BrightVolume = 0;
            RedVolume = 0;
            GreenVolume = 0;
            BlueVolume = 0;
            OpacityVolume = 0;
        }

    }
}
