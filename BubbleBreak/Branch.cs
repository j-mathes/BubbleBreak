// -----------------------------------------------------------------------------------------------
//  <copyright file="Branch.cs" company="RetroTek Software Ltd">
//      Copyright (C) 2016 RetroTek Software Ltd. All rights reserved.
//  </copyright>
//  <author>Jared Mathes</author>
// -----------------------------------------------------------------------------------------------
using System;

namespace BubbleBreak
{
	public class Branch
	{
		public int BranchNum { get; set; }
		public string BranchName { get; set; }
		public int NumberOfLevels { get; set; }
		public int UnlockNextBranch { get; set; }	// the level that needs to be completed to unlock the next branch
		public int UnlockFreePlay { get; set; }		// the level that needs to be completed to unlock the branch's sequence type in free play

		public Branch ()
		{
		}
	}
}

