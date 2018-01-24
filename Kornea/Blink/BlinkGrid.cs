using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
 

namespace Kornea.Blink
{
    public enum BlinkMode
    {
        ParentFill,
        CenteredSs,
        CenteredSc2,
        LowNotifDef,
        FlashBlinkNotifDef,
        VerticalCenteredMessage
    }
    public partial class BlinkGrid : Grid 
    {
    
        private Renderer renderer = null;
        private BlinkMode _bm;
        private Control _pcl;
        private DrawingContext _dc;
        private Size _size;


        public void PrepareFrame(Renderer r)
        {
            renderer = r;
        }


      


        public void Transit_In()
        {
            switch (_bm)
            {
                case BlinkMode.ParentFill:

                    break;
                case BlinkMode.CenteredSs:
                    Width = _size.Width;
                    Height = _size.Height;
                    RenderTransformOrigin = new Point(((_pcl.Width - Width) / 2), ((_pcl.Height - Height) / 2));
                    break;
                case BlinkMode.CenteredSc2:

                    break;
                case BlinkMode.LowNotifDef:
                    break;
                case BlinkMode.FlashBlinkNotifDef:
                    break;


            }
            this.FadeIn();
        }

        public void Transit_Out()
        {
            this.FadeOut();

        }

        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            InvalidateVisual();
        }

    

        protected override void OnRender(DrawingContext dc)
        
        {
            base.OnRender(dc);
            base.OnRender(dc);
            if (renderer != null) { renderer.Draw(dc); return; }
            
            
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }


    }
}
