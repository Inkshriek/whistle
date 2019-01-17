using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Whistle.Logbook {
    public interface ILogEntry {
        //An interface to use if you want to have a class function as an entry in the logbook.
        bool InLogbook { get; set; }
        LogEntryType EntryType { get; }
        string EntryInformation { get; }
    }

    public enum LogEntryType {
        Item,
        Monster,
        NPC,
        Lore
    }
}