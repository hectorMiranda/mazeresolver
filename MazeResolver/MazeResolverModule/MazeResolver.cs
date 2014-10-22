using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Text;
using System.Threading.Tasks;

namespace MazeResolverModule
{
    [Cmdlet(VerbsCommon.Find, "MazeSolution")]
    public class MazeResolver : PSCmdlet
    {

        [Parameter(Position = 0, Mandatory = true, HelpMessage = "Input image file, accepted types: ___ ")] //TODO: udpate this once you implement the file generation.
        public string FileName;

        [Parameter(Position = 1, Mandatory = false, HelpMessage = "Output format, PNG format will be set as default if this parameter is not provided.")]
        public int AllowedDistance;


        protected override void ProcessRecord()
        {
            var startTime = DateTime.UtcNow;
            WriteVerbose(string.Format("Starting process at {0}...", startTime));
            WriteVerbose(string.Format("Opening file: {0}...", FileName));

        }
    }
}
