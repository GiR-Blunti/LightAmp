﻿/*
 * Copyright(c) 2023 GiR-Zippo
 * Licensed under the GPL v3 license. See https://github.com/GiR-Zippo/LightAmp/blob/main/LICENSE for full license information.
 */

using Melanchall.DryWetMidi.Common;
using Melanchall.DryWetMidi.Core;
using Melanchall.DryWetMidi.Interaction;
using System.Linq;
using System.Threading.Tasks;

namespace BardMusicPlayer.Transmogrify.Song.Manipulation
{
    public static class TrackManipulations
    {
        #region Get/Set Channel

        /// <summary>
        /// Get channel number by first note on
        /// </summary>
        /// <param name="track"></param>
        /// <returns></returns>
        public static int GetChannelNumber(TrackChunk track)
        {
            var ev = track.Events.OfType<NoteOnEvent>().FirstOrDefault();
            if (ev != null)
                return ev.Channel;
            return -1;
        }

        /// <summary>
        /// Sets the channel number for a track
        /// </summary>
        /// <param name="track"></param>
        /// <param name="trackNumber"></param>
        /// <returns>MidiFile</returns>
        public static void SetChanNumber(TrackChunk track, int channelNumber)
        {
            if (channelNumber < 0)
                return;

            using (var notesManager = track.ManageNotes())
            {
                Parallel.ForEach(notesManager.Objects, note =>
                {
                    note.Channel = (FourBitNumber)channelNumber;
                });
                notesManager.SaveChanges();
            }

            using (var manager = track.ManageTimedEvents())
            {
                Parallel.ForEach(manager.Objects, midiEvent =>
                {
                    if (midiEvent.Event is ProgramChangeEvent pe)
                        pe.Channel = (FourBitNumber)channelNumber;
                    if (midiEvent.Event is ControlChangeEvent ce)
                        ce.Channel = (FourBitNumber)channelNumber;
                    if (midiEvent.Event is PitchBendEvent pbe)
                        pbe.Channel = (FourBitNumber)channelNumber;
                });
                manager.SaveChanges();
            }
        }

        #endregion

        #region Get/Set Instrument-ProgramChangeEvent
        /// <summary>
        /// Get the program number of the <see cref="TrackChunk"/>
        /// </summary>
        /// <param name="track"></param>
        /// <returns>The <see cref="int"/> representation of the instrument</returns>
        public static int GetInstrument(TrackChunk track)
        {
            var ev = track.Events.Where(e => e.EventType == MidiEventType.ProgramChange).FirstOrDefault();
            if (ev != null)
                return (ev as ProgramChangeEvent).ProgramNumber;
            return 1; //return a "None" instrument cuz we don't have all midi instrument in XIV
        }

        /// <summary>
        /// Create or overwrite the first progchange in <see cref="TrackChunk"/>
        /// </summary>
        /// <param name="track"></param>
        /// <param name="instrument"></param>
        public static void SetInstrument(TrackChunk track, int instrument)
        {
            int channel = GetChannelNumber(track);
            if (channel == -1)
                return;

            using (var events = track.ManageTimedEvents())
            {
                var ev = events.Objects.Where(e => e.Event.EventType == MidiEventType.ProgramChange).FirstOrDefault();
                if (ev != null)
                {
                    var prog = ev.Event as ProgramChangeEvent;
                    prog.ProgramNumber = (SevenBitNumber)instrument;
                }
                else
                {
                    var pe = new ProgramChangeEvent((SevenBitNumber)instrument);
                    pe.Channel = (FourBitNumber)channel;
                    events.Objects.Add(new TimedEvent(pe, 0));
                }
                events.SaveChanges();
            }
        }
        #endregion

        #region Get/Set TrackName
        /// <summary>
        /// Get the name of the <see cref="TrackChunk"/>
        /// </summary>
        /// <param name="track">TrackChunk</param>
        /// <returns></returns>
        public static string GetTrackName(TrackChunk track)
        {
            var trackName = track.Events.OfType<SequenceTrackNameEvent>().FirstOrDefault()?.Text;
            if (trackName != null)
                return trackName;
            return "No Name";
        }

        /// <summary>
        /// Sets the <see cref="TrackChunk"/> name
        /// </summary>
        /// <param name="track"></param>
        /// <param name="TrackName"></param>
        public static void SetTrackName(TrackChunk track, string TrackName)
        {
            using (var events = track.ManageTimedEvents())
            {
                var fev = events.Objects.Where(e => e.Event.EventType == MidiEventType.SequenceTrackName).FirstOrDefault();
                if (fev != null)
                {
                    (fev.Event as SequenceTrackNameEvent).Text = TrackName;
                    events.SaveChanges();
                }
                else
                {
                    SequenceTrackNameEvent name = new SequenceTrackNameEvent(TrackName);
                    track.Events.Insert(0, name);
                }

            }
        }
        #endregion

        #region Misc
        /// <summary>
        /// Remove all prog changes from <see cref="TrackChunk"/>
        /// </summary>
        /// <param name="track"></param>
        /// <returns></returns>
        public static void ClearProgChanges(TrackChunk track)
        {
            using (var manager = track.ManageTimedEvents())
            {
                manager.Objects.RemoveAll(e => e.Event.EventType == MidiEventType.ProgramChange);
                manager.Objects.RemoveAll(e => e.Event.EventType == MidiEventType.ProgramName);
                manager.SaveChanges();
            }
        }
        #endregion

    }
}
