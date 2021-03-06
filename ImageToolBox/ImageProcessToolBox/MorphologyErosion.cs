using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageProcessToolBox
{
    class MorphologyErosion : FilterTemplate, IImageProcess
    {
        private Bitmap _SourceImage;

        public MorphologyErosion()
        {
        }

        public MorphologyErosion(Bitmap bitmap)
        {
            _SourceImage = bitmap;
        }

        public Bitmap Process()
        {
            return base.convolute(_SourceImage, 3, 3);
        }

        private static Bitmap erosion(Bitmap bitmap)
        {
            int width = bitmap.Width, height = bitmap.Height;
            int w = 3, h = 3;
            Bitmap dstBitmap = new Bitmap(bitmap);

            byte[,] pix = ImageExtract.getimageArray(bitmap);
            byte[,] resPix = new byte[3, width * height];

            for (int y = 1; y < (height - 1); y++)
            {
                for (int x = 1; x < (width - 1); x++)
                {
                    //b,g,r 
                    for (int c = 0; c < 3; c++)
                    {
                        //mask
                        int current = x + y * width;
                        byte[] mask = new byte[w * h];
                        for (int my = 0; my < h; my++)
                            for (int mx = 0; mx < w; mx++)
                            {
                                int pos = current + (mx - 1) + ((my - 1) * width);
                                mask[mx + my * w] = pix[c, pos];
                            }

                        resPix[c, current] = erosionMask33(mask);
                    }
                }
            }

            ImageExtract.writeImageByArray(resPix, dstBitmap);
            return dstBitmap;
        }

        private static byte erosionMask33(byte[] gate)
        {
            bool Is = true;  //假設其符合條件
            for (int i = 0; i < 9; i++)
                if (gate[i] != 255)
                    Is = false;    //如果不是黑色則將其填白

            return (byte)((Is) ? 255 : 0);
        }
        private static byte erosionMask55(byte[] gate)   //try 5*5
        {
            bool Is = true;  //假設其符合條件
            for (int i = 0; i < 25; i++)
                if (gate[i] != 255)
                    Is = false;    //如果不是黑色則將其填白

            return (byte)((Is) ? 255 : 0);
        }

        protected override byte maskFilter(byte[] gate)
        {
            return erosionMask33(gate);
        }


        public void setResouceImage(Bitmap bitmap)
        {
            _SourceImage = bitmap;
        }
    }
}
