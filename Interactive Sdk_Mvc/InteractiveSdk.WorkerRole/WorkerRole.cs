using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Xml;
using InteractiveSdk.WorkerRole.MessageHandlers;

namespace InteractiveSdk.WorkerRole
{
	public class WorkerRole// : RoleEntryPoint
	{
		public void Run()
		{
		}

		private void InitializeHandlers()
		{
		}

		//public void ProcessMessage(CloudQueueMessage msg)
		public void ProcessMessage()
		{

		}
		
		//public override bool OnStart()
		public bool OnStart()
		{
			return false;
		}

		private static void RoleEnvironmentChanging(object sender)
		{
		}
	}
}
