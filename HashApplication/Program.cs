using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.IO;

namespace HashApplication
{
    class Program
    {
        public static long blockSize = 1000;
        public static string filePath = "E:\\screenshot.png";
        public static int bufferSize = 16384; //16kb
        public static short numOfThreads = 4;

        static void Main( string[] args )
        {
            try
            {
                Initialize();
                Run();
            }
            catch( Exception ex )
            {
                Console.WriteLine( "There was an exception, message: {0}.\nStack Trace : {1}", ex.Message, ex.StackTrace );
            }

            Console.ReadLine();
        }

        static void Run()
        {
            long fileLength = new FileInfo( filePath ).Length;
            long numOfParts = fileLength / blockSize;
            long remainings = fileLength % blockSize;

            if( bufferSize > blockSize )
            {
                bufferSize = (int)blockSize;
            }

            if( numOfThreads > numOfParts )
            {
                numOfThreads = (short)numOfParts;
            }
            long partsPerThread = numOfParts / numOfThreads;


            long currentOffset = 0;

            for( int i = 0; i < numOfThreads; ++i )
            {
                long bytesToRead = partsPerThread * blockSize;
                if( i == ( numOfThreads - 1 ) ) // if last thread - get all remainings as well
                {
                    bytesToRead += remainings;
                }
                HashComputer computer = new HashComputer( currentOffset, bytesToRead, i.ToString(), i * partsPerThread );
                computer.StartHashing();
                currentOffset += bytesToRead;
            }
        }

        static void Initialize()
        {
            Console.WriteLine( "Please, specify the filepath: " );
            filePath = Console.ReadLine();
            Console.WriteLine( "Please, specify the block size: " );
            blockSize = Convert.ToInt64( Console.ReadLine() );
        }
    }
}
