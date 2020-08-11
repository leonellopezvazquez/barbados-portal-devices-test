using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace barbados_portal_test
{
    public class FTPWatcher
    {

        private bool IsRunning;

        public event EventHandler eventoFTPWatcher;

       
        public System.IO.FileSystemWatcher watcher = null;
        private string sDirectory = "";
        private string sDirectoryPending = "/PendingEvents";
        private string sFilter = "*.axi";
        private int iRetryTime = 1;

        private Queue queue;
        private Queue queueMatch = new Queue();
        private Queue<PIPSEvent> queuePIPS = new Queue<PIPSEvent>();

        private String[] sFiles;
        //private int retryCounter;
        private System.Timers.Timer TimerPending = new System.Timers.Timer();
        private System.Timers.Timer TimerMonitor = new System.Timers.Timer();
   
   
        private System.Timers.Timer TimerMatch = new System.Timers.Timer();
        private FileInfo mFileMonitor;
        private DateTime dCurrent;

        
        private DataBase dataBase;
        //private OCR ocr;
        private string cropped;
        private bool islocal;
        private int orden;
        
        private Utilities utilities = new Utilities();
       
       
        private int iMantisCount = 0;
        private string idcamera = "";

        public FTPWatcher(string _sDirectory, string _sFilter)
        {
            queue = new Queue();
            this.sDirectory = _sDirectory;
            this.sFilter = _sFilter;           
                                         
        }


        public bool StartFTPWatcher()
        {
            sDirectoryPending = sDirectory + sDirectoryPending;
            if (!utilities.CreateDirectory(sDirectoryPending))
                return false;

            try
            {
                foreach (String file in Directory.GetFiles(sDirectory))
                {
                    FileInfo mFile = new FileInfo(file);
                    try
                    {
                        File.Move(file, sDirectoryPending + "/" + mFile.Name);
                    }
                    catch (Exception ex)
                    {
                       // Form1.log.Error("StartFTPWatcher-MoveFile{" + mFile.FullName + "}", ex);
                    }
                }
            }
            catch (Exception ex)
            {
              //  Form1.log.Error("StartFTPWatcher-GetFiles-" + sDirectory, ex);
                return false;
            }


            TimerMonitor.Interval = iRetryTime * 60000;
            TimerMonitor.Elapsed += new System.Timers.ElapsedEventHandler(TimerMonitorProcessor);
            TimerMonitor.Start();
         
       

            TimerMatch.Interval = 5000;
            TimerMatch.Elapsed += new System.Timers.ElapsedEventHandler(MatchReads);
            TimerMatch.Start();


            watcher = new FileSystemWatcher();
            watcher.Path = sDirectory;

            watcher.NotifyFilter = NotifyFilters.LastAccess
                | NotifyFilters.LastWrite
                | NotifyFilters.FileName
                | NotifyFilters.DirectoryName;
            watcher.Filter = sFilter;
            watcher.InternalBufferSize = 196608;
            watcher.Created += new FileSystemEventHandler(OnCreated);
            watcher.EnableRaisingEvents = true;

           // Form1.log.Info("Watcher Started - " + sDirectory);

            IsRunning = true;
            return true;
        }

        public bool StopFTPWatcher()
        {
            try
            {
                Thread.Sleep(200);
                //threadReads.Abort();
                if (watcher != null)
                {
                    watcher.EnableRaisingEvents = false;
                    watcher.Created -= new FileSystemEventHandler(OnCreated);
                    watcher.Dispose();
                    watcher = null;
                   // Form1.log.Info("Watcher Stopped");
                }

                TimerMonitor.Stop();
                TimerPending.Stop();
                
                TimerMatch.Stop();

                IsRunning = false;
                return true;
            }
            catch (Exception ex)
            {
                //Form1.log.Error("StopFTPWatcher Error", ex);
                return false;
            }
        }

        public void OnCreated(object source, FileSystemEventArgs e)
        {
            try
            {
                if (e.ChangeType == WatcherChangeTypes.Created)
                {
                    
                    Thread.Sleep(400);
                    Thread thread = new Thread(() => ProcessEvent(e.FullPath));
                    thread.Start();
                    
                }
            }
            catch (Exception ex)
            {
                //Form1.log.Error("Error OnCreated ", ex);
            }
        }

        private void TimerMonitorProcessor(Object myObject, EventArgs myEventArgs)
        {
            try
            {
                dCurrent = DateTime.Now.AddMinutes(-2);
                foreach (String file in Directory.GetFiles(sDirectory))
                {
                    mFileMonitor = new FileInfo(file);
                    if (mFileMonitor.CreationTime < dCurrent)
                    {
                        if (!queue.Contains(file))
                        {
                            File.Move(file, sDirectoryPending + "/" + mFileMonitor.Name);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                //Form1.log.Error("Monitor Events Error: " + ex.Message);
            }

            try
            {
                sFiles = Directory.GetFiles(sDirectoryPending);
                if (sFiles.Length > 0)
                {
                   // Form1.log.Debug("SENDING PENDING FILES");
                   // Form1.log.Debug("Processing " + sFiles.Length + " file(s) in <Pending> directory.");
                    foreach (String file in sFiles)
                    {
                        string name = Path.GetFileName(file);
                        
                        //Form1.log.Debug("SENDING PENDING:" + name);
                        Thread thread = new Thread(() => ProcessEvent(file));
                        thread.Start();
                        thread.Join();                      
                    }
                }
            }
            catch (Exception ex)
            {
                //Form1.log.Error("TimerEventProcessor", ex);
            }

        }

        private void MatchReads(Object myObject, EventArgs myEventArgs)
        {
            try
            {
                TimerMatch.Enabled = false;
                while (queueMatch.Count > 0)
                {
                    FindMatch((PIPSEvent)queueMatch.Dequeue());
                    Thread.Sleep(200);
                }
            }
            catch (Exception ex)
            {
                //Form1.log.Error("MatchReads", ex);
            }
            finally
            {
                TimerMatch.Enabled = true;
            }
        }
        private void MantisSyncReads(Object myObject, EventArgs myEventArgs)
        {
            try
            {
              
                iMantisCount++;
                if (queuePIPS.Count > 20 || iMantisCount >= 4)
                {
                    queuePIPSProcess();
                    iMantisCount = 0;
                }
            }
            catch (Exception ex)
            {
               // Form1.log.Error("BOSSSyncReads", ex);
            }
            finally
            {
              
            }
        }
             
        private string[] ObtainBothImages(string fileName)
        {
            string[] filenamesOnprocess = { "null", "null", "null" };
            try
            {
                var imageName = Path.GetFileName(fileName);
                string sameimagename = imageName.Substring(1, imageName.Length - 1);
                filenamesOnprocess[0] = fileName;
                filenamesOnprocess[1] = Path.GetDirectoryName(fileName) + "/p" + sameimagename; ;
                filenamesOnprocess[2] = sDirectoryPending + "/p" + sameimagename;
                return filenamesOnprocess;
            }
            catch (Exception ex)
            {
                //Form1.log.Error("Error de nombramiento de files ", ex);
                return null;
            }
        }

        private PIPSEvent SendToMantis(string overview_pat, string nombrepatch, string secondnombrepatch, int type)
        {
            PIPSEvent pips = new PIPSEvent();
            string overviewName, patchName = "";
            OverObject objetoimagen = null;

            try
            {
                overviewName = Path.GetFileName(overview_pat);
                //Form1.log.Debug("Inicia proceso: " + overviewName);
                patchName = Path.GetFileName(nombrepatch);
                objetoimagen = utilities.ParseImageName(overviewName);
                Image image = Image.FromFile(overview_pat);

                pips.LPN = objetoimagen.plate;
                pips.lane = objetoimagen.lane;
                pips.timestamp = objetoimagen.datetime;
                try
                {
                    pips.latitude = Double.Parse(objetoimagen.lattitude);
                    pips.longitude = Double.Parse(objetoimagen.longitude);
                }
                catch (Exception ex)
                {
                    //Form1.log.Error(ex);
                }

                //pips.bossid = -1;
                pips.idDevice = objetoimagen.idCamera;
                pips.TollingId = type.ToString();
                pips.conf = objetoimagen.conf;
               
                pips.IdCamera = objetoimagen.idCamera;
                //////////////////
                //Cropping      //
                //////////////////

                if (cropped.Equals("true"))
                    image = ImageCropper.cropper(image, objetoimagen.lane);

                pips.overview = utilities.ImageToBase64(image);
                image.Dispose();

                if (File.Exists(nombrepatch))
                    pips.patch = utilities.ImageFileToBase64(nombrepatch);
                else
                    pips.patch = utilities.ImageFileToBase64(secondnombrepatch);

                /////////////////////
                ///  Save Image    //
                /////////////////////
                              
                queuePIPS.Enqueue(pips);

                objetoimagen.Dispose();
            }
            catch (Exception ex)
            {
                //Form1.log.Error("Error de proceso Mantis", ex);
            }
            return pips;
        }

        private void queuePIPSProcess()
        {
            try
            {
                if (queuePIPS.Count > 0)
                {
                    do
                    {
                        List<string> listImages = new List<string>();
                        List<PIPSEvent> pipsEvents = new List<PIPSEvent>();

                        if (!IsRunning)
                            break;

                        int iCount = 0;
                        while (queuePIPS.Count > 0 && iCount < 20)
                        {
                            pipsEvents.Add(queuePIPS.Dequeue());
                            iCount++;
                        }

                        foreach (PIPSEvent pips in pipsEvents)
                        {
                            listImages.Add(pips.overview);
                        }
            
                        for (int i = 0; i < pipsEvents.Count; i++)
                        {
                            
                           // Thread threadDB = new Thread(() => { pipsEvents[i].id = dataBase.insert(pipsEvents[i]); });
                           // threadDB.Start();
                           // threadDB.Join();
                            if (orden == 0 || orden == 1)
                                eventoFTPWatcher(pipsEvents[i], new EventArgs());
                           
                        }

                        listImages.Clear();
                        pipsEvents.Clear();
                    }
                    while (queuePIPS.Count > 20 && IsRunning);
                }
            }
            catch (Exception ex)
            {
                //Form1.log.Error(ex);
            }
        }
           
        private void FindMatch(PIPSEvent exitEvent)
        {
            
        }
        
        private void ProcessEvent(object strFile)
        {
            try
            {
                PIPSEvent pips = new PIPSEvent();
                int offset = 1;
                int currentoffset = 0;
                var stringFile = getxmlContent(ref offset, (string)strFile);

                var AcsObject = DeserializeEvent(stringFile);

                byte[] file = System.IO.File.ReadAllBytes((string)strFile);
                offset += stringFile.Length;
                currentoffset = offset;
                pips.Patch = file.Skip(offset).Take(Int32.Parse(AcsObject.Patch.Length)).ToArray();
                         
                offset += Int32.Parse(AcsObject.Full.Offset);
                pips.IR = file.Skip(offset).Take(Int32.Parse(AcsObject.Full.Length)).ToArray();
                var test = ByteToImage(pips.IR);

                offset = Int32.Parse(AcsObject.Overview.Offset) + currentoffset; 
                pips.Overview = file.Skip(offset).Take(Int32.Parse(AcsObject.Overview.Length)).ToArray();
                test = ByteToImage(pips.Overview);
                if (eventoFTPWatcher != null)
                {
                    eventoFTPWatcher(pips, new EventArgs());
                }
                
            }
            catch (Exception ex)
            {
                //Form1.log.Error(ex);
            }
        }

        public Bitmap ByteToImage(byte[] blob)
        {
            using (var mStream = new MemoryStream())
            {
                Bitmap bm;
                byte[] pData = blob;
                mStream.Write(pData, 0, Convert.ToInt32(pData.Length));
                bm = new Bitmap(mStream, false);
                mStream.Dispose();
                return bm;
            }
        }

        private string getxmlContent(ref int offset, string path)
        {
            string line, content = "";
            using (StreamReader sr = new StreamReader(path))
            {
                do
                {
                    line = sr.ReadLine();
                  
                    content += line;
                    offset++;
                } while (!line.Contains("</acs_event_header>"));
            }
            return content;
        }

        private void deleteFile(string path)
        {
            File.Delete(path);
        }

        internal  Acs_event_header DeserializeEvent(string entry)
        {
            try
            {
                XmlSerializer serializer = new XmlSerializer(typeof(Acs_event_header));
                using (TextReader reader = new StringReader(entry))
                {
                    var result = (Acs_event_header)serializer.Deserialize(reader);
                    return result;
                }
            }
            catch (Exception ex)
            {
                //PIXIServerController.log.Error("Error deserialize delivery", ex);
                // PIXILog.Error("Error deserialize delivery", ex.Message);
                return null;
            }

        }
    }
}
