using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
//using Serialization;

public static class RPCEnabler22
{
	public static bool Others(this NetworkView networkView, string routineName, params object[] parameters)
	{
		if (networkView.isMine)
		{
			networkView.RPC(routineName, RPCMode.Others, parameters);
		}
		return !networkView.isMine;
		
	}
	
	public static bool Server(this NetworkView networkView, string routineName, params object[] parameters)
	{
		if(!Network.isServer)
			networkView.RPC(routineName, RPCMode.Server, parameters);
		return Network.isServer;
	}
}