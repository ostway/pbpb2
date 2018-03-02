﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using SnLib;

namespace pbpb
{
    public partial class FormPBPBView : Form
    {
        public static string NativeClassName;
        _PubgInputMessage pi;

        protected override void WndProc( ref Message m )
        {
            if (m.Msg == Form1.WM_SCRUPDATE) {

                if (Visible) {

                    BackgroundImage.Dispose();
                    BackgroundImage = null;

                    if (PubgStatus.RawScr != null)
                        BackgroundImage = new Bitmap(PubgStatus.RawScr);

                }
            }

            base.WndProc( ref m );
        }

        public FormPBPBView()
        {
            InitializeComponent();

            Text = Form1.ViewFormTitle;

            pi = new _PubgInputMessage { AsPostMessage = true };
        }

        private void FormView_KeyUp( object sender, KeyEventArgs e )
        {
            pi.KeyDownOrUp(e.KeyCode, true);          
        }

        private void FormView_KeyDown( object sender, KeyEventArgs e )
        {
            pi.KeyDownOrUp(e.KeyCode, false);
        }

        private void FormPBPBView_MouseClick( object sender, MouseEventArgs e )
        {
            if (e.Button == MouseButtons.Left) pi.ClickLeftMouse(1, 1);
        }
    }
}
