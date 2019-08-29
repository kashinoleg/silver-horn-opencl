using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using Cloo;
using Cloo.Bindings;

namespace SilverHorn.Cloo.Event
{
    /// <summary>
    /// Represents a list of OpenCL generated or user created events.
    /// </summary>
    public class ComputeEventList : IList<IComputeEvent>
    {
        #region Fields

        [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
        private readonly List<IComputeEvent> events;

        #endregion

        #region Constructors

        /// <summary>
        /// Creates an empty <see cref="ComputeEventList"/>.
        /// </summary>
        public ComputeEventList()
        {
            events = new List<IComputeEvent>();
        }

        /// <summary>
        /// Creates a new <see cref="ComputeEventList"/> from an existing list of event types.
        /// </summary>
        /// <param name="events"> A list of event types. </param>
        public ComputeEventList(IList<IComputeEvent> events)
        {
            events = new Collection<IComputeEvent>(events);
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the last event types on the list.
        /// </summary>
        /// <value> The last event types on the list. </value>
        public IComputeEvent Last { get { return events[events.Count - 1]; } }

        #endregion

        #region Public methods

        /// <summary>
        /// Waits on the host thread for the specified events to complete.
        /// </summary>
        /// <param name="events"> The events to be waited for completition. </param>
        public static void Wait(ICollection<IComputeEvent> events)
        {
            int eventWaitListSize;
            CLEventHandle[] eventHandles = ComputeTools.ExtractHandles(events, out eventWaitListSize);
            ComputeErrorCode error = CL10.WaitForEvents(eventWaitListSize, eventHandles);
            ComputeException.ThrowOnError(error);
        }

        /// <summary>
        /// Waits on the host thread for the event types in the <see cref="ComputeEventList"/> to complete.
        /// </summary>
        public void Wait()
        {
            ComputeEventList.Wait(events);
        }

        #endregion

        #region IList<ComputeEventBase> Members

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public int IndexOf(IComputeEvent item)
        {
            return events.IndexOf(item);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <param name="item"></param>
        public void Insert(int index, IComputeEvent item)
        {
            events.Insert(index, item);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        public void RemoveAt(int index)
        {
            events.RemoveAt(index);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public IComputeEvent this[int index]
        {
            get
            {
                return events[index];
            }
            set
            {
                events[index] = value;
            }
        }

        #endregion

        #region ICollection<IComputeEvent> Members

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        public void Add(IComputeEvent item)
        {
            events.Add(item);
        }

        /// <summary>
        /// 
        /// </summary>
        public void Clear()
        {
            events.Clear();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool Contains(IComputeEvent item)
        {
            return events.Contains(item);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="array"></param>
        /// <param name="arrayIndex"></param>
        public void CopyTo(IComputeEvent[] array, int arrayIndex)
        {
            events.CopyTo(array, arrayIndex);
        }

        /// <summary>
        /// 
        /// </summary>
        public int Count
        {
            get { return events.Count; }
        }

        /// <summary>
        /// 
        /// </summary>
        public bool IsReadOnly
        {
            get { return false; }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool Remove(IComputeEvent item)
        {
            return events.Remove(item);
        }

        #endregion

        #region IEnumerable<IComputeEvent> Members

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public IEnumerator<IComputeEvent> GetEnumerator()
        {
            return ((IEnumerable<IComputeEvent>)events).GetEnumerator();
        }

        #endregion

        #region IEnumerable Members

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)events).GetEnumerator();
        }

        #endregion
    }
}
