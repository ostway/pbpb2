using System;
using System.Drawing;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using SnLib;
using Win32;

namespace pbpb
{
	public class ____qwejqjiqojwe {}

    partial class Form1
    {

        public const int MAX_NOLASTGOOD_FOR_INPUT = ( 1000 * 60 ) / 2;

        void PubgStatusProc() {        
            
            int threadwait = 2000;          
            PubgStatuses ps = PubgStatuses.None;
            bool pubg_window_visibled = false;        
            bool lobbyinputswitcher = false;
            bool unknowassisted = false;

            Log.Add("(MAIN) StatusProc enter!");

            while (!BotStopper.WaitOne( threadwait, false )) {

                try {

                    threadwait = ( User32.IsWindowVisible( (IntPtr) User32.FindWindow( null, ViewFormTitle ) ) > 0 ) ?
                        100 : 2000;

                    if (!PubgWindow.Exists) {

                        Log.Add( "(PS) No way :( Wait..." );
                        continue;
                    }

                    PubgWindow.HideBE();

                    if (PubgInput.IsInputEvent && !PubgInput.CanInteract) {

                        Log.Add( "(PS) No idle time for actions. Wait..." );
                        goto EXIT;
                    }

                    pubg_window_visibled = PubgWindow.IsWindowVisible;
                    if (!pubg_window_visibled) {

                        PubgWindow.Show();
                    }

                    if (Setti.HiddenMode)
                        Thread.Sleep( 500 );

                    if (PubgWindow.NeedSetupWindow) {

                        bool res = PubgWindow.SetupWindow();

                        Log.Add( String.Format( "(PW) Setup {1} => {0} result: {2}", PubgWindow.PartFullHD, PubgWindow.Handle, res.ToYesNoString() ) );
                    }

                    ps = PubgStatus.Now( Pcs );

                    Log.Add( String.Format( "(PS) {0}", ps ) );

                    if (AppIsExp) goto EXIT;  //Magic EXP!

                    if (PubgStatuses.Unknown.HasFlags( ps )) {

                        Log.Append( " di: " + PubgStatus.LastDistance.ToString() );

                        if (Environment.TickCount - PubgStatus.LastGoodTime > MAX_NOLASTGOOD_FOR_INPUT) {

                            if (!unknowassisted) {

                                Log.Add( "(PS) Add input (lastgood is long)" );

                                PubgInput.ClickCenter();
                                PubgInput.ChangeViewPerson();

                                //if (RND.Next( 2 ) == 0) PubgInput.MoveMouse( 100, 100 ); else PubgInput.MoveMouse( -200, -200 ); 
                                
                                unknowassisted = true;
                            }
                            else {

                                unknowassisted = false;
                                Thread.Sleep(10000);
                            }                         
                        }
                    }

                    else if (PubgStatuses.MatchCanContinue.HasFlags( ps )) {

                        Log.Append( " di: " + Pcs[PubgControls.btnMatchCanContinue].LastDistance.ToString() );

                        int lc = Pcs[PubgControls.btnMatchCanContinue].LastClickedTick;

                        if (Environment.TickCount - lc < 45000) {

                            Log.Add("click Cancel");
                            Pcs[PubgControls.btnMatchCanContinueCancel].ClickLeftMouse();
                        }
                        else {

                            Log.Add("click Continue");
                            Pcs[PubgControls.btnMatchCanContinue].ClickLeftMouse();
                        }

                        Thread.Sleep( 2000 );
                    }

                    else if (PubgStatuses.Lobby.HasFlags( ps )) {

                        Log.Append( " di: " + Pcs[PubgControls.btnStart].LastDistance.ToString() );

                        //Thread.Sleep( 7000 );

                        PubgRound.End( !PubgRound.RewardSaved && Setti.SaveReward, "" );

                        PubgInput.EjectClickedTime = int.MaxValue;

                        PubgInput.ParachuteClickedTime = int.MaxValue;

                        if (Environment.TickCount - Pcs[PubgControls.btnStart].LastClickedTick > 30000) {

                            bool inputswitched = false;
                            if (PubgInput.IsInputMessage && PubgInput.CanInteract) {

                                if (!lobbyinputswitcher) {

                                    inputswitched = true;
                                    InitInput_event();
                                }

                                //lobbyinputswitcher = !lobbyinputswitcher;
                            }

                            Log.Add( "(PS) click Start" );

                            Pcs[PubgControls.btnStart].ClickLeftMouse();

                            if (inputswitched) InitInput_message();
                        }
                    }

                    else if (PubgStatuses.ExitToLobby.HasFlags( ps )) {

                        Log.Append( " di: " + Pcs[PubgControls.btnExit].LastDistance.ToString() );

                        bool needsavereward = !PubgRound.RewardSaved && Setti.SaveReward;

                        if (needsavereward) {

                            Thread.Sleep( 6000 );
                            PubgRound.End( needsavereward, "ExitToLobby" );
                        }

                        if (PubgInput.IsInputMessage && !PubgInput.CanInteract) {

                            Log.Add( "Can't exit (PassiveMode: no idle time)" );

                            goto EXIT;
                        }

                        bool inputswitched = false;

                        if (PubgInput.IsInputMessage) {

                            PubgInput.KeyPress( Keys.Escape );

                            inputswitched = true;

                            InitInput_event();

                        }
                        else {

                            Pcs[PubgControls.btnExit].ClickLeftMouse();

                            Log.Add( "click ExitToLobby" );
                        }

                        Thread.Sleep( 3000 );

                        Pcs[PubgControls.btnConfirmExit].ClickLeftMouse();

                        Log.Add( "click ExitToLobby confirm" );

                        if (inputswitched) InitInput_message();

                    }

                    else if (PubgStatuses.Eject.HasFlags( ps )) {

                        Log.Append( " di: " + Pcs[PubgControls.labEject].LastDistance.ToString() );

                        Thread.Sleep(RND.Next(6, 15) * 1000);

                        PubgInput.Eject();
                        PubgInput.Back();
                    }

                    else if (PubgStatuses.Parachute.HasFlags( ps )) {

                        Log.Append( " di: " + Pcs[PubgControls.labReleaseParachute].LastDistance.ToString() );

                        PubgInput.Parachute();
                        PubgInput.Back();
                    }

                    else if (PubgStatuses.Prepare.HasFlags( ps )) {

                        Log.Append( " di: " + Pcs[PubgControls.labJoined].LastDistance.ToString() );

                        PubgRound.Set();
                    }

                    else if (PubgStatuses.Water.HasFlags( ps )) {

                        Log.Append( " di: " + Pcs[PubgControls.labWater].LastDistance.ToString() );

                        if (!PubgRound.WaterAssisted && PubgInput.CanInteract) {

                            Log.Add( "(PS) Water Assist" );

                            PubgInput.AssistInWater();

                            PubgRound.WaterAssisted = true;
                        }
                    }

                    else if (PubgStatuses.Alive.HasFlags( ps )) {

                        Log.Append( " di: " + Pcs[PubgControls.labAlive].LastDistance.ToString() );

                        PubgRound.Set();

                        int cd = 3*(1000*60) + 30000;
                        if (( Environment.TickCount - PubgInput.EjectClickedTime > cd ||
                              Environment.TickCount - PubgInput.ParachuteClickedTime > cd )) {

                            PubgInput.EjectClickedTime = int.MaxValue;
                            PubgInput.ParachuteClickedTime = int.MaxValue;

                            if (PubgRound.WaterAssisted == false) {

                                PubgInput.Down();
                                PubgInput.Back();
                            }
                        }

                        if (Environment.TickCount - PubgRound.StartedTime > Setti.MaxRoundTimeRnd) {

                            Log.Add( "Try Exit (MaxRoundTime)" );

                            PubgInput.KeyPress( Keys.Escape );
                        }
                    }

                    else if (PubgStatuses.MainManuExit.HasFlags( ps )) {

                        Log.Append( " di: " + Pcs[PubgControls.labMainManu].LastDistance.ToString() );

                        if (Environment.TickCount - PubgRound.StartedTime > Setti.MaxRoundTime) {

                            Log.Add( "Try Exit (MainMenu + MaxRoundTime)" );

                            bool inputswitched = false;

                            if (PubgInput.GetType() == typeof( _PubgInputMessage )) {

                                inputswitched = true;

                                InitInput_event();
                            }

                            if (PubgInput.CanInteract) Pcs[PubgControls.btnConfirmExit].ClickLeftMouse();

                            if (inputswitched) InitInput_message();
                        }

                    }

                    EXIT:;
                    if ( pubg_window_visibled && Setti.HiddenMode ) PubgWindow.Hide();
                }
                catch (Exception e) {

                    Log.Add( "(PS) exception: " + e.Message );
                }
            }

            Log.Add("(MAIN) StatusProc leave!");
        }
    }
}