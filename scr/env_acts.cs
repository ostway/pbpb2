using System;
using System.Collections.Generic;
using System.Drawing;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using SnLib;
using Win32;

namespace pbpb {

    

    partial class Form1
    {
        public const int WM_ACTIVATEAPP = User32.WM_USER + 0x0001;

        public const int WM_SCRUPDATE = User32.WM_USER + 0x0002;

        protected override void WndProc( ref Message m )
        {

            if (m.Msg == WM_ACTIVATEAPP) {

                Show(); WindowState = FormWindowState.Normal;
            } 
            else if (m.Msg == WM_SCRUPDATE) {

                if (Visible && Setti.DrawScr && User32.FindWindow(null, Form1.ViewFormTitle) == 0)
                    try {

                        PanelView.BackgroundImage = PubgStatus.RawScr;
                    }
                    catch { }
            }

            base.WndProc( ref m );
        }

        bool CheckAppIsDup() {

            int result = User32.FindWindow( null, Form1.AppTitle );

            if (result > 0) User32.SendMessage( (IntPtr) result, Form1.WM_ACTIVATEAPP, 0, 0 );

            return ( result > 0 );
        }

        
        void Init_HotKeysMon() {

            Task.Run( () => {

                bool IsPres( Keys key ) => ( User32.GetAsyncKeyState( (int) key ) < 0 );         
                
                while (IsHandleCreated) {

                    if (IsPres(Keys.Pause)) {
                        bool launched = !BotStopper.WaitOne( 0, false );
                        string msg = "Bot " + (launched ? "stopped" : "launched");
                        if (launched) {
                            StopBotClick();
                            PubgWindow.CloseMsg(); PubgWindow.KillExecute(); PubgWindow.HideBE();
                            Log.Add( msg + " [user key]" );                        
                        }
                        else {
                            StartBotClick();
                            Log.Add( msg + " [user key]" ); 
                        }
                        tray.BalloonTipText = msg;
                        tray.ShowBalloonTip( 2000 );
                        Thread.Sleep( 3000 );
                    }
                    Thread.Sleep(10);
                }
                
            } );
        }

        // ?
        void test1(bool fromfile) {

            Bitmap scr;

            if (fromfile)
                 scr = new Bitmap(full_scr_filename);
            else
                 scr = SGraph.Scr("", PubgWindow.Width, PubgWindow.Height, PubgWindow.PosX, PubgWindow.PosY);

            PubgControl pc = Pcs[PubgControls.btnMatchCanContinueCancel];
            pc.ControlImageFromImage(scr);          
            int dist = pc.CalcDistance(true);

            Log.Add( String.Format("calc: {0} , cmp: {1}{2}=> dist: {3}", 
                pc.ControlImageHash, pc.ComparableHash, Environment.NewLine, dist) );

            scr.Save( filename_now );
            scr.Dispose();
            pc.ControlImage.Save( filename_now + ".bmp" );

        }
   }
}