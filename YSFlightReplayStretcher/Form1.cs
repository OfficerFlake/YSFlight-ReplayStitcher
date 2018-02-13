using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Text.RegularExpressions;

namespace YSFlightReplayStretcher
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void ShowDialog_R1(object sender, EventArgs e)
        {
            if (FileBrowser.ShowDialog() == DialogResult.OK)
            {
                First_FileNameHolder.Text = FileBrowser.FileName;
            }
        }

        private void ShowDialog_R2(object sender, EventArgs e)
        {
            if (FileBrowser.ShowDialog() == DialogResult.OK)
            {
                Second_FileNameHolder.Text = FileBrowser.FileName;
            }
        }

        private void Process_Click(object sender, EventArgs e)
        {
            if(!(File.Exists(First_FileNameHolder.Text)))
            {
                ProcessText.Text = "Error: First YSF Filename Invalid/Missing.";
                ProcessText.ForeColor = System.Drawing.Color.Red;
                return;
            }
            if (!(File.Exists(Second_FileNameHolder.Text)))
            {
                ProcessText.Text = "Error: Second YSF Filename Invalid/Missing.";
                ProcessText.ForeColor = System.Drawing.Color.Red;
                return;
            }
            First_FileNameHolder.ReadOnly = true;
            Second_FileNameHolder.ReadOnly = true;
            First_BrowseButton.Enabled = false;
            Second_BrowseButton.Enabled = false;
            Process.Text = "Busy...";
            Process.Enabled = false;
            ProcessText.Text = "Processing Started.";
            ProcessText.ForeColor = System.Drawing.Color.Gold;
            Replay_Processor(First_FileNameHolder.Text, Second_FileNameHolder.Text);
        }

        public void Replay_Processor(string First_FileName, string Second_FileName)
        {
            string ErrorCode;
            Replay Replay1 = new Replay();
            Replay Replay2 = new Replay();
            Replay1 = LoadReplay(First_FileName, out ErrorCode);
            //MessageBox.Show(Replay1.Field);
            Replay2 = LoadReplay(Second_FileName, out ErrorCode);
            //MessageBox.Show(Replay2.Field);

            Replay ReplayMerged = MergeReplay(Replay1, Replay2, out ErrorCode);
            //MessageBox.Show(ReplayMerged.Field);

            string _Path = Path.GetDirectoryName(First_FileName);
            string _F1 = Path.GetFileNameWithoutExtension(First_FileName);
            string _F2 = Path.GetFileNameWithoutExtension(Second_FileName);
            SaveReplay(ReplayMerged, _Path + "\\" + _F1 + "_" + _F2 +"_Merged.YFS");

            ProcessText.Text = "Done!";
            ProcessText.ForeColor = System.Drawing.Color.Green;
            Progress.Value = 100;

            First_FileNameHolder.ReadOnly = false;
            First_BrowseButton.Enabled = true;

            Second_FileNameHolder.ReadOnly = false;
            Second_BrowseButton.Enabled = true;

            //File.WriteAllLines(First_FileNameHolder.Text.Remove(First_FileNameHolder.Text.Length - 4) + "_Processed.yfs", null);
            Process.Text = "Process";
            Process.Enabled = true;

            return;
        }

        #region Aircraft
        public class AircraftRecord
        {
            public float TimeStamp; //0,0

            public float PosX; //1,0
            public float PosY; //1,1
            public float PosZ; //1,2
            public float HdgX; //1,3
            public float HdgY; //1,4
            public float HdgZ; //1,5
            public float LoadFactor; //1,6

            public byte AirState; //2,0
            public byte Anim_VGW; //2,1
            public byte Anim_Boards; //2,2
            public byte Anim_Gear; //2,3
            public byte Anim_Flap; //2,4
            public byte Anim_Brake; //2,5
            public byte Anim_Smoke; //2,6
            public byte Unknown_1; //2,7
            public byte Anim_Flags; //2,8
            public byte Strength; //2,9
            public byte Anim_Throttle; //2,10
            public sbyte Anim_Elevator; //2,11
            public sbyte Anim_Aileron; //2,12
            public sbyte Anim_Rudder; //2,13
            public sbyte Anim_Trim; //2,14
            public byte Anim_ThrustVector; //2,15
            public byte Anim_ThrustReverse; //2,16
            public byte Anim_BombBay; //2,17

            //All flight records are 4 lines long: Timestamp, Positional Data, Airstate Data, Terminator [0].
        }
        public class AircraftCommand
        {
            public string Command;
        }

        public class AircraftSet
        {
            public string Identify;
            public bool IsPlayerObject;
            public byte IFF;
            public byte ID;
            public string Tag;
            public List<AircraftCommand> Commands = new List<AircraftCommand>();

            public int RecordsVersion;
            public List<AircraftRecord> Records = new List<AircraftRecord>();
        }
        #endregion

        #region Ground
        public class GroundRecord
        {
            public float TimeStamp; //0,0

            public float PosX; //1,0
            public float PosY; //1,1
            public float PosZ; //1,2
            public float HdgX; //1,3
            public float HdgY; //1,4
            public float HdgZ; //1,5

            public byte CPU_Flags; //2,0
            public byte Strength; //2,1

            public string ExtraLine1; //3,X
            public string ExtraLine2; //4,X

        }
        public class GroundCommand
        {
            public string Command;
        }

        public class GroundSet
        {
            public string Identify;
            public bool IsPlayerObject;
            public byte IFF;
            public byte ID;
            public string Tag;
            public List<GroundCommand> Commands = new List<GroundCommand>();

            public int RecordsVersion;
            public List<GroundRecord> Records = new List<GroundRecord>();
        }
        #endregion

        #region Ordinance
        public class OrdinanceRecord
        {
            public float TimeStamp;
            public byte WeaponType;
            public float PosX;
            public float PosY;
            public float PosZ;
            public float HdgX;
            public float HdgY;
            public float HdgZ;

            public float Velocity;
            public float BurnoutDistance;
            public byte Damage; //Soji will probably change this to a float very soon!
            public string Unknown1; //A0 ??? AircraftIDX?
            public string Unknown2; //P ??? Player/NPC?

        }
        public class OrdinanceSet
        {
            public List<OrdinanceRecord> Records;
        }
        #endregion

        #region CompleteReplay
        public class Replay
        {
            public int Version;
            public string Field;
            public float X;
            public float Y;
            public float Z;
            public float H;
            public float P;
            public float B;
            public string Environement;
            public List<AircraftSet> Aircraft = new List<AircraftSet>();
            public List<GroundSet> Grounds = new List<GroundSet>();
            public List<OrdinanceSet> Ordinance = new List<OrdinanceSet>();
        }
        #endregion

        public Replay LoadReplay(string Filename, out string ErrorCode)
        {
            #region Prepare...
            ErrorCode = "???";
            string[] Contents = new string[]{};
            if (!File.Exists(Filename))
            {
                //not exist.
                ErrorCode = "File Not Found.";
                return null;
            }
            Contents = File.ReadAllLines(Filename);

            Replay ThisReplay = new Replay();
            bool Failed = false;
            int Errors = 0;
            #endregion
            #region ReadFile
            string State = "NONE";
            for (int i = 0; i < Contents.Length; i++)
            {
                #region UpdateLine
                string CurrentLine = Contents[i];
                string[] Arguments = Contents[i].SplitPreservingQuotes(' ');
                #endregion

                #region State: None
                if (State == "NONE")
                {
                    switch (Arguments[0])
                    {
                        case "YFSVERSI":
                            #region
                            if (Arguments.Length < 2)
                            {
                                ErrorCode = "YFSVERSI Line Invalid.";
                                Failed = true;
                                return null;
                            }
                            Failed |= !Int32.TryParse(Arguments[1], out ThisReplay.Version);
                            break;
                            #endregion
                        case "FIELDNAM":
                            #region
                            if (Arguments.Length < 2)
                            {
                                ErrorCode = "FIELDNAM Line Invalid.";
                                Failed = true;
                                return null;
                            }
                            ThisReplay.Field = Arguments[1];
                            try
                            {
                                Failed |= !Single.TryParse(Arguments[2], out ThisReplay.X);
                                Failed |= !Single.TryParse(Arguments[3], out ThisReplay.Y);
                                Failed |= !Single.TryParse(Arguments[4], out ThisReplay.Z);
                                Failed |= !Single.TryParse(Arguments[5], out ThisReplay.H);
                                Failed |= !Single.TryParse(Arguments[6], out ThisReplay.P);
                                Failed |= !Single.TryParse(Arguments[7], out ThisReplay.B);
                            }
                            catch
                            {
                                Errors++;
                                ErrorCode = "FIELDNAM Line Conversion Failed.";
                            }
                            break;
                            #endregion
                        case "ENVIRONM":
                            #region
                            if (Arguments.Length < 2)
                            {
                                ErrorCode = "ENVIRONM Line Invalid.";
                                Failed = true;
                                return null;
                            }
                            ThisReplay.Environement = Arguments[1];
                            break;
                            #endregion
                        case "AIRPLANE":
                            #region
                            if (Arguments.Length < 3)
                            {
                                ErrorCode = "AIRCRAFT Line Invalid.";
                                Failed = true;
                                return null;
                            }
                            AircraftSet ThisAircraft = new AircraftSet();
                            State = "AIRCRAFT";
                            ThisAircraft.Identify = Arguments[1];
                            Failed |= !Boolean.TryParse(Arguments[2], out ThisAircraft.IsPlayerObject);
                            if (Failed) ErrorCode = "AIRCRAFT Line control Invalid.";
                            while (i < Contents.Length & State == "AIRCRAFT")
                            {
                                i++; //goto next line.
                                #region UpdateLine
                                CurrentLine = Contents[i];
                                Arguments = Contents[i].SplitPreservingQuotes(' ');
                                #endregion

                                switch (Arguments[0])
                                {
                                    case "IDENTIFY":
                                        #region
                                        if (Arguments.Length < 2)
                                        {
                                            ErrorCode = "IDENTIFY Line Invalid.";
                                            Failed = true;
                                            return null;
                                        }
                                        Failed |= !Byte.TryParse(Arguments[1], out ThisAircraft.IFF);
                                        if (Failed) ErrorCode = "IDENTIFY Line IFF Invalid.";
                                        break;
                                        #endregion
                                    case "IDANDTAG":
                                        #region
                                        if (Arguments.Length < 3)
                                        {
                                            ErrorCode = "IDANDTAG Line Invalid.";
                                            Failed = true;
                                            return null;

                                        }
                                        Failed |= !Byte.TryParse(Arguments[1], out ThisAircraft.ID);
                                        if (Failed) ErrorCode = "IDANDTAG Line Invalid.";
                                        ThisAircraft.Tag = Arguments[2];
                                        break;
                                        #endregion
                                    case "AIRPCMND":
                                        #region
                                        if (Arguments.Length < 2)
                                        {
                                            ErrorCode = "AIRPCMND Line Invalid.";
                                            Failed = true;
                                            return null;

                                        }
                                        AircraftCommand ThisCommand = new AircraftCommand();
                                        ThisCommand.Command = CurrentLine.Split(new char[] { ' ' }, 2)[1];
                                        ThisAircraft.Commands.Add(ThisCommand);
                                        break;
                                        #endregion
                                    case "NUMRECOR":
                                        #region
                                        if (Arguments.Length < 3)
                                        {
                                            ErrorCode = "NUMRECOR Line Invalid.";
                                            Failed = true;
                                            return null;

                                        }
                                        int Records = 0;
                                        Failed |= !Int32.TryParse(Arguments[1], out Records);
                                        Failed |= !Int32.TryParse(Arguments[2], out ThisAircraft.RecordsVersion);

                                        if (Failed)
                                        {
                                            ErrorCode = "NUMRECOR Line Conversion Failed.";
                                            Failed = true;
                                            return null;

                                        }
                                        #region Load All Flight Records
                                        for (int j = 0; j < Records; j++)
                                        {
                                            //Load Flight Records...
                                            AircraftRecord ThisRecord = new AircraftRecord();
                                            #region Line 1
                                            i++; //goto next line.
                                            #region UpdateLine
                                            CurrentLine = Contents[i];
                                            Arguments = Contents[i].SplitPreservingQuotes(' ');
                                            #endregion

                                            Failed |= !Single.TryParse(Arguments[0], out ThisRecord.TimeStamp);
                                            if (Failed) ErrorCode = "TIMESTAMP Line Invalid.";
                                            #endregion
                                            #region Line 2
                                            i++; //goto next line.
                                            #region UpdateLine
                                            CurrentLine = Contents[i];
                                            Arguments = Contents[i].SplitPreservingQuotes(' ');
                                            #endregion

                                            Failed |= !Single.TryParse(Arguments[0], out ThisRecord.PosX);
                                            Failed |= !Single.TryParse(Arguments[1], out ThisRecord.PosY);
                                            Failed |= !Single.TryParse(Arguments[2], out ThisRecord.PosZ);
                                            Failed |= !Single.TryParse(Arguments[3], out ThisRecord.HdgX);
                                            Failed |= !Single.TryParse(Arguments[4], out ThisRecord.HdgY);
                                            Failed |= !Single.TryParse(Arguments[5], out ThisRecord.HdgZ);
                                            Failed |= !Single.TryParse(Arguments[6], out ThisRecord.LoadFactor);
                                            if (Failed) ErrorCode = "RECORD1 Line Invalid.";
                                            #endregion
                                            #region Line 3
                                            i++; //goto next line.
                                            #region UpdateLine
                                            CurrentLine = Contents[i];
                                            Arguments = Contents[i].SplitPreservingQuotes(' ');
                                            #endregion

                                            Failed |= !Byte.TryParse(Arguments[00], out ThisRecord.AirState);
                                            Failed |= !Byte.TryParse(Arguments[01], out ThisRecord.Anim_VGW);
                                            Failed |= !Byte.TryParse(Arguments[02], out ThisRecord.Anim_Boards);
                                            Failed |= !Byte.TryParse(Arguments[03], out ThisRecord.Anim_Gear);
                                            Failed |= !Byte.TryParse(Arguments[04], out ThisRecord.Anim_Flap);
                                            Failed |= !Byte.TryParse(Arguments[05], out ThisRecord.Anim_Brake);
                                            Failed |= !Byte.TryParse(Arguments[06], out ThisRecord.Anim_Smoke);
                                            Failed |= !Byte.TryParse(Arguments[07], out ThisRecord.Unknown_1);
                                            Failed |= !Byte.TryParse(Arguments[08], out ThisRecord.Anim_Flags);
                                            Failed |= !Byte.TryParse(Arguments[09], out ThisRecord.Strength);
                                            Failed |= !Byte.TryParse(Arguments[10], out ThisRecord.Anim_Throttle);
                                            Failed |= !SByte.TryParse(Arguments[11], out ThisRecord.Anim_Elevator);
                                            Failed |= !SByte.TryParse(Arguments[12], out ThisRecord.Anim_Aileron);
                                            Failed |= !SByte.TryParse(Arguments[13], out ThisRecord.Anim_Rudder);
                                            Failed |= !SByte.TryParse(Arguments[14], out ThisRecord.Anim_Trim);
                                            Failed |= !Byte.TryParse(Arguments[15], out ThisRecord.Anim_ThrustVector);
                                            Failed |= !Byte.TryParse(Arguments[16], out ThisRecord.Anim_ThrustReverse);
                                            Failed |= !Byte.TryParse(Arguments[17], out ThisRecord.Anim_BombBay);
                                            if (Failed) ErrorCode = "RECORD2 Line Invalid.";
                                            #endregion

                                            i++; //Skip Line 4 - Terminator...
                                            ThisAircraft.Records.Add(ThisRecord);
                                        }
                                        #endregion
                                        State = "NONE";
                                        ThisReplay.Aircraft.Add(ThisAircraft);
                                        break;
                                        #endregion
                                }

                            }
                            break;
                            #endregion
                        case "GROUNDOB":
                            #region
                            if (Arguments.Length < 3)
                            {
                                ErrorCode = "GROUNDOB Line Invalid.";
                                Failed = true;
                                return null;

                            }
                            GroundSet ThisGround = new GroundSet();
                            State = "GROUNDOB";
                            ThisGround.Identify = Arguments[1];
                            Failed |= !Boolean.TryParse(Arguments[2], out ThisGround.IsPlayerObject);
                            while (i < Contents.Length & State == "GROUNDOB")
                            {
                                i++; //goto next line.
                                #region UpdateLine
                                CurrentLine = Contents[i];
                                Arguments = Contents[i].SplitPreservingQuotes(' ');
                                #endregion

                                switch (Arguments[0])
                                {
                                    case "IDENTIFY":
                                        #region
                                        if (Arguments.Length < 2)
                                        {
                                            ErrorCode = "IDENTIFY Line Invalid.";
                                            Failed = true;
                                            return null;
                                        }
                                        Failed |= !Byte.TryParse(Arguments[1], out ThisGround.IFF);
                                        break;
                                        #endregion
                                    case "IDANDTAG":
                                        #region
                                        if (Arguments.Length < 3)
                                        {
                                            ErrorCode = "IDANDTAG Line Invalid.";
                                            Failed = true;
                                            return null;
                                        }
                                        Failed |= !Byte.TryParse(Arguments[1], out ThisGround.ID);
                                        ThisGround.Tag = Arguments[2];
                                        break;
                                        #endregion
                                    case "NUMGDREC":
                                        #region
                                        if (Arguments.Length < 3)
                                        {
                                            ErrorCode = "NUMGDREC Line Invalid.";
                                            Failed = true;
                                            return null;

                                        }
                                        int Records = 0;
                                        Failed |= !Int32.TryParse(Arguments[1], out Records);
                                        Failed |= !Int32.TryParse(Arguments[2], out ThisGround.RecordsVersion);

                                        if (Failed)
                                        {
                                            ErrorCode = "NUMGDREC Line Conversion Failed.";
                                            Failed = true;
                                            return null;
                                        }
                                        #region Load All Ground Records
                                        for (int j = 0; j < Records; j++)
                                        {
                                            //Load Flight Records...
                                            GroundRecord ThisRecord = new GroundRecord();
                                            #region Line 1
                                            i++; //goto next line.
                                            #region UpdateLine
                                            CurrentLine = Contents[i];
                                            Arguments = Contents[i].SplitPreservingQuotes(' ');
                                            #endregion

                                            Failed |= !Single.TryParse(Arguments[0], out ThisRecord.TimeStamp);
                                            #endregion
                                            #region Line 2
                                            i++; //goto next line.
                                            #region UpdateLine
                                            CurrentLine = Contents[i];
                                            Arguments = Contents[i].SplitPreservingQuotes(' ');
                                            #endregion

                                            Failed |= !Single.TryParse(Arguments[0], out ThisRecord.PosX);
                                            Failed |= !Single.TryParse(Arguments[1], out ThisRecord.PosY);
                                            Failed |= !Single.TryParse(Arguments[2], out ThisRecord.PosZ);
                                            Failed |= !Single.TryParse(Arguments[3], out ThisRecord.HdgX);
                                            Failed |= !Single.TryParse(Arguments[4], out ThisRecord.HdgY);
                                            Failed |= !Single.TryParse(Arguments[5], out ThisRecord.HdgZ);
                                            #endregion
                                            #region Line 3
                                            i++; //goto next line.
                                            #region UpdateLine
                                            CurrentLine = Contents[i];
                                            Arguments = Contents[i].SplitPreservingQuotes(' ');
                                            #endregion

                                            Failed |= !Byte.TryParse(Arguments[0], out ThisRecord.CPU_Flags);
                                            Failed |= !Byte.TryParse(Arguments[1], out ThisRecord.Strength);
                                            #endregion
                                            #region Line 4
                                            i++; //goto next line.
                                            #region UpdateLine
                                            CurrentLine = Contents[i];
                                            Arguments = Contents[i].SplitPreservingQuotes(' ');
                                            #endregion

                                            ThisRecord.ExtraLine1 = CurrentLine;
                                            #endregion
                                            #region Line 5
                                            i++; //goto next line.
                                            #region UpdateLine
                                            CurrentLine = Contents[i];
                                            Arguments = Contents[i].SplitPreservingQuotes(' ');
                                            #endregion

                                            ThisRecord.ExtraLine2 = CurrentLine;
                                            #endregion
                                            i++; //Skip Line 6 - Terminator...
                                            ThisGround.Records.Add(ThisRecord);
                                        }
                                        #endregion
                                        State = "NONE";
                                        ThisReplay.Grounds.Add(ThisGround);
                                        break;
                                        #endregion
                                    default:
                                        #region
                                        ThisGround.Commands.Add(new GroundCommand(){ Command = CurrentLine });
                                        break;
                                        #endregion
                                }
                            }
                            break;
                            #endregion
                    }
                    if (Failed)
                    {
                        ErrorCode = "General Logic Error - Some Error in the replay file???";
                        return null;
                    }
                }
                else
                {
                    ErrorCode = "General Logic Error - Lost track of current position in replay file???";
                    return null;
                }
                #endregion
            }
            //Successfully loaded the replay file.
            ErrorCode = "SUCCESS!";
            return ThisReplay;
            #endregion
        }

        public Replay MergeReplay(Replay R1, Replay R2, out string ErrorCode)
        {
            #region Check Compatability
            if (R1.Version != R2.Version)
            {
                MessageBox.Show("These two replays may not be compatible!/n/nThe version numbers are different. We will try and proceed anyway, but you may get errors, be warned!", "Not Compatible?");
            }
            if (R1.Field != R2.Field)
            {
                MessageBox.Show("These two replays may not be compatible!/n/nThe field names are different. We will try and proceed anyway, but you may get errors, be warned!", "Not Compatible?");
            }
            #endregion
            #region Create new Replay
            Replay NewReplay = new Replay();
            NewReplay.Version = R2.Version;
            NewReplay.Field = R2.Field;
            NewReplay.X = R2.X;
            NewReplay.Y = R2.Y;
            NewReplay.Z = R2.Z;
            NewReplay.H = R2.H;
            NewReplay.P = R2.P;
            NewReplay.B = R2.B;
            NewReplay.Environement = R2.Environement;
            #endregion

            #region Get Time To Adjust By
            //find the end value so we may adjust all the subsequent values in R2 by this number.
            float EndTimeStampR1 = 0;
            foreach (AircraftSet ThisAircraft in R1.Aircraft)
            {
                if (ThisAircraft.Records.Count() <= 0) continue;
                float ThisTimeStamp = ThisAircraft.Records.OrderByDescending(x => x.TimeStamp).FirstOrDefault().TimeStamp;
                EndTimeStampR1 = (ThisTimeStamp > EndTimeStampR1) ? ThisTimeStamp : EndTimeStampR1; //choose the higher value.
            }
            foreach (GroundSet ThisGround in R1.Grounds)
            {
                if (ThisGround.Records.Count() <= 0) continue;
                float ThisTimeStamp = ThisGround.Records.OrderByDescending(x => x.TimeStamp).FirstOrDefault().TimeStamp;
                EndTimeStampR1 = (ThisTimeStamp > EndTimeStampR1) ? ThisTimeStamp : EndTimeStampR1; //choose the higher value.
            }
            #endregion

            //Now ready to append Aircraft.
            #region Merge Aircraft.
            foreach (AircraftSet ThisAircraft in R1.Aircraft)
            {
                #region Copy R1 Aircraft to New Replay
                AircraftSet NewAircraft = new AircraftSet();
                NewAircraft.Identify = ThisAircraft.Identify;
                NewAircraft.IFF = ThisAircraft.IFF;
                NewAircraft.ID = ThisAircraft.ID;
                NewAircraft.Tag = ThisAircraft.Tag;
                NewAircraft.IsPlayerObject = ThisAircraft.IsPlayerObject;
                foreach (AircraftCommand ThisCommand in ThisAircraft.Commands)
                {
                    AircraftCommand NewCommand = new AircraftCommand();
                    NewCommand.Command = ThisCommand.Command;
                    NewAircraft.Commands.Add(NewCommand);
                }
                NewAircraft.RecordsVersion = ThisAircraft.RecordsVersion;
                foreach (AircraftRecord ThisRecord in ThisAircraft.Records)
                {
                    AircraftRecord NewRecord = new AircraftRecord();
                    #region Clone Variables Across...
                    NewRecord.TimeStamp = ThisRecord.TimeStamp;
                    NewRecord.PosX = ThisRecord.PosX;
                    NewRecord.PosY = ThisRecord.PosY;
                    NewRecord.PosZ = ThisRecord.PosZ;
                    NewRecord.HdgX = ThisRecord.HdgX;
                    NewRecord.HdgY = ThisRecord.HdgY;
                    NewRecord.HdgZ = ThisRecord.HdgZ;
                    NewRecord.LoadFactor = ThisRecord.LoadFactor;

                    NewRecord.AirState = ThisRecord.AirState;
                    NewRecord.Anim_VGW = ThisRecord.Anim_VGW;
                    NewRecord.Anim_Boards = ThisRecord.Anim_Boards;
                    NewRecord.Anim_Gear = ThisRecord.Anim_Gear;
                    NewRecord.Anim_Flap = ThisRecord.Anim_Flap;
                    NewRecord.Anim_Brake = ThisRecord.Anim_Brake;
                    NewRecord.Anim_Smoke = ThisRecord.Anim_Smoke;
                    NewRecord.Unknown_1 = ThisRecord.Unknown_1;
                    NewRecord.Anim_Flags = ThisRecord.Anim_Flags;
                    NewRecord.Strength = ThisRecord.Strength;
                    NewRecord.Anim_Throttle = ThisRecord.Anim_Throttle;
                    NewRecord.Anim_Elevator = ThisRecord.Anim_Elevator;
                    NewRecord.Anim_Aileron = ThisRecord.Anim_Aileron;
                    NewRecord.Anim_Trim = ThisRecord.Anim_Trim;
                    NewRecord.Anim_ThrustVector = ThisRecord.Anim_ThrustVector;
                    NewRecord.Anim_ThrustReverse = ThisRecord.Anim_ThrustReverse;
                    NewRecord.Anim_BombBay = ThisRecord.Anim_BombBay;
                    #endregion

                    NewAircraft.Records.Add(NewRecord);
                }
                #endregion
                #region Merge if aircraft exists in Replay 2
                if (R2.Aircraft.Where(x => x.ID == ThisAircraft.ID).ToArray().Count() > 0)
                {
                    //There is a matching Aircraft in the second replay file. Time to merge!
                    #region Merge Records...
                    AircraftSet Matching = R2.Aircraft.Where(x => x.ID == ThisAircraft.ID).ToArray()[0];
                    foreach (AircraftRecord ThisRecord in Matching.Records)
                    {
                        AircraftRecord NewRecord = new AircraftRecord();
                        #region Clone Variables Across...
                        NewRecord.TimeStamp = ThisRecord.TimeStamp;
                        NewRecord.PosX = ThisRecord.PosX;
                        NewRecord.PosY = ThisRecord.PosY;
                        NewRecord.PosZ = ThisRecord.PosZ;
                        NewRecord.HdgX = ThisRecord.HdgX;
                        NewRecord.HdgY = ThisRecord.HdgY;
                        NewRecord.HdgZ = ThisRecord.HdgZ;
                        NewRecord.LoadFactor = ThisRecord.LoadFactor;

                        NewRecord.AirState = ThisRecord.AirState;
                        NewRecord.Anim_VGW = ThisRecord.Anim_VGW;
                        NewRecord.Anim_Boards = ThisRecord.Anim_Boards;
                        NewRecord.Anim_Gear = ThisRecord.Anim_Gear;
                        NewRecord.Anim_Flap = ThisRecord.Anim_Flap;
                        NewRecord.Anim_Brake = ThisRecord.Anim_Brake;
                        NewRecord.Anim_Smoke = ThisRecord.Anim_Smoke;
                        NewRecord.Unknown_1 = ThisRecord.Unknown_1;
                        NewRecord.Anim_Flags = ThisRecord.Anim_Flags;
                        NewRecord.Strength = ThisRecord.Strength;
                        NewRecord.Anim_Throttle = ThisRecord.Anim_Throttle;
                        NewRecord.Anim_Elevator = ThisRecord.Anim_Elevator;
                        NewRecord.Anim_Aileron = ThisRecord.Anim_Aileron;
                        NewRecord.Anim_Trim = ThisRecord.Anim_Trim;
                        NewRecord.Anim_ThrustVector = ThisRecord.Anim_ThrustVector;
                        NewRecord.Anim_ThrustReverse = ThisRecord.Anim_ThrustReverse;
                        NewRecord.Anim_BombBay = ThisRecord.Anim_BombBay;
                        #endregion

                        if (NewRecord.TimeStamp <= 0) continue; //avoid duplicate entires!

                        NewRecord.TimeStamp += EndTimeStampR1; //Shift the record forwards...

                        NewAircraft.Records.Add(NewRecord);
                    }
                    #endregion
                }
                #endregion

                NewReplay.Aircraft.Add(NewAircraft);
            }
            foreach (AircraftSet ThisAircraft in R2.Aircraft)
            {
                #region Merge if aircraft NOT exists in Replay 1
                if (R1.Aircraft.Where(x => x.ID == ThisAircraft.ID).ToArray().Count() <= 0)
                {
                    //There is NO matching Aircraft in the first replay file. Time to merge!

                    #region Merge Records...
                    AircraftSet NewAircraft = new AircraftSet();
                    NewAircraft.Identify = ThisAircraft.Identify;
                    NewAircraft.IFF = ThisAircraft.IFF;
                    NewAircraft.ID = ThisAircraft.ID;
                    NewAircraft.Tag = ThisAircraft.Tag;
                    NewAircraft.IsPlayerObject = ThisAircraft.IsPlayerObject;
                    AircraftSet Matching = R2.Aircraft.Where(x => x.ID == ThisAircraft.ID).ToArray()[0];
                    NewAircraft.RecordsVersion = ThisAircraft.RecordsVersion;
                    foreach (AircraftRecord ThisRecord in Matching.Records)
                    {
                        AircraftRecord NewRecord = new AircraftRecord();
                        #region Clone Variables Across...
                        NewRecord.TimeStamp = ThisRecord.TimeStamp;
                        NewRecord.PosX = ThisRecord.PosX;
                        NewRecord.PosY = ThisRecord.PosY;
                        NewRecord.PosZ = ThisRecord.PosZ;
                        NewRecord.HdgX = ThisRecord.HdgX;
                        NewRecord.HdgY = ThisRecord.HdgY;
                        NewRecord.HdgZ = ThisRecord.HdgZ;
                        NewRecord.LoadFactor = ThisRecord.LoadFactor;

                        NewRecord.AirState = ThisRecord.AirState;
                        NewRecord.Anim_VGW = ThisRecord.Anim_VGW;
                        NewRecord.Anim_Boards = ThisRecord.Anim_Boards;
                        NewRecord.Anim_Gear = ThisRecord.Anim_Gear;
                        NewRecord.Anim_Flap = ThisRecord.Anim_Flap;
                        NewRecord.Anim_Brake = ThisRecord.Anim_Brake;
                        NewRecord.Anim_Smoke = ThisRecord.Anim_Smoke;
                        NewRecord.Unknown_1 = ThisRecord.Unknown_1;
                        NewRecord.Anim_Flags = ThisRecord.Anim_Flags;
                        NewRecord.Strength = ThisRecord.Strength;
                        NewRecord.Anim_Throttle = ThisRecord.Anim_Throttle;
                        NewRecord.Anim_Elevator = ThisRecord.Anim_Elevator;
                        NewRecord.Anim_Aileron = ThisRecord.Anim_Aileron;
                        NewRecord.Anim_Trim = ThisRecord.Anim_Trim;
                        NewRecord.Anim_ThrustVector = ThisRecord.Anim_ThrustVector;
                        NewRecord.Anim_ThrustReverse = ThisRecord.Anim_ThrustReverse;
                        NewRecord.Anim_BombBay = ThisRecord.Anim_BombBay;
                        #endregion

                        if (NewRecord.TimeStamp == 0) continue; //avoid duplicate entires!

                        NewRecord.TimeStamp += EndTimeStampR1; //Shift the record forwards...

                        NewAircraft.Records.Add(NewRecord);
                    }
                    #endregion

                    NewReplay.Aircraft.Add(NewAircraft);
                }
                #endregion
            }
            #endregion

            //Now ready to append Grounds.
            #region Merge Grounds.
            foreach (GroundSet ThisGround in R1.Grounds)
            {
                #region Copy R1 Grounds to New Replay
                GroundSet NewGround = new GroundSet();
                NewGround.Identify = ThisGround.Identify;
                NewGround.IFF = ThisGround.IFF;
                NewGround.ID = ThisGround.ID;
                NewGround.Tag = ThisGround.Tag;
                foreach (GroundCommand ThisCommand in ThisGround.Commands)
                {
                    GroundCommand NewCommand = new GroundCommand();
                    NewCommand.Command = ThisCommand.Command;
                    NewGround.Commands.Add(NewCommand);
                }
                NewGround.RecordsVersion = ThisGround.RecordsVersion;
                foreach (GroundRecord ThisRecord in ThisGround.Records)
                {
                    GroundRecord NewRecord = new GroundRecord();
                    #region Clone Variables Across...
                    NewRecord.TimeStamp = ThisRecord.TimeStamp;
                    NewRecord.PosX = ThisRecord.PosX;
                    NewRecord.PosY = ThisRecord.PosY;
                    NewRecord.PosZ = ThisRecord.PosZ;
                    NewRecord.HdgX = ThisRecord.HdgX;
                    NewRecord.HdgY = ThisRecord.HdgY;
                    NewRecord.HdgZ = ThisRecord.HdgZ;

                    NewRecord.CPU_Flags = ThisRecord.CPU_Flags;
                    NewRecord.Strength = ThisRecord.Strength;

                    NewRecord.ExtraLine1 = ThisRecord.ExtraLine1;

                    NewRecord.ExtraLine2 = ThisRecord.ExtraLine2;
                    #endregion

                    NewGround.Records.Add(NewRecord);
                }
                #endregion
                #region Merge if ground exists in Replay 2
                if (R2.Grounds.Where(x => x.ID == ThisGround.ID).ToArray().Count() > 0)
                {
                    //There is a matching Aircraft in the second replay file. Time to merge!
                    #region Merge Records...
                    GroundSet Matching = R2.Grounds.Where(x => x.ID == ThisGround.ID).ToArray()[0];
                    foreach (GroundRecord ThisRecord in Matching.Records)
                    {
                        GroundRecord NewRecord = new GroundRecord();
                        #region Clone Variables Across...
                        NewRecord.TimeStamp = ThisRecord.TimeStamp;
                        NewRecord.PosX = ThisRecord.PosX;
                        NewRecord.PosY = ThisRecord.PosY;
                        NewRecord.PosZ = ThisRecord.PosZ;
                        NewRecord.HdgX = ThisRecord.HdgX;
                        NewRecord.HdgY = ThisRecord.HdgY;
                        NewRecord.HdgZ = ThisRecord.HdgZ;

                        NewRecord.CPU_Flags = ThisRecord.CPU_Flags;
                        NewRecord.Strength = ThisRecord.Strength;

                        NewRecord.ExtraLine1 = ThisRecord.ExtraLine1;

                        NewRecord.ExtraLine2 = ThisRecord.ExtraLine2;
                        #endregion

                        if (NewRecord.TimeStamp == 0) continue; //avoid duplicate entires!

                        NewRecord.TimeStamp += EndTimeStampR1; //Shift the record forwards...

                        NewGround.Records.Add(NewRecord);
                    }
                    #endregion
                }
                #endregion

                NewReplay.Grounds.Add(NewGround);
            }
            foreach (GroundSet ThisGround in R2.Grounds)
            {
                #region Merge if ground NOT exists in Replay 1
                if (R1.Grounds.Where(x => x.ID == ThisGround.ID).ToArray().Count() <= 0)
                {
                    //There is NO matching Aircraft in the first replay file. Time to merge!
                    #region Merge Records...
                    GroundSet NewGround = new GroundSet();
                    NewGround.Identify = ThisGround.Identify;
                    NewGround.IFF = ThisGround.IFF;
                    NewGround.ID = ThisGround.ID;
                    NewGround.Tag = ThisGround.Tag;
                    GroundSet Matching = R2.Grounds.Where(x => x.ID == ThisGround.ID).ToArray()[0];
                    NewGround.RecordsVersion = ThisGround.RecordsVersion;
                    foreach (GroundRecord ThisRecord in Matching.Records)
                    {
                        GroundRecord NewRecord = new GroundRecord();
                        #region Clone Variables Across...
                        NewRecord.TimeStamp = ThisRecord.TimeStamp;
                        NewRecord.PosX = ThisRecord.PosX;
                        NewRecord.PosY = ThisRecord.PosY;
                        NewRecord.PosZ = ThisRecord.PosZ;
                        NewRecord.HdgX = ThisRecord.HdgX;
                        NewRecord.HdgY = ThisRecord.HdgY;
                        NewRecord.HdgZ = ThisRecord.HdgZ;

                        NewRecord.CPU_Flags = ThisRecord.CPU_Flags;
                        NewRecord.Strength = ThisRecord.Strength;

                        NewRecord.ExtraLine1 = ThisRecord.ExtraLine1;

                        NewRecord.ExtraLine2 = ThisRecord.ExtraLine2;
                        #endregion

                        if (NewRecord.TimeStamp == 0) continue; //avoid duplicate entires!

                        NewRecord.TimeStamp += EndTimeStampR1; //Shift the record forwards...

                        NewGround.Records.Add(NewRecord);
                    }
                    #endregion

                    NewReplay.Grounds.Add(NewGround);
                }
                #endregion
            }
            #endregion

            //We're done! Return the new replay!
            ErrorCode = "Merge Successful!";
            return NewReplay;
        }

        public bool SaveReplay(Replay YFS, string Filename)
        {
            List<string> Output = new List<string>();
            Output.Add("YFSVERSI " + YFS.Version.ToString());
            Output.Add("FIELDNAM " + YFS.Field.ToString() + " " +
                                     YFS.X.ToString() + " " +
                                     YFS.Y.ToString() + " " +
                                     YFS.Z.ToString() + " " +
                                     YFS.H.ToString() + " " + 
                                     YFS.P.ToString() + " " +
                                     YFS.B.ToString() + " " +
                                     "FALSE LOADAIR:FALSE"
                                     );
            Output.Add("ENVIRONM " + YFS.Environement);
            foreach (AircraftSet ThisAircraft in YFS.Aircraft)
            {
                Output.Add("AIRPLANE " + "\"" + ThisAircraft.Identify + "\" " + 
                                         ThisAircraft.IsPlayerObject.ToString().ToUpperInvariant());

                Output.Add("IDENTIFY " + ThisAircraft.IFF);

                Output.Add("IDANDTAG " + ThisAircraft.ID + " \"" + ThisAircraft.Tag + "\"");

                foreach (AircraftCommand ThisCommand in ThisAircraft.Commands)
                {
                    Output.Add("AIRPCMND " + ThisCommand.Command);
                }

                #region Flight Records...
                Output.Add("NUMRECOR " + ThisAircraft.Records.Count() + " " + ThisAircraft.RecordsVersion);

                foreach (AircraftRecord ThisRecord in ThisAircraft.Records)
                {
                    Output.Add(ThisRecord.TimeStamp.ToString());
                    Output.Add(ThisRecord.PosX.ToString() + " " +
                               ThisRecord.PosY.ToString() + " " +
                               ThisRecord.PosZ.ToString() + " " +
                               ThisRecord.HdgX.ToString() + " " +
                               ThisRecord.HdgY.ToString() + " " +
                               ThisRecord.HdgZ.ToString() + " " +
                               ThisRecord.LoadFactor.ToString());

                    Output.Add(ThisRecord.AirState.ToString() + " " +
                               ThisRecord.Anim_VGW.ToString() + " " +
                               ThisRecord.Anim_Boards.ToString() + " " +
                               ThisRecord.Anim_Gear.ToString() + " " +
                               ThisRecord.Anim_Flap.ToString() + " " +
                               ThisRecord.Anim_Brake.ToString() + " " +
                               ThisRecord.Anim_Smoke.ToString() + " " +
                               ThisRecord.Unknown_1.ToString() + " " +
                               ThisRecord.Anim_Flags.ToString() + " " +
                               ThisRecord.Strength.ToString() + " " +
                               ThisRecord.Anim_Throttle.ToString() + " " +
                               ThisRecord.Anim_Elevator.ToString() + " " +
                               ThisRecord.Anim_Aileron.ToString() + " " +
                               ThisRecord.Anim_Rudder.ToString() + " " +
                               ThisRecord.Anim_Trim.ToString() + " " +
                               ThisRecord.Anim_ThrustVector.ToString() + " " +
                               ThisRecord.Anim_ThrustReverse.ToString() + " " +
                               ThisRecord.Anim_BombBay.ToString());

                    Output.Add("0"); //Terminator...
                }
                #endregion
            }
            foreach (GroundSet ThisGround in YFS.Grounds)
            {
                Output.Add("GROUNDOB " + "\"" + ThisGround.Identify + "\" " +
                                         ThisGround.IsPlayerObject.ToString().ToUpperInvariant());

                Output.Add("IDENTIFY " + ThisGround.IFF);

                Output.Add("IDANDTAG " + ThisGround.ID + " \"" + ThisGround.Tag + "\"");

                foreach (GroundCommand ThisCommand in ThisGround.Commands)
                {
                    Output.Add(ThisCommand.Command);
                }

                #region Ground Records...
                Output.Add("NUMGDREC " + ThisGround.Records.Count() + " " + ThisGround.RecordsVersion);

                foreach (GroundRecord ThisRecord in ThisGround.Records)
                {
                    Output.Add(ThisRecord.TimeStamp.ToString());

                    Output.Add(ThisRecord.PosX.ToString() + " " +
                               ThisRecord.PosY.ToString() + " " +
                               ThisRecord.PosZ.ToString() + " " +
                               ThisRecord.HdgX.ToString() + " " +
                               ThisRecord.HdgY.ToString() + " " +
                               ThisRecord.HdgZ.ToString());

                    Output.Add(ThisRecord.CPU_Flags.ToString() + " " +
                               ThisRecord.Strength.ToString());

                    Output.Add(ThisRecord.ExtraLine1.ToString());

                    Output.Add(ThisRecord.ExtraLine2.ToString());

                    Output.Add("0"); //Terminator...
                }
                #endregion
            }

            File.WriteAllLines(Filename, Output.ToArray());
            return true;
        }
    }

    public static class Extensions
    {
        /// <summary>
        /// Uses a RegEx to split a string by white spaces - preserving quoted blocks.
        /// </summary>
        /// <param name="Input"></param>
        /// <returns></returns>
        public static string[] SplitPreservingQuotes(this string Input, char splittingchar)
        {
            if (Input == null) Input = "";
            while (Input.Contains("\t\t"))
            {
                Input = Input.Replace("\t\t", "\t");
            }
            Input = Input.Replace("\t", " ");
            List<string> Strings = Regex.Matches(Input, @"[\""].+?[\""]|[^" + splittingchar + "]+")
                .Cast<Match>()
                .Select(m => m.Value)
                .ToList();

            List<string> Output = new List<string>();
            for (int i = 0; i < Strings.Count; i++)
            {
                Output.Add(Strings[i].Split('\t')[0].Split(';')[0].Replace("\"", ""));
            }

            return Output.ToArray();
        }
    }
}
