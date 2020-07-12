// -----------------------------------------------------------------------------------------------
//  <copyright file="BranchState.cs" company="RetroTek Software Ltd">
//      Copyright (C) 2016 RetroTek Software Ltd. All rights reserved.
//  </copyright>
//  <author>Jared Mathes</author>
// -----------------------------------------------------------------------------------------------

namespace BubbleBreak
{
	public class BranchProgress
	{
		public BranchType BranchIsType { get; set; }
		public bool IsLocked { get; set; }
		public CompletionState BranchState { get; set; }
		public int LastLevelCompleted { get; set; }

		public BranchProgress ()
		{
			
		}
	}
}

