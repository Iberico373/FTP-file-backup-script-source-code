using System;
using System.Collections.Generic;
using WinSCP;

public class FTPBackupScript
{
    public static int Main()
    {        
        List<string> filesToCopy = new List<string>();
        string today = DateTime.Now.Date.ToString("dd-MM-yyyy");
        string localDir = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory) + @"\SteWARdS Json Files"; 

        try
        {
            // Set up session options
            SessionOptions sessionOptions = new SessionOptions
            {
                Protocol = Protocol.Ftp,
                HostName = "insertHostNameHere",
                PortNumber = 0, // Insert port number here
                UserName = "insertUserNameHere",
                Password = "insertPasswordHere",
                FtpSecure = FtpSecure.Explicit,
                TlsHostCertificateFingerprint = "insertCertHere",
            };

            sessionOptions.AddRawSettings("ProxyPort", "0");

            using (Session session = new Session())
            {
                // Connect to session
                session.Open(sessionOptions);

                // Get current directory info
                RemoteDirectoryInfo dir = session.ListDirectory("/");

                // Add all .json file locations to list
                foreach (RemoteFileInfo fileInfo in dir.Files)
                {
                    if (fileInfo.Name.Contains(".json"))
                    {
                        filesToCopy.Add(fileInfo.FullName);
                    }
                }

                TransferEventArgs transferResult;

                // Download json files from remote directory to a local folder
                foreach (string fileLocation in filesToCopy)
                {
                    transferResult = session.GetFileToDirectory(fileLocation, localDir);
                    Console.WriteLine("Download of {0} succeeded", transferResult.FileName);
                }

                // Create a new folder for today
                session.CreateDirectory(today);

                // Upload downloaded files to new folder
                foreach (string fileLocation in filesToCopy)
                {
                    transferResult = session.PutFileToDirectory(localDir + fileLocation, "/" + today);
                    Console.WriteLine("Upload of {0} succeeded", transferResult.FileName);
                }

                Console.WriteLine("All {0} files succesfully backed up\nPress 'enter' to continue", filesToCopy.Count.ToString());
                Console.ReadLine();
            }

            return 0;        
        }

        catch (Exception e)
        {
            Console.WriteLine("Error: {0}\nPress 'enter' to continue", e);
            Console.ReadLine();
            return 1;
        }
    }
}