using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.IO;
using System.Security.Cryptography;

namespace HashApplication
{
    class HashComputer
    {
        public long Offset { get; set; }
        public long BytesToRead { get; set; }
        public string ThreadName { get; set; }
        public long HashCounter { get; set; }

        public HashComputer( long offset, long bytesToRead, string threadName, long hashCounter )
        {
            Offset = offset;
            BytesToRead = bytesToRead;
            ThreadName = threadName;
            HashCounter = hashCounter;
        }

        public void StartHashing()
        {
            Thread newThread = new Thread( new ThreadStart( Start ) );
            newThread.Name = ThreadName;
            newThread.Start();
        }

        public void PrintHash( byte[] bytes )
        {
            StringBuilder str = new StringBuilder();

            for( int i = 0; i < bytes.Length; i++ )
                str.AppendFormat( "{0:X2}", bytes[i] );

            Console.WriteLine( "Block №{0} is: {1}", HashCounter ,str.ToString() );
            ++HashCounter;
        }


        private void Start()
        {
            Console.WriteLine( "Thread {0} is prepared for work! I neeed to read {1} bytes from {2} with offset {3}.", Thread.CurrentThread.Name, BytesToRead, Program.filePath, Offset );
            using( FileStream fileStream = File.Open( Program.filePath, FileMode.Open, FileAccess.Read, FileShare.Read ) )
            {
                fileStream.Seek( Offset, SeekOrigin.Begin );
                int currBytesRead;
                long blockBytesRead = 0;
                long totalBytesRead = 0;
                int currBufferSize = Program.bufferSize;
                byte[] buffer = new byte[currBufferSize];
                long bytesLeft;
                SHA256Managed sha = new SHA256Managed();

                while( ( currBytesRead = fileStream.Read( buffer, 0, currBufferSize ) ) > 0 )
                {
                    blockBytesRead += currBytesRead;
                    totalBytesRead += currBytesRead;
                    sha.TransformBlock( buffer, 0, currBytesRead, buffer, 0 );

                    //End of chunk
                    if( totalBytesRead == BytesToRead )
                    {
                        sha.TransformFinalBlock( buffer, 0, 0 );
                        PrintHash( sha.Hash );
                        break;
                    }
                   
                    else if( blockBytesRead == Program.blockSize )
                    {
                        sha.TransformFinalBlock( buffer, 0, 0 );
                        PrintHash( sha.Hash );

                        blockBytesRead = 0;
                        currBufferSize = Program.bufferSize;
                        sha = new SHA256Managed();
                    }
                    
                    else if( ( bytesLeft = Program.blockSize - blockBytesRead ) < currBufferSize )
                    {
                        currBufferSize = (int)bytesLeft;
                    }

                        buffer = new byte[currBufferSize];
                }
            }
        }
    }
}
