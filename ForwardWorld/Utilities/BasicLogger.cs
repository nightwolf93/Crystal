using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Crystal.WorldServer.Utilities
{
    public class BasicLogger
    {
        public string Path { get; set; }
        public StreamWriter Writer { get; set; }

        public BasicLogger(string path)
        {
            this.Path = path;
            this.Writer = new StreamWriter(path);
            this.Writer.AutoFlush = true;
            this.Writer.Flush();
        }

        public void WriteLine(string message)
        {
            this.Writer.WriteLine(message);
        }
    }
}
