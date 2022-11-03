﻿using System;
using UnityEngine;

namespace ReadyPlayerMe.Core
{
    public class FailureEventArgs : EventArgs
    {
        public string Url { get; set; }
        public string Message { get; set; }
        public FailureType Type { get; set; }
    }

    public class ProgressChangeEventArgs : EventArgs
    {
        public string Url { get; set; }
        public float Progress { get; set; }
        public string Operation { get; set; }
    }

    public class CompletionEventArgs : EventArgs
    {
        public string Url { get; set; }
        public GameObject Avatar { get; set; }
        //TODO make CompletionEventArgs virtual so we can override in Avatar Loader
        //public AvatarMetadata Metadata { get; set; }
    }
}
